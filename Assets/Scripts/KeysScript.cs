using System.Collections;
using System.Collections.Generic;
using MidiJack;
using UnityEngine;
using UnityEngine.UI;

public class KeysScript : MonoBehaviour
{

    public int index;
    public string notename;
    public GameObject C4K;
    public AudioClip audioclip;
    private AudioSource _audioSource;
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AttachListenerToButton();
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(Activate());

        
#if UNITY_ANDROID


        
        MidiKeyIndicate();

        //check if the midi cable is connected to the phone, if yes, then use the MidiKeyIndicate method
        /*if (!MidiNoteReceptor.isMicrophonePitchDetectionUsed)
        {
            MidiKeyIndicate();

        }
        else
        {
            PlayOrIndicateNote(); 
        }*/
        


#endif
        


    }


    #region Midi Key Indicator
    
    
    
    void MidiKeyIndicate()
    {

        int noteNumber = index + 21;
       
        
        if (MidiMaster.GetKeyDown(noteNumber) )
        {

            MidiNoteReceptor.midiNoteID = index;
            
            OnMidiIndicate();
            
            //_ShowAndroidToastMessage($"Piano Key: {index}");
            
        }
        else
        {
            OffMidiIndicate(); 
        }



        


        float vel =  MidiMaster.GetKey(0,noteNumber);


        if (vel >= 1f)
        {
            
            OnMidiIndicate();

            
        }
        else
        {
           OffMidiIndicate();
           
           
        }
        
       
        
        
        
    }


    void OnMidiIndicate()
    {
        C4K.SetActive(true);
    }

    void OffMidiIndicate()
    {
        C4K.SetActive(false);
        
       
    }
    
    IEnumerator OffMidiIndicateCoroutineCall()
    {
        
        yield return new WaitForSeconds(0.2f);
                
        C4K.SetActive(false);
        
    }


    #endregion

    
    


    void PlayOrIndicateNote()
    {

      
            if (MidiNoteReceptor.isMidiSongNoteRecieved)
            {

                if (MidiNoteReceptor.note != null)
                {
                    
                    if (MidiNoteReceptor.note.Equals(notename))
                    {
                    
                        //indicate the note and also play the audioclip
                        IndicateNote();
                        PlayNote();
                    
                    }
                    
                }
                
            }else if (NoteReceptor.isMicrophoneNoteReceived)
            {


                int calcNoteID = (NoteReceptor.noteID - 20) + 1;

                if (calcNoteID == index)
                {
                    IndicateNote();
                }


            }else if (MidiNoteReceptor.isJustIndicate)
            {
                if (MidiNoteReceptor.noteID == index)
                {
                    IndicateNote();
                }
            }else if (MidiNoteReceptor.isChordIndicate)
            {
                
                if ((MidiNoteReceptor.chordID1 == index)||(MidiNoteReceptor.chordID2 == index)
                                                        ||(MidiNoteReceptor.chordID3 == index)||(MidiNoteReceptor.chordID4 == index))
                {
                    IndicateNote();
                }
            }


    }


    void IndicateNote()
    {

        StartCoroutine(IndicateKey());

    }

    void PlayNote()
    {
        _audioSource.clip = audioclip;
        _audioSource.Play();
    }

    void AttachListenerToButton()
    {
        GetComponent<Button>().onClick.AddListener(() => { PressKey();});
    }

    void PressKey()
    {
        if (MidiNoteReceptor.isPianoKeyActiveToClick)
        {
            _audioSource.clip = audioclip;
            _audioSource.Play();
        }
    }

    IEnumerator IndicateKey()
    {
        
        C4K.SetActive(true);
                
                
                
        yield return new WaitForSeconds(0.2f);
                
        C4K.SetActive(false);
        
    }

    IEnumerator Activate()
    {
        if (NotesInteractionHandler.noteID >= 59 && NotesInteractionHandler.noteID <= 89)
        {
            int noteIndex = (NotesInteractionHandler.noteID - 60) + 1;

            if (index == noteIndex)
            {
                C4K.SetActive(true);
                
                
                
                yield return new WaitForSeconds(0.2f);
                
                C4K.SetActive(false);
                
            }

            
        }
    }
    
    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass =
                new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity,
                        message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
