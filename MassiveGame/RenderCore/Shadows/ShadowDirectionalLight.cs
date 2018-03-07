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
            var lightEye = LightSource.Position + (LightSource.Direction.Normalized() * 250);
            var lightTarget = LightSource.Position + LightSource.Direction * 100;
            return Matrix4.LookAt(lightEye, lightTarget, new Vector3(0, 1, 0));
        }

        public override Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            // temporary make extremely large ortho cube
            //return Builder.CreateOrthographicProjection(ViewerCamera, ref projectionMatrix);
            return Matrix4.CreateOrthographic(300, 300, 1 , 500);
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
