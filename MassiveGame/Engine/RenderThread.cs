using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using TextureLoader;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.PostFX;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore.Shadows;

namespace MassiveGame.Engine
{
    public class RenderThread
    {
        public DefaultFrameBuffer DefaultFB { set; get; }
        public PostProcessStageRenderer PostProcessStage { set; get; }

#if DEBUG || ENGINE_EDITOR
        public bool IsSeparatedScreen { set; get; } = true;
#endif
        //private ComputeShader cs;

        public RenderThread()
        {
            DefaultFB = new DefaultFrameBuffer(EngineStatics.globalSettings.DomainFramebufferRezolution);
            PostProcessStage = new PostProcessStageRenderer();
        }

        private void VisibilityCheckPass()
        {
            // Find which primitives are visible for current frame
            VisibilityCheckApi.CheckMeshIsVisible(GameWorld.GetWorldInstance().VisibilityCheckCollection, ref EngineStatics.ProjectionMatrix, GameWorld.GetWorldInstance().GetLevel().Camera.GetViewMatrix());

            // Find which light sources effects on meshes
            LightHitCheckApi.CheckLightSourceHitsMesh(GameWorld.GetWorldInstance().LitCheckCollection, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData());
        }

        private void PreDrawClearBuffers()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(Color.Black);
        }

        public void ThreadExecution(Point actualScreenRezolution, bool bInitialDraw)
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

#if DEBUG || ENGINE_EDITOR
            DebugFramePanelsPass(ref actualScreenRezolution);
#endif
        }

        #region Draw passes

        private void BasePassDraw(ref Point actualScreenRezolution)
        {
            DefaultFB.Bind();
            RenderBaseMeshes(GameWorld.GetWorldInstance().GetLevel().Camera);
#if DEBUG || ENGINE_EDITOR
            RenderDebugInfo(ref actualScreenRezolution);
#endif
            DefaultFB.Unbind();
        }

        private void DepthPassDraw(ref Point actualScreenRezolution)
        {
            // Global light source
            DirectionalLight globalLight = GameWorld.GetWorldInstance().GetLevel().DirectionalLight;
            if (globalLight != null)
            {
                if (globalLight.GetHasShadow())
                {
                    (globalLight as DirectionalLightWithShadow).WriteDepth(GameWorld.GetWorldInstance().ShadowCastCollection, ref EngineStatics.ProjectionMatrix);
                }
                GL.Viewport(0, 0, actualScreenRezolution.X, actualScreenRezolution.Y);
            }
        }

        private void DistortionsPassDraw()
        {
            if (GameWorld.GetWorldInstance().GetLevel().Water.GetData() != null && GameWorld.GetWorldInstance().GetLevel().Water.GetData().IsInCameraView)
            {
                GameWorld.GetWorldInstance().GetLevel().Water.GetData().SetReflectionRendertarget();
                RenderToReflectionRenderTarget(GameWorld.GetWorldInstance().GetLevel().Camera, new Vector4(0, -1, 0, GameWorld.GetWorldInstance().GetLevel().Water.GetData().WaterHeight), GameWorld.GetWorldInstance().GetLevel().Water.GetData().Quality);

                GameWorld.GetWorldInstance().GetLevel().Water.GetData().SetRefractionRendertarget();
                RenderToRefractionRenderTarget(GameWorld.GetWorldInstance().GetLevel().Camera, new Vector4(0, -1, 0, GameWorld.GetWorldInstance().GetLevel().Water.GetData().WaterHeight), GameWorld.GetWorldInstance().GetLevel().Water.GetData().Quality);
            }
        }

        private void PostProcessPass(ITexture colorTexture, ITexture depthTexture, ref Point actualScreenRezolution)
        {
            PostProcessStage.ExecutePostProcessPass(colorTexture, depthTexture, ref actualScreenRezolution);
        }

        private void DebugFramePanelsPass(ref Point actualScreenRezolution)
        {
            GL.Disable(EnableCap.DepthTest);
            RenderDebugFramePanels(ref actualScreenRezolution);
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

            if (GameWorld.GetWorldInstance().GetLevel().Skybox != null)
                GameWorld.GetWorldInstance().GetLevel().Skybox.renderSkybox(GameWorld.GetWorldInstance().GetLevel().Camera, GameWorld.GetWorldInstance().GetLevel().DirectionalLight.Direction, EngineStatics.ProjectionMatrix);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            if (GameWorld.GetWorldInstance().GetLevel().Terrain.GetData() != null) GameWorld.GetWorldInstance().GetLevel().Terrain.GetData().renderTerrain(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData(), camera, EngineStatics.ProjectionMatrix);

            if (GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData() != null)
            {
                if (GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().IsInCameraView)
                {
                    GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().Render(GameWorld.GetWorldInstance().GetLevel().Camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().Water.GetData() != null && GameWorld.GetWorldInstance().GetLevel().Water.GetData().IsInCameraView)
            {
                GameWorld.GetWorldInstance().GetLevel().Water.GetData().RenderWater(GameWorld.GetWorldInstance().GetLevel().Camera, ref EngineStatics.ProjectionMatrix,
                        EngineStatics.NEAR_CLIPPING_PLANE, EngineStatics.FAR_CLIPPING_PLANE, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData());
            }

            GL.Disable(EnableCap.CullFace);

            if (GameWorld.GetWorldInstance().GetLevel().Grass != null) GameWorld.GetWorldInstance().GetLevel().Grass.renderEntities(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, EngineStatics.ProjectionMatrix, GameWorld.GetWorldInstance().GetLevel().Terrain.GetData());
            if (GameWorld.GetWorldInstance().GetLevel().Plant != null) GameWorld.GetWorldInstance().GetLevel().Plant.renderEntities(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, EngineStatics.ProjectionMatrix, GameWorld.GetWorldInstance().GetLevel().Terrain.GetData());

            if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null)
            {
                foreach (Building house in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection)
                {
                    if (!house.IsVisibleByCamera) continue;
                    house.renderObject(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData(), camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().Player.GetData() != null)
            {
                if (GameWorld.GetWorldInstance().GetLevel().Player.GetData().IsVisibleByCamera)
                {
                    GameWorld.GetWorldInstance().GetLevel().Player.GetData().RenderEntity(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData(), camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().Bots != null)
            {
                foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots)
                {
                    if (bot.IsVisibleByCamera)
                        bot.RenderEntity(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData(), camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            GameWorld.GetWorldInstance().GetLevel().SkeletalMesh?.RenderEntity(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection.GetData(), camera, ref EngineStatics.ProjectionMatrix);

#if ENGINE_EDITOR
            GameWorld.GetWorldInstance().GetLevel().EditorGrid.Render(GameWorld.GetWorldInstance().GetLevel().Camera, ref EngineStatics.ProjectionMatrix);
#endif
            // ITS for TEST! COMPUTE SHADERS!
            //Matrix4 worldMatrix = Matrix4.CreateScale(1);
            //worldMatrix *= Matrix4.CreateTranslation(new Vector3(0, 60, 0));
            //cs.Render(worldMatrix, DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix);
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
            GameWorld.GetWorldInstance().GetLevel().Water.GetData().StencilPass(camera, ref EngineStatics.ProjectionMatrix);

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

            if (GameWorld.GetWorldInstance().GetLevel().Skybox != null) GameWorld.GetWorldInstance().GetLevel().Skybox.RenderWaterReflection(GameWorld.GetWorldInstance().GetLevel().Water.GetData(), camera, GameWorld.GetWorldInstance().GetLevel().DirectionalLight.Direction, EngineStatics.ProjectionMatrix, clipPlane);

            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.CullFace(CullFaceMode.Back);

            if (GameWorld.GetWorldInstance().GetLevel().Terrain.GetData() != null) GameWorld.GetWorldInstance().GetLevel().Terrain.GetData().RenderWaterReflection(GameWorld.GetWorldInstance().GetLevel().Water.GetData(), GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);

            /*TO DO : true - enable building reflections
             false - disable building reflections*/
            if (quality.EnableBuilding)
            {
                if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null) foreach (Building house in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection)
                    {
                        house.RenderWaterReflection(GameWorld.GetWorldInstance().GetLevel().Water.GetData(), GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                    }
            }

            if (quality.EnableMovableEntities)
            {
                if (GameWorld.GetWorldInstance().GetLevel().Player.GetData() != null) GameWorld.GetWorldInstance().GetLevel().Player.GetData().RenderWaterReflection(GameWorld.GetWorldInstance().GetLevel().Water.GetData(), GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                if (GameWorld.GetWorldInstance().GetLevel().Bots != null) foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots) { bot.RenderWaterReflection(GameWorld.GetWorldInstance().GetLevel().Water.GetData(), GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane); }
            }

            if (GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData() != null)
            {
                GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().RenderWaterReflection(GameWorld.GetWorldInstance().GetLevel().Water.GetData(), camera, ref EngineStatics.ProjectionMatrix, clipPlane);
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
            GameWorld.GetWorldInstance().GetLevel().Water.GetData().StencilPass(camera, ref EngineStatics.ProjectionMatrix);

            GL.DepthMask(true); // Enable write depth
            GL.ColorMask(true, true, true, true); // Enable write color
            GL.StencilFunc(StencilFunction.Equal, 2, 0xFF); // Set stencil function
            GL.StencilMask(0x00);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep); // Set stencil operation

            /*TO DO : true - enable EngineSingleton.Grass refractions
            false - disable EngineSingleton.Grass refractions*/
            if (quality.EnableGrassRefraction)
            {
                if (GameWorld.GetWorldInstance().GetLevel().Grass != null) GameWorld.GetWorldInstance().GetLevel().Grass.renderEntities(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, EngineStatics.ProjectionMatrix, GameWorld.GetWorldInstance().GetLevel().Terrain.GetData(), clipPlane);
                if (GameWorld.GetWorldInstance().GetLevel().Plant != null) GameWorld.GetWorldInstance().GetLevel().Plant.renderEntities(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, EngineStatics.ProjectionMatrix, GameWorld.GetWorldInstance().GetLevel().Terrain.GetData(), clipPlane);
            }

            /*TO DO : true - enable building refractions
             false - disable building refractions*/
            if (quality.EnableBuilding)
            {
                if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null) foreach (Building house in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection)
                        house.RenderWaterRefraction(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
            }

            /*TO DO : true - enable EngineSingleton.Player and EngineSingleton.Enemy refractions
            false - disable EngineSingleton.Player and EngineSingleton.Enemy refractions*/
            if (quality.EnableMovableEntities)
            {
                if (GameWorld.GetWorldInstance().GetLevel().Player.GetData() != null)
                {
                    if (GameWorld.GetWorldInstance().GetLevel().Player.GetData().IsVisibleByCamera)
                    {
                        GameWorld.GetWorldInstance().GetLevel().Player.GetData().RenderWaterRefraction(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                    }
                }

                if (GameWorld.GetWorldInstance().GetLevel().Bots != null)
                {
                    foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots)
                    {
                        if (bot.IsVisibleByCamera)
                            bot.RenderWaterRefraction(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
                    }
                }
            }

            /*TO DO :
             * Culling back faces of terrain, cause they don't refract in EngineSingleton.Water*/
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            if (GameWorld.GetWorldInstance().GetLevel().Terrain.GetData() != null) GameWorld.GetWorldInstance().GetLevel().Terrain.GetData().RenderWaterRefraction(GameWorld.GetWorldInstance().GetLevel().DirectionalLight, camera, ref EngineStatics.ProjectionMatrix, clipPlane);
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
#if DEBUG || ENGINE_EDITOR
            /*TO DO :
             * If point lights exist - show them */
            if (GameWorld.GetWorldInstance().GetLevel().PointLightDebugRenderer != null)
            {
                GameWorld.GetWorldInstance().GetLevel().PointLightDebugRenderer.Render(GameWorld.GetWorldInstance().GetLevel().Camera, EngineStatics.ProjectionMatrix);
            }
#endif
        }

        private void RenderBoundingBoxes()
        {
            Matrix4 viewMatrix = GameWorld.GetWorldInstance().GetLevel().Camera.GetViewMatrix();

            if (GameWorld.GetWorldInstance().GetLevel().Player.GetData() != null)
            {
                if (GameWorld.GetWorldInstance().GetLevel().Player.GetData().IsVisibleByCamera)
                    GameWorld.GetWorldInstance().GetLevel().Player.GetData().RenderBound(ref EngineStatics.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
            }

            if (GameWorld.GetWorldInstance().GetLevel().Bots != null)
            {
                foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots)
                {
                    if (bot.IsVisibleByCamera)
                        bot.RenderBound(ref EngineStatics.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null)
            {
                foreach (var item in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection)
                {
                    if (item.IsVisibleByCamera)
                        item.RenderBound(ref EngineStatics.ProjectionMatrix, ref viewMatrix, System.Drawing.Color.Red);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData() != null && GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().CQuad != null)
            {
                var matrix = GameWorld.GetWorldInstance().GetLevel().Camera.GetViewMatrix();
                matrix[3, 0] = 0.0f;
                matrix[3, 1] = 0.0f;
                matrix[3, 2] = 0.0f;
                GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().CQuad.RenderQuad(matrix, ref EngineStatics.ProjectionMatrix);
            }
        }

        private void RenderDebugFramePanels(ref Point actualScreenRezoltuion)
        {
            GameWorld.GetWorldInstance().GetUiFrameCreator().RenderFrames();
#if DEBUG || ENGINE_EDITOR
            if (IsSeparatedScreen)
            {
                GameWorld.GetWorldInstance().GetUiFrameCreator().RenderSeparatedScreen(DefaultFB.GetColorTexture(), actualScreenRezoltuion);
            }
#endif
        }

#endregion

    }
}
