using GpuGraphics;
using MassiveGame.API.Collector;
using MassiveGame.RenderCore;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrameMaster
    {
        private VAO _buffer;
        private UiFrameShader _shader;
        private bool _postConstructor;

        public List<ITexture> frameTextures;
        public readonly Int32 MAX_FRAME_COUNT = 3;

        public UiFrameMaster()
        {
            frameTextures = new List<ITexture>(MAX_FRAME_COUNT);
            _postConstructor = true;
        }

        public void PushFrame(ITexture texture)
        {
            if (frameTextures.Count < MAX_FRAME_COUNT)
            {
                frameTextures.Add(texture);
            }
        }

        public void PopFrame()
        {
            if (frameTextures.Count != 0)
            {
                frameTextures.Remove(frameTextures.Last());
            }
        }

        public void RenderFrames()
        {
            for (Int32 i = 0; i< frameTextures.Count; i++)
            {
                ITexture frameTexture = frameTextures[i];
                Render(frameTexture, i);
            }
        }

        private Matrix4 GetScreenSpaceMatrix(Int32 layoutIndex)
        {
            Matrix4 resultMatrix = Matrix4.Identity;

            Vector2 Origin = new Vector2(-0.75f, -0.7f);
            Vector2 translation = new Vector2(Origin.X, (layoutIndex * 0.5f) + (0.15f * layoutIndex) + Origin.Y);

            resultMatrix *= Matrix4.CreateScale(0.2f, 0.25f, 1);
            resultMatrix *= Matrix4.CreateTranslation(new Vector3(translation.X, translation.Y, 0.0f));

            return resultMatrix;
        }

        private void Render(ITexture renderTexture, Int32 index)
        {
            PostConstructor();
            var screenSpaceMatrix = GetScreenSpaceMatrix(index);
            _shader.startProgram();
            renderTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetUiTextureSampler(0);
            _shader.SetScreenSpaceMatrix(screenSpaceMatrix);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public void RenderInputTexture(ITexture renderTexture, Point screenRezolution)
        {
            PostConstructor();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, screenRezolution.X, screenRezolution.Y);
            _shader.startProgram();
            renderTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetUiTextureSampler(0);
            _shader.SetScreenSpaceMatrix(Matrix4.Identity);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        private void PostConstructor()
        {
            if (_postConstructor)
            {
                _buffer = ScreenQuad.GetScreenQuadBuffer();

                _shader = (UiFrameShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "uiVS.glsl",
                    ProjectFolders.ShadersPath + "uiFS.glsl", "", typeof(UiFrameShader));
                _postConstructor = false;

            }
        }

        #region Cleaning

        public void CleanUp()
        {
            ResourcePool.ReleaseShaderProgram(_shader);
        }

        #endregion
    }
}
