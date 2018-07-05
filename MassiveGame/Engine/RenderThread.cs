using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MassiveGame.RenderCore;
using MassiveGame.Optimization;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.PostFX;
using TextureLoader;
using MassiveGame.Core;

namespace MassiveGame.Engine
{
    public class RenderThread
    {
        public DefaultFrameBuffer DefaultFB { set; get; }
        public PostProcessStageRenderer postProcessStage { set; get; }
        //private ComputeShader ch;

        public RenderThread()
        {
            DefaultFB = new DefaultFrameBuffer(EngineStatics.globalSettings.DomainFramebufferRezolution);
            postProcessStage = new PostProcessStageRenderer();
        }

        private void VisibilityCheckPass()
        {
            // Find which primitives are visible for current frame
            VisibilityCheckApi.CheckMeshIsVisible(EngineStatics.RenderableMeshCollection, ref EngineStatics.ProjectionMatrix, EngineStatics.Camera.GetViewMatrix());

            // Find which light sources effects on meshes
            LightHitCheckApi.CheckLightSourceHitsMesh(EngineStatics.LitByLightSourcesMeshCollection, EngineStatics.PointLight);
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
            RenderBaseMeshes(EngineStatics.Camera);
#if DEBUG
            RenderDebugInfo(ref actualScreenRezolution);
#endif
            DefaultFB.Unbind();
        }

        private void DepthPassDraw(ref Point actualScreenRezolution)
        {
            EngineStatics.Sun.GetShadow().WriteDepth(EngineStatics.shadowList, ref EngineStatics.ProjectionMatrix);
            GL.Viewport(0, 0, actualScreenRezolution.X, actualScreenRezolution.Y);
        }

        private void DistortionsPassDraw()
        {
            if (EngineStatics.Water != null && EngineStatics.Water.IsInCameraView)
            {
                EngineStatics.Water.SetReflectionRendertarget();
                RenderToReflectionRenderTarget(EngineStatics.Camera, new Vector4(0, -1, 0, EngineStatics.Water.WaterHeight), EngineStatics.Water.Quality);

                EngineStatics.Water.SetRefractionRendertarget();
                RenderToRefractionRenderTarget(EngineStatics.Camera, new Vector4(0, -1, 0, EngineStatics.Water.WaterHeight), EngineStatics.Water.Quality);
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

        private void RenderBaseMeshes(BaseCamera camera)
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

            if (EngineStatics.Skybox != null)
                EngineStatics.Skybox.renderSkybox(EngineStatics.Camera, EngineStatics.Sun, EngineStatics.ProjectionMatrix);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            if (EngineStatics.terrain != null) EngineStatics.terrain.renderTerrain(EngineStatics.Mode, EngineStatics.Sun, EngineStatics.PointLight, camera, EngineStatics.ProjectionMatrix);

            if (!Object.Equals(EngineStatics.SunReplica, null))
            {
                if (EngineStatics.SunReplica.IsInCameraView)
                {
                    EngineStatics.SunReplica.renderSun(EngineStatics.Camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (EngineStatics.Water != null && EngineStatics.Water.IsInCameraView)
            {
                EngineStatics.Water.renderWater(EngineStatics.Camera, ref EngineStatics.ProjectionMatrix, (float)EngineStatics.RENDER_TIME,
                        EngineStatics.NEAR_CLIPPING_PLANE, EngineStatics.FAR_CLIPPING_PLANE, EngineStatics.Sun, EngineStatics.PointLight);
            }

            GL.Disable(EnableCap.CullFace);

            if (EngineStatics.Grass != null) EngineStatics.Grass.renderEntities(EngineStatics.Sun, camera, EngineStatics.ProjectionMatrix, (float)EngineStatics.RENDER_TIME, EngineStatics.terrain);
            if (EngineStatics.Plant1 != null) EngineStatics.Plant1.renderEntities(EngineStatics.Sun, camera, EngineStatics.ProjectionMatrix, (float)EngineStatics.RENDER_TIME, EngineStatics.terrain);

            if (EngineStatics.City != null)
            {
                foreach (Building house in EngineStatics.City)
                {
                    if (!house.IsInCameraView) continue;
                    house.renderObject(EngineStatics.Mode, EngineStatics.NormalMapTrigger, EngineStatics.Sun, EngineStatics.PointLight, camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (EngineStatics.Player != null)
            {
                if (EngineStatics.Player.IsInCameraView)
                {
                    EngineStatics.Player.renderObject(EngineStatics.Mode, EngineStatics.NormalMapTrigger, EngineStatics.Sun, EngineStatics.PointLight, camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (EngineStatics.Enemy != null)
            {
                if (EngineStatics.Enemy.IsInCameraView)
                {
                    EngineStatics.Enemy.renderObject(EngineStatics.Mode, EngineStatics.NormalMapTrigger, EngineStatics.Sun, EngineStatics.PointLight, camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            // ITS for TEST! COMPUTE SHADERS!
            //Matrix4 worldMatrix = Matrix4.CreateScale(1);
            //worldMatrix *= Matrix4.CreateTranslation(new Vector3(0, 60, 0));
            //ch.Render(worldMatrix, DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix);
        }

        private void RenderToReflectionRenderTarget(BaseCamera camera, Vector4 clipPlane, WaterQuality quality)
        {
            GL.Enable(EnableCap.StencilTest); // Enable stencil test
            GL.DepthMask(false); // Disable write depth
            GL.ColorMask(false, false, false, false); // Disable write color
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF); // Write 1 to stencil
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace); // Replace when depth test pass
            GL.StencilMask(0xFF); // Write to stencil
            GL.Clear(ClearBufferMask.StencilBufferBit); // Clear stencil buffer

            // prepass for stencil only
            EngineStatics.Water.StencilPass(camera, ref EngineStatics.ProjectionMatrix);

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

            if (EngineStatics.Skybox != null) EngineStatics.Skybox.RenderWaterReflection(EngineStatics.Water, camera, EngineStatics.Sun, EngineStatics.ProjectionMatrix, clipPlane);

            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.CullFace(CullFaceMode.Back);

            if (EngineStatics.terrain != null) EngineStatics.terrain.RenderWaterReflection(EngineStatics.Water, EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);

            /*TO DO : true - enable building reflections
             false - disable building reflections*/
            if (quality.EnableBuilding)
            {
                if (EngineStatics.City != null) foreach (Building house in EngineStatics.City)
                    {
                        house.RenderWaterReflection(EngineStatics.Water, EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                    }
            }

            if (quality.EnablePlayer)
            {
                if (EngineStatics.Player != null) EngineStatics.Player.RenderWaterReflection(EngineStatics.Water, EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                if (EngineStatics.Enemy != null) EngineStatics.Enemy.RenderWaterReflection(EngineStatics.Water, EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            }

            if (!Object.Equals(EngineStatics.SunReplica, null))
            {
                EngineStatics.SunReplica.RenderWaterReflection(EngineStatics.Water, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            }

            GL.Disable(EnableCap.StencilTest); // Disable stencil test 
        }

        private void RenderToRefractionRenderTarget(BaseCamera camera, Vector4 clipPlane, WaterQuality quality)
        {
            GL.Enable(EnableCap.StencilTest); // Enable stencil test
            GL.DepthMask(false); // Disable write depth
            GL.ColorMask(false, false, false, false); // Disable write color
            GL.StencilFunc(StencilFunction.Always, 2, 0xFF); // Write 2 to stencil
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace); // Replace when depth test pass
            GL.StencilMask(0xFF); // Write to stencil
            GL.Clear(ClearBufferMask.StencilBufferBit); // Clear stencil buffer

            // prepass for stencil only
            EngineStatics.Water.StencilPass(camera, ref EngineStatics.ProjectionMatrix);

            GL.DepthMask(true); // Enable write depth
            GL.ColorMask(true, true, true, true); // Enable write color
            GL.StencilFunc(StencilFunction.Equal, 2, 0xFF); // Set stencil function
            GL.StencilMask(0x00);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep); // Set stencil operation

            /*TO DO : true - enable EngineSingleton.Grass refractions
            false - disable EngineSingleton.Grass refractions*/
            if (quality.EnableGrassRefraction)
            {
                if (EngineStatics.Grass != null) EngineStatics.Grass.renderEntities(EngineStatics.Sun, camera, EngineStatics.ProjectionMatrix, (float)EngineStatics.RENDER_TIME, EngineStatics.terrain, clipPlane);
                if (EngineStatics.Plant1 != null) EngineStatics.Plant1.renderEntities(EngineStatics.Sun, camera, EngineStatics.ProjectionMatrix, (float)EngineStatics.RENDER_TIME, EngineStatics.terrain, clipPlane);
            }

            /*TO DO : true - enable building refractions
             false - disable building refractions*/
            if (quality.EnableBuilding)
            {
                if (EngineStatics.City != null) foreach (Building house in EngineStatics.City)
                        house.RenderWaterRefraction(EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            }

            /*TO DO : true - enable EngineSingleton.Player and EngineSingleton.Enemy refractions
            false - disable EngineSingleton.Player and EngineSingleton.Enemy refractions*/
            if (quality.EnablePlayer)
            {
                if (EngineStatics.Player != null)
                {
                    if (EngineStatics.Player.IsInCameraView)
                    {
                        EngineStatics.Player.RenderWaterRefraction(EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                    }
                }
                if (EngineStatics.Enemy != null) EngineStatics.Enemy.RenderWaterRefraction(EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            }

            /*TO DO :
             * Culling back facies of terrain, cause they don't refract in EngineSingleton.Water*/
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            if (EngineStatics.terrain != null) EngineStatics.terrain.RenderWaterRefraction(EngineStatics.Sun, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
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
            if (EngineStatics.PointLight != null)
            {
                EngineStatics.pointLightDebugRenderer.Render(EngineStatics.Camera, EngineStatics.ProjectionMatrix);
            }
        }

        private void RenderBoundingBoxes()
        {
            Matrix4 viewMatrix = EngineStatics.Camera.GetViewMatrix();

            if (EngineStatics.Player != null)
            {
                EngineStatics.Player.RenderBound(ref EngineStatics.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (!Object.Equals(EngineStatics.Enemy, null))
            {
                EngineStatics.Enemy.RenderBound(ref EngineStatics.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (EngineStatics.City != null)
            {
                foreach (var item in EngineStatics.City)
                    item.RenderBound(ref EngineStatics.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (EngineStatics.SunReplica != null && EngineStatics.SunReplica.CQuad != null)
            {
                var matrix = EngineStatics.Camera.GetViewMatrix();
                matrix[3, 0] = 0.0f;
                matrix[3, 1] = 0.0f;
                matrix[3, 2] = 0.0f;
                EngineStatics.SunReplica.CQuad.renderQuad(matrix, ref EngineStatics.ProjectionMatrix);
            }
        }

        private void RenderDebugFramePanels()
        {
            EngineStatics.uiFrameCreator.RenderFrames();
        }

        #endregion

    }
}
