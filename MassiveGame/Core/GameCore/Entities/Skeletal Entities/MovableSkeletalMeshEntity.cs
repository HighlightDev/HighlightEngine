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
using MassiveGame.Core.RenderCore;
using ShaderPattern;
using VBO;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class MovableSkeletalMeshEntity : MovableEntity
    {
        private List<AnimationSequence> m_animations;
        private AnimationHolder m_animationHolder;

        class TestAnimationNodeShader : ShaderBase
        {
            private Uniform u_worldMatrix, u_viewMatrix, u_projectionMatrix;
            private Uniform[] u_skeletonMatrices;

            public TestAnimationNodeShader() : base() { }

            public TestAnimationNodeShader(string vsPath, string fsPath, string gsPath) : base("Test Skeleton Node Shader", vsPath, fsPath, gsPath) { }

            protected override void getAllUniformLocations()
            {
                try
                {
                    u_worldMatrix = GetUniform("worldMatrix");
                    u_viewMatrix = GetUniform("viewMatrix");
                    u_projectionMatrix = GetUniform("projectionMatrix");
                    u_skeletonMatrices = new Uniform[40];
                    for (Int32 i = 0; i < u_skeletonMatrices.Length; i++)
                    {
                        u_skeletonMatrices[i] = GetUniform(String.Format("skeletonMatrices[{0}]", i));
                    }
                }
                catch (ArgumentNullException ex)
                {

                }
            }

            protected override void SetShaderMacros() { }

            public void SetTransformationMatrices(ref Matrix4 worldMatrix, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
            {
                u_worldMatrix.LoadUniform(ref worldMatrix);
                u_viewMatrix.LoadUniform(ref viewMatrix);
                u_projectionMatrix.LoadUniform(ref projectionMatrix);
            }

            public void SetSkeletonMatrices(Matrix4[] skeletonMatrices)
            {
                for (Int32 i = 0; i < skeletonMatrices.Length; ++i)
                {
                    u_skeletonMatrices[i].LoadUniform(ref skeletonMatrices[i]);
                }
            }
        }

        private VertexArrayObject m_skeletonVAO;

        private TestAnimationNodeShader m_skeletonShader;

        public MovableSkeletalMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
          base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            m_animations = PoolProxy.GetResource<ObtainAnimationPool, AnimationAllocationPolicy, string, List<AnimationSequence>>(modelPath);
            m_animationHolder = new AnimationHolder(m_animations);
            m_animationHolder.SetAnimationByNameNoBlend(m_animations[0].GetName());

            m_skeletonShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<TestAnimationNodeShader>, string, TestAnimationNodeShader>
                (String.Format("{0}{1},{0}{2},{0}{3}", ProjectFolders.ShadersPath, "skeletonVS.glsl", "skeletonFS.glsl", "skeletonGS.glsl"));

            m_skeletonVAO = new VertexArrayObject();
            VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(new float[1, 3] { { 0, 0, 0 } }, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            m_skeletonVAO.AddVBO(verticesVBO);
            m_skeletonVAO.BindBuffersToVao();
        }

        private SkeletalMeshEntityShader GetShader()
        {
            return m_shader as SkeletalMeshEntityShader;
        }

        private AnimatedSkin GetSkin()
        {
            var skin = m_skin as AnimatedSkin;
            if (skin == null)
                throw new ArgumentException("Model that is loaded doesn't support animation.");

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
            Matrix4 currentBoneMatrix = srcTransformation[parentBone.GetId()].GetLocalOffsetMatrix() * parentMatrix;
            dstMatrices.Add(currentBoneMatrix);
            foreach (var bone in parentBone.GetChildren())
            {
                CollectAnimatedMatrices(bone, currentBoneMatrix, srcTransformation, ref dstMatrices);
            }
        }

        public override void RenderEntity(PrimitiveType mode, bool bEnableNormalMapping, DirectionalLight Sun, List<PointLight> lights, BaseCamera camera, ref Matrix4 projectionMatrix, Vector4 clipPlane = default(Vector4))
        {
            var worldMatrix = GetWorldMatrix();
            var viewMatrix = camera.GetViewMatrix();
            
            List<BoneTransformation> relevantBoneTransformations = m_animationHolder.GetAnimationSkinningTransformations();

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

            //bool bResult = GetSkin().GetRootBone().SetSkeletonUpdatedTransforms(relevantBoneTransformations);
            //if (!bResult)
            //    throw new ArgumentOutOfRangeException("Too much matrices for bones");

            GetShader().startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);
            GetShader().SetAlbedoTexture(0);
            GetShader().SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            GetShader().SetSkinningMatrices(skinningMatrices);
            GetSkin().Buffer.RenderVAO(PrimitiveType.Triangles);
            GetShader().stopProgram();

            //m_skeletonShader.startProgram();
            //m_skeletonShader.SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            //m_skeletonShader.SetSkeletonMatrices(GetSkin().GetRootBone().GetAlignedWithIdListOffsetMatrices().ToArray());
            //GL.PointSize(10);
            //m_skeletonVAO.RenderVAO(PrimitiveType.Points);
            //m_skeletonShader.stopProgram();

            m_animationHolder.UpdateAnimationLoopTime(0.005f);
        }
    }
}
