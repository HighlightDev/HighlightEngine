using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Core
{
    public class PlayerController
    {
        public Camera playerCamera;
        public MovableEntity player;

        public PlayerController(MovableEntity player)
        {
            this.player = player;
        }
    }
}
