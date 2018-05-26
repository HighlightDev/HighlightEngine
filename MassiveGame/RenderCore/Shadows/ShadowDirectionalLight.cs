using FramebufferAPI;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame.RenderCore.Shadows
{
    public class ShadowDirectionalLight : ShadowBase
    {
        private DirectionalLight LightSource;
        private ShadowMapCache cache;
        private ShadowOrthoBuilder Builder;
        private LiteCamera ViewerCamera;

        public ShadowDirectionalLight(LiteCamera viewerCamera, RenderTargetParams ShadowMapSettings, DirectionalLight LightSource) : base(ShadowMapSettings)
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
            Vector3 delta = DOUEngine.Player.ComponentTranslation;
            delta.Y = 0;
            var lightEye = LightSource.Position + delta + normLightDir * 300;
            var lightTarget = LightSource.Position + delta + (LightSource.Direction * 100);
            return Matrix4.LookAt(lightEye, lightTarget, new Vector3(0, 1, 0));
        }

        public override Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            return Matrix4.CreateOrthographic(300, 300, 1, 500);
        }

        protected override void PrepareRenderTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, RenderTargetHandler, 0);
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
