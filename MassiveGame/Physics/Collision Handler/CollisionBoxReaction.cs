using MassiveGame.API.IDgenerator;
using PhysicsBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Physics.Collision_Handler
{
    public class CollisionBoxReaction : IReaction
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
                        DOUEngine.Player.popPositionStack();
                    }
                    else
                    {
                        DOUEngine.Enemy.popPositionStack();
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
