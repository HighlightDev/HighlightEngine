using MassiveGame.RenderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.PostFX
{
    public interface PostProcessSubsequenceType { };
    public interface ApplySubsequentPostProcessResult : PostProcessSubsequenceType { };
    public interface DiscardSubsequentPostProcessResult : PostProcessSubsequenceType { };

    public class PostProcessShaderBase<T> : ShaderBase where T : PostProcessSubsequenceType
    {
        protected readonly PostProcessSubsequenceType_Inner PreviousPostProcessResult = typeof(T) == typeof(ApplySubsequentPostProcessResult) ?
            PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess : PostProcessSubsequenceType_Inner.DiscardPreviousPostProcess;

        Int32 previousPostProcessResultSampler = -1;

        public PostProcessShaderBase(string shaderName, string VertexShaderFile, string FragmentShaderFile)
            : base(shaderName, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            if (PreviousPostProcessResult == PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess)
            {
                previousPostProcessResultSampler = getUniformLocation("previousPostProcessResultSampler");
            }
        }

        public void SetPreviousPostProcessResultSampler(Int32 prevPostProcessResultSampler)
        {
            if (PreviousPostProcessResult == PostProcessSubsequenceType_Inner.ApplyPreviousPostProcess)
                loadInteger(previousPostProcessResultSampler, prevPostProcessResultSampler);
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
