using MassiveGame.Core.RenderCore;
using ShaderPattern;
using OpenTK;
using System;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class SkeletalMeshEntityShader : ShaderBase
    {
        private const string ShaderName = "SkeletalMesh Shader";
        private const Int32 MaxWeigths = 3;

        private Uniform u_worldMatrix, u_viewMatrix, u_projectionMatrix, u_albedoTexture;
        private Uniform[] u_bonesMatrices = new Uniform[MaxWeigths];

        public SkeletalMeshEntityShader() : base() { }

        public SkeletalMeshEntityShader(string vsPath, string fsPath) : base(ShaderName, vsPath, fsPath) { }

        protected override void getAllUniformLocations()
        {
            u_worldMatrix = GetUniform("worldMatrix");
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_albedoTexture = GetUniform("albedoTexture");
            //for (Int32 i = 0; i < MaxWeigths; i++)
            //    u_bonesMatrices[i] = GetUniform("bonesMatrices[" + i + "]");
        }

        public void SetTransformationMatrices(ref Matrix4 modelMatrix, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            u_worldMatrix.LoadUniform(ref modelMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetSkinningMatrices(Matrix4[] matrices)
        {
            for (Int32 i = 0; i < MaxWeigths; i++)
            {
                u_bonesMatrices[i].LoadUniform(ref matrices[i]);
            }
        }

        public void SetAlbedoTexture(Int32 textureSampler)
        {
            u_albedoTexture.LoadUniform(textureSampler);
        }

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader, "MaxWeigths", MaxWeigths);
        }
    }
}
