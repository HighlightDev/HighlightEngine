using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using TextureLoader;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.DepthOfField
{
    public class DepthOfFieldPostProcess<SubsequenceType> : PostProcessBase
        where SubsequenceType : PostProcessSubsequenceType
    {
        #region Definitions
       
        private float blurStartEdge;
        public float BlurStartEdge
        {
            set
            {
                blurStartEdge = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
            get { return blurStartEdge; }
        }

        private float blurEndEdge;
        public float BlurEndEdge
        {
            set
            {
                blurEndEdge = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
            get { return blurEndEdge; }
        }

        private DepthOfFieldFramebufferObject renderTarget;
        private DepthOfFieldShader<SubsequenceType> dofShader;

        #endregion

        public DepthOfFieldPostProcess()
            : base()
        {
            this.BlurWidth = 8;
            this.BlurStartEdge = 0.5f;
            this.BlurEndEdge = 1f;
            this.BlurPassCount = 1;
        }

        private void PostConstructor()
        {
            if (bPostConstructor)
            {
                renderTarget = new DepthOfFieldFramebufferObject();
                dofShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<DepthOfFieldShader<SubsequenceType>>, string, DepthOfFieldShader<SubsequenceType>>(ProjectFolders.ShadersPath + "depthOfFieldVS.glsl" + "," + ProjectFolders.ShadersPath + "depthOfFieldFS.glsl");

                bPostConstructor = false;
            }
        }

        public override ITexture GetPostProcessResult(ITexture frameColorTexture, ITexture frameDepthTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            PostConstructor();

            if (this.blurWidthChanged)
            {
                this.blurWeights = normalizedWeights(BlurWidth);
                this.blurWidthChanged = false;
            }

            GL.Disable(EnableCap.DepthTest);
            renderTarget.renderToFBO(1, renderTarget.VerticalBlurTexture.GetTextureRezolution());
            dofShader.startProgram();
            frameColorTexture.BindTexture(TextureUnit.Texture0);
            dofShader.setDownsamplerUniforms(0);
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            dofShader.stopProgram();
          
            /*Gauss blur*/
            for (Int32 i = 0; i < BlurPassCount; i++)
            {
                /*Horizontal blur of image*/
                renderTarget.renderToFBO(2, renderTarget.HorizontalBlurTexture.GetTextureRezolution());
                dofShader.startProgram();
                renderTarget.VerticalBlurTexture.BindTexture(TextureUnit.Texture0);
                dofShader.setHorizontalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.HorizontalBlurTexture.GetTextureRezolution());
                quadBuffer.RenderVAO(PrimitiveType.Triangles);
                dofShader.stopProgram();

                /*Vertical blur of image*/
                renderTarget.renderToFBO(1, renderTarget.VerticalBlurTexture.GetTextureRezolution());
                dofShader.startProgram();
                renderTarget.HorizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                dofShader.setVerticalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.VerticalBlurTexture.GetTextureRezolution());
                quadBuffer.RenderVAO(PrimitiveType.Triangles);
                dofShader.stopProgram();
            }

            // Blend DoF post process result with previous post process result, if such exists
            renderTarget.renderToFBO(3, renderTarget.DepthOfFieldResultTexture.GetTextureRezolution());
            dofShader.startProgram();

            if (previousPostProcessResult != null)
            {
                previousPostProcessResult.BindTexture(TextureUnit.Texture2);
                dofShader.SetPreviousPostProcessResultSampler(2);
            }
            else
            {
                frameColorTexture.BindTexture(TextureUnit.Texture2);
                dofShader.SetFrameTextureSampler(2);
            }

            renderTarget.VerticalBlurTexture.BindTexture(TextureUnit.Texture0);
            frameDepthTexture.BindTexture(TextureUnit.Texture1);
            dofShader.setDoFUniforms(0, 1, BlurStartEdge, BlurEndEdge);
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            dofShader.stopProgram();

            renderTarget.unbindFramebuffer();
            GL.Enable(EnableCap.DepthTest);

            return renderTarget.DepthOfFieldResultTexture;
        }

        public override void CleanUp()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<DepthOfFieldShader<SubsequenceType>>, string, DepthOfFieldShader<SubsequenceType>>(dofShader);
            renderTarget.cleanUp();
        }
    }
}
