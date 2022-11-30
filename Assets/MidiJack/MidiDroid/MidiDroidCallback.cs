using System;
using UnityEngine;

namespace MidiJack
{
    public class MidiDroidCallback: AndroidJavaProxy
    {
        
        
        public MidiDroidCallback(string javaInterface) : base(javaInterface)
        {
        }

        public MidiDroidCallback(AndroidJavaClass javaInterface) : base(javaInterface)
        {
        }
        
        /*public MidiDroidCallback() : base("mmmlabs.com.mididroid.MidiCallback")
        {
           
        }*/
        
        
        public delegate void RawMidiDelegate(object sender, MidiMessage m);
        public event RawMidiDelegate DroidMidiEvent;
        


       

        public void MidiJackMessage(int device, int status, int data1, int data2)
        {
            DebugMidiCode.check2 = "1 -- Inside midiDroidJackMessage";

            DebugMidiCode.check7 = $"Device number: {device}";

            
            // check if a midi cable is conneect and make MidiNoteReceptor.isMidiCableConnected True, once the user click on the keboard.
            if (device < 17)
            {

                //TODO : MidiNoteReceptor.isMidiCableConnected = true;
            }

            try
            {
                
                DebugMidiCode.check3 = "1 -- Before DroidMidiEvent called";
                
                DroidMidiEvent(this, new MidiMessage((uint)device, (byte)status, (byte)data1, (byte)data2));
               


            }
            catch (Exception e)
            {
                if(DroidMidiEvent != null)
                {
                    DebugMidiCode.check3 = "1 -- Before DroidMidiEvent called";
                
                    DroidMidiEvent(this, new MidiMessage((uint)device, (byte)status, (byte)data1, (byte)data2));
                    
                }
            }
            
            
            
           


        }

       
    }
}