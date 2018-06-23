﻿using MassiveGame.API.Collector;
using OpenTK;
using System;
using TextureLoader;
using OpenTK.Graphics.OpenGL;
using GpuGraphics;
using System.Drawing;

namespace MassiveGame.PostFX.LensFlare
{
    public class LensFlarePostProcess<T> : PostProcessBase where T : PostProcessSubsequenceType
    {
        private LensFlareFramebufferObject renderTarget;
        private LensFlareShader<T> lensShader;
        private Texture1D lensColor;
      
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

        public LensFlarePostProcess() : base()
        {
            this.BlurWidth = 20;
            this.ghostDispersal = 0.37f;
            this.ghosts = 5;
            this.haloWidth = 0.55f;
            this.distortion = 3.50f;
            this.threshold = 0.5f;
            this.lensColor = new Texture1D(new string[] { ProjectFolders.LensFlareTexturePath + "lenscolor.png" }, false, 0);
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                renderTarget = new LensFlareFramebufferObject();
                lensShader = (LensFlareShader<T>)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "lensFlareVS.glsl",
                    ProjectFolders.ShadersPath + "lensFlareFS.glsl", "", typeof(LensFlareShader<T>));
                bPostConstructor = false;
            }
        }

        public override ITexture GetPostProcessResult(ITexture frameColorTexture, ITexture frameDepthTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            postConstructor();

            /*Extracting bright parts*/
            renderTarget.renderToFBO(3, renderTarget.frameTextureLowRezolution.GetTextureRezolution());
            base.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution);

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
            for (int i = 0; i < BlurPassCount; i++)
            {
                /*Horizontal Blur effect*/
                renderTarget.renderToFBO(2, this.renderTarget.horizontalBlurTexture.GetTextureRezolution());

                lensShader.startProgram();
                renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
                lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(BlurWidth), getPixOffset(BlurWidth), actualScreenRezolution);
                VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
                lensShader.stopProgram();

                /*Vertical Blur effect*/
                renderTarget.renderToFBO(1, this.renderTarget.verticalBlurTexture.GetTextureRezolution());

                lensShader.startProgram();
                renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(BlurWidth), getPixOffset(BlurWidth), actualScreenRezolution);
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

            renderTarget.unbindFramebuffer();

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
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (DOUEngine.Skybox != null)
            {
                DOUEngine.Skybox.renderSkybox(DOUEngine.Camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix);
            }

            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (DOUEngine.terrain != null)
            {
                DOUEngine.terrain.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);
            }

            if (DOUEngine.City != null)
            {
                foreach (Building house in DOUEngine.City)
                {
                    if (house.IsInCameraView)
                        house.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            if (DOUEngine.Player != null && DOUEngine.Player.IsInCameraView)
            {
                DOUEngine.Player.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
            }

            if (DOUEngine.Enemy != null && DOUEngine.Enemy.IsInCameraView)
            {
                DOUEngine.Enemy.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
            }

            if (DOUEngine.Water != null && DOUEngine.Water.IsInCameraView)
            {
                DOUEngine.Water.StencilPass(camera, ref DOUEngine.ProjectionMatrix);
            }

            /*Disable color masking*/
            GL.ColorMask(true, true, true, true);
            if (DOUEngine.SunReplica != null && DOUEngine.SunReplica.IsInCameraView)
            {
                DOUEngine.SunReplica.renderSun(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix, new Vector3(DOUEngine.SunReplica.LENS_FLARE_SUN_SIZE / DOUEngine.SunReplica.SUN_SIZE));
            }

            /*Stop culling*/
            GL.Disable(EnableCap.CullFace);
        }

        public override void CleanUp()
        {
            renderTarget.cleanUp();
            ResourcePool.ReleaseShaderProgram(lensShader);
        }
    }
}