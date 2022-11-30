using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SheetNoteSymbolScript : MonoBehaviour
{

    public int colornum = -1;
    public Image noteImage;

    public int noteID;
    public string notename;

    private GameObject theBlueIndicator;
    private GameObject theBlueIndicator1;
    private int noteIDRecieved;
    private string notenameReceived;

    private GameObject theStartPoint;
    private GameObject theEndPoint;

    private bool shouldmove =false;
    
    
    // Start is called before the first frame update
    void Start()
    {
     //  noteImage = transform.GetChild(0).GetComponent<Image>();
       
       
       
    }

    // Update is called once per frame
    void Update()
    {
        
       MoveNote();
       if(noteImage != null){
           if (colornum == 0)
           {
           
               noteImage.color = Color.black;


           }else if (colornum == 1)
           {

               noteImage.color = Color.green;

           }else if (colornum == 2)
           {

               noteImage.color = Color.yellow;

           }else if (colornum == 3)
           {
               noteImage.color = Color.red;
           }else if (colornum == 4)
           {
               noteImage.color = Color.cyan;

           }
       }


       

       




    }


   public  void Setup(GameObject BlueIndicate, GameObject BlueIndicate1, GameObject startpoint, GameObject endpoint)
    {
        theBlueIndicator = BlueIndicate;
        theBlueIndicator1 = BlueIndicate1;
        theEndPoint = endpoint;
        theStartPoint = startpoint;
    }


   
   /*void FixedUpdate()
   {
       MoveNote();
   }*/

   void MoveNote()
    {


        if (shouldmove)
        {
            if (theEndPoint != null)
            {
                //gameObject.transform.position = new Vector3(10*Time.fixedDeltaTime,theStartPoint.transform.position.y,theStartPoint.transform.position.z);
                    
                    
                    transform.position = Vector3.MoveTowards(gameObject.transform.position,
                    theEndPoint.transform.position
                    , Time.deltaTime*200f);
                    
                    
                    if (theBlueIndicator != null ) 
                    {
                        /*float distance = Vector3.Distance(theBlueIndicator.transform.position, Vector3.MoveTowards(gameObject.transform.position,
                            theEndPoint.transform.position
                            , Time.deltaTime*200f));*/

                        
                        float distance = theBlueIndicator.transform.position.x - Vector3.MoveTowards(
                                             gameObject.transform.position,
                                             theEndPoint.transform.position
                                             , Time.deltaTime * 200f).x;
                            
                        float dis =  theBlueIndicator.transform.position.x - transform.localPosition.x;
                        float dis1 = theBlueIndicator1.transform.position.x - transform.localPosition.x;

                       // MidiNoteReceptor.distance = (int)theBlueIndicator.transform.position.x - transform.localPosition.x;
                       
                        

                        if ( distance > 10f)
                        {
                            
                            /*dis >= 10f || dis1 >= 10f*/

                            int calcNoteID = (NoteReceptor.noteID - 20) + 1;
                            if (noteID == calcNoteID)
                            {
                                if (noteImage != null)
                                {
                                    noteImage.color = Color.green;
                                }
                            }
                            else
                            {
                                if (noteImage != null)
                                {
                                    noteImage.color = Color.red;
                                }
            
               
                            }
                        }
                    }

            }  
            
        }
        

/*

        if (MidiNoteReceptor.isMoveNoteSymbolActive)
        {
            float speed = (float)MidiNoteReceptor.tempo / 100f;

            transform.position = speed * Time.deltaTime * new  Vector2(-1,transform.position.y);

        }*/
    }

   public void ShouldMove(bool shouldMove)
   {
       shouldmove = shouldMove;
   }


}
