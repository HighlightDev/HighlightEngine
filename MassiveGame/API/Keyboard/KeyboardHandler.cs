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
        private Dictionary<Keys, bool> keyboardMaskMap;

        public KeyboardHandler()
        {
            keyboardMaskMap = new Dictionary<Keys, bool>();
        }

        public void AllocateKey(Keys key)
        {
            keyboardMaskMap.Add(key, false);
        }

        public void KeyPress(Keys key)
        {
            if (keyboardMaskMap.ContainsKey(key))
                keyboardMaskMap[key] = true;
        }

        public void KeyRelease(Keys key)
        {
            if (keyboardMaskMap.ContainsKey(key))
                keyboardMaskMap[key] = false;
        } 
        
        public bool GetKeyState(Keys key)
        {
            bool bEnabled = false;

            if (keyboardMaskMap.ContainsKey(key))
                bEnabled = keyboardMaskMap[key];

            return bEnabled;
        }  
    }
}
