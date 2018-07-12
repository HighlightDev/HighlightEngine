using System.Collections.Generic;
using System.Windows.Forms;

namespace MassiveGame.API.Keyboard
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
