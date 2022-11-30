using UnityEngine;
using MidiJack;

public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;

    void Update()
    {
        transform.localScale = Vector3.one * (0.1f + MidiMaster.GetKey(noteNumber));

        
        
        /*var color = MidiMaster.GetKeyDown(noteNumber) ? Color.red : Color.white;
        GetComponent<Renderer>().material.color = color;*/

        if (MidiMaster.GetKeyDown(noteNumber))
        {
            var color = Color.red;
            GetComponent<Renderer>().material.color = color;

            DebugMidiCode.keynumber = (int) MidiMaster.GetKey(noteNumber);
        }
        else
        {
            var color = Color.white;
            GetComponent<Renderer>().material.color = color;

           DebugMidiCode.keynumber = (int) MidiMaster.GetKey(noteNumber);
        }
        
        
        
        
    }
}
