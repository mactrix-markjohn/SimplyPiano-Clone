using System;
using System.Collections;
using System.Collections.Generic;
using Pitch;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioSensor : MonoBehaviour
{
    
    private AudioSource _audioSource;
    public static AudioSource Audsou;

    private int keynum = 30;
    private int oldkeynum = 24;
    
    
    //Microphone input
    public AudioClip _audioClip;
    public bool _useMicrophone;
    public string _selectedDevice;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;
    string selectedDevice;
    string[] micDevices;
    
    //detect the loudness of the sound;
    public static float[] samplesdB = new float[1024];
    public float decibelDetectionClosingValue = 25.0f;
    
    
    private PitchTracker _pitchTracker;
    FastYin fastYin;


    //Used Stored Samples
    public static float[] samples = new float[2048];
   
    
    //variables
    String note = "Note Display";
    float storeddB = 0.0f;
    float storeVal = 0.0f;
    float fundamentalFrequencyOutput = 0.0f;
    
    private float rmsValue = 0.0f;
    private float dbValue = 0.0f;
    private float refValue;
    

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        refValue = 0.001f;


        _pitchTracker = new PitchTracker();
        _pitchTracker.SampleRate = 44100;
        
        
        fastYin = new FastYin(44100, 1024);
        
        
        _audioSource = GetComponent<AudioSource>();
        Audsou = _audioSource;
        
        // Microphone input

        if (_useMicrophone)
        {
            

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }


            if (Microphone.devices.Length > 0)
            {
                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;
                _audioSource.clip = Microphone.Start(Microphone.devices[0], true, 600, 44100);
                _audioSource.Play();
            }
            
            
            
        }else if (!_useMicrophone)
        {
            _audioSource.outputAudioMixerGroup = _mixerGroupMaster;
            _audioSource.clip = _audioClip;
            //play the audioSource clip
            _audioSource.Play();
        }




        
    }

   // Update is called once per frame
    void Update()
    {
        
        GetSpectrumAudioSource();
       
        
    }

    
    void GetSpectrumAudioSource()
    {
        
        
        
        if (!Mathf.Approximately(_audioSource.clip.frequency, (float)_pitchTracker.SampleRate))
        {
            _pitchTracker.SampleRate = _audioSource.clip.frequency;
        }
        
        
        
        
        //We are using hamming window because it reduces the sidelobes.
        //_audioSource.GetSpectrumData(bufferSamples, 0, FFTWindow.Blackman);

        var buffer = new float[1024];
        _audioSource.GetOutputData(buffer, 0);




        //TODO -> jobify + burst
        
        
        var result = fastYin.getPitch(buffer);
            
        var pitch = result.getPitch();
        var midiNote = 0;
        var midiCents = 0;

        PitchDsp.PitchToMidiNote(pitch, out midiNote, out midiCents);
        
        ConvertMidiNoteToKey(pitch,midiNote,midiCents);



        /*TODO Note
          
          This simply return a float array of sample of the song on real time with the size of 2048
          index 1     = 0 Hz
          index 2048  = 24000 Hz
          
          The Frequency band is givens 
          
          24000 Hz / 2048 = 11.7 Hz
          
          This calculation simply make it easy to compute by reducing the frequency from 24000 to 2048. 
          
          We can at least work with 2048 frequencies.
          */
    }
    
    //Mic Detection is in charge of start/stop the pitch algorithm
    private bool DecibelDetectionClosing(float closingdBs)
    {
        //Debug.Log("Current dB value " + dbValue);
        _audioSource.GetOutputData(samplesdB, 0); // fill array with samples
        int i = 0;
        float sum = 0;
        for (i = 0; i < samplesdB.Length; i++)
        {
            sum += samplesdB[i] * samplesdB[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / samplesdB.Length); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
        if (dbValue < closingdBs)
        {
            if (storeddB < dbValue)
            {
                //Debug.Log(dbValue + " Accepted Closing");
                storeddB = 0.0f;
                return true;
            }
            else
            {
                storeddB = dbValue;
                return false;
            }
        }


        return false;
    }


    void ConvertMidiNoteToKey(float pitch, int midinote, int midicent)
    {
        
      
        string note = PitchDsp.GetNoteName(midinote,true,true);

        NoteReceptor.note = note;
        NoteReceptor.noteID = midinote;
        NoteReceptor.pitch = pitch;



    }
}
