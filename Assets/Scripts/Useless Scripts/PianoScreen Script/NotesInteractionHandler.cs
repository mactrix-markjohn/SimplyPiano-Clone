using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NotesInteractionHandler : MonoBehaviour
{
    

    //My variables
    public static int noteID;
    public static int keyID;

    public static string note;
    private string savedNote = "";
    
    
    int saveNoteID = -1;

    private int noteNum = -1;
    private int keynum = 30;
    public Button C4,C_4 ,D4, D_4, E4, F4,F_4, G4, G_4, A4, A_4, B4 ,C5, C_5,D5, D_5,E5,F5, F_5,G5, G_5,A5, A_5,B5, C6, C_6,D6, D_6, E6, F6;

    private string[] musicalNotes;
    private List<string> PianoNotes;
    private AudioSource audioSource;

    
    //Lists
    public List<AudioClip> audioClips;
    public List<GameObject> DarkObject;
    
    

    //Debug Text UI
    public Text debugReceiveNoteIndex;
    public Text debugKeyIDMethodCalled;
    

    // Start is called before the first frame update
    void Start()
    {

        musicalNotes = new string[] { "C4","C#4","D4","D#4","E4","F4","F#4",
            "G4","G#4","A4","A#4","B4","C5","C#5","D5","D#5","E5","F5","F#5","G5","G#5",
            "A5","A#5","B5","C6","C#6","D6","D#6","E6","F6"};

        PianoNotes = new List<string>();
        PianoNotes.AddRange(musicalNotes);
        
        
        
        noteID = -1;

        audioSource = GetComponent<AudioSource>();
        AttachClickListener();
        
    }

    // Update is called once per frame
    void Update()
    {
      
        UpdateVirtualPiano();
       

        
       
    }
    
     void UpdateVirtualPiano()
    {
        debugReceiveNoteIndex.text = $"Midi Note: {noteID}";
        debugKeyIDMethodCalled.text = $"Midi note: {note}";
        
        
        //Debug.Log(noteID);        
        if (saveNoteID != noteID)
        {

            // everything should be non active
            foreach (var darkobject in DarkObject)
            {
                darkobject.SetActive(false);
            }

        }
        
        //saved or last entered noteID
        saveNoteID = noteID;

        if (noteID >= 59 && noteID <= 89)
        {
           int  index = noteID - 60;

            DarkObject[index+1].SetActive(true);
            
        }
        

    }
     
     IEnumerator PressWaitKey(int id)
     {
         DarkObject[id].SetActive(true);

         yield return new WaitForSeconds(1f);

         DarkObject[id].SetActive(false);

     }
     
     /*
     public void PressKey(int id)
     {
         


     }
     */

    

     
    void AttachClickListener()
    {
        C4.onClick.AddListener(() => { PlayClip(0,C4);});
        C_4.onClick.AddListener(() => { PlayClip(1,C_4);});
        D4.onClick.AddListener(() => { PlayClip(2,D4);});
        D_4.onClick.AddListener(() => { PlayClip(3,D_4);});
        E4.onClick.AddListener(() => { PlayClip(4,E4);});
        F4.onClick.AddListener(() => { PlayClip(5,F4);});
        F_4.onClick.AddListener(() => { PlayClip(6,F_4);});
        G4.onClick.AddListener(() => { PlayClip(7,G4);});
        G_4.onClick.AddListener(() => { PlayClip(8,G_4);});
        A4.onClick.AddListener(() => { PlayClip(9,A4);});
        A_4.onClick.AddListener(() => { PlayClip(10,A_4);});
        B4.onClick.AddListener(() => { PlayClip(11,B4);});
        C5.onClick.AddListener(() => { PlayClip(12,C5);});
        C_5.onClick.AddListener(() => { PlayClip(13,C_5);});
        D5.onClick.AddListener(() => { PlayClip(14,D5);});
        D_5.onClick.AddListener(() => { PlayClip(15,D_5);});
        E5.onClick.AddListener(() => { PlayClip(16,E5);});
        F5.onClick.AddListener(() => { PlayClip(17,F5);});
        F_5.onClick.AddListener(() => { PlayClip(18,F_5);});
        G5.onClick.AddListener(() => { PlayClip(19,G5);});
        G_5.onClick.AddListener(() => { PlayClip(20,G_5);});
        A5.onClick.AddListener(() => { PlayClip(21,A5);});
        A_5.onClick.AddListener(() => { PlayClip(22,A_5);});
        B5.onClick.AddListener(() => { PlayClip(23,B5);});
        C6.onClick.AddListener(() => { PlayClip(24,C6);});
        C_6.onClick.AddListener(() => { PlayClip(25,C_6);});
        D6.onClick.AddListener(() => { PlayClip(26,D6);});
        D_6.onClick.AddListener(() => { PlayClip(27,D_6);});
        E6.onClick.AddListener(() => { PlayClip(28,E6);});
        F6.onClick.AddListener(() => { PlayClip(29,F6);});
        
        
        
    }
    
    void PlayClip(int index, Button button)
    {

        AudioSource audioSource = button.GetComponent<AudioSource>();
        audioSource.clip = audioClips[index];
        audioSource.Play();

    }
    





    


   
    //TODO: Achieve Code. For Future reference
    


    void UpdateKeys()
    {
        if (keyID == -1)
        {
            // everything should be non active
            foreach (var darkobject in DarkObject)
            {
                darkobject.SetActive(false);
            }
        }
        else
        {
            // do something
            DarkObject[keyID].SetActive(true);

            keyID = -1;
        }
    }

    

    public void PitchToPianoKey(string note)
    {

        
        if (PianoNotes.Contains(note))
        {
            int index = PianoNotes.FindIndex( x => x.Equals(note));

            //PressWaitKey(index);
        }

    }

   

    
     void UpdatePianoOriginal()
        {

            debugReceiveNoteIndex.text = $"Note Interaction Handler received ID: {noteID}";


            //Debug.Log(noteID);        
            if (saveNoteID != noteID)
            {

                // everything should be non active
                foreach (var darkobject in DarkObject)
                {
                    darkobject.SetActive(false);
                }


                C4.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                C_4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                D4.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                D_4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                E4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                F4.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                F_4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                G4.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                G_4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                A4.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                A_4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                B4.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                C5.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                C_5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                D5.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                D_5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                E5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                F5.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                F_5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                G5.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                G_5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                A5.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                A_5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                B5.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                C6.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                C_6.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                D6.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                D_6.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                E6.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                F6.transform.GetChild(0).GetComponent<Image>().color = Color.white;

            }

            //saved or last entered noteID
            saveNoteID = noteID;

            if (noteID >= 60 && noteID <= 89)
            {
                int index = noteID - 60;
                DarkObject[index].SetActive(true);

            }

            switch (noteID)
            {
                case 60:
                    C4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 61:
                    C_4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 62:
                    D4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 63:
                    D_4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 64:
                    E4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 65:
                    F4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 66:
                    F_4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 67:
                    G4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 68:
                    G_4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 69:
                    A4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 70:
                    A_4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 71:
                    B4.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 72:
                    C5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 73:
                    C_5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 74:
                    D5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 75:
                    D_5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 76:
                    E5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 77:
                    F5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 78:
                    F_5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 79:
                    G5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 80:
                    G_5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 81:
                    A5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 82:
                    A_5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 83:
                    B5.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 84:
                    C6.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 85:
                    C_6.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 86:
                    D6.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 87:
                    D_6.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 88:
                    E6.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;
                case 89:
                    F6.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
                    break;






            }

        }

        void UpdatePianoPitchNote()
        {



            if (savedNote != note)
            {
                // everything should be non active
                foreach (var darkobject in DarkObject)
                {
                    darkobject.SetActive(false);
                }
            }

            savedNote = note;

            NoteSwitch(note);


        }

        void NoteSwitch(string notekey)
        {
            switch (notekey)
            {
                case "C4":
                    debugKeyIDMethodCalled.text = $"Note in List - C4";
                    DarkObject[0].SetActive(true);

                    break;
                case "C#4":
                    DarkObject[1].SetActive(true);
                    break;
                case "D4":
                    debugKeyIDMethodCalled.text = $"Note in List - D4";
                    DarkObject[2].SetActive(true);
                    break;
                case "D#4":
                    DarkObject[3].SetActive(true);
                    break;
                case "E4":
                    DarkObject[4].SetActive(true);
                    break;
                case "F4":
                    DarkObject[5].SetActive(true);
                    break;
                case "F#4":
                    debugKeyIDMethodCalled.text = $"Note in List - F#4";
                    DarkObject[6].SetActive(true);
                    break;
                case "G4":
                    DarkObject[7].SetActive(true);
                    break;
                case "G#4":
                    DarkObject[8].SetActive(true);
                    break;
                case "A4":
                    DarkObject[9].SetActive(true);
                    break;
                case "A#4":
                    DarkObject[10].SetActive(true);
                    break;
                case "B4":
                    DarkObject[11].SetActive(true);
                    break;
                case "C5":
                    DarkObject[12].SetActive(true);
                    break;
                case "C#5":
                    DarkObject[13].SetActive(true);
                    break;
                case "D5":
                    DarkObject[14].SetActive(true);
                    break;
                case "D#5":
                    DarkObject[15].SetActive(true);
                    break;
                case "E5":
                    DarkObject[16].SetActive(true);
                    break;
                case "F5":
                    DarkObject[17].SetActive(true);
                    break;
                case "F#5":
                    DarkObject[18].SetActive(true);
                    break;
                case "G5":
                    DarkObject[19].SetActive(true);
                    break;
                case "G#5":
                    DarkObject[20].SetActive(true);
                    break;
                case "A5":
                    DarkObject[21].SetActive(true);
                    break;
                case "A#5":
                    DarkObject[22].SetActive(true);
                    break;
                case "B5":
                    DarkObject[23].SetActive(true);
                    break;
                case "C6":
                    DarkObject[24].SetActive(true);
                    break;
                case "C#6":
                    DarkObject[25].SetActive(true);
                    break;
                case "D6":
                    DarkObject[26].SetActive(true);
                    break;
                case "D#6":
                    DarkObject[27].SetActive(true);
                    break;
                case "E6":
                    DarkObject[28].SetActive(true);
                    break;
                case "F6":
                    DarkObject[29].SetActive(true);
                    break;








            }
        }


}
