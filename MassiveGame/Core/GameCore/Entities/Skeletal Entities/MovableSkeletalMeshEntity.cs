using MassiveGame.Core.GameCore.Entities.MoveEntities;
using System.Collections.Generic;
using OpenTK;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.Mesh;
using System;
using MassiveGame.Core.AnimationCore;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.Pools;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool.PoolHandling;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        private List<AnimationSequence> m_animations;

        public MovableSkeletalMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
          base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            m_animations = PoolProxy.GetResource<ObtainAnimationPool, AnimationAllocationPolicy, string, List<AnimationSequence>>(modelPath);
            GetSkin();
        }

        private AnimatedSkin GetSkin()
        {
            var skin = m_skin as AnimatedSkin;
            if (skin == null)
                throw new ArgumentException("Model you have loaded doesn't support animation.");

            return skin;
        }

        private void postConstructor()
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
            
            // send to shader
            // render 
        }
    }
}
