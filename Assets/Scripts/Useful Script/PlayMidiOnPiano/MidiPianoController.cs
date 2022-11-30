using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MidiPianoController : MonoBehaviour
{

    public static MidiPianoController Instance;

    public Text SongName;
    public Button backButton;
    public Button replayButton;
    
    // Midi file converter and tools variabels
    private MidiFileInspector _midi; 
    MidiNote[] MidiNotes;

    public bool testMode = false;
    
    //Sheet lanes

    public SheetLaneScript[] lanes;

    public GameObject loadingNotice;
    
    
    // Midi file info variable
    private string midiReceivedName;
    public string midifilename;


    private int _noteIndex = 0;
    private double _timer = 0;
    public double GlobalSpeed = 1;
    public float noteSpeed = 2.5f;
    public static float noteSpeednum;

    public static float NoteLength = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
        backButton.onClick.AddListener(() => { SceneManager.LoadScene(ConstantStrings.SongSelection);});
        replayButton.onClick.AddListener(() => { _noteIndex = 0;});
        
        replayButton.gameObject.SetActive(false);
        
        //Instantiate the class to be use everywhere
        Instance = this;
        MidiNoteReceptor.shouldmove = false;

        NoteLength = 0f;
        
        //Receive file name from Song selection scene and extract the midi
        ReceiveAndExtractMidi();
    }

    // Update is called once per frame
    void Update()
    {
        noteSpeednum = noteSpeed;
        InstatiateNotes();
    }



    void InstatiateNotes()
    {
        if (_midi != null && MidiNotes.Length > 0 && _noteIndex < MidiNotes.Length)
        {
            _timer += Time.deltaTime * GlobalSpeed * 10 * (float)MidiNotes[_noteIndex].Tempo;
            
            if (true)
            {

               // MidiNotes[_noteIndex].StartTime < _timer
                
                double tempo = MidiNotes[_noteIndex].Tempo;
                //Send the note to the Lane Scripts
               
                lanes[0].SetandInstantiatePrefrab(MidiNotes[_noteIndex].Note,tempo);
                lanes[1].SetandInstantiatePrefrab(MidiNotes[_noteIndex].Note,tempo);
                NoteLength += MidiNotes[_noteIndex].Length; //(float)(MidiNotes[_noteIndex].StartTime / MidiNotes[_noteIndex].Tempo); //MidiNotes[_noteIndex].Length;
                
                
                //MidiNoteReceptor.note = MidiNotes[_noteIndex].Note;
                
                

                _noteIndex++;
                
                loadingNotice.SetActive(true);
            }
        }

        if (_noteIndex >= MidiNotes.Length)
        {
            MidiNoteReceptor.shouldmove = true;
            
            loadingNotice.SetActive(false);
            
            
        }
    }



    void ReceiveAndExtractMidi()
    {
#if UNITY_ANDROID //&& !UNITY_EDITOR


        if (testMode)
        {
            midiReceivedName = midifilename;
        }
        else
        {
         
            midiReceivedName = SongSelectionScript.midifiledir;
        }

        
        SongName.text = midiReceivedName;

        //Get the midifile name from Songg Selection Script and extract the midi notes from the file
       
        string midifilePath = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, midiReceivedName);
        
        

        string oriPath = midifilePath;
  
        // Android only use WWW to read file
        WWW reader = new WWW(oriPath);
        while ( ! reader.isDone) {}
  
        string realPath = Application.persistentDataPath + "/db";
        System.IO.File.WriteAllBytes(realPath, reader.bytes);
  
        string dbPath = realPath;
        ExtractMidiNotes(dbPath);


#endif
        
        
#if UNITY_EDITOR
        
        
        TestExtractingMidiNote();
        
        
#endif
    }


    void ExtractMidiNotes(string path)
    {
        _midi = new MidiFileInspector(path);
        MidiNotes = _midi.GetNotes();
        
        
    }
    
    void TestExtractingMidiNote()
    {
        SongName.text = midifilename;
        string path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, midifilename);
        
        _midi = new MidiFileInspector(path);
        MidiNotes = _midi.GetNotes();

    }

    public static double GetAudioTime()
    {
        double timeD = 0;
        

        return timeD;
    }


    private void OnDisable()
    {
       // MidiNoteReceptor.shouldmove = false;
    }
}
