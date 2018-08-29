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
using MassiveGame.Settings;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        private List<AnimationSequence> m_animations;
        private AnimationHolder m_animationHolder;

        public MovableSkeletalMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
          base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            m_animations = PoolProxy.GetResource<ObtainAnimationPool, AnimationAllocationPolicy, string, List<AnimationSequence>>(modelPath);
            m_animationHolder = new AnimationHolder(m_animations);
            m_animationHolder.SetAnimationByNameNoBlend(m_animations[0].GetName());
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
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<SkeletalMeshEntityShader>, string, SkeletalMeshEntityShader>(GetShader());
        }

        protected override void InitShader()
        {
            m_shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<SkeletalMeshEntityShader>, string, SkeletalMeshEntityShader>(ProjectFolders.ShadersPath + "skeletalMeshVS.glsl" + "," + ProjectFolders.ShadersPath + "skeletalMeshFS.glsl");
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                
                bPostConstructor = false;
            }
        }

        // This is just for another one attempt to start animation working and ... now it works!!
        private void CollectAnimatedMatrices(Bone parentBone, Matrix4 parentMatrix, List<BoneTransformation> srcTransformation, ref List<Matrix4> dstMatrices)
        {
            Matrix4 currentBoneMatrix = srcTransformation[parentBone.GetBoneId()].GetLocalOffsetMatrix() * parentMatrix;
            dstMatrices.Add(currentBoneMatrix);
            foreach (var bone in parentBone.GetBoneChildren())
            {
                CollectAnimatedMatrices(bone, currentBoneMatrix, srcTransformation, ref dstMatrices);
            }
        }

        public override void RenderEntity(PrimitiveType mode, bool bEnableNormalMapping, DirectionalLight Sun, List<PointLight> lights, BaseCamera camera, ref Matrix4 projectionMatrix, Vector4 clipPlane = default(Vector4))
        {
            var worldMatrix = GetWorldMatrix();
            var viewMatrix = camera.GetViewMatrix();
            
            List<BoneTransformation> relevantBoneTransformations = m_animationHolder.GetAnimatedPoseTransformsList();

            List<Matrix4> animatedMatrices = new List<Matrix4>();
            CollectAnimatedMatrices(GetSkin().GetRootBone(), Matrix4.Identity, relevantBoneTransformations, ref animatedMatrices);

            List<Matrix4> offsetBones = GetSkin().GetRootBone().GetAlignedWithIdListOffsetMatrices();

            Matrix4[] skinningMatrices = new Matrix4[animatedMatrices.Count];

            for (Int32 i = 0; i < animatedMatrices.Count; i++)
            {
                Matrix4 animatedBoneMatrix = animatedMatrices[i];
                Matrix4 offsetBoneMatrix = offsetBones[i];

                skinningMatrices[i] = offsetBoneMatrix * animatedBoneMatrix;
            }

            GetShader().startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);
            GetShader().SetAlbedoTexture(0);
            GetShader().SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            GetShader().SetSkinningMatrices(skinningMatrices);
            GetSkin().Buffer.RenderVAO(PrimitiveType.Triangles);
            GetShader().stopProgram();

            m_animationHolder.UpdateAnimationLoopTime(0.005f);
        }
    }
}
