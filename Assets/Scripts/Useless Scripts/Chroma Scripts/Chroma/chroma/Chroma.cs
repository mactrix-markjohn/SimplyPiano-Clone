/*
 * Chroma - Pitch and Chroma implementation in Java
 * Copyright (C) 2015 Yossi Adi, E-Mail: yossiadidrum@gmail.com
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */





/**
 * Created by adiyoss on 2/5/15.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class Chroma {

    // data members
    private iSignal2Chroma signal2Chroma = new Pitch2Chroma();

    /**
     *
     * @param x - audio signal, should be normalized
     * @return the chroma
     */
    public float[][] signal2ChromaMethod(float[] x) {

        int[] fs_pitch = new int[ChromaConsts.pitch_size];
        int[] fs_index = new int[ChromaConsts.pitch_size];
        int wav_size = x.Length;

        //====================================//
        //          RESAMPLE THE WAVE		  //
        //====================================//
        initPitchAndIndex(fs_pitch,fs_index);
        float[][] pcm_ds = new float[3][];
        pcm_ds[0] = x;
        pcm_ds[1] = resample(pcm_ds[0],1,5);
        pcm_ds[2] = resample(pcm_ds[1],1,5);
        //====================================//

        //====================================//
        //  COMPUTE FEATURES FOR ALL PITCHES  //
        //====================================//
        List<float[]> f_pitch_energy = new List<float[]>();
        List<int> seg_pcm_num = new List<int>();
        List<int> seg_pcm_start = new List<int>();
        List<int> seg_pcm_stop = new List<int>();
        List<FilterCoefficientsEntity> coefficients;

        // initialize vectors
        int step_size = ChromaConsts.winLenSTMSP - ChromaConsts.winOvSTMSP;
        int group_delay = (int) Math.Round((float) (ChromaConsts.winLenSTMSP / 2));
        seg_pcm_start.Add(1);
        for(int i = 0; i<wav_size/step_size+1 ; i++)
            seg_pcm_start.Add(1 + step_size*i);
        for (int i = 0; i<seg_pcm_start.Count ; i++)
            seg_pcm_stop.Add(Math.Min(seg_pcm_start[i] + ChromaConsts.winLenSTMSP, wav_size));
        seg_pcm_stop.Insert(0, Math.Min(group_delay, wav_size));
        seg_pcm_num.Insert(0,seg_pcm_start.Count);

        for(int i=0 ; i<ChromaConsts.sizeOfPitchFs ; i++) {
            float[] row = new float[seg_pcm_num[0]];
            for (int j = 0; j < row.Length; j++)
                row[j] = 0.0f;
            f_pitch_energy.Add(row);
        }
        //====================================//
        coefficients = getIIRCoeff(); // Load the IIR Filter Parameters

        //====================================//
        //   	  COMPUTE F_PITCH_ENERGY 	  //
        //====================================//
        for(int p=ChromaConsts.midiMin ; p<ChromaConsts.midiMax ; p++){
            // IIR Filter
            int index = fs_index[p];
            float[] f_filtfilt = filter(coefficients[p].b, coefficients[p].a, pcm_ds[index]);
            float[] f_square = square_arr(f_filtfilt);

            int factor = ChromaConsts.fs/fs_pitch[p];
            for (int k = 0; k<seg_pcm_num[0]; k++){
                int start = (int)Math.Ceiling(((float)seg_pcm_start[k] / ChromaConsts.fs) * fs_pitch[p]);
                int stop = (int)Math.Floor(((float)seg_pcm_stop[k] / ChromaConsts.fs) * fs_pitch[p]);
                float tmp = 0;
                for(int t = start-1 ; t< stop-1 ; t++)
                    tmp += f_square[t]*factor;
                f_pitch_energy[p][k]=tmp;
            }
        }

        // creating the chroma by the desired algorithm
        float[][] chroma = signal2Chroma.signal2Chroma(f_pitch_energy,false);
        return chroma;
    }

    /**
     * This function gets as input a wave signal and its a and b coefficients for the iir filter and returns the filtered signal using iir filter
     * @param b
     * @param a
     * @param x
     * @return filtered signal
     */
    private float[] filter(double[] b, double[] a, float[] x)
    {
        float[] y = new float[x.Length];
        int na = a.Length;
        int nb = b.Length;

        for(int n = 0; n<x.Length ; n++){
            y[n] = (float)b[0]*x[n];
            for(int i=0 ; i<Math.Min(n,nb-1); i++)
                y[n] +=  (float)b[i + 1] * x[n - i - 1];
            for(int i=0 ; i<Math.Min(n,na-1); i++)
                y[n] -= (float)a[i + 1] * y[n - i - 1];
        }

        return y;
    }

    /**
     * This function gets as input an array list of doubles and returns the square array
     * @param f_filtfilt
     * @return filter square
     */
    private float[] square_arr(float[] f_filtfilt){
        float[] result = new float[f_filtfilt.Length];
        for(int i=0 ; i<f_filtfilt.Length ; i++)
            result[i] = Mathf.Pow(f_filtfilt[i],2);
        return result;
    }

    /**
     * resample the signal x to be from (x.length*p)/q
     * @param x
     * @param p
     * @param q
     * @return the new signal
     */
    private float[] resample(float[] x, int p, int q){
        float[] y = new float[(x.Length*p)/q];
        for(int k=0 ; k<y.Length ; k++)
            y[k] = x[(int) Math.Round((double) ((k)*q/p))];
        return y;
    }



    /**
     * initialize the pitch values and index values
     * @param pitch
     * @param index
     */
    private void initPitchAndIndex(int[] pitch, int[] index){
        int start_882 = 20;
        int start_4410 = 59;
        int start_22050 = 95;
        int end_22050 = 120;
        int i = 0;

        for(i=0 ; i<start_882 ; i++){
            pitch[i] = 0;
            index[i] = -1;
        }
        for(i=start_882 ; i<start_4410 ; i++){
            pitch[i] = 882;
            index[i] = 2;
        }
        for(i=start_4410 ; i<start_22050 ; i++){
            pitch[i] = 4410;
            index[i] = 1;
        }
        for(i=start_22050 ; i<end_22050 ; i++){
            pitch[i] = 22050;
            index[i] = 0;
        }
        for(i=end_22050 ; i<pitch.Length ; i++){
            pitch[i] = 0;
            index[i] = -1;
        }
    }

    private List<FilterCoefficientsEntity> getIIRCoeff(){
        List<FilterCoefficientsEntity> coefficients = new List<FilterCoefficientsEntity>();

        // populate the coefficients array
        for(int i=0 ; i< IirFilterCoeffA.a.Length ; i++){
            FilterCoefficientsEntity coefficient = new FilterCoefficientsEntity();
            coefficient.a =  IirFilterCoeffA.a[i];
            coefficient.b = IirFilterCoeffB.b[i];
            coefficients.Add(coefficient);
        }
        return coefficients;
    }
}
