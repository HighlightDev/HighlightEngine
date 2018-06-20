using GpuGraphics;
using MassiveGame.API.Collector;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.PostFX
{
    public class BloomPostProcess<T> : PostProcessBase where T : PostProcessSubsequenceType
    {
        public const Int32 BLOOM_MAX_PASS_COUNT = 40;
        public const Int32 BLOOM_MIN_PASS_COUNT = 1;
        public const Int32 MAX_BLUR_WIDTH = 10;
        public const Int32 MIN_BLUR_WIDTH = 2;

        private bool blurWidthChanged;
        private float[] blurWeights;

        private Int32 blurWidth;
        public Int32 BlurWidth
        {
            set
            {
                blurWidth = value < MIN_BLUR_WIDTH ? MIN_BLUR_WIDTH :
                    value > MAX_BLUR_WIDTH ? MAX_BLUR_WIDTH : value;
                this.blurWidthChanged = true;
            }
            get { return blurWidth; }
        }

        private float bloomThreshold;
        public float BloomThreshold
        {
            set { bloomThreshold = value; }
            get { return bloomThreshold; }
        }

        private Int32 bloomPass;
        public Int32 BloomPass
        {
            set { bloomPass = value < BLOOM_MIN_PASS_COUNT ? 1 : value > BLOOM_MAX_PASS_COUNT ? BLOOM_MAX_PASS_COUNT : value; }
            get { return this.bloomPass; }
        }

        private BloomFramebufferObject renderTarget;
        private BloomShader<T> bloomShader;

        public BloomPostProcess() : base()
        {
            BloomThreshold = 0.9f;
            BloomPass = 2;
            BlurWidth = 10;
            blurWidthChanged = true;
        }

        private void postConstructor()
        {
            renderTarget = new BloomFramebufferObject();
            bloomShader = (BloomShader<T>)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "bloomVS.glsl",
                ProjectFolders.ShadersPath + "bloomFS.glsl", "", typeof(BloomShader<T>));
            bPostConstructor = false;
            DOUEngine.uiFrameCreator.PushFrame(renderTarget.bloomResultTexture);
        }

        public override ITexture GetPostProcessResult(ITexture frameTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            if (bPostConstructor)
                postConstructor();

            if (this.blurWidthChanged)
            {
                this.blurWeights = normalizedWeights(blurWidth);
                this.blurWidthChanged = false;
            }

            /*Render bright parts of the image to vertical blur render target*/
            renderTarget.renderToFBO(1, renderTarget.verticalBlurTexture.GetTextureRezolution());
            GL.Disable(EnableCap.DepthTest);
            bloomShader.startProgram();
            frameTexture.BindTexture(TextureUnit.Texture0);
            bloomShader.setExtractingBrightPixelsUniforms(0, BloomThreshold);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            bloomShader.stopProgram();

            /*Gauss blur*/
            for (Int32 i = BLOOM_MIN_PASS_COUNT; i < BloomPass; i++)
            {
                /*Horizontal blur of image*/
                renderTarget.renderToFBO(2, renderTarget.horizontalBlurTexture.GetTextureRezolution());
                bloomShader.startProgram();
                renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
                bloomShader.setHorizontalBlurUniforms(0, blurWeights, getPixOffset(blurWidth), renderTarget.verticalBlurTexture.GetTextureRezolution());
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                bloomShader.stopProgram();

                /*Vertical blur of bright parts of image*/
                renderTarget.renderToFBO(1, renderTarget.verticalBlurTexture.GetTextureRezolution());
                bloomShader.startProgram();
                renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                bloomShader.setVerticalBlurUniforms(0, blurWeights, getPixOffset(blurWidth), renderTarget.horizontalBlurTexture.GetTextureRezolution());
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                bloomShader.stopProgram();
            }

            // Blend bloom post process result with previous post process result, if such exists
            renderTarget.renderToFBO(3, renderTarget.bloomResultTexture.GetTextureRezolution());

            if (previousPostProcessResult != null)
            {
                previousPostProcessResult.BindTexture(TextureUnit.Texture1);
                bloomShader.SetPreviousPostProcessResultSampler(1);
            }

            bloomShader.startProgram();
            renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
            bloomShader.setEndBloomUniforms(0);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            bloomShader.stopProgram();

            renderTarget.unbindFramebuffer();
            GL.Enable(EnableCap.DepthTest);

            return renderTarget.bloomResultTexture;
        }

        public override void CleanUp()
        {
            renderTarget.cleanUp();
            ResourcePool.ReleaseShaderProgram(bloomShader);
        }
    }
}
