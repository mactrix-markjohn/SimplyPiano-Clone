using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        /*if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }*/

        
        
        StartCoroutine(SplashWait());
    }


    IEnumerator SplashWait()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(ConstantStrings.CourseTaskScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
