using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using System.Drawing;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.LightShafts
{
    public class LightShaftPostProcess<SubsequenceType> : PostProcessBase
        where SubsequenceType: PostProcessSubsequenceType
    {
        public LightShaftFramebufferObject renderTarget;

        private LightShaftShader<SubsequenceType> shader;

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
            if (bPostConstructor)
            {
                renderTarget = new LightShaftFramebufferObject();
                shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<LightShaftShader<SubsequenceType>>, string, LightShaftShader<SubsequenceType>>(ProjectFolders.ShadersPath + "lightShaftsVS.glsl" + "," + ProjectFolders.ShadersPath + "lightShaftsFS.glsl");
                bPostConstructor = false;
            }
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

        public override ITexture GetPostProcessResult(ITexture frameColorTexture, ITexture frameDepthTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            postConstructor();

            // Rendering bright objects to render target
            renderTarget.renderToFBO(1, renderTarget.RadialBlurAppliedTexture.GetTextureRezolution());
            base.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution);

            viewportMatrix = new Matrix4(new Vector4(actualScreenRezolution.X * 0.5f, 0, actualScreenRezolution.X * 0.5f, 0),
              new Vector4(0, actualScreenRezolution.Y * 0.5f, actualScreenRezolution.Y * 0.5f, 0),
              new Vector4(0, 0, 1, 0),
              new Vector4(0, 0, 0, 1));

            var radialBlurScreenSpacePosition = getRadialPos(EngineStatics.Sun.Position, EngineStatics.Camera.GetViewMatrix(), ref EngineStatics.ProjectionMatrix, ref viewportMatrix);

            // Render to light shaft result render target
            renderTarget.renderToFBO(2, renderTarget.LightShaftsResultTexture.GetTextureRezolution());

            shader.startProgram();

            if (previousPostProcessResult != null)
            {
                previousPostProcessResult.BindTexture(TextureUnit.Texture1);
                shader.SetPreviousPostProcessResultSampler(1);
            }

            renderTarget.RadialBlurAppliedTexture.BindTexture(TextureUnit.Texture0);

            shader.SetBrightPartsTextureSampler(0);
            shader.SetRadialBlurCenterPositionInScreenSpace(radialBlurScreenSpacePosition);
            shader.SetRadialBlurExposure(Exposure);
            shader.SetRadialBlurDecay(Decay);
            shader.SetRadialBlurDensity(Density);
            shader.SetRadialBlurNumberOfSamples(NumSamples);
            shader.SetRadialBlurWeight(Weight);
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            shader.stopProgram();

            renderTarget.unbindFramebuffer();

            return renderTarget.LightShaftsResultTexture;
        }

        protected override void RenderScene(BaseCamera camera)
        {
            // Culling back faces and enabling color masking 
            GL.ColorMask(false, false, false, true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (EngineStatics.Skybox != null)
            {
                EngineStatics.Skybox.renderSkybox(EngineStatics.Camera, EngineStatics.Sun, EngineStatics.ProjectionMatrix);
            }

            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (EngineStatics.terrain != null)
            {
                EngineStatics.terrain.renderTerrain(EngineStatics.Mode, EngineStatics.Sun, EngineStatics.PointLight, camera, EngineStatics.ProjectionMatrix);
            }

            GL.Disable(EnableCap.CullFace);

            if (EngineStatics.Plant1 != null)
            {
                EngineStatics.Plant1.renderEntities(EngineStatics.Sun, camera, EngineStatics.ProjectionMatrix, EngineStatics.terrain);
            }

            if (EngineStatics.City != null)
            {
                foreach (Building house in EngineStatics.City)
                {
                    if (house.IsVisibleByCamera)
                        house.renderObject(EngineStatics.Mode, EngineStatics.NormalMapTrigger, EngineStatics.Sun, EngineStatics.PointLight, camera, ref EngineStatics.ProjectionMatrix);
                }
            }

            if (EngineStatics.Player != null && EngineStatics.Player.IsVisibleByCamera)
            {
                EngineStatics.Player.RenderEntity(EngineStatics.Mode, EngineStatics.NormalMapTrigger, EngineStatics.Sun, EngineStatics.PointLight, camera, ref EngineStatics.ProjectionMatrix);
            }

            if (EngineStatics.Enemy != null && EngineStatics.Enemy.IsVisibleByCamera)
            {
                EngineStatics.Enemy.RenderEntity(EngineStatics.Mode, EngineStatics.NormalMapTrigger, EngineStatics.Sun, EngineStatics.PointLight, camera, ref EngineStatics.ProjectionMatrix);
            }

            if (EngineStatics.Water != null && EngineStatics.Water.IsInCameraView)
            {
                EngineStatics.Water.StencilPass(camera, ref EngineStatics.ProjectionMatrix);
            }

            GL.ColorMask(true, true, true, true);

            if (EngineStatics.SunReplica != null && EngineStatics.SunReplica.IsInCameraView)
            {
                EngineStatics.SunReplica.renderSun(EngineStatics.Camera, ref EngineStatics.ProjectionMatrix);
            }

            /*Stop culling*/
            GL.Disable(EnableCap.CullFace);
        }

        public override void CleanUp()
        {
            renderTarget.cleanUp();
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<LightShaftShader<SubsequenceType>>, string, LightShaftShader<SubsequenceType>>(shader);
        }
    }
}
