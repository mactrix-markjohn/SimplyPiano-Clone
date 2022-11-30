using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PianoKeys : MonoBehaviour
{
    [SerializeField]
    private Color m_RootKeyHighlight;
    [SerializeField]
    private Color m_OtherKeysHighlight;

    [SerializeField]
    private int m_MaxCount = 24;
    [SerializeField]
    private int m_Threshold = 4;
    [SerializeField]
    private float m_Exponent = 1;

    private MaterialPropertyBlock m_MPB;
    private Dictionary<int, int> m_RootKeyCount;
    private Dictionary<int, int> m_OtherKeysCount;
    private Dictionary<int, Color> m_DefColors;
    private Dictionary<int, Renderer> m_Renderers;
    private List<int> m_Keys;
    
    
    //Test Text UI elements
    public Text firsttext;
    public Text secondtext;
    public Text thirdtext;
    public Text fourthtext;
    public Text fifthtext;
    public Text sixthtext;
    public Text seventhtext;


    public void OnAgentGuess(Chord chord)
    {


        /*firsttext.text = $"Chord Keys: {chord.Key}";
        secondtext.text = $"Chord Type: {chord.Type.ToString()}";
        thirdtext.text = $"Chord details: {chord.ToString()}";
        fourthtext.text = $"Chord RoteNote: {chord.RootNote.ToString()}";*/
        
        

        bool shift = chord.Key < 4;
        var pianoKeys = chord.PianoKeys.Select(k => shift ? k + 12 : k);
        var root = pianoKeys.First();
        var rest = pianoKeys.Skip(1);

        /*fifthtext.text = $"pianoKeys.First() : {root}";

        sixthtext.text = $"{string.Join(" ",chord.exitNotes.Select(name => name.NoteName))}";
        seventhtext.text = $"{string.Join(" ",chord.exitNotes.Select(name => name.NoteID))}";*/

        firsttext.text = $"NoteName: {string.Join(" ",chord.exitNotes.Select(name => name.NoteName))}\n\n" +
                         $"NoteID: {string.Join(" ",chord.exitNotes.Select(name => name.NoteID))} ";
        


        MidiNoteReceptor.isChordIndicate = true;
        MidiNoteReceptor.isMidiSongNoteRecieved = false;
        MidiNoteReceptor.isPianoKeyActiveToClick = true;
        NoteReceptor.isMicrophoneNoteReceived = false;

        MidiNoteReceptor.chordID1 = chord.exitNotes[0].NoteID;
        MidiNoteReceptor.chordID2 = chord.exitNotes[1].NoteID;
        MidiNoteReceptor.chordID3 = chord.exitNotes[2].NoteID;

        if (chord.exitNotes.Count >= 4)
        {
            MidiNoteReceptor.chordID4 = chord.exitNotes[3].NoteID;
        }




        /*foreach (int k in m_Keys)
        {
            int c = m_RootKeyCount[k];
            c += k == root ? 1 : -1;
            m_RootKeyCount[k] = Mathf.Clamp(c, 0, m_MaxCount);

          //  sixthtext.text = $"root C : {c} and \n rootKey Count: {Mathf.Clamp(c, 0, m_MaxCount)}";

            c = m_OtherKeysCount[k];
            c += rest.Contains(k) ? 1 : -1;
            m_OtherKeysCount[k] = Mathf.Clamp(c, 0, m_MaxCount);
            
           // seventhtext.text = $"root C : {c} and \n rootKey Count: {Mathf.Clamp(c, 0, m_MaxCount)}";
        }

        SetColors();*/
    }

    public void OnAgentGuess(Chord chord, string chordType, string chordInterval)
    {
        bool shift = chord.Key < 4;
        var pianoKeys = chord.PianoKeys.Select(k => shift ? k + 12 : k);
        var root = pianoKeys.First();
        var rest = pianoKeys.Skip(1);

        /*fifthtext.text = $"pianoKeys.First() : {root}";

        sixthtext.text = $"{string.Join(" ",chord.exitNotes.Select(name => name.NoteName))}";
        seventhtext.text = $"{string.Join(" ",chord.exitNotes.Select(name => name.NoteID))}";*/
        
        if(Int32.Parse(chordInterval) == 0){
            
            firsttext.text = $"Chord Played : \n{chord.exitNotes[0].NoteType} {chordType}" +
                             $"\n\nNoteName: {string.Join(" ",chord.exitNotes.Select(name => name.NoteName))}\n\n" +
                             $"NoteID: {string.Join(" ",chord.exitNotes.Select(name => name.NoteID))} ";
            
        }
        else
        {
            firsttext.text = $"Chord Played : \n{chord.exitNotes[0].NoteType} {chordType}{chordInterval}" +
                             $"\n\nNoteName: {string.Join(" ",chord.exitNotes.Select(name => name.NoteName))}\n\n" +
                             $"NoteID: {string.Join(" ",chord.exitNotes.Select(name => name.NoteID))} ";  
        }

       
        


        MidiNoteReceptor.isChordIndicate = true;
        MidiNoteReceptor.isMidiSongNoteRecieved = false;
        MidiNoteReceptor.isPianoKeyActiveToClick = true;
        NoteReceptor.isMicrophoneNoteReceived = false;

        MidiNoteReceptor.chordID1 = chord.exitNotes[0].NoteID;
        MidiNoteReceptor.chordID2 = chord.exitNotes[1].NoteID;
        MidiNoteReceptor.chordID3 = chord.exitNotes[2].NoteID;

        if (chord.exitNotes.Count >= 4)
        {
            MidiNoteReceptor.chordID4 = chord.exitNotes[3].NoteID;
        }
    }




    private void Awake()
    {
        m_MPB = new MaterialPropertyBlock();
        m_RootKeyCount = new Dictionary<int, int>();
        m_OtherKeysCount = new Dictionary<int, int>();
        m_DefColors = new Dictionary<int, Color>();
        m_Renderers = new Dictionary<int, Renderer>();
        m_Keys = new List<int>();

        /*for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            var r = c.GetComponent<Renderer>();
            int k = short.Parse(c.name);
            m_RootKeyCount.Add(k, 0);
            m_OtherKeysCount.Add(k, 0);
            m_DefColors.Add(k, r.material.color);
            m_Renderers.Add(k, r);
            m_Keys.Add(k);
        }*/
    }

    private void SetColors()
    {
        foreach (int k in m_Keys)
        {
            var col = m_DefColors[k];
            col = Color.Lerp(col, m_OtherKeysHighlight, Interpolation(m_OtherKeysCount[k]));
            col = Color.Lerp(col, m_RootKeyHighlight, Interpolation(m_RootKeyCount[k]));
            m_MPB.SetColor("_Color", col);
            m_Renderers[k].SetPropertyBlock(m_MPB);
        }
    }

    private float Interpolation(int count)
    {
        if (count <= m_Threshold)
        {
            return 0;
        }

        float v = (count - m_Threshold) / (float)(m_MaxCount - m_Threshold);
        return Mathf.Pow(v, m_Exponent);
    }
}
