using FramebufferAPI;
using GpuGraphics;
using MassiveGame.API.Collector;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace MassiveGame.RenderCore.Shadows
{
    public abstract class ShadowBase
    {
        public abstract ShadowTypes GetShadowType();
        public abstract Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix);
        public abstract Matrix4 GetShadowViewMatrix();
        protected abstract void PrepareRenderTarget();

        protected TextureParameters RTParams;
        protected ITexture ShadowMapTexture;
        protected Int32 FramebufferHandler;

        protected BasicShadowShader shadowShader;

        protected Matrix4 ShadowBiasMatrix = new Matrix4(
                0.5f, 0, 0, 0,
                0, 0.5f, 0, 0,
                0, 0, 0.5f, 0,
                0.5f, 0.5f, 0.5f, 1
                );

        public Matrix4 GetShadowMatrix(ref Matrix4 ModelMatrix, ref Matrix4 ProjectionMatrix)
        {
            Matrix4 result = ModelMatrix;
            result *= GetShadowViewMatrix();
            result *= GetShadowProjectionMatrix(ref ProjectionMatrix);
            result *= ShadowBiasMatrix;
            return result;
        }

        public void AllocateRenderTarget(TextureParameters shadowMapSettings)
        {
            RTParams = shadowMapSettings;
            ShadowMapTexture = ResourcePool.GetRenderTarget(shadowMapSettings);
            FramebufferHandler = GL.GenFramebuffer();
        }

        public void DeallocateRenderTarget()
        {
            ResourcePool.ReleaseRenderTarget(ShadowMapTexture);
            GL.DeleteFramebuffer(FramebufferHandler);
        }

        public void WriteDepth(IList<IDrawable> CastingShadowActors, ref Matrix4 ProjectionMatrix)
        {
            GL.Viewport(0, 0, RTParams.TexBufferWidth, RTParams.TexBufferHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 shadowProjectionMatrix = GetShadowProjectionMatrix(ref ProjectionMatrix);
            Matrix4 shadowViewMatrix = GetShadowViewMatrix();

            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);
            shadowShader.startProgram();
            foreach (IDrawable actor in CastingShadowActors)
            {
                if (actor != null)
                {
                    shadowShader.SetUniformValues(actor.GetWorldMatrix(), shadowViewMatrix, shadowProjectionMatrix);
                    VAOManager.renderBuffers(actor.GetModel(), OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
                }
            }
            shadowShader.stopProgram();
            //GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public ShadowBase(TextureParameters ShadowMapSettings)
        {
            shadowShader = (BasicShadowShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "basicShadowVS.glsl", ProjectFolders.ShadersPath + "basicShadowFS.glsl", "", typeof(BasicShadowShader));
            AllocateRenderTarget(ShadowMapSettings);
            PrepareRenderTarget();
        }

        public void CleanUp()
        {
            DeallocateRenderTarget();
        }

        public ITexture GetShadowMapTexture()
        {
            return ShadowMapTexture;
        }
    }
}
