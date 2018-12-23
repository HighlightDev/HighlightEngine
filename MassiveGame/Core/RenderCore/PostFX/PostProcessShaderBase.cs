using MassiveGame.Core.DebugCore;
using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore.PostFX
{
    public interface PostProcessSubsequenceType { };
    public interface ApplySubsequentPostProcessResult : PostProcessSubsequenceType { };
    public interface DiscardSubsequentPostProcessResult : PostProcessSubsequenceType { };

    public class PostProcessShaderBase<SubsequenceType> : ShaderBase
        where SubsequenceType : PostProcessSubsequenceType
    {
        protected readonly PostProcessSubsequenceType_Inner PreviousPostProcessResult = typeof(SubsequenceType) == typeof(ApplySubsequentPostProcessResult) ?
            PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess : PostProcessSubsequenceType_Inner.DiscardPreviousPostProcess;

        private Uniform u_previousPostProcessResultSampler;

        public PostProcessShaderBase() : base() { }

        public PostProcessShaderBase(string shaderName, string VertexShaderFile, string FragmentShaderFile)
            : base(shaderName, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void getAllUniformLocations()
        {
            try
            {
                base.getAllUniformLocations();
                if (PreviousPostProcessResult == PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess)
                {
                    u_previousPostProcessResultSampler = GetUniform("previousPostProcessResultSampler");
                }
            }
            catch (ArgumentNullException innerException)
            {
                Log.AddToFileStreamLog(innerException.Message);
                Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        public void SetPreviousPostProcessResultSampler(Int32 prevPostProcessResultSampler)
        {
            if (PreviousPostProcessResult == PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess)
                u_previousPostProcessResultSampler.LoadUniform(prevPostProcessResultSampler);
        }

        protected override void SetShaderMacros()
        {
            Int32 bHasPreviousStage = 0;
            if (PreviousPostProcessResult == PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess)
                bHasPreviousStage = 1;

            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "HAS_PREVIOUS_STAGE", bHasPreviousStage);
        }

        protected enum PostProcessSubsequenceType_Inner
        {
            ApplyPreviousPostProcess = 0,
            DiscardPreviousPostProcess = 1
        }
    }
}
