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

public class Pitch2Chroma : iSignal2Chroma {

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
        float[][] f_chroma = new float[seg_num][];
        
        for (int i = 0; i < f_chroma.Length; i++)
        {
            
            f_chroma[i] = new float[ChromaConsts.size_of_chroma];
            
        }
        

        // apply log-normalize
        if(applyLogNormalize) {
            for (int i = 0; i < f_pitch_energy.Count; i++) {
                for (int j = 0; j < seg_num; j++)
                    f_pitch_energy[i][j] = Mathf.Log10(ChromaConsts.addTermLogCompr + f_pitch_energy[i][j] * ChromaConsts.factorLogCompr);
            }
        }

        //calculate energy for each chroma band
        for(int p = 0; p<f_pitch_energy.Count; p++){
            int chroma = (p+1)%ChromaConsts.size_of_chroma;
            for(int j = 0; j<seg_num ; j++){
                f_chroma[j][chroma] += f_pitch_energy[p][j];
            }
        }

        // normalize each vector with its l^2 norm
        f_chroma = helpers.normalizeChroma(f_chroma);

        return f_chroma;
    }
}
