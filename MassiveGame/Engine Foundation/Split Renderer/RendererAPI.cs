using System;
using AudioEngine;
using MassiveGame.RenderCore.Lights;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.UI
{
    partial class MainUI
    {
        #region Render functions
        private void RenderAll(bool redraw)
        {
            renderBasePass(DOUEngine.Camera, redraw);
            renderLamps();
            renderCollisionBoxes();
            TickEntities();
        }

        private void DepthPass()
        {
            if (!DOUEngine.PostConstructor)
            {
                DOUEngine.Sun.GetShadowHandler().WriteDepth(shadowList, this.Width, this.Height, ref DOUEngine.ProjectionMatrix);
                RestoreViewport();
            }
        }

        private void RestoreViewport()
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        private void renderLamps()
        {
            /*TO DO :
             * If point lights exist - show them */
            if (DOUEngine.ShowLightSource)
            {
                if (DOUEngine.PointLight != null)
                {
                    DOUEngine.Lights.render(DOUEngine.Camera, DOUEngine.ProjectionMatrix);
                }
            }
        }

        private void renderCollisionBoxes()
        {
            /*TO DO :
             * Render EngineSingleton.Collision boxes for debugging  */
            if (DOUEngine.CollisionBoxRender) //Render boxes for debugging
            {
                if (DOUEngine.Player != null)
                {
                    if (DOUEngine.Player.IsInCameraView)
                    {
                        DOUEngine.Player.Box.renderBox(DOUEngine.Camera.getViewMatrix(), ref DOUEngine.ProjectionMatrix);
                    }
                }

                if (!Object.Equals(DOUEngine.Enemy, null))
                {
                    if (DOUEngine.Enemy.IsInCameraView)
                    {
                        DOUEngine.Enemy.Box.renderBox(DOUEngine.Camera.getViewMatrix(), ref DOUEngine.ProjectionMatrix);
                    }
                }

                DOUEngine.Collision.renderWallBoxes(DOUEngine.Camera.getViewMatrix(), ref DOUEngine.ProjectionMatrix);
                var matrix = DOUEngine.Camera.getViewMatrix();
                matrix[3, 0] = 0.0f;
                matrix[3, 1] = 0.0f;
                matrix[3, 2] = 0.0f;
                if (!Object.Equals(DOUEngine.SunReplica, null)) DOUEngine.SunReplica.CQuad.renderQuad(matrix, ref DOUEngine.ProjectionMatrix);
            }
        }

        private void TickEntities()
        {
            Matrix4 viewMatrix = DOUEngine.Camera.getViewMatrix();

            if (DOUEngine.Player != null)
            {
                if (DOUEngine.Player.IsInCameraView)
                {
                    DOUEngine.Player.Tick(ref DOUEngine.ProjectionMatrix, ref viewMatrix);
                }
            }

            if (!Object.Equals(DOUEngine.Enemy, null))
            {
                if (DOUEngine.Enemy.IsInCameraView)
                {
                    DOUEngine.Enemy.Tick(ref DOUEngine.ProjectionMatrix, ref viewMatrix);
                }
            }
        }

        private void renderBasePass(Camera camera, bool redraw = false)
        {
            /*TO DO :
             * Culling back facies of EngineSingleton.Skybox (cause we don't see them)
             * Culling back facies of terrain
             * Culling back face of EngineSingleton.Sun
             * Clearing depth buffer, cause EngineSingleton.Skybox is infinite   */

            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            if (DOUEngine.Skybox != null)
                DOUEngine.Skybox.renderSkybox(DOUEngine.Camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix);

            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (DOUEngine.Map != null) DOUEngine.Map.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);

            if (!Object.Equals(DOUEngine.SunReplica, null))
            {
                if (DOUEngine.SunReplica.IsInCameraView)
                {
                    DOUEngine.SunReplica.renderSun(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            GL.Disable(EnableCap.CullFace);

            if (DOUEngine.Grass != null) DOUEngine.Grass.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime, DOUEngine.Map);
            if (DOUEngine.Plant1 != null) DOUEngine.Plant1.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime, DOUEngine.Map);

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
                    DOUEngine.Player.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, DOUEngine.Map, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            if (DOUEngine.Enemy != null)
            {
                if (DOUEngine.Enemy.IsInCameraView)
                {
                    DOUEngine.Enemy.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, DOUEngine.Map, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            if (redraw)
            {
                foreach (Building building in DOUEngine.Buildings)
                {
                    building.renderObject(DOUEngine.Mode, false, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            /*TO DO :
             true - allow EngineSingleton.Water rendering 
             false - disallow */
            if (DOUEngine.Water != null && DOUEngine.Water.IsInCameraView)
            {
                GL.CullFace(CullFaceMode.Back);
                GL.Enable(EnableCap.CullFace);

                DOUEngine.Water.renderWater(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime,
                        DOUEngine.NEAR_CLIPPING_PLANE, DOUEngine.FAR_CLIPPING_PLANE, DOUEngine.Sun, DOUEngine.PointLight);

                GL.Disable(EnableCap.CullFace);
            }
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

            /* Culling back facies of terrain
             * Culling back facies of EngineSingleton.Skybox
             * Disable depth test cause skybox is infinite     */

            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            if (DOUEngine.Skybox != null) DOUEngine.Skybox.RenderWaterReflection(DOUEngine.Water, camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix, clipPlane);

            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.CullFace(CullFaceMode.Back);

            if (DOUEngine.Map != null) DOUEngine.Map.RenderWaterReflection(DOUEngine.Water, DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
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

        /*Render settings for EngineSingleton.Water refractions*/
        private void RenderToRefractionRenderTarget(LiteCamera camera, Vector4 clipPlane, WaterQuality quality, bool redraw = false)
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
                if (DOUEngine.Grass != null) DOUEngine.Grass.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime, DOUEngine.Map, clipPlane);
                if (DOUEngine.Plant1 != null) DOUEngine.Plant1.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime, DOUEngine.Map, clipPlane);
            }

            /*TO DO : true - enable building refractions
             false - disable building refractions*/
            if (quality.EnableBuilding)
            {
                if (DOUEngine.City != null) foreach (Building house in DOUEngine.City)
                        house.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);

                // add code of adding EngineSingleton.Models here
                if (redraw)
                {
                    foreach (Building building in DOUEngine.Buildings)
                    {
                        building.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
                    }
                }
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
            if (DOUEngine.Map != null) DOUEngine.Map.RenderWaterRefraction(DOUEngine.Sun, camera, ref DOUEngine.ProjectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.StencilTest); // Enable stencil test
        }

        private void RenderToWaterRendertargets()
        {
            if (DOUEngine.Water != null && DOUEngine.Water.IsInCameraView)
            {
                DOUEngine.Water.SetReflectionRendertarget();
                RenderToReflectionRenderTarget(DOUEngine.Camera, new Vector4(0, -1, 0, DOUEngine.Water.WaterHeight), DOUEngine.Water.Quality);

                DOUEngine.Water.SetRefractionRendertarget();
                RenderToRefractionRenderTarget(DOUEngine.Camera, new Vector4(0, -1, 0, DOUEngine.Water.WaterHeight), DOUEngine.Water.Quality);
            }
        }

        private void renderToLensFlareScene(Camera camera, bool redraw = false)
        {
            /*TO DO :
             * Culling back facies of all objects on scene
             * and enabling color masking */

            GL.ColorMask(false, false, false, true);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            if (DOUEngine.Map != null) DOUEngine.Map.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);
            if (DOUEngine.City != null) foreach (Building house in DOUEngine.City)
                { house.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix); }

            if (DOUEngine.Player != null)
            {
                if (DOUEngine.Player.IsInCameraView)
                {
                    DOUEngine.Player.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, DOUEngine.Map, camera, ref DOUEngine.ProjectionMatrix);
                }
            }
            if (DOUEngine.Enemy != null) DOUEngine.Enemy.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, DOUEngine.Map, camera, ref DOUEngine.ProjectionMatrix);

            // add code of adding EngineSingleton.Models here
            if (redraw)
            {
                foreach (Building building in DOUEngine.Buildings)
                {
                    building.renderObject(DOUEngine.Mode, false, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

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

        private void renderToGodRaysScene(Camera camera, bool redraw = false)
        {
            /*TO DO :
            * Culling back facies and enabling color masking */
            GL.ColorMask(false, false, false, true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (DOUEngine.Skybox != null) DOUEngine.Skybox.renderSkybox(DOUEngine.Camera, DOUEngine.Sun, DOUEngine.ProjectionMatrix);

            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (DOUEngine.Map != null) DOUEngine.Map.renderTerrain(DOUEngine.Mode, DOUEngine.Sun, DOUEngine.PointLight, camera, DOUEngine.ProjectionMatrix);

            GL.Disable(EnableCap.CullFace);


            if (DOUEngine.Plant1 != null) DOUEngine.Plant1.renderEntities(DOUEngine.Sun, camera, DOUEngine.ProjectionMatrix, (float)DOUEngine.RenderTime, DOUEngine.Map);

            if (DOUEngine.City != null) foreach (Building house in DOUEngine.City)
                { house.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix); }

            if (DOUEngine.Player != null)
            {
                if (DOUEngine.Player.IsInCameraView)
                {
                    DOUEngine.Player.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, DOUEngine.Map, camera, ref DOUEngine.ProjectionMatrix);
                }
            }
            if (DOUEngine.Enemy != null) DOUEngine.Enemy.renderObject(DOUEngine.Mode, DOUEngine.NormalMapTrigger, DOUEngine.Sun, DOUEngine.PointLight, DOUEngine.Map, camera, ref DOUEngine.ProjectionMatrix);

            // add code of adding EngineSingleton.Models here
            if (redraw)
            {
                foreach (Building building in DOUEngine.Buildings)
                {
                    building.renderObject(DOUEngine.Mode, false, DOUEngine.Sun, DOUEngine.PointLight, camera, ref DOUEngine.ProjectionMatrix);
                }
            }

            GL.ColorMask(true, true, true, true);

            if (!Object.Equals(DOUEngine.SunReplica, null))
            {
                if (DOUEngine.SunReplica.IsInCameraView)
                {
                    DOUEngine.SunReplica.renderSun(DOUEngine.Camera, ref DOUEngine.ProjectionMatrix);
                }
            }
        }

        private bool blurEnable = false;    //temp
        #endregion

        #region RenderFrame functions
        private void RenderFrame(double time)
        {
            renderTime.Start();
            DisplayGraphics(DOUEngine.RedrawScene);
            DOUEngine.RenderTime = (float)renderTime.Elapsed.TotalSeconds;
            renderTime.Reset();
        }

        private void DisplayGraphics(bool redraw = false)
        {
            GL.Enable(EnableCap.DepthTest); //Включаем тест глубины
            ClearScreen();
            postConstructor();
            gameLogics(redraw);
            RenderLoop(redraw);
            if (DOUEngine.PostConstructor)
            {
                DOUEngine.PostConstructor = !DOUEngine.PostConstructor;
            }
            GLControl.SwapBuffers();
        }

        private void UpdateFrame()
        {
            AdjustMouseCursor();

            AudioMaster.SetListenerData(
                DOUEngine.Camera.getPosition().X,
                DOUEngine.Camera.getPosition().Y,
                DOUEngine.Camera.getPosition().Z
            );
        }
        #endregion
    }
}
