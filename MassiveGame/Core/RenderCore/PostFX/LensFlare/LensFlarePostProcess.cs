using OpenTK;
using TextureLoader;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MassiveGame.Core.GameCore;
using MassiveGame.Settings;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.LensFlare
{
    public class LensFlarePostProcess<SubsequenceType> : PostProcessBase
        where SubsequenceType : PostProcessSubsequenceType
    {
        private LensFlareFramebufferObject renderTarget;
        private LensFlareShader<SubsequenceType> lensShader;
      
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
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                renderTarget = new LensFlareFramebufferObject();
                lensShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<LensFlareShader<SubsequenceType>>, string, LensFlareShader<SubsequenceType>>(ProjectFolders.ShadersPath + "lensFlareVS.glsl" + "," + ProjectFolders.ShadersPath + "lensFlareFS.glsl");
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
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            lensShader.stopProgram();

            /*Lens effect*/
            renderTarget.renderToFBO(1, this.renderTarget.verticalBlurTexture.GetTextureRezolution());

            lensShader.startProgram();
            renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
            lensShader.setUniformValuesLens(0, this.Ghosts, this.HaloWidth, this.Distortion, this.GhostDispersal);
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            lensShader.stopProgram();

            /*Extra passes for blur*/
            for (int i = 0; i < BlurPassCount; i++)
            {
                /*Horizontal Blur effect*/
                renderTarget.renderToFBO(2, this.renderTarget.horizontalBlurTexture.GetTextureRezolution());

                lensShader.startProgram();
                renderTarget.verticalBlurTexture.BindTexture(TextureUnit.Texture0);
                lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(BlurWidth), getPixOffset(BlurWidth), actualScreenRezolution);
                quadBuffer.RenderVAO(PrimitiveType.Triangles);
                lensShader.stopProgram();

                /*Vertical Blur effect*/
                renderTarget.renderToFBO(1, this.renderTarget.verticalBlurTexture.GetTextureRezolution());

                lensShader.startProgram();
                renderTarget.horizontalBlurTexture.BindTexture(TextureUnit.Texture0);
                lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(BlurWidth), getPixOffset(BlurWidth), actualScreenRezolution);
                quadBuffer.RenderVAO(PrimitiveType.Triangles);
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
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            lensShader.stopProgram();

            renderTarget.unbindFramebuffer();

            return renderTarget.lensFlareResultTexture;
        }

        protected override void RenderScene(BaseCamera camera)
        {
            /*TO DO :
             * Culling back faces of all objects on scene
             * and enabling color masking */

            GL.ColorMask(false, false, false, true);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (GameWorld.GetWorldInstance().GetLevel().Skybox != null)
            {
                GameWorld.GetWorldInstance().GetLevel().Skybox.renderSkybox(GameWorld.GetWorldInstance().GetLevel().Camera, GameWorld.GetWorldInstance().GetLevel().DirectionalLight.Direction, EngineStatics.ProjectionMatrix);
            }

            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (GameWorld.GetWorldInstance().GetLevel().Terrain.GetData() != null)
            {
                GameWorld.GetWorldInstance().GetLevel().Terrain.GetData().renderTerrain(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection, camera, EngineStatics.ProjectionMatrix);
            }

            if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null)
            {
                foreach (Building house in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection)
                {
                    if (house.IsVisibleByCamera)
                        house.renderObject(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection, camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().Player.GetData() != null && GameWorld.GetWorldInstance().GetLevel().Player.GetData().IsVisibleByCamera)
            {
                GameWorld.GetWorldInstance().GetLevel().Player.GetData().RenderEntity(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection, camera, ref EngineStatics.ProjectionMatrix);
            }

            if (GameWorld.GetWorldInstance().GetLevel().Bots != null)
            {
                foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots)
                {
                    if (bot.IsVisibleByCamera)
                        bot.RenderEntity(EngineStatics.Mode, GameWorld.GetWorldInstance().GetLevel().DirectionalLight, GameWorld.GetWorldInstance().GetLevel().PointLightCollection, camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().Water.GetData() != null && GameWorld.GetWorldInstance().GetLevel().Water.GetData().IsInCameraView)
            {
                GameWorld.GetWorldInstance().GetLevel().Water.GetData().StencilPass(camera, ref EngineStatics.ProjectionMatrix);
            }

            /*Disable color masking*/
            GL.ColorMask(true, true, true, true);
            if (GameWorld.GetWorldInstance().GetLevel().SunRenderer != null && GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().IsInCameraView)
            {
                GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().Render(GameWorld.GetWorldInstance().GetLevel().Camera, ref EngineStatics.ProjectionMatrix, true);
            }

            /*Stop culling*/
            GL.Disable(EnableCap.CullFace);
        }

        public override void CleanUp()
        {
            renderTarget.cleanUp();
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<LensFlareShader<SubsequenceType>>, string, LensFlareShader<SubsequenceType>>(lensShader);
        }
    }
}