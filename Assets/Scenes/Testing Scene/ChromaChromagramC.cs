using System;
using System.Collections;
using System.Collections.Generic;
using NAudio.Dsp;
using UnityEngine;

public class ChromaChromagramC
{

    private float referenceFrequency = 110.0f;
    private int bufferSize = 8192;
    private int frameSize = 8192;
    
    private static int chromagramNumHarmonics = 3;
    private static int chromagramNumOctaves = 2;
    private static int chromagramNumBinsToSearch = 3;
    
    private int numHarmonics = 3;
    private int numOctaves = 2;
    private int numBinsToSearch = 3;
    private int SEMITONES = 12;
    private float[] noteFrequencies;
    
    int samplingFrequency;
    int inputAudioFrameSize;
    int downSampledAudioFrameSize;
    int numSamplesSinceLastCalculation;
    int chromaCalculationInterval;
    bool chromaReady;
    
    private float[] chromagram;
    private float[] magnitudeSpectrum;
    float[] window;
    float[] buffer;
    

    private float[] audioBuffer;
    
    float[] downsampledInputAudioFrame;
    private int chromagramAlgorithm;


    public ChromaChromagramC()
    {
        noteFrequencies = new float[12];
        chromagram = new float[12];
        bufferSize = 8192;
        
     
         numHarmonics = 3;
         numOctaves = 2;
         numBinsToSearch = 3;
        
        
        
        
        
        // calculate note frequencies
        for (int i = 0; i < 12; i++)
        {
            noteFrequencies[i] = referenceFrequency * Mathf.Pow (2,(((float) i) / 12));
        }
        
        // initialise chromagram
        for (int i = 0; i < 12; i++)
        {
            chromagram[i] = 0.0f;
        }
        
        
    }

    public ChromaChromagramC(float[] SpectrumData, int samplingFreq, int bufsize)
    {
        referenceFrequency = 110.0f;
        noteFrequencies = new float[12];
        chromagram = new float[12];
        bufferSize = 8192;
        frameSize = 8192;
        numHarmonics = 3;
        numOctaves = 2;
        numBinsToSearch = 3;
        
        
        audioBuffer = SpectrumData;
        
        samplingFrequency = samplingFreq;
        bufferSize = bufsize;
        
        // calculate note frequencies
        for (int i = 0; i < 12; i++)
        {
            noteFrequencies[i] = referenceFrequency * Mathf.Pow (2,(((float) i) / 12));
        }
        
        // initialise chromagram
        for (int i = 0; i < 12; i++)
        {
            chromagram[i] = 0.0f;
        }
        
    }



    public void Chromagram(int frameSize, int sampleFreq, int bufferSize, int numHarmonics, int numOctaves, int numBinsToSearch)
    {
        
        // :  referenceFrequency(55.0)          // A1
        // referenceFrequency(110.0)         // A2
// :  referenceFrequency(130.81278265)  // C2
        
        referenceFrequency = 110.0f; //55.0 110.0 130.81278265
        this.bufferSize = 8192;
       this.frameSize = 8192;
       this.numHarmonics = 3;
       this.numOctaves = 2;
       this.numBinsToSearch = 3;
        
        sampleFreq = 44100;
        samplingFrequency = sampleFreq;
        // setup magnitude spectrum vector
        magnitudeSpectrum = new float[((this.bufferSize/2) + 1)];
        
        noteFrequencies = new float[12];
        chromagram = new float[12];
        // set buffer size
        buffer = new float[this.bufferSize];
        
        // calculate note frequencies
        for (int i = 0; i < 12; i++)
        {
            noteFrequencies[i] = referenceFrequency * Mathf.Pow (2,(((float) i) / 12));
        }
        
        // initialise chromagram
        for (int i = 0; i < 12; i++)
        {
            chromagram[i] = 0.0f;
        }
        
        // set input audio frame size
        setInputAudioFrameSize(this.frameSize);
    
        // initialise num samples counter
        numSamplesSinceLastCalculation = 0;
    
        // set chroma calculation interval (in samples at the input audio sampling frequency)
        chromaCalculationInterval = 4096;
    
        // initialise chroma ready variable
        chromaReady = false;
        

    }
    
   public  void setInputAudioFrameSize(int frameSize) {
        inputAudioFrameSize = frameSize;
    
        downsampledInputAudioFrame = new float[(inputAudioFrameSize / 4)];
    
        downSampledAudioFrameSize = (int) downsampledInputAudioFrame.Length;
    }

   public  void setSamplingFrequency(int sampleRate)
    {
        samplingFrequency = sampleRate;
    }

    public void setChromaCalculationInterval(int numSamples) {
        chromaCalculationInterval = numSamples;
        chromaCalculationInterval = 4096;
    }
    
    
    public void setChordDetectionAlgorithm(int algorithm) {
        switch(algorithm) {
            case 0:
                chromagramAlgorithm = 0;
                break;
            case 1:
                chromagramAlgorithm = 1;
                break;
            case 2:
            default:
                chromagramAlgorithm = 2;
                break;
        }
    }
    
    
   public void setMagnitudeSpectrum(float[] spectrumSamples) {
        for (int i = 0; i < bufferSize / 2; i++) {
            magnitudeSpectrum[i] = spectrumSamples[i];
        }
    }
    
    
    public void processAudioFrame(int inputAudioFrame)
    {
        // create a vector

        chromagramAlgorithm = inputAudioFrame;

        if (chromagramAlgorithm == 0)
        {
            calculateChromagramFromAdamStarkAlgorithm();
        }else if (chromagramAlgorithm == 1)
        {
           chromagramEversongAlgorithm1(); 
        }
        else if(chromagramAlgorithm == 2)
        {
            chromagramEversongAlgorithm2();
        }

        
    }

    private void calculateChromagramFromAdamStarkAlgorithm()
    {
        float divisorRatio = (((float) samplingFrequency) / 4.0f) / ((float)bufferSize);
        float sumOfAllSemitones = 0.0f;

        for (int n = 0; n < SEMITONES; n++) {
            float chromaSum = 0.0f;

            for (int octave = 1; octave <= numOctaves; octave++) {
                float noteSum = 0.0f;

                for (int harmonic = 1; harmonic <= numHarmonics; harmonic++) {
                    int centerBin = (int)Math.Round((noteFrequencies[n] * octave * harmonic) / divisorRatio);
                    int minBin = centerBin - (numBinsToSearch * harmonic);
                    int maxBin = centerBin + (numBinsToSearch * harmonic);

                    float maxVal = 0.0f;

                    for (int k = minBin; k < maxBin; k++) {
                        if (magnitudeSpectrum[k] > maxVal) {
                            maxVal = magnitudeSpectrum[k];
                        }
                    }

                    noteSum += (maxVal / (float) harmonic);
                }

                chromaSum += noteSum;
            }
            sumOfAllSemitones += chromaSum;
            chromagram[n] = chromaSum;
        }
        applyChromagramAmplitudeThreshold(sumOfAllSemitones);
        chromaReady = true;
    }
    
    public float[] GetChromagram()
    {
        return chromagram;
    }
    
    
    
    private void applyChromagramAmplitudeThreshold(float sumOfAllSemitones)
    {
        
    } 
    
    
    
    void chromagramEversongAlgorithm1() {
    
        float maxChromagramValue = 0.0f;
    
        float sumOfAllSemitones = 0.0f;

    
        
        for (int n = 0; n < SEMITONES; n++) {
        
            float chromaSum = 0.0f;

        
            for (int octave = 1; octave <= numOctaves; octave++) {
            
                float noteSum = 0.0f;
            
                //TODO check this 7 semitones shifting (n + 7)
            
                int noteChecking = (int)Math.Round(noteFrequencies[(n + 7) % SEMITONES] * octave);
            
                int binWidth = (int)Math.Round((double) (noteChecking * (samplingFrequency / 360448)));

           
                for (int i = noteChecking - ((binWidth - 1) / 2); i <= noteChecking + ((binWidth - 1) / 2); i++) {
                
                    noteSum += magnitudeSpectrum[i];
                    
                }
            
                chromaSum += noteSum;
            
            
            }

        
            if (maxChromagramValue < chromaSum) {
            
                maxChromagramValue = chromaSum;
                
            }

       
            sumOfAllSemitones += chromaSum;
       
            chromagram[n] = chromaSum; 
        }

    
        applyChromagramAmplitudeThreshold(sumOfAllSemitones);
    
        chromaReady = true;
}


    void chromagramEversongAlgorithm2() {
    
        float divisorRatio = (((float) samplingFrequency) / 4.0f) / ((float)bufferSize);
    
        float sumOfAllSemitones = 0.0f;

    
        for (int n = 0; n < SEMITONES; n++) {
        
            float chromaSum = 0.0f;

        
            for (int octave = 1; octave <= numOctaves; octave++) {
            
                float noteSum = 0.0f;

            
                for (int harmonic = 1; harmonic <= numHarmonics; harmonic++) {
                
                    int centerBin = (int)Math.Round((noteFrequencies[n] * octave * harmonic) / divisorRatio);
                
                    int minBin = centerBin - (numBinsToSearch * harmonic);
                
                    int maxBin = centerBin + (numBinsToSearch * harmonic);

                
                    float sumVal = 0.0f;

                
                    for (int k = minBin; k < maxBin; k++) {
                    
                        sumVal += magnitudeSpectrum[k];
                        
                    }
                
                    sumVal /= (maxBin - minBin);

                
                    noteSum += (sumVal / (float) harmonic);
                    
                }

            
                chromaSum += noteSum;
                
            }
        
            
            sumOfAllSemitones += chromaSum;
        
            chromagram[n] = chromaSum;
            
        }
    
        applyChromagramAmplitudeThreshold(sumOfAllSemitones);
    
        chromaReady = true;
}
    
    
   
    
    
    
    



}
