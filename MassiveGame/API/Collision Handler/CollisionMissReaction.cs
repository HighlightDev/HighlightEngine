using MassiveGame.API.IDgenerator;
using PhysicsBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collision_Handler
{
    public class CollisionMissReaction : IReaction
    {
        public void Reaction(object sender, EventArgs e)
        {
            CollisionSphereBox callBox = sender as CollisionSphereBox;
            try
            {
                if (IdGenerator.IsPlayerId(callBox.ID))
                {
                    var id = callBox.ID;

                    if (DOUEngine.Player.Box.ID == id)
                    {
                        DOUEngine.Player.pushPositionStack();
                    }
                    else
                    {
                        DOUEngine.Enemy.pushPositionStack();
                    }
                }
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

    }
}
