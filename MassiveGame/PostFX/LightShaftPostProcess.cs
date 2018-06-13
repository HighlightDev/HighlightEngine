﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.API.Collector;
using GpuGraphics;

namespace MassiveGame.PostFX
{
    public class LightShaftPostProcess : BasePostProcess
    {
        public GodRaysFBO RenderTarget;
        private GodRaysShader shader;
        private Matrix4 viewportMatrix;

        public float Exposure { set; get; }
        public float Decay { set; get; }
        public float Weight { set; get; }
        public float Density { set; get; }
        public Int32 NumSamples { set; get; }

        public LightShaftPostProcess() : base()
        {
            Exposure = 0.0064f;
            Decay = 1.05f;
            Density = 0.84f;
            Weight = 5.65f;
            NumSamples = 25;
        }

        private void postConstructor()
        {
            RenderTarget = new GodRaysFBO();
            shader = (GodRaysShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "godrayVS.glsl", ProjectFolders.ShadersPath + "godrayFS.glsl", "", typeof(GodRaysShader));
            DOUEngine.uiFrameCreator.PushFrame(RenderTarget.LightShaftsResultTexture);
            bPostConstructor = false;
        }

        private Vector2 getRadialPos(Vector3 translation, Matrix4 viewMatrix, ref Matrix4 projectionMatrix,
           ref Matrix4 viewportMatrix)
        {
            viewMatrix[3, 0] = 0.0f;    //Restrict x-axis translation
            viewMatrix[3, 1] = 0.0f;    //Restrict y-axis translation
            viewMatrix[3, 2] = 0.0f;    //Restrict z-axis translation
            Vector4 temp = new Vector4(translation, 1.0f);
            Matrix4 transformationMatrix = viewMatrix * projectionMatrix;

            Vector4 clippedSpace = Vector4.Transform(temp, transformationMatrix);
            Vector2 ndc = clippedSpace.Xy / clippedSpace.W;
            ndc.X = (ndc.X * 0.5f) + 0.5f;
            ndc.Y = (ndc.Y * 0.5f) + 0.5f;

            //Calibration
            ndc.Y -= 0.003f;
            return ndc;
        }

        public override ITexture GetPostProcessResult(ITexture frameTexture, Int32 actualScreenWidth, Int32 actualScreenHeight)
        {
            if (bPostConstructor)
                postConstructor();

            // Rendering bright objects to render target
            RenderTarget.renderToFBO(2, RenderTarget.RadialBlurAppliedTexture.GetTextureRezolution());
            base.GetPostProcessResult(null, 0, 0);

            viewportMatrix = new Matrix4(new Vector4(actualScreenWidth / 2, 0, actualScreenWidth / 2, 0),
              new Vector4(0, actualScreenHeight / 2, actualScreenHeight / 2, 0),
              new Vector4(0, 0, 1, 0),
              new Vector4(0, 0, 0, 1));

            var radialBlurScreenSpacePosition = getRadialPos(DOUEngine.Sun.Position, DOUEngine.Camera.getViewMatrix(), ref DOUEngine.ProjectionMatrix, ref viewportMatrix);

            // Render to light shaft result render target
            RenderTarget.renderToFBO(4, RenderTarget.LightShaftsResultTexture.GetTextureRezolution());

            shader.startProgram();
            shader.SetFrameTextureSampler(0);
            shader.SetBrightPartsTextureSampler(1);
            shader.SetRadialBlurCenterPositionInScreenSpace(radialBlurScreenSpacePosition);
            shader.SetRadialBlurExposure(Exposure);
            shader.SetRadialBlurDecay(Decay);
            shader.SetRadialBlurDensity(Density);
            shader.SetRadialBlurNumberOfSamples(NumSamples);
            shader.SetRadialBlurWeight(Weight);
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            shader.stopProgram();

            return RenderTarget.LightShaftsResultTexture;
        }

        protected override void RenderScene(LiteCamera camera)
        {
            /*TO DO :
           * Culling back faces and enabling color masking */
            GL.ColorMask(false, false, false, true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (DOUEngine.Skybox != null) DOUEngine.Skybox.renderSkybox(DOUEngine.Camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix);

            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (DOUEngine.terrain != null) DOUEngine.terrain.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);

            GL.Disable(EnableCap.CullFace);


            if (DOUEngine.Plant1 != null) DOUEngine.Plant1.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime, DOUEngine.terrain);

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

            GL.ColorMask(true, true, true, true);

            if (!Object.Equals(DOUEngine.SunReplica, null))
            {
                if (DOUEngine.SunReplica.IsInCameraView)
                {
                    DOUEngine.SunReplica.renderSun(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix);
                }
            }
        }
    }
}
