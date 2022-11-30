using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiNoteReceptor : MonoBehaviour
{
    
    //static variables
    public static int noteID;

    public static int midiNoteID = -1;
    
    //special chord index
    public static int chordID1;
    public static int chordID2;
    public static int chordID3;
    public static int chordID4;
    
    
    public static string note;
    public static bool isMidiSongNoteRecieved = false;
    public static bool isJustIndicate = false;
    public static bool isChordIndicate = false;
    public static bool isPianoKeyActiveToClick = false;
    public static bool isMicrophonePitchDetectionUsed = false;

    public static bool isMidiCableConnected = false;
    public static int isMidiJackConneected = -1;
    
    
    public static bool isMoveNoteSymbolActive = false;
    public static double tempo;
    
    public static int keyID;

    public static float distance;

    public static bool shouldmove = false;
    public static int ColumnPlayIndex = 0;

   
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
