using System;
using System.Collections;
using System.Collections.Generic;
using Pitch;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioSense : MonoBehaviour
{
    private AudioSource _audioSource;

    private int keynum = 30;
    private int oldkeynum = 24;
    
    
    //Microphone input
    public AudioClip _audioClip;
    public bool _useMicrophone;
    public string _selectedDevice;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;
    public float decibelDetectionClosingValue = 25.0f;
    public float detectionOpeningValue = 0.005f;
    string selectedDevice;
    string[] micDevices;

    public NotesInteractionHandler NotesInteractionHandler;
   
    
    private PitchTracker _pitchTracker;
    FastYin fastYin;
    
    
    public PitchDetection pitchDetector;
    
    //Storing Samples
    public static float[] _samples = new float[512];
    public static float[] _freqband = new float[8];
    private float[] _freqBandHightest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] bufferSamples = new float[1024];
    
    //Used Stored Samples
    public static float[] samples = new float[2048];
    public static float[] samplesdB = new float[2048];
    public static float[] samplesVal = new float[2048];    
    public static float[,] samplesValStored = new float[512,3];
    float[] storedMaxVal = new float[3];
    float[] samplesSavedFreq = new float[10];
    FFTFreqBand[] samplesStored = new FFTFreqBand[10];
    
    //variables
    String note = "Note Display";
    float storeddB = 0.0f;
    float storeVal = 0.0f;
    float fundamentalFrequencyOutput = 0.0f;
    float freqperBand;

    private float rmsValue = 0.0f;
    private float dbValue = 0.0f;
    private float refValue;
    
    
    //debug Text

    public Text debugSpectrumSampleText;
    public Text debugFundamentalFrequency;
    public Text debugNoteID;
    public Text debugNoteKey;
    
    
    struct FFTFreqBand
    {
        public float Value;
        public float Index;
        public float Freq;

        public FFTFreqBand(float value, float index, float freq)
        {
            this.Value = value;
            this.Index = index;
            this.Freq = freq;
        }
    }
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        refValue = 0.001f;
        freqperBand = 24000.0f / samples.Length;
        _pitchTracker = new PitchTracker();
        _pitchTracker.SampleRate = 44100;
        
        
        fastYin = new FastYin(44100, 1024);
        
        
        _audioSource = GetComponent<AudioSource>();
        
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
       
       
       
       
        //AvgPitchCalc();
        //GetNoteName(); 
        
        //GetSpectrunAudioSourceMine();
        //MakeFrequencyBands();
        //CreateAudioBand();
    }

    
    void GetSpectrumAudioSource()
    {
        
        if (!Mathf.Approximately(_audioSource.clip.frequency, (float)_pitchTracker.SampleRate))
        {
            debugFundamentalFrequency.text = "upating sample rate to " + _audioSource.clip.frequency;
            _pitchTracker.SampleRate = _audioSource.clip.frequency;
        }
        
        
        
        
        //We are using hamming window because it reduces the sidelobes.
        //_audioSource.GetSpectrumData(bufferSamples, 0, FFTWindow.Blackman);

        var buffer = new float[1024];
        _audioSource.GetOutputData(buffer, 0);

        debugSpectrumSampleText.text = $"Spectrum: {buffer[20]} :: {buffer[120]}";
        
        
        //TODO -> jobify + burst
        
        
        var result = fastYin.getPitch(buffer);
            
        var pitch = result.getPitch();
        var midiNote = 0;
        var midiCents = 0;

        PitchDsp.PitchToMidiNote(pitch, out midiNote, out midiCents);
        
        ConvertMidiNoteToKey(pitch,midiNote,midiCents);

       
        
       /*

        _pitchTracker.ProcessBuffer(buffer);
        debugFundamentalFrequency.text = $"Pitch : {_pitchTracker.CurrentPitchRecord.Pitch}";
        debugNoteID.text = $"MidiNote : {_pitchTracker.CurrentPitchRecord.MidiNote}";
        debugNoteKey.text = $"MidiCent : {_pitchTracker.CurrentPitchRecord.MidiCents}";*/
        

       /*pitchDetector.Pitch = _pitchTracker.CurrentPitchRecord.Pitch; 
       pitchDetector.MidiNote = _pitchTracker.CurrentPitchRecord.MidiNote;
       pitchDetector.MidiCents = _pitchTracker.CurrentPitchRecord.MidiCents;*/


       /*
       pitchDetector.Pitch = pitch;
       pitchDetector.MidiNote = midiNote;
       pitchDetector.MidiCents = midiCents;
        */

      


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


    void ConvertMidiNoteToKey(float pitch, int midinote, int midicent)
    {
        
        debugFundamentalFrequency.text = $"Pitch : {pitch}";
        debugNoteID.text = $"MidiNote : {midinote}";
        string note = $"MidiCent : {PitchDsp.GetNoteName(midinote,true,true)}";

        debugNoteKey.text = note;
        NotesInteractionHandler.note = note;
        NotesInteractionHandler.noteID = midinote;

        // NotesInteractionHandler.PitchToPianoKey(note);




    }


    private void AvgPitchCalc()
    {
        DecibelDetectionClosing(decibelDetectionClosingValue);

        debugFundamentalFrequency.text = $"AvgPitchCalc method called";
        
        if (true)//ValueDetection(detectionOpeningValue)
        {
            //Checking all the samples and populate sampleStored with Frequency from 0 Hz - 8000 Hz
            //Using 512 samples and 11.7 Hz frequency band

            debugFundamentalFrequency.text = $"The ValuaDetection if-condition";
            
            for (int i = 0; i < (samples.Length / 4); ++i)
            {
                //Get the value of a sample
                float v = samples[i];

                //high-pass filter
                if (v > 0.001f)
                {
                    //Iterate though saved values
                    float itFreq = i * freqperBand; //frequency of 512 sample multiply by the frequency band of 11.7
                    FFTFreqBand itV = new FFTFreqBand(v, i, itFreq);

                    InsertArray(samplesStored, samplesStored.Length, itV);

                    debugFundamentalFrequency.text = $"InsertArray has returned with sampleStored : {samplesStored[5]}";
                }
            }

            if (samplesStored[0].Freq > 27.5f) //we use 27.5hz since it's the lowest frequency a piano can make.
            {

                debugFundamentalFrequency.text = $"sampleStored[0] > 27.5 if-statement";
                
                FFTFreqBand itVal = new FFTFreqBand(0.0f, 0.0f, 0.0f);

                //Delete the frequency samples that their band indexs are adjecent based on their lowest value                
                for (int i = 0; i < samplesStored.Length; i++)
                {
                    itVal = samplesStored[i];

                    debugFundamentalFrequency.text = $"Delete loop with sample value : {itVal}";
                    
                    for (int j = 0; j < samplesStored.Length; j++)
                    {
                        //Don't compare the iterated frequency with itself.
                        if (i != j)
                        {
                            //Look for adjecent frequency bands
                            if (itVal.Index + 1 == samplesStored[j].Index || itVal.Index - 1 == samplesStored[j].Index)
                            {
                                //Delete the band with less value than the other.
                                if (itVal.Value > samplesStored[j].Value)
                                {
                                    samplesStored[j] = new FFTFreqBand(0.0f, 0.0f, 0.0f);

                                    debugFundamentalFrequency.text = $"Deleted sample value at index: {j}";
                                }
                            }

                        }
                    }
                    samplesStored[i] = itVal;
                }

                //Save the frequencies in another list for optimized calculations
                for (int i = 0; i < samplesStored.Length; i++)
                {
                    samplesSavedFreq[i] = samplesStored[i].Freq;
                }

                fundamentalFrequencyOutput = 4186.0f;
                FFTFreqBand fundamentalSample = new FFTFreqBand(0.0f, 0.0f, 0.0f);
                
                for (int i = 0; i < samplesSavedFreq.Length; i++)
                {
                    //Get the lowest frequency detected and save it.
                    if (samplesSavedFreq[i] > 0.0f && samplesSavedFreq[i] < fundamentalFrequencyOutput)
                    {
                        fundamentalFrequencyOutput = samplesSavedFreq[i];

                        debugFundamentalFrequency.text = $"Fundamental Freq - {fundamentalFrequencyOutput}";
                    }
                }
            }
        }
        else
        {
            //avgPitchDisplay.text = "AVGPitch";            
            samplesStored = new FFTFreqBand[10];
        }
        
        /*TODO
         Note:
         
         From AvgPitchCalc, the return result or output are 
         1. The lowest fundamental frequency output
         2. A sample Frequency of size 10 and it is also a float data type
         3. A filled array of FFTFrequencyBand which content of 
            a. values of sample
            b. index of the FFT Frequency Band
            c. The Frequency itself 
         */
    }
     
     private void GetNoteName()
    {
        //This are the frequencies of the piano TODO - 27.5 Hz to 4186.0 Hz
        
        //We set it to 246 since it's the lowest frequency we are looking 
        if (fundamentalFrequencyOutput > 246.5f)
        {
            note = null;
            //Relate pitch to musical notation with harmonics.
            
            for (int i = 0; i < keynum; i++) //Check from Do 261.6freq to Do 522freq
            {
                //Get the pitch of the note we are comparing
                float testPitchIterator = Mathf.Pow(2.0f, i / 12.0f) * 261.6f;

                //Get variable tolerance
                //tolerance[0] is the difference of testPitch and testPitch-1
                //tolernace[1] is the difference of testPitch+1 and testPitch
                float[] tolerance = GetTolerance(i);

                //Identify the most low freq and look for its musical notation.
                if (fundamentalFrequencyOutput > testPitchIterator - tolerance[0])
                {
                    if (fundamentalFrequencyOutput < testPitchIterator + tolerance[1])
                    {
                        //Check its harmonics
                        for (int j = 0; j < samplesSavedFreq.Length; j++)
                        {
                            if (samplesSavedFreq[j] > 0.0f)
                            {
                                note = HarmonicDetection(samplesSavedFreq[j], testPitchIterator, tolerance, 2.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    NotesInteractionHandler.keyID = i;
                                   // NotesInteractionHandler.PressKey(i);
                                    

                                    debugNoteID.text = $"Note ID: {NotesInteractionHandler.noteID}\n" +
                                                       $"Index: {i}";
                                    
                                    debugNoteKey.text = $"Key Note: {note.ToString()}";
                                    
                                    break;
                                }
                                
                                note = HarmonicDetection(samplesSavedFreq[j], testPitchIterator, tolerance, 3.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    NotesInteractionHandler.keyID = i;
                                   // NotesInteractionHandler.PressKey(i);
                                    debugNoteID.text = $"Note ID: {NotesInteractionHandler.noteID}\n" +
                                                       $"Index: {i}";
                                    debugNoteKey.text = $"Key Note: {note.ToString()}";
                                    
                                    break;
                                }
                                
                                note = HarmonicDetection(samplesSavedFreq[j], testPitchIterator, tolerance, 4.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    NotesInteractionHandler.keyID = i;
                                    //NotesInteractionHandler.PressKey(i);
                                    debugNoteID.text = $"Note ID: {NotesInteractionHandler.noteID}\n" +
                                                       $"Index: {i}";
                                    
                                    debugNoteKey.text = $"Key Note: {note.ToString()}";
                                    
                                    break;
                                }
                            }
                        }
                    }
                }
                if (note != null) break;
            }
        }

        note = null;
        fundamentalFrequencyOutput = 0;
        
        
        /*TODO Note
         
         In the GetNoteName method
         
         1. We are going through all the key notes from C4 to B5 (There standard frequency)
         2. We will get the tolerance level, which is usually 18 Hz
         3. We compare the fundamental frequency (The lowest frequency from the note)
            with the key note and its 18 Hz offset tolerance
         4. If the fundamental frequency falls in between this range of the key note tolerance,
            then we continue to evaluate the Key note 
         5. Next we loop through all the content of the sample frequencies of 10 band  
         6. We pass in the key note, each content of the sample frequencies, tolerance array
            and harmonics number into the Harmonic detector method
         7. Then we go to Harmonic detector method to detect or confirm our key note   
         
         TODO Rememeber
         
         The key note frequency is actually the fundamental frequency
         */
    }
    
    
    private bool ValueDetection(float value)
    {
        int i = 0;
        float maxVal = 0;
        
        for (i = 0; i < (samples.Length / 4); i++)
        {
            //Get Highest Value    
            if (samples[i] > maxVal)
            {
                maxVal = samples[i];

            }
        }
        
        //Debug.Log(maxVal);
        if (maxVal < 0.00001) maxVal = 0; // clamp it to 0 val
        if (maxVal > value)
        {
            if (storedMaxVal[0] > maxVal && storedMaxVal[1] > maxVal && storedMaxVal[2] > maxVal)
            {
                //Debug.Log(maxVal + " ------------------------------------------------------Accepted Opening");
                for (int y = 0; y < 512;y++)
                {
                    samples[y] = samplesValStored[y,0];
                }
                storedMaxVal = new float[3];

                return true;                                                   
            }
            else
            {
                for (int it = 0; it < storedMaxVal.Length; it++)
                {
                    if (storedMaxVal[it] < maxVal)
                    {
                        storedMaxVal[it] = maxVal;
                        for (i = 0; i < (samples.Length / 4); i++)
                        {
                            samplesValStored[i, it] = samples[i];
                        }
                        break;
                    }
                }
                storeVal = maxVal;
                return false;
            }
        }
        return false;
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
                NotesInteractionHandler.noteID = -1;
                NotesInteractionHandler.keyID = -1;
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
    
    void InsertArray(FFTFreqBand[] sampleArr, int size, FFTFreqBand sample)
    {
        List<FFTFreqBand> store = new List<FFTFreqBand>();
        for (int i = 0; i < sampleArr.Length; i++)
        {
            //Is the new value higher than the value from the list?
            if (sample.Value < sampleArr[i].Value)
            {
                //Save the value from the original list
                store.Add(sampleArr[i]);
            }
            else
            {
                //Save the new value
                store.Add(sample);
                //Save the remaining part of the list
                for (int j = i; j < size; j++)
                {
                    //As long as it's not empty
                    if (sampleArr[j].Value > 0.0f) { store.Add(sampleArr[j]); }
                    else { break; }
                }
                break;
            }
        }
        int count = 0;
        foreach (FFTFreqBand ret in store)
        {
            if (count < size)
            {
                sampleArr[count] = ret;
                count++;
            }
            else
                break;
        }
    }
    
    private float[] GetTolerance(int index)
    {
        float[] ret = { 0.0f, 0.0f };
        float testPitchIterator = Mathf.Pow(2.0f, index / 12.0f) * 261.6f;
        float testNoteIteratorBelow = Mathf.Pow(2.0f, (index - 1) / 12.0f) * 261.6f;
        float testNoteIteratorAbove = Mathf.Pow(2.0f, (index + 1) / 12.0f) * 261.6f;
        ret[0] = testPitchIterator - testNoteIteratorBelow; //Minus Tolerance
        ret[1] = testNoteIteratorAbove - testPitchIterator; //Plus Tolerance


        return ret;
    }

    private String HarmonicDetection(float sample, float testNoteIterator, float[] tolerance, float harmonicNumber)
    {
        float harmPitch = testNoteIterator * harmonicNumber;
        if (sample > harmPitch - tolerance[0])
        {
            if (sample < harmPitch + tolerance[1])
            {
                //Debug.Log("Harmonic number "+ harmonicNumber + "of " + pitchToNote(testNoteIterator)  + " Detected!");
                return pitchToNote(testNoteIterator);
            }
        }
        return null;
        
        
        /*TODO Note
         
         In the HarmonicDetection method
         
         1. We want to check if the key note frequency has a 2nd, 3rd or 4th Harmonic from the Sample frequencies.
         2. If it does, then we proceed to identify the particular key on the keyboar using pitchToNotes method
         3. We simply pass the Key note frequency to the pitchToNote method.
         
         */
        
        
        
    }

    private String pitchToNote(float pitch)
    {
        String[] musicalNotes = new string[] { "C4","C_4","D4","D_4","E4","F4","F_4",
            "G4","G_4","A4","A_4","B4","C5","C_5","D5","D_5","E5","F5","F_5","G5","G_5",
            "A5","A_5","B5","C6","C_6","D6","D_6","E6","F6"};
        
        
        for (int i = 0; i < keynum; i++)
        {
            float testNoteIterator = Mathf.Pow(2.0f, i / 12.0f) * 261.6f;
            float[] tolerance = GetTolerance(i);
            if (pitch > testNoteIterator - tolerance[0])
            {
                if (pitch < testNoteIterator + tolerance[1])
                {
                    return musicalNotes[i];
                }
            }
        }
        return null;
        
        /*TODO Note
         
         In the pitchToNote method
         
         1. We kind of repeat how we found the Key note frequency initially by comparing it again
            with 24 key note frequencies and it tolerance range.
         2. Once it satisfies the tolerance range, then we get the Index of the Key note of the array.
         3. Using the index, we can then retrieve the key from the array of keys.    
         
         */
        
        
    }

    private void DebugAudioFile()
    {
        Debug.Log(samples.ToString());
        float samplespersecond = _audioClip.samples / _audioClip.length;
        Debug.Log(samplespersecond.ToString());
        Debug.Log(_audioClip.samples.ToString());
    }
    
    public void DropDownValueChanged(int value)
    {
        Microphone.End(selectedDevice);
        _audioSource.Stop();
        _audioSource.clip = null;


        _audioSource.clip = Microphone.Start(micDevices[value], true, 600, AudioSettings.outputSampleRate);
        selectedDevice = micDevices[value];
        _audioSource.Play();
    }
    
    
    void CreateAudioBand()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqband[i] > _freqBandHightest[i])
            {
                _freqBandHightest[i] = _freqband[i];
            }

            _audioBand[i] = (_freqband[i] / _freqBandHightest[i]);
        }
    }
    void GetSpectrunAudioSourceMine()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }
    void MakeFrequencyBands()
    {
        /*
         * 22050 / 512 = 43 hertz per sample
         *
         * 20 - 60 Hz
         * 60 - 250 Hz
         * 250 - 500 Hz
         *
         *
         *
         * 0 - 2 = 86 Hz
         * 1 - 4 = 172 Hz - 87 - 258
         * 2 - 8 = 344 Hz - 259 - 602
         * 3 - 16
         * 4 - 32
         * 
         * 
         */


        int count = 0;

        for (int i = 0; i < 8; i++)
        {

            float average = 0;
            int sampleCount = (int) Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;

            _freqband[i] = average * 10;
        }

        {
            
        }
    }
   

}
