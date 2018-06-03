using GpuGraphics;
using MassiveGame.API.Collector;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using System.Drawing;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrame
    {
        #region Definitions

        private VAO _buffer;
        private UiFrameShader _shader;
        private bool _postConstructor;

        public float X0, Y0, Width, Height;

        #endregion
        
        public void Render(ITexture renderTexture)
        {
            PostConstructor();
            var textureRezolution = renderTexture.GetTextureRezolution();
            GL.Viewport(0, 0, textureRezolution.X, textureRezolution.Y);

            _shader.startProgram();
            renderTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetUiTextureSampler(0);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public void Render(ITexture renderTexture, Point screenRezolution)
        {
            PostConstructor();
            GL.Viewport(0, 0, screenRezolution.X, screenRezolution.Y);

            _shader.startProgram();
            renderTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetUiTextureSampler(0);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public UiFrame(Vector2 origin, Vector2 extent) : this(origin.X, origin.Y, extent.X, extent.Y)
        {
        }

        public UiFrame(float x, float y, float width, float height)
        {
            _postConstructor = true;
            _buffer = BuildUiMesh(x, y, width, height);
            VAOManager.genVAO(_buffer);
            VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);
        }

        private void PostConstructor()
        {
            if (_postConstructor)
            {
                _shader = (UiFrameShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "uiVS.glsl",
                    ProjectFolders.ShadersPath + "uiFS.glsl", "", typeof(UiFrameShader));
                _postConstructor = false;
            }
        }

        private VAO BuildUiMesh(float TexCoordsX0, float TexCoordsY0, float TexCoordsWidth, float TexCoordsHeight)
        {
            X0 = TexCoordsX0;
            Y0 = TexCoordsY0;
            Height = TexCoordsHeight;

            float NormalizedDeviceCoordinatesX0 = (TexCoordsX0 - 0.5f) * 2; 
            float NormalizedDeviceCoordinatesY0 = (TexCoordsY0 - 0.5f) * 2;
            float NormalizedDeviceCoordinatesX1 = ((TexCoordsX0 + TexCoordsWidth) - 0.5f) * 2;
            float NormalizedDeviceCoordinatesY1 = ((TexCoordsY0 + TexCoordsHeight) - 0.5f) * 2;

            VAO result = null;
            Vector2 p1 = new Vector2(NormalizedDeviceCoordinatesX0, NormalizedDeviceCoordinatesY0), p2 = new Vector2(NormalizedDeviceCoordinatesX1, NormalizedDeviceCoordinatesY0),
                p3 = new Vector2(NormalizedDeviceCoordinatesX1, NormalizedDeviceCoordinatesY1),
                p4 = p3, p5 = new Vector2(NormalizedDeviceCoordinatesX0, NormalizedDeviceCoordinatesY1), p6 = p1;

            var attributes = new VBOArrayF(
                new float[6, 3] { { p1.X, p1.Y, 0.0f }, { p2.X, p2.Y, 0.0f }, { p3.X, p3.Y, 0.0f },
                { p4.X, p4.Y, 0.0f }, { p5.X, p5.Y, 0.0f }, { p6.X, p6.Y, 0.0f} },
                new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, null
            );
            result = new VAO(attributes);           
            return result;
        }

        #region Cleaning

        public void CleanUp()
        {
            VAOManager.cleanUp(_buffer);
            ResourcePool.ReleaseShaderProgram(_shader);
        }

        #endregion
    }
}
