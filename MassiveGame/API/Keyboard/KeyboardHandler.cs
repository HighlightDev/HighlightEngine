using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MassiveGame.API.EventHandlers
{
    public class KeyboardHandler
    {
        public void Event_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {

        }

        public void Event_KeyRelease(object sender, System.Windows.Forms.KeyEventArgs args)
        {

        }

        private static bool[] keyboardMaskArray;

        public KeyboardHandler()
        {
            keyboardMaskArray = new bool[5];
        }

        public bool this[Int32 i] { set { keyboardMaskArray[i] = value; } get { return keyboardMaskArray[i]; } }

        public bool[] GetWASDKeysMask()
        {
            bool[] moveMask = new bool[4];
            for (Int32 i = 0; i < 4;i ++)
            {
                moveMask[i] = keyboardMaskArray[i];
            }
            return moveMask;
        }  
        
        public bool GetSpacebarKeyMask()
        {
            return keyboardMaskArray[4];
        }    
    }
}
