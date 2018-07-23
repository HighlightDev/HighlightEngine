using MassiveGame.Core.AnimationCore;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using System.Collections.Generic;
using OpenTK;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        // skeleton
        // this is the root bone, it is the parent of all child bones
        private Bone rootBone;

        public MovableSkeletalMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
          base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            rootBone = new Bone(1, "root");
        }

        void postConstructor()
        {
            if (bPostConstructor)
            {

                bPostConstructor = false;
            }
        }

        public override void RenderEntity(PrimitiveType mode, bool bEnableNormalMapping, DirectionalLight Sun, List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane = default(Vector4))
        {
            // say hello to my little friend!

            var worldMatrix = GetWorldMatrix();
            var viewMatrix = camera.GetViewMatrix();
            var skeletonSkinningMatrices = GetSkeletonSkinningMatrices();
            
            // send to shader
            // render 
        }

        private List<Matrix4> GetSkeletonSkinningMatrices()
        {
            return rootBone.GetAlignedWithIdSkinningMatrices();
        }
    }
}
