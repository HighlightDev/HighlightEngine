using MassiveGame.Core.AnimationCore;
using MassiveGame.Core.GameCore.Entities.MoveEntities;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        // this is the root joint, it is the parent of all child joints
        private Joint rootJoint;

        public MovableSkeletalMeshEntity()
        {
            rootJoint = new Joint(1);
        }
    }
}
