using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoursePianoPanelScript : MonoBehaviour
{
    private List<GameObject> noteList;

    private int note3 = 0;
    private int notef = 1;
    private int mainch = 2;
    
    int courseType = -1;

    public Button backButton;

    public CourseSheetLaneScript LaneScript;
    
    //Self
    public static CoursePianoPanelScript Instance;

   
    
    int storeGenCount =0;
    
    public static float posInc = 0f;
    
    
    //public variable of the note C, D, E and F prefab
    public GameObject C;
    public GameObject D;
    public GameObject E;
    public GameObject F;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        courseType = CourseTouchIntroScript.courseType;

        MidiNoteReceptor.shouldmove = false;
        
        backButton.onClick.AddListener(() => { SceneManager.LoadScene(ConstantStrings.CourseTouchIntroScene); });
        
        Setup();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (noteList != null)
        {
            
            if (MidiNoteReceptor.ColumnPlayIndex >= storeGenCount)
            {

                if (LaneScript != null)
                {
                    LaneScript.ClearInstantiated();
                }

                if (courseType == note3)
                {
                    Generate5NoteFor3Note();
                
                }else if (courseType == notef)
                {
                    Generate10NoteForNoteF();
                }else if (courseType == mainch)
                {
                    Generate20NoteForMainChallenge();
                }

            }
            
        }


       
    }

    void Setup()
    {
        if (courseType == 0)
        {
            
            Setup(C,D,E,courseType);
            
        }else if (courseType == 1)
        {
            
            Setup(C,D,E,F,courseType);
            
        }else if (courseType == 2)
        {
         
            Setup(C,D,E,F,courseType);
        }
    }


    public void Setup(GameObject C, GameObject D, GameObject E, int coursetype)
    {
        noteList = new List<GameObject>();
        noteList.Add(C);
        noteList.Add(D);
        noteList.Add(E);

        courseType = coursetype;
        
        if (LaneScript != null)
        {
            LaneScript.ClearInstantiated();
        }
        
        Generate5NoteFor3Note();

    }

    public void Setup(GameObject C, GameObject D, GameObject E, GameObject F, int coursetype)
    {
        noteList = new List<GameObject>();
        noteList.Add(C);
        noteList.Add(D);
        noteList.Add(E);
        noteList.Add(F);

        courseType = coursetype;
        
        if (LaneScript != null)
        {
            LaneScript.ClearInstantiated();
        }

        if (courseType == notef)
        {
            Generate10NoteForNoteF();
        }
        else
        {
            Generate20NoteForMainChallenge();
        }


    }

    public void Generate5NoteFor3Note()
    {
        MidiNoteReceptor.ColumnPlayIndex = 0;
        storeGenCount = 0;
        posInc = 0;
        
        
        for (int i = 0; i < 5; i++)
        {

            int ranNum = Random.Range(0, 3);

            GameObject noteObject = noteList[ranNum];
            
            LaneScript.InstantiateNotes(noteObject,0,i);

            storeGenCount++;


        }
    }
    
     public void Generate10NoteForNoteF()
    {
        MidiNoteReceptor.ColumnPlayIndex = 0;
        storeGenCount = 0;
        posInc = 0;
        
        for (int i = 0; i < 10; i++)
        {

            int ranNum = Random.Range(0, 4);

            GameObject noteObject = noteList[ranNum];
            
            LaneScript.InstantiateNotes(noteObject,1,i);

            storeGenCount++;

        }
    }
    
    public void Generate20NoteForMainChallenge()
    {
        MidiNoteReceptor.ColumnPlayIndex = 0;
        storeGenCount = 0;
        posInc = 0;
        
        for (int i = 0; i < 50; i++)
        {

            int ranNum = Random.Range(0, 4);

            GameObject noteObject = noteList[ranNum];
            
            LaneScript.InstantiateNotes(noteObject,2,i);

            storeGenCount++;

        }

        MidiNoteReceptor.shouldmove = true;
    }


    private void OnDisable()
    {
        //MidiNoteReceptor.shouldmove = false;
    }
}
