using TextureLoader;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MassiveGame.Core.SettingsCore;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore
{
    public static class TextureResolver
    {
        private static CopyTextureShader copyShader;
        private static ResolvePostProcessResultToDefaultFramebufferShader resolvePostProcessShader;

        static TextureResolver()
        {
            copyShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<CopyTextureShader>, string, CopyTextureShader>(ProjectFolders.ShadersPath + "copyTextureVS.glsl" + "," + ProjectFolders.ShadersPath + "copyTextureFS.glsl");

            resolvePostProcessShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<ResolvePostProcessResultToDefaultFramebufferShader>, string, ResolvePostProcessResultToDefaultFramebufferShader>(ProjectFolders.ShadersPath + "resolvePostProcessResultToDefaultFramebufferVS.glsl" + "," + ProjectFolders.ShadersPath + "resolvePostProcessResultToDefaultFramebufferFS.glsl");
        }

        public static void ResolvePostProcessResultToDefaultFramebuffer(ITexture frameTexture, ITexture postProcessResultTexture, Point actualScreenRezolution)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, actualScreenRezolution.X, actualScreenRezolution.Y);

            //GL.Disable(EnableCap.DepthTest);
            resolvePostProcessShader.startProgram();

            frameTexture.BindTexture(TextureUnit.Texture0);
            postProcessResultTexture.BindTexture(TextureUnit.Texture1);
            resolvePostProcessShader.setPostProcessResultSampler(1);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            resolvePostProcessShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
        }

        public static ITexture CopyTexture(ITexture src)
        {
            ITexture dst = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(src.GetTextureParameters());

            var renderTarget = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, src.GetTextureParameters().TexTarget, dst.GetTextureDescriptor(), 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            //GL.Disable(EnableCap.DepthTest);
            GL.Viewport(0, 0, src.GetTextureRezolution().X, src.GetTextureRezolution().Y);
            // start copy texture to render target
            copyShader.startProgram();
            src.BindTexture(TextureUnit.Texture0);
            copyShader.SetUniformValues(0);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            copyShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(renderTarget);

            return dst;
        }

        public static void CleanUp()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<CopyTextureShader>, string, CopyTextureShader>(copyShader);
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<ResolvePostProcessResultToDefaultFramebufferShader>, string, ResolvePostProcessResultToDefaultFramebufferShader>(resolvePostProcessShader);
        }
    }
}
