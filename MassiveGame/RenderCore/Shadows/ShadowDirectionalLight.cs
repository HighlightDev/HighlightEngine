using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;
using TextureLoader;
using MassiveGame.Core;

namespace MassiveGame.RenderCore.Shadows
{
    public class ShadowDirectionalLight : ShadowBase
    {
        private DirectionalLight LightSource;
        private ShadowMapCache cache;
        private ShadowOrthoBuilder Builder;
        private Camera ViewerCamera;

        public ShadowDirectionalLight(Camera viewerCamera, TextureParameters ShadowMapSettings, DirectionalLight LightSource) : base(ShadowMapSettings)
        {
            ViewerCamera = viewerCamera;
            Builder = new ShadowOrthoBuilder();
            cache = null;
            this.LightSource = LightSource;
        }

        public override ShadowTypes GetShadowType()
        {
            return ShadowTypes.DirectionalLight;
        }

        public override Matrix4 GetShadowViewMatrix()
        {
            Vector3 normLightDir = LightSource.Direction.Normalized();

            Vector3 targetPositon = ViewerCamera.GetThirdPersonTarget().GetCharacterCollisionBound().GetOrigin();
            Vector3 shadowCastPosition = new Vector3(targetPositon.X, LightSource.Position.Y, targetPositon.Z);
            Vector3 lightTranslatedPosition = normLightDir * 300;
            var lightEye = new Vector3(shadowCastPosition.X - lightTranslatedPosition.X, shadowCastPosition.Y + lightTranslatedPosition.Y, shadowCastPosition.Z - lightTranslatedPosition.Z);
            var lightTarget = shadowCastPosition + (LightSource.Direction * 100);
            return Matrix4.LookAt(lightEye, lightTarget, new Vector3(0, 1, 0));
        }

        public override Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            return Matrix4.CreateOrthographic(400, 400, 1, 500);
        }

        protected override void PrepareRenderTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, ShadowMapTexture.GetTextureDescriptor(), 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void CreateShadowMapCache()
        {
            cache = new ShadowMapCache();
        }

    }
}
