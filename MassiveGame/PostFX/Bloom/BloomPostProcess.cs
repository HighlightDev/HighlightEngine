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

namespace MassiveGame.PostFX.Bloom
{
    public class BloomPostProcess<T> : PostProcessBase where T : PostProcessSubsequenceType
    {
        private float bloomThreshold;
        public float BloomThreshold
        {
            set { bloomThreshold = value; }
            get { return bloomThreshold; }
        }

        private BloomFramebufferObject renderTarget;
        private BloomShader<T> bloomShader;

        public BloomPostProcess() : base()
        {
            BloomThreshold = 0.9f;
            BlurWidth = 10;
            BlurPassCount = 4;
            blurWidthChanged = true;
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                renderTarget = new BloomFramebufferObject();
                bloomShader = (BloomShader<T>)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "bloomVS.glsl",
                    ProjectFolders.ShadersPath + "bloomFS.glsl", "", typeof(BloomShader<T>));
                bPostConstructor = false;
            }
        }

        public override ITexture GetPostProcessResult(ITexture frameColorTexture, ITexture frameDepthTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            postConstructor();

            if (this.blurWidthChanged)
            {
                this.blurWeights = normalizedWeights(BlurWidth);
                this.blurWidthChanged = false;
            }

            /*Render bright parts of the image to vertical blur render target*/
            renderTarget.renderToFBO(1, renderTarget.verticalBlurTexture.GetTextureRezolution());
            //GL.Disable(EnableCap.DepthTest);
            bloomShader.startProgram();
            frameColorTexture.BindTexture(TextureUnit.Texture0);
            bloomShader.setExtractingBrightPixelsUniforms(0, BloomThreshold);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            bloomShader.stopProgram();

            /*Gauss blur*/
            for (Int32 i = 0; i < BlurPassCount; i++)
            {
                /*Horizontal blur of image*/
                renderTarget.renderToFBO(2, renderTarget.horizontalBlurTexture.GetTextureRezolution());
                bloomShader.startProgram();
                renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
                bloomShader.setHorizontalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.verticalBlurTexture.GetTextureRezolution());
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                bloomShader.stopProgram();

                /*Vertical blur of bright parts of image*/
                renderTarget.renderToFBO(1, renderTarget.verticalBlurTexture.GetTextureRezolution());
                bloomShader.startProgram();
                renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                bloomShader.setVerticalBlurUniforms(0, blurWeights, getPixOffset(BlurWidth), renderTarget.horizontalBlurTexture.GetTextureRezolution());
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                bloomShader.stopProgram();
            }

            // Blend bloom post process result with previous post process result, if such exists
            renderTarget.renderToFBO(3, renderTarget.bloomResultTexture.GetTextureRezolution());

            bloomShader.startProgram();
            if (previousPostProcessResult != null)
            {
                previousPostProcessResult.BindTexture(TextureUnit.Texture1);
                bloomShader.SetPreviousPostProcessResultSampler(1);
            }
           
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
