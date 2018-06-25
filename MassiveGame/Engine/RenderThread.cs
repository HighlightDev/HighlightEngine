﻿using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MassiveGame.RenderCore;
using MassiveGame.Optimization;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.PostFX;
using TextureLoader;

namespace MassiveGame.Engine
{
    public class RenderThread
    {
        public DefaultFrameBuffer DefaultFB { set; get; }
        public PostProcessStageRenderer postProcessStage { set; get; }
        //private ComputeShader ch;

        public RenderThread()
        {
            DefaultFB = new DefaultFrameBuffer(DOUEngine.globalSettings.DomainFramebufferRezolution);
            postProcessStage = new PostProcessStageRenderer();
        }

        private void VisibilityCheckPass()
        {
            // Find which primitives are visible for current frame
            VisibilityCheckApi.CheckMeshIsVisible(DOUEngine.RenderableMeshCollection, ref DOUEngine.ProjectionMatrix, DOUEngine.Camera.getViewMatrix());

            // Find which light sources effects on meshes
            LightHitCheckApi.CheckLightSourceHitsMesh(DOUEngine.LitByLightSourcesMeshCollection, DOUEngine.PointLight);
        }

        private void PreDrawClearBuffers()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(Color.Black);
        }

        public void ThreadExecution(ref Point actualScreenRezolution, bool bInitialDraw)
        {
            VisibilityCheckPass();
            PreDrawClearBuffers();
            if (!bInitialDraw)
            {
                DepthPassDraw(ref actualScreenRezolution);
            }
            DistortionsPassDraw();
            BasePassDraw(ref actualScreenRezolution);
            PostProcessPass(DefaultFB.GetColorTexture(), DefaultFB.GetDepthStencilTexture(), ref actualScreenRezolution);

#if DEBUG
            DebugFramePanelsPass();
#endif
        }

        #region Draw passes

        private void BasePassDraw(ref Point actualScreenRezolution)
        {
            DefaultFB.Bind();
            RenderBaseMeshes(DOUEngine.Camera);
#if DEBUG
            RenderDebugInfo(ref actualScreenRezolution);
#endif
            DefaultFB.Unbind();
        }

        private void DepthPassDraw(ref Point actualScreenRezolution)
        {
            DOUEngine.Sun.GetShadow().WriteDepth(DOUEngine.shadowList, ref DOUEngine.ProjectionMatrix);
            GL.Viewport(0, 0, actualScreenRezolution.X, actualScreenRezolution.Y);
        }

        private void DistortionsPassDraw()
        {
            if (DOUEngine.Water != null && DOUEngine.Water.IsInCameraView)
            {
                DOUEngine.Water.SetReflectionRendertarget();
                RenderToReflectionRenderTarget(DOUEngine.Camera, new Vector4(0, -1, 0, DOUEngine.Water.WaterHeight), DOUEngine.Water.Quality);

                DOUEngine.Water.SetRefractionRendertarget();
                RenderToRefractionRenderTarget(DOUEngine.Camera, new Vector4(0, -1, 0, DOUEngine.Water.WaterHeight), DOUEngine.Water.Quality);
            }
        }

        private void PostProcessPass(ITexture colorTexture, ITexture depthTexture, ref Point actualScreenRezolution)
        {
            postProcessStage.ExecutePostProcessPass(colorTexture, depthTexture, ref actualScreenRezolution);
        }

        private void DebugFramePanelsPass()
        {
            GL.Disable(EnableCap.DepthTest);
            RenderDebugFramePanels();
        }

        #endregion

        #region Render functions

        private void RenderBaseMeshes(Camera camera)
        {
            /*TO DO :
             * Culling back faces of Skybox (cause we don't see them)
             * Culling back faces of terrain
             * Culling back face of Sun
             * Clearing depth buffer, cause Skybox is infinite   */

            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            if (DOUEngine.Skybox != null)
                DOUEngine.Skybox.renderSkybox(DOUEngine.Camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            if (DOUEngine.terrain != null) DOUEngine.terrain.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);

            if (!Object.Equals(DOUEngine.SunReplica, null))
            {
                if (DOUEngine.SunReplica.IsInCameraView)
                {
                    DOUEngine.SunReplica.renderSun(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            if (DOUEngine.Water != null && DOUEngine.Water.IsInCameraView)
            {
                DOUEngine.Water.renderWater(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix, (float)DOUEngine.RENDER_TIME,
                        DOUEngine.NEAR_CLIPPING_PLANE, DOUEngine.FAR_CLIPPING_PLANE, DOUEngine.Sun, DOUEngine.PointLight);
            }

            GL.Disable(EnableCap.CullFace);

            if (DOUEngine.Grass != null) DOUEngine.Grass.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RENDER_TIME, DOUEngine.terrain);
            if (DOUEngine.Plant1 != null) DOUEngine.Plant1.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RENDER_TIME, DOUEngine.terrain);

            if (DOUEngine.City != null)
            {
                foreach (Building house in DOUEngine.City)
                {
                    if (!house.IsInCameraView) continue;
                    house.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            if (DOUEngine.Player != null)
            {
                if (DOUEngine.Player.IsInCameraView)
                {
                    DOUEngine.Player.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            if (DOUEngine.Enemy != null)
            {
                if (DOUEngine.Enemy.IsInCameraView)
                {
                    DOUEngine.Enemy.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            // ITS for TEST! COMPUTE SHADERS!
            //Matrix4 worldMatrix = Matrix4.CreateScale(1);
            //worldMatrix *= Matrix4.CreateTranslation(new Vector3(0, 60, 0));
            //ch.Render(worldMatrix, DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix);
        }

        private void RenderToReflectionRenderTarget(LiteCamera camera, Vector4 clipPlane, WaterQuality quality)
        {
            GL.Enable(EnableCap.StencilTest); // Enable stencil test
            GL.DepthMask(false); // Disable write depth
            GL.ColorMask(false, false, false, false); // Disable write color
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF); // Write 1 to stencil
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace); // Replace when depth test pass
            GL.StencilMask(0xFF); // Write to stencil
            GL.Clear(ClearBufferMask.StencilBufferBit); // Clear stencil buffer

            // prepass for stencil only
            DOUEngine.Water.StencilPass(camera, ref DOUEngine.ProjectionMatrix);

            GL.DepthMask(true); // Enable write depth
            GL.ColorMask(true, true, true, true); // Enable write color
            GL.StencilFunc(StencilFunction.Equal, 1, 0xFF); // Set stencil function
            GL.StencilMask(0x00);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep); // Set stencil operation

            /* Culling back faces of terrain
             * Culling back faces of EngineSingleton.Skybox
             * Disable depth test cause skybox is infinite     */

            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            if (DOUEngine.Skybox != null) DOUEngine.Skybox.RenderWaterReflection(DOUEngine.Water, camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix, clipPlane);

            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.CullFace(CullFaceMode.Back);

            if (DOUEngine.terrain != null) DOUEngine.terrain.RenderWaterReflection(DOUEngine.Water, DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);

            /*TO DO : true - enable building reflections
             false - disable building reflections*/
            if (quality.EnableBuilding)
            {
                if (DOUEngine.City != null) foreach (Building house in DOUEngine.City)
                    {
                        house.RenderWaterReflection(DOUEngine.Water, DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
                    }
            }

            if (quality.EnablePlayer)
            {
                if (DOUEngine.Player != null) DOUEngine.Player.RenderWaterReflection(DOUEngine.Water, DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
                if (DOUEngine.Enemy != null) DOUEngine.Enemy.RenderWaterReflection(DOUEngine.Water, DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            }

            if (!Object.Equals(DOUEngine.SunReplica, null))
            {
                DOUEngine.SunReplica.RenderWaterReflection(DOUEngine.Water, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            }

            GL.Disable(EnableCap.StencilTest); // Disable stencil test 
        }

        private void RenderToRefractionRenderTarget(LiteCamera camera, Vector4 clipPlane, WaterQuality quality)
        {
            GL.Enable(EnableCap.StencilTest); // Enable stencil test
            GL.DepthMask(false); // Disable write depth
            GL.ColorMask(false, false, false, false); // Disable write color
            GL.StencilFunc(StencilFunction.Always, 2, 0xFF); // Write 2 to stencil
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace); // Replace when depth test pass
            GL.StencilMask(0xFF); // Write to stencil
            GL.Clear(ClearBufferMask.StencilBufferBit); // Clear stencil buffer

            // prepass for stencil only
            DOUEngine.Water.StencilPass(camera, ref DOUEngine.ProjectionMatrix);

            GL.DepthMask(true); // Enable write depth
            GL.ColorMask(true, true, true, true); // Enable write color
            GL.StencilFunc(StencilFunction.Equal, 2, 0xFF); // Set stencil function
            GL.StencilMask(0x00);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep); // Set stencil operation

            /*TO DO : true - enable EngineSingleton.Grass refractions
            false - disable EngineSingleton.Grass refractions*/
            if (quality.EnableGrassRefraction)
            {
                if (DOUEngine.Grass != null) DOUEngine.Grass.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RENDER_TIME, DOUEngine.terrain, clipPlane);
                if (DOUEngine.Plant1 != null) DOUEngine.Plant1.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RENDER_TIME, DOUEngine.terrain, clipPlane);
            }

            /*TO DO : true - enable building refractions
             false - disable building refractions*/
            if (quality.EnableBuilding)
            {
                if (DOUEngine.City != null) foreach (Building house in DOUEngine.City)
                        house.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            }

            /*TO DO : true - enable EngineSingleton.Player and EngineSingleton.Enemy refractions
            false - disable EngineSingleton.Player and EngineSingleton.Enemy refractions*/
            if (quality.EnablePlayer)
            {
                if (DOUEngine.Player != null)
                {
                    if (DOUEngine.Player.IsInCameraView)
                    {
                        DOUEngine.Player.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
                    }
                }
                if (DOUEngine.Enemy != null) DOUEngine.Enemy.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            }

            /*TO DO :
             * Culling back facies of terrain, cause they don't refract in EngineSingleton.Water*/
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            if (DOUEngine.terrain != null) DOUEngine.terrain.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.StencilTest); // Enable stencil test
        }

        private void RenderDebugInfo(ref Point actualScreenRezoltuion)
        {
            RenderLamps();
            RenderBoundingBoxes();
        }

        private void RenderLamps()
        {
            /*TO DO :
             * If point lights exist - show them */
            if (DOUEngine.PointLight != null)
            {
                DOUEngine.pointLightDebugRenderer.Render(DOUEngine.Camera, DOUEngine.ProjectionMatrix);
            }
        }

        private void RenderBoundingBoxes()
        {
            Matrix4 viewMatrix = DOUEngine.Camera.getViewMatrix();

            if (DOUEngine.Player != null)
            {
                DOUEngine.Player.RenderBound(ref DOUEngine.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (!Object.Equals(DOUEngine.Enemy, null))
            {
                DOUEngine.Enemy.RenderBound(ref DOUEngine.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (DOUEngine.City != null)
            {
                foreach (var item in DOUEngine.City)
                    item.RenderBound(ref DOUEngine.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (DOUEngine.SunReplica != null && DOUEngine.SunReplica.CQuad != null)
            {
                var matrix = DOUEngine.Camera.getViewMatrix();
                matrix[3, 0] = 0.0f;
                matrix[3, 1] = 0.0f;
                matrix[3, 2] = 0.0f;
                DOUEngine.SunReplica.CQuad.renderQuad(matrix, ref DOUEngine.ProjectionMatrix);
            }
        }

        private void RenderDebugFramePanels()
        {
            DOUEngine.uiFrameCreator.RenderFrames();
        }

        #endregion

    }
}
