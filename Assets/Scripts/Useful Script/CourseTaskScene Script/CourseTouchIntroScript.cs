using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CourseTouchIntroScript : MonoBehaviour
{
    
    public Button songsSceneButton;
    public Button changeCourse;
    public Button first3NoteButton;
    public Button theNoteF;
    public Button mainChallange;
    
    
   
    
    //Self
    public static CourseTouchIntroScript Instance;

    public static int courseType = -1;
    
    
    // Start is called before the first frame update
    void Start()
    {

        Instance = this;
        
        songsSceneButton.onClick.AddListener(() => {SongSceneButtonClick();});
        changeCourse.onClick.AddListener(() => { SceneManager.LoadScene(ConstantStrings.CourseTaskScene);});
        first3NoteButton.onClick.AddListener(() => { First3NoteClick();});
        theNoteF.onClick.AddListener(() => { TheNoteFClick();});
        mainChallange.onClick.AddListener(() => { MainChallenge();});
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SongSceneButtonClick()
    {
        SceneManager.LoadScene(ConstantStrings.SongSelection);
    }

    void First3NoteClick()
    {


        courseType = 0;
        SceneManager.LoadScene(ConstantStrings.CourseTouchPlayScene);
        
       
        
    }

    void TheNoteFClick()
    {
        
        
        courseType = 1;
        SceneManager.LoadScene(ConstantStrings.CourseTouchPlayScene);
     
       
    }

    void MainChallenge()
    {
        
        courseType = 2;
        SceneManager.LoadScene(ConstantStrings.CourseTouchPlayScene);
        
        
       
    }
    



}
