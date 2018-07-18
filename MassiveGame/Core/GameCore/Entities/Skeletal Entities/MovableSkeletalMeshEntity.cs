using MassiveGame.Core.AnimationCore;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using OpenTK;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        // this is the root joint, it is the parent of all child joints
        private Joint rootJoint;

        void postConstructor()
        {
            if (bPostConstructor)
            {
                
                

                bPostConstructor = false;
            }
        }

        public MovableSkeletalMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, Speed, translation, rotation, scale)
        {
            rootJoint = new Joint(1);
        }
    }
}
