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
        public readonly Int32 MAX_FRAME_COUNT = 12;
        private Int32 maxRowCount = 3;

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
            Int32 frameCount = frameTextures.Count;
            float koef = ((float)frameCount / MAX_FRAME_COUNT);
            Vector2 vecOffset = new Vector2(koef);
            Int32 rowIndex = frameCount / maxRowCount;
            Int32 columnIndex = layoutIndex % maxRowCount;
            Vector3 translation = new Vector3((vecOffset.X * columnIndex) - 1, ((vecOffset.Y * rowIndex) * 2) - 1, 0);
            return Matrix4.CreateTranslation(translation);
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
