using MassiveGame.Core.RenderCore;
using ShaderPattern;
using OpenTK;
using System;

namespace MassiveGame.Core.GameCore.Entities.Skeletal_Entities
{
    public class SkeletalMeshEntityShader : ShaderBase
    {
        private const string ShaderName = "SkeletalMesh Shader";
        private const Int32 MaxWeights = 3;
        private const Int32 MaxBones = 55;

        private Uniform u_worldMatrix, u_viewMatrix, u_projectionMatrix, u_albedoTexture;
        private Uniform[] u_bonesMatrices = new Uniform[MaxBones];

        public SkeletalMeshEntityShader() : base() { }

        public SkeletalMeshEntityShader(string vsPath, string fsPath) : base(ShaderName, vsPath, fsPath) { }

        protected override void getAllUniformLocations()
        {
            try
            {
                u_worldMatrix = GetUniform("worldMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_albedoTexture = GetUniform("albedoTexture");
                for (Int32 i = 0; i < MaxBones; i++)
                    u_bonesMatrices[i] = GetUniform("bonesMatrices[" + i + "]");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        public void SetTransformationMatrices(ref Matrix4 worldMatrix, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            u_worldMatrix.LoadUniform(ref worldMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetSkinningMatrices(Matrix4[] matrices)
        {
            for (Int32 i = 0; i < matrices.Length; i++)
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
            SetDefine<Int32>(ShaderTypeFlag.VertexShader, "MaxWeights", MaxWeights);
            SetDefine<Int32>(ShaderTypeFlag.VertexShader, "MaxBones", MaxBones);
        }
    }
}
