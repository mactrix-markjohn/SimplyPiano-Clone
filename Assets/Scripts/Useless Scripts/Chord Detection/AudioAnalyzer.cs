using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioAnalyzer : MonoBehaviour
{
    
    protected int samplingFrequency;
    protected int frameShift;
    protected int frameLength;
    protected int head;
    protected int atFrame;
    protected int maxFrames;
   protected AudioData audioData;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
   public abstract void process(AudioData audioData);
    public abstract void processingFinished();
    
    
    public static float[] shortToFloat(short[] audio) {
        float[] output = new float[audio.Length];
        for (int i = 0; i < output.Length; i++) {
            output[i] = (float) audio[i] / 32768f;
        }
        return output;
    }

    public static double[] shortToDouble(short[] audio) {
        double[] output = new double[audio.Length];
        for (int i = 0; i < output.Length; i++) {
            output[i] = (double)audio[i] / 32768;
        }
        return output;
    }
    
    
    //TODO this is similar to extracting output sample data from Audiosource
    
    protected double[] getNextFrame()
    {
        double[] outputBuffer;
        if(atFrame <= maxFrames){
            atFrame ++;
            if(head + frameLength > audioData.length()){
                //zero pad the end;

                outputBuffer = new double[audioData.length()];
                for (int i = head; i < audioData.length(); i++)
                {

                    outputBuffer[i] = audioData.audioBuffer[i];
                }

                /*outputBuffer = (Array.Copy(Arrays.copyOfRange(audioData.audioBuffer,head,audioData.length()-1), frameLength)).clone();*/
                
                head = audioData.length()-1;
                return outputBuffer;
            } else {
                //get regular frame
                
                outputBuffer = new double[head+frameLength+1];
                for (int i = head; i < head+frameLength+1; i++)
                {

                    outputBuffer[i] = audioData.audioBuffer[i];
                }
                
                
                /*outputBuffer = Arrays.copyOfRange(audioData.audioBuffer,head,head+frameLength);*/
                
               head = head+frameShift-1;
                return outputBuffer;
            }

        } else {
            //return null to signal that the end is reached
            return null;
        }
        
    }
    
}
