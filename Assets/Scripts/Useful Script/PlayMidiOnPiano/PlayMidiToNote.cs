using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayMidiToNote : MonoBehaviour
{
    
    // TODO: Please delete this Text variable, it is not needed...
    public Text MusicNoteText;
    public Text TempoText;
    public Text Velocitytext;
    public Text timerText;
    public Text startTime;
    
    
    
    
    // Midi file info variable
    public string midifilePath;
    public string midifilename;
    
    
    // Midi file converter and tools variabels
    private MidiFileInspector _midi; 
    MidiNote[] MidiNotes;
    
    //Primitive data variable
    public float GlobalSpeed = 1;
    int _noteIndex = 0;
    float _timer = 0;
    
    
    // Gameobjects and Button varibales
    public Button PauseButton;
    public Button CancelSettingButton;
    public Button PlayMidiSongOnlyButton;
    public Button PracticeMidiSongButton;
    public Button ReplaySongButton;

    public Text Songname;
    
    // Container to contain all the sheet music note symbol
    
    //Treble note container with its start and end target point
    public GameObject TrebleClefNoteContainer;
    public GameObject TrebleClefNoteContainerWithHorizonLay;
    public GameObject TrebleStartPoint;
    public GameObject TrebleEndPoint;
    
    //Bass note container with its start and end target point
    public GameObject BassClefNoteContainer;
    public GameObject BassClefNoteContainerWithHorizonLay;
    public GameObject BassStartPoint;
    public GameObject BassEndPoint;
    
    //The blue indicate in whihc each note will check their distance
    public GameObject BlueIndication;
    public GameObject BlueIndication1;
    
    
    public GameObject SettingPanel;
    
    public List<KeyGameObjectHolder> TrebleSheetNotes;
    public List<KeyGameObjectHolder> BassSheetNotes;
    public GameObject EmptyNote;
   
    
    //hold all the instantiated sheet music note
    private List<KeyGameObjectHolder> InstantiatedSheetMusicNotes;
    
    
    // An array to hold maximum number of note
    private List<KeyGameObjectHolder> maxTrebleKeyArray;
    private List<KeyGameObjectHolder> maxBassKeyArray;
    private KeyGameObjectHolder currentKeyNote;
    private string savedNotename = "savedNote";
    private string currentNotename = "currentNote";
    private bool pianopressed = false;

    enum NoteEqual
    {
        Equal,NotEqual,Delay
    }

    private NoteEqual noteequal;
   
   
   
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        //TODO: Please remember to call the static file path of the midi file from SongSelection script in SongSelection Scene
        //TODO: ie. midifilePath = SongSelectionScript.midifilePath;

        noteequal = NoteEqual.NotEqual;
        
       
        
        InstantiatedSheetMusicNotes = new List<KeyGameObjectHolder>();
        maxTrebleKeyArray = new List<KeyGameObjectHolder>();
        maxBassKeyArray = new List<KeyGameObjectHolder>();
        


            // Make all the neccessary boolean in the receptor true.
        // (Boolean for Listening to Microphone pitch detection and Being able to click on the keyboard)
        // This will affect the KeysScript of each key in the keyboard

        NoteReceptor.isMicrophoneNoteReceived = true;
        MidiNoteReceptor.isPianoKeyActiveToClick = true;
        MidiNoteReceptor.isMidiSongNoteRecieved = false;
        MidiNoteReceptor.isMoveNoteSymbolActive = false;
        MidiNoteReceptor.isMicrophonePitchDetectionUsed = false;
        
        AttachClickListener();


        
        
        
#if UNITY_ANDROID && !UNITY_EDITOR

 
        midifilename = SongSelectionScript.midifiledir;

        //Get the midifile name from Songg Selection Script and extract the midi notes from the file
       
        midifilePath = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, midifilename);
        
        
        
        // all the xmls


        /*//BetterStreaming Assets
        BetterStreamingAssets.Initialize();
        string[] paths = BetterStreamingAssets.GetFiles("\\", "*.mid", SearchOption.AllDirectories);
        midifilePath = paths[0];*/
        
        
        // Android
        // string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, "db.bytes");

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
        
        
       
        
        

        
        // Get the Midi file directory from the calling Song Selection Script scene
        
        //dbPath = SongSelectionScript.midifiledir;
        //ExtractMidiNotes(SongSelectionScript.midifiledir);


        // Note is extracted from the midi files
        //TODO ExtractMidiNotes(midifilePath);
        
        //ExtractMidiNotes(midifilePath);
        
        //Populate the treble and bass sheet staff with the note symbols
        //PopulateSheetStaff();
        
        StartCoroutine(PopulateGradually());

        /*while (spawnIndex < 20)
        {
            
            /*MidiNotes.Length#1#
            SpawnAndMoveSheetMusicNote();
        }*/


        TempoText.text = $"Midi Key ID {MidiNoteReceptor.midiNoteID}";



    }

    // Update is called once per frame
    void Update()
    {
        
        TempoText.text = $"Midi Key ID {MidiNoteReceptor.midiNoteID}";

        if (MidiNoteReceptor.isMidiSongNoteRecieved)
        {
            PlayMidiNote();
        }

        if (MidiNoteReceptor.isMoveNoteSymbolActive)
        {
            StartMovingNote();
        }




         PracticeAndPlayNote();

        // TestPlayingMidiFile();

    }

    private int shouldmoveindex = 0;
    private int previousecond;
    void StartMovingNote()
    {
        if (shouldmoveindex < InstantiatedSheetMusicNotes.Count)
        {

            MusicNoteText.text = $"Distance : {MidiNoteReceptor.distance} Blue: {BlueIndication.transform.position.x}";
            /*MusicNoteText.text = $"Millisecond: {DateTime.Now.Millisecond}";
            TempoText.text = $"Ticks: {DateTime.Now.Ticks}";
            Velocitytext.text = $"Total seconds: {DateTime.Now.TimeOfDay.TotalSeconds}";*/

            int minussecond = (int)DateTime.Now.TimeOfDay.TotalSeconds - previousecond;

            if (minussecond >= 1 )
            {
                KeyGameObjectHolder holder = InstantiatedSheetMusicNotes[shouldmoveindex];
                GameObject objectNote = (GameObject) holder.NoteSymbolObject;
        
                objectNote.GetComponent<SheetNoteSymbolScript>().ShouldMove(true);

                shouldmoveindex++;

                previousecond = (int) DateTime.Now.TimeOfDay.TotalSeconds;

                //yield return new WaitForSeconds(10f);
            }

            
            
        }
        

    }


    void ExtractMidiNotes(string path)
    {
        _midi = new MidiFileInspector(path);
        MidiNotes = _midi.GetNotes();
    }


    private int spawnIndex = 0;
    private int spawnspace = 1;

    private float spawntimer;
    
    //Spawn shheet music note and move them at a constant speed
    void SpawnAndMoveSheetMusicNote()
    {
        
        
        if (_midi != null && MidiNotes.Length > 0 && spawnIndex < MidiNotes.Length)
        {
            
            MidiNote note = MidiNotes[spawnIndex];
                
            foreach (var trebleNote in TrebleSheetNotes){
                    
                if (trebleNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {


                    GameObject sheetnote = (GameObject) Instantiate((GameObject) trebleNote.NoteSymbolObject,
                        TrebleStartPoint.transform.position, Quaternion.identity, TrebleClefNoteContainer.transform);

                    sheetnote.GetComponent<SheetNoteSymbolScript>().Setup(BlueIndication,BlueIndication1,TrebleStartPoint,TrebleEndPoint);
                    
                    
                    GameObject Empty = Instantiate(EmptyNote,BassStartPoint.transform.position ,Quaternion.identity, BassClefNoteContainer.transform);
                    
                    // sheetnote keyholder
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = trebleNote.Notename;
                    keyGameObject.Notenum = trebleNote.Notenum;
                    
                    //bass empty keyholder
                    KeyGameObjectHolder emptykey = new KeyGameObjectHolder();
                    emptykey.NoteSymbolObject = Empty;
                    emptykey.Notename = "Empty";
                    
                    
                    
                    
                    
                    // store the instantiated key in an array and list
                    currentKeyNote = keyGameObject;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                    
                    maxTrebleKeyArray.Add(keyGameObject);
                    maxBassKeyArray.Add(emptykey);
                    
                    break;
                    
                }
            }
            
            foreach (var bassNote in BassSheetNotes)
            {
                if (bassNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {

                    GameObject sheetnote = (GameObject) Instantiate(bassNote.NoteSymbolObject,BassStartPoint.transform.position,Quaternion.identity, BassClefNoteContainer.transform);
                   
                    sheetnote.GetComponent<SheetNoteSymbolScript>().Setup(BlueIndication,BlueIndication1,BassStartPoint,BassEndPoint);
                    
                    
                    
                    GameObject Empty = Instantiate(EmptyNote,TrebleStartPoint.transform.position ,Quaternion.identity, TrebleClefNoteContainer.transform);
                    
                    //sheetnote keyholder
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = bassNote.Notename;
                    keyGameObject.Notenum = bassNote.Notenum;
                    
                    //bass empty keyholder
                    KeyGameObjectHolder emptykey = new KeyGameObjectHolder();
                    emptykey.NoteSymbolObject = Empty;
                    emptykey.Notename = "Empty";
                    
                    // store the instantiated key in an array and list
                    currentKeyNote = keyGameObject;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                    
                    maxBassKeyArray.Add(keyGameObject);
                    maxTrebleKeyArray.Add(emptykey);


                    break;
                }
            }




            spawnIndex++;
            spawnspace++;

        }
        
        
    }





    private int tracklastIndex = 0;
    IEnumerator PopulateGradually()
    {
        
        // Once the object count get to 15 destroy the object and clear the list
        /*if (maxTrebleKeyArray.Count == 15 || maxBassKeyArray.Count == 15)
        {*/

            foreach (var key in maxTrebleKeyArray)
            {
                DestroyImmediate(key.NoteSymbolObject);
            }

            foreach (var key in maxBassKeyArray)
            {
                DestroyImmediate(key.NoteSymbolObject);
            }
            
            maxBassKeyArray.Clear();
            maxTrebleKeyArray.Clear();
            
        //}

        if ((tracklastIndex + 15) < MidiNotes.Length)
        {
            for (int i = 0; i < 15; i++)
            {
             
                LoopContent(i);
                
                yield return new WaitForSeconds(0.1f);

            }
            
        }
        else
        {
            for (int i = tracklastIndex; i < MidiNotes.Length; i++)
            {
                
                LoopContent(i);
                
            
                yield return new WaitForSeconds(0.1f);
            }
        }







        //yield return new WaitForSeconds(2f);
    }


    void LoopContent(int i)
    {
        
         int count = tracklastIndex + i;
                
                MidiNote note = MidiNotes[count];
                
                foreach (var trebleNote in TrebleSheetNotes){
                    
                if (trebleNote.Notename.ToLower().Equals(note.Note.ToLower())) {
                    
                    
                    GameObject sheetnote = (GameObject) Instantiate(trebleNote.NoteSymbolObject,TrebleClefNoteContainerWithHorizonLay.transform);
                    GameObject Empty = Instantiate(EmptyNote, BassClefNoteContainerWithHorizonLay.transform);
                    
                    // sheetnote keyholder
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = trebleNote.Notename;
                    keyGameObject.Notenum = trebleNote.Notenum;
                    
                    //bass empty keyholder
                     KeyGameObjectHolder emptykey = new KeyGameObjectHolder();
                    emptykey.NoteSymbolObject = Empty;
                    emptykey.Notename = "Empty";
                    
                    
                    
                    
                    
                    // store the instantiated key in an array and list
                    currentKeyNote = keyGameObject;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                    
                    maxTrebleKeyArray.Add(keyGameObject);
                    maxBassKeyArray.Add(emptykey);
                    
                }
            }

            foreach (var bassNote in BassSheetNotes)
            {
                if (bassNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {

                    GameObject sheetnote = (GameObject) Instantiate(bassNote.NoteSymbolObject, BassClefNoteContainerWithHorizonLay.transform);
                    GameObject Empty = Instantiate(EmptyNote, TrebleClefNoteContainerWithHorizonLay.transform);
                    
                    //sheetnote keyholder
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = bassNote.Notename;
                    keyGameObject.Notenum = bassNote.Notenum;
                    
                    //bass empty keyholder
                    KeyGameObjectHolder emptykey = new KeyGameObjectHolder();
                    emptykey.NoteSymbolObject = Empty;
                    emptykey.Notename = "Empty";
                    
                    // store the instantiated key in an array and list
                    currentKeyNote = keyGameObject;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                    
                    maxBassKeyArray.Add(keyGameObject);
                    maxTrebleKeyArray.Add(emptykey);


                }
            }

            if (i == 14)
            {
                tracklastIndex = count;
            }

            
        
    }


    private int sheetcount = 0;
    private string savedNote;
    
    void PracticeAndPlayNote()
    {

       

        if (sheetcount <= tracklastIndex)
        {
            KeyGameObjectHolder keyGameObjectHolder = InstantiatedSheetMusicNotes[sheetcount];

           


            int calcNoteID = MidiNoteReceptor.midiNoteID + 1;//(NoteReceptor.noteID - 20) + 1;

           // calcNoteID = NoteReceptor.noteID; //remove later

           timerText.text = $" Key Game {keyGameObjectHolder.Notenum}";



           if (MidiNoteReceptor.midiNoteID != -1)
           {
               if (keyGameObjectHolder.Notenum == calcNoteID)
               {
                   GameObject sheetnote = (GameObject)  keyGameObjectHolder.NoteSymbolObject;
                   sheetnote.GetComponent<SheetNoteSymbolScript>().colornum = 1;

                  
                
                
                   if (sheetcount == InstantiatedSheetMusicNotes.Count-1)
                   {
                       tracklastIndex = InstantiatedSheetMusicNotes.Count;
                       StartCoroutine(PopulateGradually());

                   }

                   sheetcount++;



               } else if(keyGameObjectHolder.Notenum != calcNoteID) {
                   GameObject sheetnote = (GameObject)  keyGameObjectHolder.NoteSymbolObject;
                   sheetnote.GetComponent<SheetNoteSymbolScript>().colornum = 3;
                   //sheetcount++;
                   
                   if (sheetcount == InstantiatedSheetMusicNotes.Count-1)
                   {
                       tracklastIndex = InstantiatedSheetMusicNotes.Count;
                       StartCoroutine(PopulateGradually());

                   }

                   sheetcount++;

               }
               
               MidiNoteReceptor.midiNoteID = -1;
        

               savedNote = NoteReceptor.note;
           }



          
        }

        
    }


    //New populate Keynote

    private int midicount = 0;
   
    IEnumerator PopulateNoteSlowly()
    {
       
        //yield return new WaitUntil(() => noteequal == NoteEqual.Equal);
        
        yield return new WaitForSeconds(2f);


        // Once the object count get to 15 destroy the object and clear the list
        if (maxTrebleKeyArray.Count == 15 || maxBassKeyArray.Count == 15)
        {

            foreach (var key in maxTrebleKeyArray)
            {
                DestroyImmediate(key.NoteSymbolObject);
            }

            foreach (var key in maxBassKeyArray)
            {
                DestroyImmediate(key.NoteSymbolObject);
            }
            
            maxBassKeyArray.Clear();
            maxTrebleKeyArray.Clear();
            
        }
        
        
        // loop through and spawn the key note

        if (midicount < MidiNotes.Length)
        {
             MidiNote note = MidiNotes[midicount];

            foreach (var trebleNote in TrebleSheetNotes)
            {
                if (trebleNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {
                    
                    
                    GameObject sheetnote = (GameObject) Instantiate(trebleNote.NoteSymbolObject,TrebleClefNoteContainer.transform);
                    GameObject Empty = Instantiate(EmptyNote, BassClefNoteContainer.transform);
                    
                    // sheetnote keyholder
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = trebleNote.Notename;
                    
                    //bass empty keyholder
                     KeyGameObjectHolder emptykey = new KeyGameObjectHolder();
                    emptykey.NoteSymbolObject = Empty;
                    emptykey.Notename = "Empty";
                    
                    
                    
                    
                    
                    // store the instantiated key in an array and list
                    currentKeyNote = keyGameObject;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                    
                    maxTrebleKeyArray.Add(keyGameObject);
                    maxBassKeyArray.Add(emptykey);
                    
                }
            }

            foreach (var bassNote in BassSheetNotes)
            {
                if (bassNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {

                    GameObject sheetnote = (GameObject) Instantiate(bassNote.NoteSymbolObject, BassClefNoteContainer.transform);
                    GameObject Empty = Instantiate(EmptyNote, TrebleClefNoteContainer.transform);
                    
                    //sheetnote keyholder
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = bassNote.Notename;
                    
                    //bass empty keyholder
                    KeyGameObjectHolder emptykey = new KeyGameObjectHolder();
                    emptykey.NoteSymbolObject = Empty;
                    emptykey.Notename = "Empty";
                    
                    // store the instantiated key in an array and list
                    currentKeyNote = keyGameObject;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                    
                    maxBassKeyArray.Add(keyGameObject);
                    maxTrebleKeyArray.Add(emptykey);


                }
            }

            midicount++;
        }


        







    }


    void CheckCorrectnessOfPianoKey()
    {
        
        currentNotename = NoteReceptor.note;
        
        if (currentKeyNote != null)
        {

           
            if (!savedNotename.Equals(currentNotename))
            {
                currentNotename = "nothing";
                pianopressed = false;
                noteequal = NoteEqual.NotEqual;
            }


            savedNotename = currentNotename;


           
                if (currentKeyNote.Notename.Equals(currentNotename))
                {
                    pianopressed = true;
                    noteequal = NoteEqual.Equal;
                    
                    GameObject sheetnote = (GameObject)  currentKeyNote.NoteSymbolObject;
                    sheetnote.GetComponent<SheetNoteSymbolScript>().colornum = 1;

                  


                }
                else
                {
                   // pianopressed = true;
                   noteequal = NoteEqual.NotEqual;
                   
                    GameObject sheetnote = (GameObject)  currentKeyNote.NoteSymbolObject;
                    sheetnote.GetComponent<SheetNoteSymbolScript>().colornum = 3;
                    

                }
            




        }
    }



    void PlayMidiNote()
    {
        
        if (_midi != null && MidiNotes.Length > 0 && _noteIndex < MidiNotes.Length)
        {
            _timer += Time.deltaTime * GlobalSpeed * (float)MidiNotes[_noteIndex].Tempo;
            
            if (MidiNotes[_noteIndex].StartTime < _timer)
            {

                MidiNoteReceptor.note = MidiNotes[_noteIndex].Note;
                
                

                _noteIndex++;
            }
        }
    }


    
    void MoveNoteInSheetOn()
    {
        MidiNoteReceptor.isMoveNoteSymbolActive = true;
        MidiNoteReceptor.tempo = MidiNotes[0].Tempo;

    }

    void MoveNoteInSheetOff()
    {
        MidiNoteReceptor.isMoveNoteSymbolActive = false;
    }


    void AttachClickListener()
    {


        PauseButton.onClick.AddListener(() => { PauseButtonFunc();});
 
        CancelSettingButton.onClick.AddListener(() => { CancelSettingFunc();});
   
        PlayMidiSongOnlyButton.onClick.AddListener(() => { PlayMidiToNoteFunc();});
    
        PracticeMidiSongButton.onClick.AddListener(() => { PracticeMidiSongFunc();});
    
        ReplaySongButton.onClick.AddListener(() => {ReplaySongFunc(); });
        
        
    }

    void PauseButtonFunc()
    {
        //SettingPanel.SetActive(true);
        MidiNoteReceptor.isMoveNoteSymbolActive = true;
    }

    void CancelSettingFunc()
    {
        SettingPanel.SetActive(false);
    }

    void PlayMidiToNoteFunc()
    {
        MidiNoteReceptor.isMidiSongNoteRecieved = true;
        MidiNoteReceptor.isPianoKeyActiveToClick = false;
        
        NoteReceptor.isMicrophoneNoteReceived = false;

        _noteIndex = 0;
        
        MoveNoteInSheetOn();
        
    }

    void PracticeMidiSongFunc()
    {
     
        NoteReceptor.isMicrophoneNoteReceived = true;
        MidiNoteReceptor.isPianoKeyActiveToClick = true;
        
        MidiNoteReceptor.isMidiSongNoteRecieved = false;
    }

    void ReplaySongFunc()
    {
        
        
    }
    
    
    void PopulateSheetStaff()
    {
        foreach (var note in MidiNotes)
        {

            foreach (var trebleNote in TrebleSheetNotes)
            {
                if (trebleNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {
                    
                    
                    GameObject sheetnote = (GameObject) Instantiate(trebleNote.NoteSymbolObject,TrebleClefNoteContainer.transform);
                    Instantiate(EmptyNote, BassClefNoteContainer.transform);
                    
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = trebleNote.Notename;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);
                }
            }

            foreach (var bassNote in BassSheetNotes)
            {
                if (bassNote.Notename.ToLower().Equals(note.Note.ToLower()))
                {

                    GameObject sheetnote = (GameObject) Instantiate(bassNote.NoteSymbolObject, BassClefNoteContainer.transform);
                    Instantiate(EmptyNote, TrebleClefNoteContainer.transform);
                    
                    KeyGameObjectHolder keyGameObject = new KeyGameObjectHolder();
                    keyGameObject.NoteSymbolObject = sheetnote;
                    keyGameObject.Notename = bassNote.Notename;
                    InstantiatedSheetMusicNotes.Add(keyGameObject);


                }
            }
            
            
            
        }
    }
    
    




    void TestExtractingMidiNote()
    {
        
        string path = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, midifilename);
        
        _midi = new MidiFileInspector(path);
        MidiNotes = _midi.GetNotes();

    }
    


    void TestPlayingMidiFile()
    {
        
        if (_midi != null && MidiNotes.Length > 0 && _noteIndex < MidiNotes.Length)
        {
            _timer += Time.deltaTime * GlobalSpeed * (float)MidiNotes[_noteIndex].Tempo;
            timerText.text = "Timer: "+_timer;
            TempoText.text = "Tempo: "+(float)MidiNotes[_noteIndex].Tempo;
            startTime.text = "starttime : " + MidiNotes[_noteIndex].StartTime;
            Velocitytext.text = "Velocity: " + MidiNotes[_noteIndex].Velocity;

            if (MidiNotes[_noteIndex].StartTime < _timer)
            {

                MusicNoteText.text = MidiNotes[_noteIndex].Note;
                
                /*if (PianoKeyDetector.PianoNotes.ContainsKey(MidiNotes[_noteIndex].Note))
                    PianoKeyDetector.PianoNotes[MidiNotes[_noteIndex].Note].Play(MidiNotes[_noteIndex].Velocity, 
                                                                                MidiNotes[_noteIndex].Length, 
                                                                                PianoKeyDetector.MidiPlayer.GlobalSpeed * MIDISongs[_midiIndex].Speed);*/

                _noteIndex++;
            }
        }
    }
    
    
}

[Serializable]
public class SheetMusicNotes
{
#if UNITY_EDITOR
    public UnityEngine.Object NoteSymbolObject;
#endif
    public string Notename;
    public int Notenum;
    [TextArea]
    public string Details;
}
