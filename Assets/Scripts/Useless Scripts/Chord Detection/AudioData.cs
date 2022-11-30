using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    
    public float[] audioBuffer;
    public int samplingFrequency;
    
    
    // Start is called before the first frame update
    void Start()
    {
     
        audioBuffer = new float[1024];
    }

    // Update is called once per frame
    void Update()
    {
        GetAudioSource.AudioSource.GetSpectrumData(audioBuffer,0, FFTWindow.Hamming);
        samplingFrequency = 44100;
    }
    
    
    public void  AudioDataStart(double[] aBuf , int fs){
       // audioBuffer =(double[]) aBuf.Clone();
        //samplingFrequency = fs;
    }
    
    public int length(){
        return audioBuffer.Length;
    }

    public float getSignalPower(){
        float acc = 0;
        float normalized;
        for (int i = 0; i < audioBuffer.Length; i++) {
            normalized = ((((float) audioBuffer[i] / 32768)) )*10;//values are really low without the *10! yup. dirty hack.
//	        Log.d("yarro", Double.toString(normalized));
            acc +=  normalized * normalized;
        }
        return acc / (float) audioBuffer.Length;
    }
}
