using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using GpuGraphics;
using ShaderPattern;
using FramebufferAPI;
using MassiveGame.RenderCore;
using TextureLoader;
using MassiveGame.Debug.UiPanel;
using MassiveGame.PostFX;

namespace MassiveGame
{
    public class GodRaysRenderer
    {
        #region Definitions

        private VBOArrayF _attribs;
        private VAO _buffer;
        private bool _postConstructor;
        public GodRaysFBO _fbo;
        private GodRaysShader<PostProcessSubsequenceType> _shader;
        private Matrix4 _viewportMatrix;

        public uint FilterResult { private set; get; }
        public float Exposure{ set; get; }
        public float Decay { set; get; }
        public float Weight { set; get; }
        public float Density { set; get; }
        public Int32 NumSamples { set; get; }
        public static bool GodRaysEnabled = false;

        #endregion

        #region Renderer

        public void beginGodRaysSimple()
        {
            postConstructor();
            _fbo.renderToFBO(1, _fbo.FrameTexture.GetTextureRezolution().X, _fbo.FrameTexture.GetTextureRezolution().Y);
        }

        public void beginGodRaysSpecial()
        {
            postConstructor();
            _fbo.renderToFBO(2, _fbo.RadialBlurAppliedTexture.GetTextureRezolution().X, _fbo.RadialBlurAppliedTexture.GetTextureRezolution().Y);
        }

        public void endGodRaysWithoutPostprocess(Int32 width, Int32 height, Vector3 translation, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            _viewportMatrix = new Matrix4(new Vector4(width / 2, 0, width / 2, 0),
                new Vector4(0, height / 2, height / 2, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));

            _fbo.unbindFramebuffer();
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            Vector2 radialPosition = getRadialPos(translation, viewMatrix, projectionMatrix, _viewportMatrix);


            _shader.startProgram();
            _fbo.FrameTexture.BindTexture(TextureUnit.Texture0);
            _fbo.RadialBlurAppliedTexture.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValuesRadialBlur(0, 1, Exposure, Decay, Weight, Density, NumSamples, radialPosition);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public void endGodRaysWithPostprocess(Int32 width, Int32 height, Vector3 translation,
            Matrix4 viewMatrix, Matrix4 projectionMatrix, uint filterResult)
        {
            _viewportMatrix = new Matrix4(new Vector4(width / 2, 0, width / 2, 0),
                new Vector4(0, height / 2, height / 2, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));

            _fbo.unbindFramebuffer();
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vector2 radialPosition = getRadialPos(translation, viewMatrix, projectionMatrix, _viewportMatrix);

            _shader.startProgram();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, filterResult);
            _fbo.RadialBlurAppliedTexture.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValuesRadialBlur(0, 1, Exposure, Decay, Weight, Density, NumSamples, radialPosition);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public void sendGodRaysWithPostprocessToNextStage(Int32 width, Int32 height, Vector3 translation,
            Matrix4 viewMatrix, Matrix4 projectionMatrix, uint filterResult)
        {

            _viewportMatrix = new Matrix4(new Vector4(width / 2, 0, width / 2, 0),
                new Vector4(0, height / 2, height / 2, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));

             _fbo.renderToFBO(1, _fbo.FrameTexture.GetTextureRezolution().X, _fbo.FrameTexture.GetTextureRezolution().X);

            Vector2 radialPosition = getRadialPos(translation, viewMatrix, projectionMatrix, _viewportMatrix);


            _shader.startProgram();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, filterResult);
            _fbo.RadialBlurAppliedTexture.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValuesRadialBlur(0, 1, Exposure, Decay, Weight, Density, NumSamples, radialPosition);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();

            FilterResult = _fbo.FrameTexture.GetTextureDescriptor();
        }

        public void sendGodRaysWithoutPostprocessToNextStage(Int32 width, Int32 height, Vector3 translation,
            Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {

            _viewportMatrix = new Matrix4(new Vector4(width / 2, 0, width / 2, 0),
                new Vector4(0, height / 2, height / 2, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));

            _fbo.renderToFBO(3, _fbo.LensFlareTexture.GetTextureRezolution().X, _fbo.LensFlareTexture.GetTextureRezolution().Y);

            Vector2 radialPosition = getRadialPos(translation, viewMatrix, projectionMatrix, _viewportMatrix);


            _shader.startProgram();
            _fbo.FrameTexture.BindTexture(TextureUnit.Texture0);
            _fbo.RadialBlurAppliedTexture.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValuesRadialBlur(0, 1, Exposure, Decay, Weight, Density, NumSamples, radialPosition);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _shader.stopProgram();

            FilterResult = _fbo.LensFlareTexture.GetTextureDescriptor();
        }

        #endregion 

        #region Calculations

        private Vector2 getRadialPos(Vector3 translation, Matrix4 viewMatrix, Matrix4 projectionMatrix,
            Matrix4 viewportMatrix)
        {
            viewMatrix[3, 0] = 0.0f;    //Restrict x-axis translation
            viewMatrix[3, 1] = 0.0f;    //Restrict y-axis translation
            viewMatrix[3, 2] = 0.0f;    //Restrict z-axis translation
            Vector4 temp = new Vector4(translation, 1.0f);
            Matrix4 transformationMatrix = viewMatrix * projectionMatrix;

            Vector4 clippedSpace = VMath.VectorMath.multMatrix(transformationMatrix, temp);
            Vector2 ndc = clippedSpace.Xy / clippedSpace.W;
            ndc.X = ndc.X / 2 + 0.5f;
            ndc.Y = ndc.Y / 2 + 0.5f;

            //Calibration
            ndc.Y -= 0.003f;
            return ndc;
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (_postConstructor)
            {
                DOUEngine.uiFrameCreator.PushFrame(_fbo.RadialBlurAppliedTexture);
                
                  _postConstructor = !_postConstructor;
            }
        }

        public GodRaysRenderer()
        {
            this._attribs = new VBOArrayF(new float[6, 3] { { -1.0f, -1.0f, 0.0f }, { 1.0f, -1.0f, 0.0f }, { 1.0f, 1.0f, 0.0f },
            { 1.0f, 1.0f, 0.0f }, { -1.0f, 1.0f, 0.0f } ,  { -1.0f, -1.0f, 0.0f} },
               new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, null);
            _buffer = new VAO(_attribs);
            _postConstructor = true;
            Exposure = 0.0064f;
            Decay = 1.05f;
            Density = 0.84f;
            Weight = 5.65f;	
            NumSamples = 25;
            GodRaysEnabled = true;

            VAOManager.genVAO(_buffer);
            VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);
            //_shader = new GodRaysShader<PostProcessSubsequenceType>(ProjectFolders.ShadersPath + "godrayVS.glsl", ProjectFolders.ShadersPath + "godrayFS.glsl");
            _fbo = new GodRaysFBO();
        }

        public GodRaysRenderer(float Exposure, float Decay, float Density, float Weight, byte NumSamples)
        {
            this._attribs = new VBOArrayF(new float[6, 3] { { -1.0f, -1.0f, 0.0f }, { 1.0f, -1.0f, 0.0f }, { 1.0f, 1.0f, 0.0f },
            { 1.0f, 1.0f, 0.0f }, { -1.0f, 1.0f, 0.0f } ,  { -1.0f, -1.0f, 0.0f} },
               new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, null);
            _buffer = new VAO(_attribs);
            _postConstructor = true;
            this.Exposure = Exposure;
            this.Decay = Decay;
            this.Density = Density;
            this.Weight = Weight;
            this.NumSamples = NumSamples;
            GodRaysEnabled = true;
        }

        #endregion 

        #region Cleaning

        public void cleanUp()
        {
            VAOManager.cleanUp(_buffer);
            if (_shader != null) _shader.cleanUp();
            if (_fbo != null) _fbo.cleanUp();
        }

        #endregion 
    }
}
