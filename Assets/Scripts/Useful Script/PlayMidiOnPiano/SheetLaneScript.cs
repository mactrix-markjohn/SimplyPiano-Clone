using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetLaneScript : MonoBehaviour
{

    public GameObject[] SheetNotePrefab;
    public string[] SheetNoteName;
    public GameObject blueIndicate;

    public GameObject beginObject;
    public GameObject endObject;

    public static bool shouldMove = false;
    
    
    public static SheetLaneScript Instance;

    private List<string> ArrayNoteName;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
        ArrayNoteName = new List<string>(SheetNoteName);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetandInstantiatePrefrab(string noteName, double timer)
    {

        if (ArrayNoteName.Contains(noteName))
        {
            int index = ArrayNoteName.IndexOf(noteName);

            GameObject objectj = SheetNotePrefab[index];

            var note = Instantiate(objectj, transform);
            note.transform.localPosition = beginObject.transform.localPosition + new Vector3(MidiPianoController.NoteLength,0,0); ;
            
            //note.transform.localPosition += 

            //MidiPianoController.NoteLength = note.transform.localPosition.x;
            

            var script = note.GetComponent<SheetNoteScript>();
            script.isToMove = shouldMove;
            script.Setup(timer,blueIndicate.transform,endObject.transform);
            
            

            



        }




    }

}
