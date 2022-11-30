using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectionScript : MonoBehaviour
{

    public Button courseTaskSceneButton;
    public Button addSongsButton;
    public Button odeToJoySongButton;
    public Button billySongButton;
    public Button thingySongButton;
    public Button adeleEasyOnMe;


    public static string midifiledir;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        courseTaskSceneButton.onClick.AddListener(() => { CourseTaskClick();});
        addSongsButton.onClick.AddListener(() => { AddSongClick();});
        odeToJoySongButton.onClick.AddListener(() => { OdeToJoyClick(); });
        billySongButton.onClick.AddListener(() => { BillyJoelClicked();});
        thingySongButton.onClick.AddListener(() => { ThingyClick();});
        adeleEasyOnMe.onClick.AddListener(() => { AdeleEasyOnMeClick();});
        
        
        
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    private void FadedClicked()
    {

        //midifiledir = GetMidiFileDir("AlanWalkerFaded");
        midifiledir = "AlanWalkerFaded";
        SceneManager.LoadScene(ConstantStrings.PlayMidiOnPiano);
    }
    
    private void BillyJoelClicked()
    {

        //midifiledir = GetMidiFileDir("AlanWalkerFaded");
        midifiledir = "BillyJoel";
        SceneManager.LoadScene(ConstantStrings.PlayMidiOnPiano);
    }
    
    

    private void ThingyClick()
    {

        //midifiledir = GetMidiFileDir("JadonPianoThingy");
        
        midifiledir = "JadonPianoThingy";
        SceneManager.LoadScene(ConstantStrings.PlayMidiOnPiano);
    }

    private void OdeToJoyClick()
    {
        //midifiledir = GetMidiFileDir("OdeToJoy");

        midifiledir = "OdeToJoy";
        SceneManager.LoadScene(ConstantStrings.PlayMidiOnPiano);
    }
 
    private void AdeleEasyOnMeClick()
    {
        //midifiledir = GetMidiFileDir("OdeToJoy");

        midifiledir = "AdeleEasyOnMe";
        SceneManager.LoadScene(ConstantStrings.PlayMidiOnPiano);
    }

    private void AddSongClick()
    {
      
    }

    private void CourseTaskClick()
    {
        SceneManager.LoadScene(ConstantStrings.CourseTaskScene);
    }


    string GetMidiFileDir(string filename)
    {
        string midifilename = filename;
        
        string midifilePath = string.Format("{0}/MIDI/{1}.mid", Application.streamingAssetsPath, midifilename);

        string oriPath = midifilePath;
  
        // Android only use WWW to read file
        WWW reader = new WWW(oriPath);
        while ( ! reader.isDone) {}
  
        string realPath = Application.persistentDataPath + "/db";
        System.IO.File.WriteAllBytes(realPath, reader.bytes);
  
        string dbPath = realPath;

        return realPath;
    }

}
