using MassiveGame.Core.GameCore.Entities.MoveEntities;
using System.Collections.Generic;
using OpenTK;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.Mesh;
using System;
using MassiveGame.Core.AnimationCore;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.Core.SettingsCore;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        private List<AnimationSequence> m_animations;
        private AnimationHolder m_animationHolder;

        public MovableSkeletalMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
          base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            TryLoadAnimation(modelPath);
            m_animationHolder = new AnimationHolder(m_animations);
            m_animationHolder.SetAnimationByNameNoBlend(m_animations[0].GetName());
        }

        private bool TryLoadAnimation(string modelPath)
        {
            if (m_skin as AnimatedSkin == null)
                throw new ArgumentException("Mesh that is loaded doesn't support animation.");

            m_animations = PoolProxy.GetResource<GetAnimationPool, AnimationAllocationPolicy, string, List<AnimationSequence>>(modelPath);

            return true;
        }

        private SkeletalMeshEntityShader GetShader()
        {
            return m_shader as SkeletalMeshEntityShader;
        }

        private AnimatedSkin GetSkin()
        {
            var skin = m_skin as AnimatedSkin;
            if (skin == null)
                throw new ArgumentException("Mesh that is loaded doesn't support animation.");

            return skin;
        }

        protected override void FreeShader()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<SkeletalMeshEntityShader>, string, SkeletalMeshEntityShader>(GetShader());
        }

        protected override void InitShader()
        {
            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<SkeletalMeshEntityShader>, string, SkeletalMeshEntityShader>(ProjectFolders.ShadersPath + "skeletalMeshVS.glsl" + "," + ProjectFolders.ShadersPath + "skeletalMeshFS.glsl");
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                
                bPostConstructor = false;
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            m_animationHolder.UpdateAnimationLoopTime(deltaTime);
        }

        public override void RenderEntity(PrimitiveType mode, DirectionalLight Sun, List<PointLight> lights, BaseCamera camera, ref Matrix4 projectionMatrix, Vector4 clipPlane = default(Vector4))
        {
            var worldMatrix = GetWorldMatrix();
            var viewMatrix = camera.GetViewMatrix();

            var skinningMatrices = m_animationHolder.GetAnimatedOffsetedMatrices(GetSkin().GetRootBone());

            GetShader().startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);
            GetShader().SetAlbedoTexture(0);
            GetShader().SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            GetShader().SetSkinningMatrices(skinningMatrices);
            GetSkin().Buffer.RenderVAO(PrimitiveType.Triangles);
            GetShader().stopProgram();
        }
    }
}
