using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ChromaChordTest : MonoBehaviour
{
    
    public Text ShowCord;
    private float[] sample;
    private float[] sampleOutput;
    private Chroma chroma;
    public static float[][] chrom;
    public PianoKeys PianoKeys;
    private static int chromagramNumHarmonics = 3;
    private static int chromagramNumOctaves = 2;
    private static int chromagramNumBinsToSearch = 3;
    
    public static int SAMPLE_RATE = 44100;                  // The sampling rate (11025, 16000, 22050, 44100)
    public static int BUFFER_SIZE = 8192;                   // It must be a power of 2 (2048, 4096, 8192, 16384...)
    public static int HOP_SIZE = 2048;                      // It must be a power of 2
    public static int FRAMES_PER_SECOND = 10;
    public static int BANDPASS_FILTER_LOW_FREQ = 55;
    public static int BANDPASS_FILTER_HIGH_FREQ = 4000;
    
    
    public enum ChordQuality
    {
        Minor,
        Major,
        Suspended,
        Dominant,
        Dimished5th,
        Augmented5th
    };
    
    
    public enum ChordType : int
    {
	    Major = 0,
	    Minor = 1,
	    Diminished = 2,
	    Augmented = 3,
	    Suspended2 = 4,
	    Suspended4 = 5,
	    MajorSixth = 6,
	    MinorSixth = 7,
	    MajorSeventh = 8,
	    MinorSeventh = 9,
	    DominantSeventh = 10
    }
    
    

    public ChordQuality quality;
    public ChordType chordType;
    
    float[] chromagram;
    float[][] chordProfiles;
    float[] chord;
    float bias;
    
    /** The root note of the detected chord */
    int rootNote;
    
    /** The quality of the detected chord (Major, Minor, etc) */
    //TODO: int quality;
    
    /** Any other intervals that describe the chord, e.g. 7th */
    int intervals;

    private ChromaChromagramC chromaC;

    private FastYinChecker yinChecker;
    private FastYinCheckerChord yinCheckerChord;

    // Start is called before the first frame update
    void Start()
    {
	    
	    //chord detectoion algorithm..................................
       
	    
	    
	    
	    chromagram = new float[12];
        chordProfiles = new float[108][];

        for (int i = 0; i < chordProfiles.Length; i++)
        {
            chordProfiles[i] = new float[12];
        }

        chord = new float[108];
        bias = 1.06f;
        
        makeChordProfiles();
        
        
       //end of the chord detection ..................................................... 
       
      chromaC = new ChromaChromagramC();
       
        sample = new float[4096];
        sampleOutput = new float[4096];
        chroma = new Chroma();
        yinChecker = new FastYinChecker(44100,sampleOutput.Length);
        yinCheckerChord = new FastYinCheckerChord(44100,sampleOutput.Length);
        
    }

    // Update is called once per frame
    void Update()
    {
        GetAudioSource.AudioSource.GetSpectrumData(sample, 0,FFTWindow.BlackmanHarris);
        GetAudioSource.AudioSource.GetOutputData(sampleOutput, 0);
        MidiNoteReceptor.isChordIndicate = false;
        
        

       // float[][] chromas =  chroma.signal2ChromaMethod(sample);
        //chrom = chromas;
        
        int tauEstimate;
        yinChecker.difference(sampleOutput);
        yinChecker.cumulativeMeanNormalizedDifference();
        tauEstimate = yinChecker.absoluteThreshold();

        if (tauEstimate != -1)
        {
	        // A single note is played
	        
	        ShowCord.text = $"Single note is played";
        }
        else
        {
	        
	        // A chord is played
	        
	        yinCheckerChord.difference(sampleOutput);
	        yinCheckerChord.cumulativeMeanNormalizedDifference();
	        tauEstimate = yinCheckerChord.absoluteThreshold();

	        if (tauEstimate != -1)
	        {
		        //pitch found
		        MidiNoteReceptor.isChordIndicate = true;
		        //chromaC.calculateChromagram(sample,44100,sample.Length);
		        //chromaC.getchromagram( sampleOutput, 44100, sample.Length);

		        // chromagram = chromas[0];
	        
		        chromaC.Chromagram(BUFFER_SIZE,SAMPLE_RATE,BUFFER_SIZE,chromagramNumHarmonics,chromagramNumOctaves,chromagramNumBinsToSearch);
		        chromaC.setInputAudioFrameSize(BUFFER_SIZE);
		        chromaC.setSamplingFrequency(SAMPLE_RATE);
		        chromaC.setChromaCalculationInterval(BUFFER_SIZE);
	        
		        chromaC.setMagnitudeSpectrum(sample);
		        chromaC.setChordDetectionAlgorithm(2);
		        chromaC.processAudioFrame(2);

		        float[] chromaSample = chromaC.GetChromagram();
		        chromagram = chromaSample;
       

		        classifyChromagram();
	        
        
		        /*ShowCord.text = $"Chromas[0] : {chromas[0][10]}\nChromas[1]: {chromas[1][10]}\n\nChroma Size: {chromas.Length}\n\nChroma 2nd dem: {chromas[0].Length}  \n\nSamples: {sample[0]}\n\nSample size: {sample.Length}" +
		                        $"\n\nChord Quality: {quality.ToString()}\n\nRoot Note: {rootNote}\n\nInterval: {intervals}";*/
        
	        
	        
		        if (rootNote == 10)
		        {
			        rootNote = 0 + 36;
		        }else if (rootNote == 11)
		        {
			        rootNote = 1 + 36;
		        }
		        else
		        {
			        rootNote += (2 + 36 );
		        }


		        /*ShowCord.text = $"Chromas[0] : {chromaSample[10]}\nChromas[1]: {chromaSample[10]}\n\nChroma Size: {chromaSample.Length}\n\nChroma 2nd dem: {chromaSample.Length}  \n\nSamples: {sample[0]}\n\nSample size: {sample.Length}" +
		                        $"\n\nChord Quality: {quality.ToString()}\n\nRoot Note: {rootNote}\n\nInterval: {intervals}";*/


		       ShowCord.text = $"Root Note of Chord: \n{rootNote}";



		        var chord = new Chord((int)chordType,rootNote);
		        PianoKeys.OnAgentGuess(chord,quality.ToString(),$"{intervals}");
	        }
        }





    }
    
    
    void classifyChromagram()
{
	int i;
	int j;
	int fifth;
	int chordindex;
	
	// remove some of the 5th note energy from chromagram
	for (i = 0; i < 12; i++)
	{
		fifth = (i+7) % 12;
		chromagram[fifth] = chromagram[fifth] - (0.1f * chromagram[i]);
		
		if (chromagram[fifth] < 0)
		{
			chromagram[fifth] = 0;
		}
	}
	
	// major chords
	for (j = 0; j < 12; j++)
	{
		chord[j] = calculateChordScore (chromagram,chordProfiles[j], bias, 3);
	}
	
	// minor chords
	for (j = 12; j < 24; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], bias, 3);
	}
	
	// diminished 5th chords
	for (j = 24; j < 36; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], bias, 3);
	}
	
	// augmented 5th chords
	for (j = 36; j < 48; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], bias, 3);
	}
	
	// sus2 chords
	for (j = 48; j < 60; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], 1, 3);
	}
	
	// sus4 chords
	for (j = 60; j < 72; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], 1, 3);
	}
	
	// major 7th chords
	for (j = 72; j < 84; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], 1, 4);
	}
	
	// minor 7th chords
	for (j = 84; j < 96; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], bias, 4);
	}

	// dominant 7th chords
	for (j = 96; j < 108; j++)
	{
		chord[j] = calculateChordScore (chromagram, chordProfiles[j], bias, 4);
	}
	
	chordindex = minimumIndex (chord, 108);
	
	// major
	if (chordindex < 12)
	{
		rootNote = chordindex;
		quality = ChordQuality.Major;
		chordType = ChordType.Major;
		intervals = 0;
	}
	
	// minor
	if ((chordindex >= 12) && (chordindex < 24))
	{
		rootNote = chordindex-12;
		quality = ChordQuality.Minor;
		chordType = ChordType.Minor;
		intervals = 0;
	}
	
	// diminished 5th
	if ((chordindex >= 24) && (chordindex < 36))
	{
		rootNote = chordindex-24;
		quality = ChordQuality.Dimished5th;
		chordType = ChordType.Diminished;
		intervals = 0;
	}
	
	// augmented 5th
	if ((chordindex >= 36) && (chordindex < 48))
	{
		rootNote = chordindex-36;
		quality = ChordQuality.Augmented5th;
		chordType = ChordType.Augmented;
		intervals = 0;
	}
	
	// sus2
	if ((chordindex >= 48) && (chordindex < 60))
	{
		rootNote = chordindex-48;
		quality = ChordQuality.Suspended;
		chordType = ChordType.Suspended2;
		intervals = 2;
	}
	
	// sus4
	if ((chordindex >= 60) && (chordindex < 72))
	{
		rootNote = chordindex-60;
		quality = ChordQuality.Suspended;
		chordType = ChordType.Suspended4;
		intervals = 4;
	}
	
	// major 7th
	if ((chordindex >= 72) && (chordindex < 84))
	{
		rootNote = chordindex-72;
		quality = ChordQuality.Major;
		chordType = ChordType.MajorSeventh;
		intervals = 7;
	}
	
	// minor 7th
	if ((chordindex >= 84) && (chordindex < 96))
	{
		rootNote = chordindex-84;
		quality = ChordQuality.Minor;
		chordType = ChordType.MinorSeventh;
		intervals = 7;
	}
	
	// dominant 7th
	if ((chordindex >= 96) && (chordindex < 108))
	{
		rootNote = chordindex-96;
		quality = ChordQuality.Dominant;
		chordType = ChordType.DominantSeventh;
		intervals = 7;
	}
}

//=======================================================================
float calculateChordScore (float[] chroma, float[] chordProfile, float biasToUse, float N)
{
	float sum = 0;
	float delta;

	for (int i = 0; i < 12; i++)
	{
		sum = sum + ((1 - chordProfile[i]) * (chroma[i] * chroma[i]));
	}

	delta = Mathf.Sqrt (sum) / ((12 - N) * biasToUse);
	
	return delta;
}

//=======================================================================
int minimumIndex (float[] array, int arrayLength)
{
	float minValue = 100000;
	int minIndex = 0;
	
	for (int i = 0;i < arrayLength;i++)
	{
		if (array[i] < minValue)
		{
			minValue = array[i];
			minIndex = i;
		}
	}
	
	return minIndex;
}

//=======================================================================
void makeChordProfiles()
{
	int i;
	int t;
	int j = 0;
	int root;
	int third;
	int fifth;
	int seventh;
	
	float v1 = 1;
	float v2 = 1;
	float v3 = 1;
	
	// set profiles matrix to all zeros
	for (j = 0; j < 108; j++)
	{
		for (t = 0;t < 12;t++)
		{
			chordProfiles[j][t] = 0;
		}
	}
	
	// reset j to zero to begin creating profiles
	j = 0;
	
	// major chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 4) % 12;
		fifth = (i + 7) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		
		j++;				
	}

	// minor chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 3) % 12;
		fifth = (i + 7) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		
		j++;				
	}

	// diminished chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 3) % 12;
		fifth = (i + 6) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		
		j++;				
	}	
	
	// augmented chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 4) % 12;
		fifth = (i + 8) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		
		j++;				
	}	
	
	// sus2 chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 2) % 12;
		fifth = (i + 7) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		
		j++;				
	}
	
	// sus4 chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 5) % 12;
		fifth = (i + 7) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		
		j++;				
	}		
	
	// major 7th chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 4) % 12;
		fifth = (i + 7) % 12;
		seventh = (i + 11) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		chordProfiles[j][seventh] = v3;
		
		j++;				
	}	
	
	// minor 7th chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 3) % 12;
		fifth = (i + 7) % 12;
		seventh = (i + 10) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		chordProfiles[j][seventh] = v3;
		
		j++;				
	}
	
	// dominant 7th chords
	for (i = 0; i < 12; i++)
	{
		root = i % 12;
		third = (i + 4) % 12;
		fifth = (i + 7) % 12;
		seventh = (i + 10) % 12;
		
		chordProfiles[j][root] = v1;
		chordProfiles[j][third] = v2;
		chordProfiles[j][fifth] = v3;
		chordProfiles[j][seventh] = v3;
		
		j++;				
	}
}
    
}
