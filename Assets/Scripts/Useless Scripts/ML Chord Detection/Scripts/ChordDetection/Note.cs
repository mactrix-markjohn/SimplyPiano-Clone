using UnityEngine;

public class Note
{
    public static string GetKeyName(int pianoKey)
    {
        return c_KeyNames[pianoKey % 12];
    }

    public static string GetKeyName(Note note)
    {
        return GetKeyName(note.PianoKey);
    }

    private static readonly string[] c_KeyNames = new string[]
        {"G#/Ab", "A", "A#/Bb", "B", "C", "C#/Db", "D", "D#/Eb", "E", "F", "F#/Gb", "G"};
    
    

    private static readonly string[] c_KeyNoteNames = new string[]
        {"G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G"};

    
    public int PianoKey { get; private set; }
    public int ModKey => PianoKey % 12;
    public int Octave => Mathf.FloorToInt((PianoKey + 8) / 12f);
    public string Name => $"{PianoKey} [{c_KeyNames[ModKey]}{Octave}]";

    public string NoteName => $"{c_KeyNoteNames[ModKey]}{Octave}";
    public int NoteID => PianoKey;
    public string NoteType => $"{c_KeyNoteNames[ModKey]}";
    

    public Note(int pianoKey)
    {
        PianoKey = pianoKey;
    }

    public override string ToString()
    {
        return Name;
    }
}