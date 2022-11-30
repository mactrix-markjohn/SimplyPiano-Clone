using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaChordDetector : MonoBehaviour
{
    public enum ChordQuality
    {
        Minor,
        Major,
        Suspended,
        Dominant,
        Dimished5th,
        Augmented5th
    };

    public ChordQuality chordQuality;
    
    float[] chromagram;
    float[][] chordProfiles;
    float[] chord;
    float bias;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        chromagram = new float[12];
        chordProfiles = new float[108][];

        for (int i = 0; i < chordProfiles.Length; i++)
        {
            chordProfiles[i] = new float[12];
        }

        chord = new float[108];
        bias = 1.06f;
        
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
