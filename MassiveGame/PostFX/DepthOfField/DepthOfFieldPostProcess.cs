﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System.Drawing;
using TextureLoader;
using MassiveGame.API.Collector;
using GpuGraphics;

namespace MassiveGame.PostFX.DepthOfField
{
    public class DepthOfFieldPostProcess<T> : PostProcessBase where T : PostProcessSubsequenceType
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
        private DepthOfFieldShader<T> dofShader;

        #endregion

        public DepthOfFieldPostProcess()
            : base()
        {
        }

        private void PostConstructor()
        {
            if (bPostConstructor)
            {
                renderTarget = new DepthOfFieldFramebufferObject();
                dofShader = (DepthOfFieldShader<T>)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "depthOfFieldVS.glsl",
                    ProjectFolders.ShadersPath + "depthOfFieldFS.glsl", "", typeof(DepthOfFieldShader<T>));
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

            // First blur pass is necessary, so I can easily enter blur cycle with result from blur in vertical blur render target
            GL.Disable(EnableCap.DepthTest);
            renderTarget.renderToFBO(1, renderTarget.VerticalBlurTexture.GetTextureRezolution());
            dofShader.startProgram();
            frameColorTexture.BindTexture(TextureUnit.Texture0);
            dofShader.setVerticalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.VerticalBlurTexture.GetTextureRezolution());
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);

            renderTarget.renderToFBO(2, renderTarget.HorizontalBlurTexture.GetTextureRezolution());
            renderTarget.VerticalBlurTexture.BindTexture(TextureUnit.Texture0);
            dofShader.setHorizontalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.VerticalBlurTexture.GetTextureRezolution());
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);

            /*Gauss blur*/
            for (Int32 i = 1; i < BlurPassCount; i++)
            {
                /*Vertical blur of bright parts of image*/
                renderTarget.renderToFBO(1, renderTarget.VerticalBlurTexture.GetTextureRezolution());
                dofShader.startProgram();
                renderTarget.HorizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                dofShader.setHorizontalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.HorizontalBlurTexture.GetTextureRezolution());
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);

                /*Horizontal blur of image*/
                renderTarget.renderToFBO(2, renderTarget.HorizontalBlurTexture.GetTextureRezolution());
                dofShader.startProgram();
                renderTarget.VerticalBlurTexture.BindTexture(TextureUnit.Texture0);
                dofShader.setVerticalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.VerticalBlurTexture.GetTextureRezolution());
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            }

            // Blend DoF post process result with previous post process result, if such exists
            renderTarget.renderToFBO(3, renderTarget.DepthOfFieldResultTexture.GetTextureRezolution());

            if (previousPostProcessResult != null)
            {
                previousPostProcessResult.BindTexture(TextureUnit.Texture2);
                dofShader.SetPreviousPostProcessResultSampler(2);
            }

            dofShader.startProgram();
            renderTarget.HorizontalBlurTexture.BindTexture(TextureUnit.Texture0);
            renderTarget.DepthOfFieldResultTexture.BindTexture(TextureUnit.Texture1);
            dofShader.setDoFUniforms(0, 1, BlurStartEdge, BlurEndEdge);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            dofShader.stopProgram();

            renderTarget.unbindFramebuffer();
            GL.Enable(EnableCap.DepthTest);

            return renderTarget.DepthOfFieldResultTexture;
        }

        public override void CleanUp()
        {
            ResourcePool.ReleaseShaderProgram(dofShader);
            renderTarget.cleanUp();
        }
    }
}
