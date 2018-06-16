using MassiveGame.API.Collector;
using OpenTK;
using System;
using TextureLoader;
using OpenTK.Graphics.OpenGL;
using GpuGraphics;
using System.Drawing;

namespace MassiveGame.PostFX
{
    public class LensFlarePostProcess<T> : PostProcessBase where T : PostProcessSubsequenceType
    {
        public const int MAX_BLUR_WIDTH = 30;
        private const int MIN_BLUR_WIDTH = 2;
        private const int BLUR_MAX_PASS_COUNT = 20;

        private LensFlareFramebufferObject renderTarget;
        private LensFlareShader<T> lensShader;
        private Texture1D lensColor;

        private int blurWidth;
        public int BlurWidth
        {
            set
            {
                blurWidth = value < MIN_BLUR_WIDTH ? MIN_BLUR_WIDTH :
                    value > MAX_BLUR_WIDTH ? MAX_BLUR_WIDTH : value;
            }
            get { return blurWidth; }
        }

        private float ghostDispersal;
        public float GhostDispersal
        {
            set { ghostDispersal = value < 0f ? 0.0f : value; }
            get { return this.ghostDispersal; }
        }

        private int ghosts;
        public int Ghosts
        {
            set { ghosts = value < 0 ? 0 : value; }
            get { return this.ghosts; }
        }

        private float haloWidth;
        public float HaloWidth
        {
            set { haloWidth = value < 0f ? 0.0f : value; }
            get { return this.haloWidth; }
        }

        private float distortion;
        public float Distortion
        {
            set { distortion = value < 0f ? 0.0f : value; }
            get { return this.distortion; }
        }

        private float threshold;
        public float Threshold
        {
            set { threshold = value < 0f ? 0.0f : value; }
            get { return this.threshold; }
        }

        private int blurPassCount;
        public int BlurPassCount
        {
            set { blurPassCount = value < 1 ? 1 : value > BLUR_MAX_PASS_COUNT ? BLUR_MAX_PASS_COUNT : value; }
            get { return blurPassCount; }
        }

        public LensFlarePostProcess() : base()
        {
            this.blurWidth = 20;
            this.ghostDispersal = 0.37f;
            this.ghosts = 5;
            this.haloWidth = 0.55f;
            this.distortion = 3.50f;
            this.threshold = 0.5f;
            this.blurPassCount = 4;
            this.lensColor = new Texture1D(new string[] { ProjectFolders.LensFlareTexturePath + "lenscolor.png" }, false, 0);
        }

        private void postConstructor()
        {
            renderTarget = new LensFlareFramebufferObject();
            lensShader = (LensFlareShader<T>)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "lensFlareVS.glsl",
                ProjectFolders.ShadersPath + "lensFlareFS.glsl", "", typeof(LensFlareShader<T>));
            bPostConstructor = false;
            DOUEngine.uiFrameCreator.PushFrame(renderTarget.lensFlareResultTexture);
        }

        public override ITexture GetPostProcessResult(ITexture frameTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            if (bPostConstructor)
                postConstructor();

            /*Extracting bright parts*/
            renderTarget.renderToFBO(3, renderTarget.frameTextureLowRezolution.GetTextureRezolution());
            base.GetPostProcessResult(null, actualScreenRezolution);

            renderTarget.renderToFBO(2, renderTarget.horizontalBlurTexture.GetTextureRezolution());

            lensShader.startProgram();
            renderTarget.frameTextureLowRezolution.BindTexture(TextureUnit.Texture0);
            lensShader.setUniformValuesThreshold(0, this.threshold);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            lensShader.stopProgram();

            /*Lens effect*/
            renderTarget.renderToFBO(1, this.renderTarget.verticalBlurTexture.GetTextureRezolution());

            lensShader.startProgram();
            renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
            lensColor.bindTexture1D(TextureUnit.Texture1, lensColor.TextureID[0]);
            lensShader.setUniformValuesLens(0, 1, this.Ghosts, this.HaloWidth, this.Distortion, this.GhostDispersal);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            lensShader.stopProgram();

            /*Extra passes for blur*/
            for (int i = 1; i < BlurPassCount; i++)
            {
                /*Horizontal Blur effect*/
                renderTarget.renderToFBO(2, this.renderTarget.horizontalBlurTexture.GetTextureRezolution());

                lensShader.startProgram();
                renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
                lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(blurWidth), getPixOffset(blurWidth), actualScreenRezolution);
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                lensShader.stopProgram();

                /*Vertical Blur effect*/
                renderTarget.renderToFBO(1, this.renderTarget.verticalBlurTexture.GetTextureRezolution());

                lensShader.startProgram();
                renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(blurWidth), getPixOffset(blurWidth), actualScreenRezolution);
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                lensShader.stopProgram();
            }

            renderTarget.renderToFBO(4, renderTarget.lensFlareResultTexture.GetTextureRezolution());

            lensShader.startProgram();

            // If this is one post process stage from many
            if (previousPostProcessResult != null)
            {
                previousPostProcessResult.BindTexture(TextureUnit.Texture1);
                lensShader.SetPreviousPostProcessResultSampler(1);
            }

            renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
            lensShader.setUniformValuesMod(0);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            lensShader.stopProgram();

            return renderTarget.lensFlareResultTexture;
        }

        protected override void RenderScene(LiteCamera camera)
        {
            /*TO DO :
             * Culling back faces of all objects on scene
             * and enabling color masking */

            GL.ColorMask(false, false, false, true);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            if (DOUEngine.terrain != null) DOUEngine.terrain.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);
            if (DOUEngine.City != null) foreach (Building house in DOUEngine.City)
                { house.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix); }

            if (DOUEngine.Player != null)
            {
                if (DOUEngine.Player.IsInCameraView)
                {
                    DOUEngine.Player.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }
            if (DOUEngine.Enemy != null) DOUEngine.Enemy.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);

            /*Disable color masking*/
            GL.ColorMask(true, true, true, true);
            if (!Object.Equals(DOUEngine.SunReplica, null))
            {
                if (DOUEngine.SunReplica.IsInCameraView)
                {
                    DOUEngine.SunReplica.renderSun(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix, new Vector3(DOUEngine.SunReplica.LENS_FLARE_SUN_SIZE / DOUEngine.SunReplica.SUN_SIZE));
                }
            }

            /*Stop culling*/
            GL.Disable(EnableCap.CullFace);
        }

        public override void CleanUp()
        {
            renderTarget.cleanUp();
            renderTarget.lensFlareResultTexture.CleanUp();
            ResourcePool.ReleaseShaderProgram(lensShader);
        }
    }
}