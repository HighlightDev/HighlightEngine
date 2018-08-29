using MassiveGame.Core.RenderCore;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.AnimationCore
{
    class SkeletonShader : ShaderBase
    {
        private Uniform u_worldMatrix, u_viewMatrix, u_projectionMatrix;
        private Uniform[] u_skeletonMatrices;

        public SkeletonShader() : base() { }

        public SkeletonShader(string vsPath, string fsPath) : base("Test Skeleton Node Shader", vsPath, fsPath) { }

        protected override void getAllUniformLocations()
        {
            try
            {
                u_worldMatrix = GetUniform("worldMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_skeletonMatrices = new Uniform[25];
                for (Int32 i = 0; i < u_skeletonMatrices.Length; i++)
                {
                    u_skeletonMatrices[i] = GetUniform(String.Format("skeletonMatrices[{0}]", i));
                }
            }
            catch (ArgumentNullException ex)
            {

            }
        }

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader, "MAX_BONE_COUNT", 25);
        }

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
}
