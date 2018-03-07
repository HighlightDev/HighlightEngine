using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collision_Handler
{
    public interface IReaction
    {
        void Reaction(object sender, EventArgs e);
    }
}
