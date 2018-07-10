using MassiveGame.API.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Core
{
    public class PlayerController
    {
        private ThirdPersonCamera mainCamera;
        private MovableEntity player;
        private KeyboardHandler keyboard;

        private void SetBindingKeyboardKey(char key)
        {
            
        }

        private void InitBindingsKeyboardKeys()
        {
            
        }

        public PlayerController(MovableEntity player)
        {
            this.player = player;
        }
    }
}
