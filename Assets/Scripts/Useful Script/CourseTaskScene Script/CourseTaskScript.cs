using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CourseTaskScript : MonoBehaviour
{
    public Button songsSceneButton;
    public Button courseTaskTouchIntroButton;
    public Button courseTaskStayInTouchButton;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        songsSceneButton.onClick.AddListener(() => {SongSceneButtonClick();});
        courseTaskTouchIntroButton.onClick.AddListener(() => {CourseTaskTouchIntroClick();});
        courseTaskStayInTouchButton.onClick.AddListener(() => { CourseTaskStayInTouchClick();});
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void SongSceneButtonClick()
    {
        SceneManager.LoadScene(ConstantStrings.SongSelection);
    }

    void CourseTaskTouchIntroClick()
    {
        //TODO: Load a Scene to a practice scene for the course task

        SceneManager.LoadScene(ConstantStrings.CourseTouchIntroScene);
    }

    void CourseTaskStayInTouchClick()
    {
        //TODO: Load a Scene to a practice scene for the course task 
        _ShowAndroidToastMessage("Stay In Touch Task Coming soon... ");
    }
    
    
    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass =
                new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity,
                        message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
