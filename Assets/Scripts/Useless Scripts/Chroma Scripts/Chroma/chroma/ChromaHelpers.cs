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
using UnityEngine;

public class ChromaHelpers {
    /**
     *
     * @param x - the chroma values
     * @return the normalized chroma
     */
    public float[][] normalizeChroma(float[][] x){
        float[][] normalizedChroma = new float[x.Length][];
        for (int i = 0; i < normalizedChroma.Length; i++)
        {
            
            normalizedChroma[i] = new float[ChromaConsts.size_of_chroma];
            
        }
        
        float[] unitVector = new float[ChromaConsts.size_of_chroma];

        // initializing and normalizing the unit vector
        for(int i=0 ; i<unitVector.Length ; i++)
            unitVector[i] = 1;
        float norm = norm_2(unitVector);
        for(int i=0 ; i<unitVector.Length ; i++)
            unitVector[i] /= norm;


        // normalise the vectors according to the l^2 norm
        for(int k=0 ; k<x.Length ; k++){
            float n = norm_2(x[k]);
            if (n < ChromaConsts.normalize_threshold)
                for(int i=0 ; i<unitVector.Length;  i++)
                    normalizedChroma[k][i] = unitVector[i];
            else {
                for(int i=0 ; i<x[k].Length ; i++)
                    normalizedChroma[k][i] = x[k][i]/n;
            }
        }
        return normalizedChroma;
    }

    /**
     * compute norm 2 for x
     * @param x
     * @return
     */
    private float norm_2(float[] x){
        float res = 0;
        for(int i=0 ; i<x.Length ; i++)
            res += x[i]*x[i];
        res = Mathf.Sqrt(res);
        return res;
    }
}
