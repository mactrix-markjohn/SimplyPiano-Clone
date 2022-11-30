using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class SheetNoteScript : MonoBehaviour
{

    public string noteName;
    public int noteIndex;
    public int colornum = -1;
    
    private int noteIDRecieved;
    private string noteNameReceived;

    public GameObject theNote;

    private double timerSPeed;
    private Transform blueIndicate;

    private Transform beginPos;
    private Transform endPos;


    public static bool shouldMove = false;
    public bool isToMove = false;
    
    
    //countdown variable
    private float startTime = 10f;
    float currenttime = 10f;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        //move and check input
        MoveNote();
        
        //Check Input for display sheet music
        CheckInputForDisplaySheetMusic();
        
       
    }

    public void Setup(double timer, Transform indicate, Transform endPosition)
    {
        timerSPeed = timer;
        blueIndicate = indicate;
        endPos = endPosition;

    }

    public void Setup(int columnIndex)
    {
        colornum = columnIndex;
    }

    
    
    //Check for Display only sheet music
    void CheckInputForDisplaySheetMusic()
    {
        if (!MidiNoteReceptor.shouldmove)
        {
            if (colornum == MidiNoteReceptor.ColumnPlayIndex)
            {
                int calcNoteID = MidiNoteReceptor.midiNoteID; //+ 1;//(NoteReceptor.noteID - 20) + 1;



                //countdown algorithm to prompt the user to click on the a key
                currenttime -= 1 * Time.deltaTime;
                int intCurrentTime = (int) currenttime;

                if (intCurrentTime <= 0)
                {
                    currenttime = 0f;
                
                    theNote.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            
            

                if (MidiNoteReceptor.midiNoteID != -1)
                {
                
                


                    if (noteIndex == calcNoteID)
                    {
                    
                        theNote.GetComponent<SpriteRenderer>().color = Color.green;
                        currenttime = 10f;
                        MidiNoteReceptor.ColumnPlayIndex++;

                    } else if(noteIndex != calcNoteID) {
                    
                        theNote.GetComponent<SpriteRenderer>().color = Color.red;
                        currenttime = 10f;
                        MidiNoteReceptor.ColumnPlayIndex++;

                    }
               
                    MidiNoteReceptor.midiNoteID = -1;
        
                
                }
            }
        }
    }


    void MoveNote()
    {

        shouldMove = MidiNoteReceptor.shouldmove;

        if (shouldMove)
        {
            //shouldMove || isToMove
            
            int speed = (int) timerSPeed / 2;

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, endPos.localPosition, Time.deltaTime * 2.5f);//Vector3.Lerp(transform.localPosition,endPos.localPosition, 1 * Time.deltaTime); 
            theNote.GetComponent<SpriteRenderer>().enabled = true;
            
            int differenceX =(int) Math.Abs(transform.localPosition.x - endPos.localPosition.x);

            if (differenceX == 0)
            {
                Destroy(gameObject);
            }

            
            
            CheckInput();

        }

        
        
    }

    void CheckInput()
    {
        int differenceX =(int) Math.Abs(transform.localPosition.x - blueIndicate.localPosition.x);

        if (differenceX < 1 )
        {
            //TODO : check if the input is correct

            int calcNoteID = MidiNoteReceptor.midiNoteID; //+ 1;//(NoteReceptor.noteID - 20) + 1;



            //countdown algorithm to prompt the user to click on the a key
            currenttime -= 1 * Time.deltaTime;
            int intCurrentTime = (int) currenttime;

            if (intCurrentTime <= 0)
            {
                currenttime = 0f;
                
                theNote.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            
            

            if (MidiNoteReceptor.midiNoteID != -1)
            {
                
                


                if (noteIndex == calcNoteID)
                {
                    
                    theNote.GetComponent<SpriteRenderer>().color = Color.green;
                    currenttime = 10f;
                    

                } else if(noteIndex != calcNoteID) {
                    
                    theNote.GetComponent<SpriteRenderer>().color = Color.red;
                    currenttime = 10f;
                   

                }
               
                MidiNoteReceptor.midiNoteID = -1;
        
                
            }
            
          
            
        }
        

    }
    
    
    



}
