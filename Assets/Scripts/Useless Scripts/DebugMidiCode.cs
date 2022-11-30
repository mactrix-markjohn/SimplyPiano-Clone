using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMidiCode : MonoBehaviour
{

    public Text text;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public Text text5;
    public Text text6;
    public Text text7;



    public static string check1 = "0";
    public static string check2 = "0";
    public static string check3 = "0";
    public static string check4 = "0";
    public static string check5 = "0";
    public static string check6 = "0";
    public static string check7 = "0";


    public static int keynumber = -1;
    
    List<string> storeDebugString;
    
    // Start is called before the first frame update
    void Start()
    {
        storeDebugString = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {

        text.text = check1;
        text1.text = check2;
        text2.text = check3;
        text3.text = check4;
        text4.text = check5;
        text5.text = check6;
        text6.text = check7;

        if (!storeDebugString.Contains(check7))
        {
            storeDebugString.Add(check7);

            text6.text = string.Join("\n", storeDebugString);
        }


        text7.text = $"Piano Key number: {keynumber}";

    }
}
