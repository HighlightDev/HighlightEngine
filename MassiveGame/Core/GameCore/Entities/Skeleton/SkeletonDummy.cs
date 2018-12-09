using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.Core.AnimationCore;
using MassiveGame.Settings;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using VBO;

namespace MassiveGame.Core.GameCore.Entities.Skeleton
{
    public class SkeletonDummy
    {
        private SkeletonShader m_skeletonShader;
        private VertexArrayObject m_skeletonVAO;

        private List<AnimationSequence> m_animations;
        private AnimationHolder m_animationHolder;

        public SkeletonDummy(string pathToAnimatedMesh)
        {
            m_skeletonShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<SkeletonShader>, string, SkeletonShader>
              (String.Format("{0}{1},{0}{2},{0}{3}", ProjectFolders.ShadersPath, "skeletonVS.glsl", "skeletonFS.glsl"));

            m_skeletonVAO = new VertexArrayObject();
            InitBonesVAO();

            m_animations = PoolProxy.GetResource<GetAnimationPool, AnimationAllocationPolicy, string, List<AnimationSequence>>(pathToAnimatedMesh);
            m_animationHolder = new AnimationHolder(m_animations);
            m_animationHolder.SetAnimationByNameNoBlend(m_animations[0].GetName());
        }

        private void InitBonesVAO()
        {
            Int32 bonesCount = m_animationHolder.GetCurrentSequence().GetBonesCount();

            VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(new float[bonesCount, 3], BufferTarget.ArrayBuffer, 
                0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            Int32[] boneIndices = new Int32[bonesCount];
            for (Int32 i = 0; i < bonesCount; i++)
                boneIndices[i] = i;

            VertexBufferObjectOneDimension<Int32> boneIndicesVBO = new VertexBufferObjectOneDimension<Int32>(boneIndices, BufferTarget.ArrayBuffer,
                1, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            m_skeletonVAO.AddVBO(verticesVBO, boneIndicesVBO);
            m_skeletonVAO.BindBuffersToVao();
        }

        public void RenderDummy(BaseCamera camera, ref Matrix4 ProjectionMatrix)
        {
            Matrix4 worldMatrix = Matrix4.Identity;
            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projectionMatrix = ProjectionMatrix;

            m_skeletonShader.startProgram();
            m_skeletonShader.SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            m_skeletonShader.SetSkeletonMatrices(m_animationHolder.GetAnimatedNotOffsetedPoseMatricesList().ToArray());
            GL.PointSize(10);
            m_skeletonVAO.RenderVAO(PrimitiveType.Points);
            m_skeletonShader.stopProgram();

            m_animationHolder.UpdateAnimationLoopTime(0.005f);
        }

        public void CleanUp()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<SkeletonShader>, string, SkeletonShader>(m_skeletonShader);
            PoolProxy.FreeResourceMemory<GetAnimationPool, AnimationAllocationPolicy, string, List<AnimationSequence>>(m_animations);
        }
    }
}
