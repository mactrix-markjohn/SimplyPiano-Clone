using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseSheetLaneScript : MonoBehaviour
{
    
    public GameObject blueIndicate;

    public GameObject beginObject;
    public GameObject endObject;

    public static bool shouldMove = false;

    public static CourseSheetLaneScript Instance;

   // private float posInc = 0f;


    private List<GameObject> genetatedNoteObject;

    private void Awake()
    {
        genetatedNoteObject = new List<GameObject>();  
    }

    // Start is called before the first frame update
    void Start()
    {
        
        Instance = this;
        blueIndicate.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InstantiateNotes(GameObject noteObject, int courseType, int index)
    {
        if (courseType == 0 || courseType == 1)
        {
            //just display the notes
           
            
            var note = Instantiate(noteObject, transform);
            note.transform.localPosition = endObject.transform.localPosition + new Vector3(CoursePianoPanelScript.posInc,0,0); 

            CoursePianoPanelScript.posInc += 2f;


            genetatedNoteObject.Add(note);

            var script = note.GetComponent<SheetNoteScript>();
            //script.isToMove = MidiNoteReceptor.shouldmove;
            script.Setup(index);
            
            
            
            
            
            
            
        }else if (courseType == 2)
        {
            
            
            if (index >=19)
            {
                MidiNoteReceptor.shouldmove = true;
            }

            blueIndicate.SetActive(true);
            
            // move the note and activate the blue indicate, use the same logic as in MidiPianoController script
            
            var note = Instantiate(noteObject, transform);
            note.transform.localPosition = beginObject.transform.localPosition + new Vector3(CoursePianoPanelScript.posInc,0,0); 

            CoursePianoPanelScript.posInc += 2f;

            genetatedNoteObject.Add(note);

            float timer = 3f;
            
            
            var script = note.GetComponent<SheetNoteScript>();
           
            script.Setup(timer,blueIndicate.transform,endObject.transform);

           
            
            
            
        }

    }


    public void ClearInstantiated()
    {

        foreach (var noteObject in genetatedNoteObject)
        {
            
            Destroy(noteObject);
            
        }
        
        genetatedNoteObject.Clear();
        
    }
}
