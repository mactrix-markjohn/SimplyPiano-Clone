using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class ChordDetector : MonoBehaviour
{


    public Text ShowCord;
    
    private float[] tempBuffer;
    private List<Chordj> targetChords;
    public Chordj detectedChord;
    
    protected int samplingFrequency;
    protected int frameShift;
    protected int frameLength;
    protected int head;
    protected int atFrame;
    protected int maxFrames;
    public AudioData audioData;
    
    
    //Plot
    protected float[] magnitudeSpectrum;
    protected float volume = 0f;
    protected bool readLock = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        targetChords = new List<Chordj>();
        
        String[] majorRoots = new String[] {"A","C","D","E","F","G"};
        String[] minorRoots = new String[] {"A","B","D","E"};
        for (int i = 0; i < majorRoots.Length; i++) {
            targetChords.Add(new Chordj(majorRoots[i],"maj"));
        }
        for (int i = 0; i < minorRoots.Length; i++) {
            targetChords.Add(new Chordj(minorRoots[i],"m"));
        }
        
        
        ChordDetectorStart(44100,1024,4,targetChords);
        
    }

    // Update is called once per frame
    void Update()
    {
        process(audioData);
    }

   


    public void ChordDetectorStart(int samplingFrequency, int frameLength,  int frameShift,  List<Chordj> targetChords) {
//        int frameLength = Math.round ((float)audioData.samplingFrequency * CHROMAGRAM_FRAME_LENGTH_IN_S);
//        int frameShift = frameLength / 4;
//        final float CHROMAGRAM_FRAME_LENGTH_IN_S = 0.75f;
        this.samplingFrequency = samplingFrequency;
        this.frameLength = frameLength;
        this.frameShift = frameShift;
        this.head = -1;
        this.atFrame = -1;
        this.maxFrames = -1;
        this.targetChords = targetChords;

        this.samplingFrequency = 44100;
        this.frameLength = 1024;

    }
    
    
     private float[] getChromagram(float[] audioBuffer){
        //Make sure the buffer length is even
        float[] tmpAudio;

        if((audioBuffer.Length % 2) == 0){
            tmpAudio = (float[])audioBuffer.Clone();
        } else{
            tmpAudio = new float[audioBuffer.Length+1];
            for (int i = 0; i < audioBuffer.Length; i++) {
                tmpAudio[i] = audioBuffer[i];
            }
            tmpAudio[audioBuffer.Length] = 0;
        }



        float[] buf = audioBuffer;
	    for (int i = 0; i < buf.Length; i++) {
		    buf[i] -= 0.5f; //center the signal on 0 before windowing
	    }



        /*FFT*/
	    float[] window = getHammingWindow(buf.Length);
	    for (int i = 0; i < buf.Length; i++) {
		    buf[i] *= window[i];
	    }
        
        //TODO: Important
        //DoubleFFT_1D fft = new DoubleFFT_1D(buf.Length);
        //fft.realForward(buf);*/



        FloatFFT fft = new FloatFFT(buf.Length);
        fft.complexForward(buf);
        
        //TODO: Lock/unlock magnitudeSpectrum while writing
        //Get Magnitude spectrum from FFT
        readLock = true;
        magnitudeSpectrum = new float[buf.Length/2];
        magnitudeSpectrum[0] = Math.Abs(buf[0]);
        magnitudeSpectrum[magnitudeSpectrum.Length-1] = Math.Abs(buf[1]);
        for (int i = 1; i < magnitudeSpectrum.Length-1; i++) {
            magnitudeSpectrum[i] = ((buf[2*i]*buf[2*i]) + (buf[2*i+1]*buf[2*i+1]));
        }

	    

        //Normalize by max peak
        float maxVal = 0f;
        for (int i = 0; i < magnitudeSpectrum.Length; i++) {
            if(magnitudeSpectrum[i] > maxVal) maxVal = magnitudeSpectrum[i];
        }
        float normalizationFactor = maxVal;
        for (int i = 0; i < magnitudeSpectrum.Length; i++) {
		    //The sqrt comes from the paper. It's for making the peak differences smaller
		    magnitudeSpectrum[i] = Mathf.Sqrt(magnitudeSpectrum[i]/ normalizationFactor);
	    }
        readLock = false;



        float A1 = 55; //reference note A1 in Hz
        int peakSearchWidth = 2;
	    int kprime,k0,k1;
	    float[] chromagram = new float[12];
        
	   // Array.Fill(chromagram,0);
        Fill(chromagram,0);
        
        for (int interval = 0; interval < 12; interval++) {
            for (int phi = 1; phi <= 5; phi++) {
                for (int harmonic = 1; harmonic <= 2; harmonic++) {
                    kprime = (int) Math.Round( frequencyFromInterval(A1,interval) * (double)phi * (double)harmonic / ((double)samplingFrequency/(double)frameLength) );
                    k0 = kprime - (peakSearchWidth*harmonic);
                    k1 = kprime + (peakSearchWidth*harmonic);
                    chromagram[interval] += findMaxValue(magnitudeSpectrum, k0, k1) / harmonic;
                }
            }
        }

	    return chromagram;
    }
    
      private Chordj detectChord(List<Chordj> targetChords, float[] chromagram){
        //Take the square of chromagram so the peak differences are more pronounced. see paper.
        for (int i = 0; i < chromagram.Length; i++) {
            chromagram[i] *= chromagram[i];
        }

	   // Log.d("Chromagram", Arrays.toString(chromagram));

        float[] deltas = new float[targetChords.Count];
        //Arrays.fill(deltas,0);
        Fill(deltas,0);
        float[] bitMask = new float[12];
        for (int i = 0; i < targetChords.Count; i++) {
            //Generate bit mask for target chord
            //Arrays.fill(bitMask,1);
            
            Fill(bitMask,1);
            int[] notes = targetChords[i].getNotes();
            for (int j = 0; j < notes.Length; j++) {
                bitMask[notes[j]-1] = 0;
            }



	        //Calculate the normalized total difference with target chord pattern
            for (int j = 0; j < chromagram.Length; j++) {
                deltas[i] += chromagram[j] * bitMask[j];
            }
            deltas[i] /= 12-notes.Length;
            deltas[i] = Mathf.Sqrt(deltas[i]);
        }
//	    Log.d("deltas", Arrays.toString(deltas));
        int chordIndex = findMinIndex(deltas);
//	    Log.d("minIndex", Integer.toString(chordIndex));
        return targetChords[chordIndex];
    }

    private float frequencyFromInterval(float baseNote, int intervalInSemitones){
        return baseNote * Mathf.Pow(2,(float)intervalInSemitones/12);
    }

    private float findMaxValue(float[] arr, int beginIndex, int endIndex){
        //TODO: array safety
        float maxVal = -float.MaxValue;
        for (int i = 0; i < arr.Length; i++) {
            if(arr[i] > maxVal) maxVal = arr[i];
        }
        return maxVal;
    }

    private int findMinIndex(float[] arr){
        float minVal = float.MaxValue;
        int minIndex = -1;

        for (int i = 0; i < arr.Length; i++) {
            if(arr[i] < minVal){
                minVal = arr[i];
                minIndex = i;
            }
        }
        return minIndex;
    }

    
    public void process(AudioData inputAudioData)
    {
        
        audioData = inputAudioData;
        volume =  audioData.getSignalPower();
//		Log.d("volume",Double.toString(volume));
        //TODO: Fix this in PitchDetector too!
        if(audioData.length() > frameLength){
            maxFrames = (int) Math.Ceiling( (double)(audioData.length() - frameLength) / (double) frameShift);
        } else {
            maxFrames = 1;
        }
        atFrame = 1;
        head = 0;
        float[] chromagram;
        /*while((tempBuffer = getNextFrame()) != null ){
            chromagram = getChromagram(tempBuffer);
            detectedChord = detectChord(targetChords, chromagram);
        }*/

        tempBuffer = audioData.audioBuffer;
        chromagram = getChromagram(tempBuffer);
        detectedChord = detectChord(targetChords, chromagram);

        //detectedChord = detectChord(targetChords, ChromaChordTest.chrom[0]);
        
        processingFinished();
     
        
    }
    
   
   
   
    

    public void processingFinished()
    {
        ShowCord.text = detectedChord.getChordString();
    }
    
    private float[] getHammingWindow(int windowLength){
        float alpha = 0.54f;
        float beta = 1 - alpha;
        float [] window = new float[windowLength];

        for (int i = 0; i < windowLength; i++) {
            window[i] = alpha - beta * Mathf.Cos( (2*Mathf.PI * i) / (windowLength - 1));
        }

        return window;
    }

    void Fill(float[] collection, float fillnumber)
    {

        for (int i = 0; i < collection.Length; i++)
        {
            collection[i] = fillnumber;
        }
        
    }
}
