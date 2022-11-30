using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NoteReceptor : MonoBehaviour
{
    
    // static variables
    public static int noteID;
    public static int midiKeyID = -1;
    public static string note;
    public static float pitch;
    public static bool isMicrophoneNoteReceived = false;
    
    public static int keyID;
    private string savedNote = "";
    
    
    int saveNoteID = -1;

    private int noteNum = -1;
    private int keynum = 30;
    
    
   
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
