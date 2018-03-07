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
        private Dictionary<Keys, bool> bitmap;

        public KeyboardHandler()
        {
            bitmap = new Dictionary<Keys, bool>();
        }

        public void SetBit(Keys key, bool bit)
        {
            bitmap[key] = bit;
        }

        public bool GetBit(Keys key)
        {
            return bitmap[key];
        }
    }
}
