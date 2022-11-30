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
 * Created by adiyoss on 2/7/15.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class Pitch2CENS : iSignal2Chroma{

    // data members
    ChromaHelpers helpers = new ChromaHelpers();

    /**
     * create chroma representation from the pitch energy
     * @param f_pitch_energy
     * @return chroma
     */
    public float[][] signal2Chroma(List<float[]> f_pitch_energy, bool applyLogNormalize){

        // parameters
        int seg_num = f_pitch_energy[0].Length;
        float[] quantSteps = {0.4f,0.2f,0.1f,0.05f};
        float[] quantWeights = {0.25f,0.25f,0.25f,0.25f};
        
        float[][] f_chroma_energy = new float[seg_num][];
        for (int i = 0; i < f_chroma_energy.Length; i++)
        {
            
            f_chroma_energy[i] = new float[ChromaConsts.size_of_chroma];
            
        }
        
        float[][] f_chroma_energy_distr = new float[seg_num][];
        for (int i = 0; i < f_chroma_energy_distr.Length; i++)
        {
            
            f_chroma_energy_distr[i] = new float[ChromaConsts.size_of_chroma];
            
        }
        
        
        float[][] f_CENS;

        // apply log-normalize
        if(applyLogNormalize) {
            for (int i = 0; i < f_pitch_energy.Count; i++) {
                for (int j = 0; j < seg_num; j++)
                    f_pitch_energy[i][j] = Mathf.Log10(ChromaConsts.addTermLogCompr + f_pitch_energy[i][j] * ChromaConsts.factorLogCompr);
            }
        }

        // calculate energy for each chroma band
        for(int p = 0; p<f_pitch_energy.Count; p++){
            int chroma = (p+1)%ChromaConsts.size_of_chroma;
            for(int j = 0; j<f_pitch_energy[p].Length ; j++){
                f_chroma_energy[j][chroma] += f_pitch_energy[p][j];
            }
        }

        // normalize the chroma vectors
        for(int k = 0; k<seg_num ; k++){
            if(sumOverThreshold(f_chroma_energy[k],ChromaConsts.normalize_threshold) > 0){
                float seg_energy_square = sum(f_chroma_energy[k]);
                for(int i=0 ; i<f_chroma_energy[k].Length ; i++)
                    f_chroma_energy_distr[k][i] = f_chroma_energy[k][i] / seg_energy_square;
            }
        }

        // ====== calculate a CENS feature ====== //
        // component-wise quantisation of the normalized chroma vectors
        float[][] f_start_help = new float[seg_num][];
        
        for (int i = 0; i < f_start_help.Length; i++)
        {
            
            f_start_help[i] = new float[ChromaConsts.size_of_chroma];
            
        }
        
        
        for (int n = 0; n< quantSteps.Length; n++){
            for(int i=0 ; i<f_start_help.Length ; i++){
                for(int j=0 ; j<f_start_help[i].Length ; j++)
                    if(f_chroma_energy_distr[i][j] > quantSteps[n])
                        f_start_help[i][j] += quantWeights[n];
            }
        }

        // last step: normalize each vector with its l^2 norm
        f_CENS = helpers.normalizeChroma(f_start_help);
        return f_CENS;
    }


    /**
     * Calculate the sum of the vector x
     * @param x
     * @return the sum
     */
    private float sum(float[] x){
        float res = 0.0f;
        for(int i=0 ; i<x.Length ; i++)
            res+=x[i];
        return res;
    }

    /**
     * Counts how many items in x where above threshold
     * @param x
     * @param threshold
     * @return
     */
    private int sumOverThreshold(float[] x, float threshold){
        int res = 0;
        for(int i=0 ; i<x.Length ; i++){
            if(x[i] > threshold)
                res++;
        }

        return res;
    }
}
