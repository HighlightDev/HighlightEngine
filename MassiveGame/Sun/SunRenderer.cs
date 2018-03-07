using GpuGraphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;
using System;
using TextureLoader;
using PhysicsBox;
using MassiveGame.Optimization;
using System.Threading;
using MassiveGame.API.Collector;

namespace MassiveGame
{
    public class SunRenderer : IVisible
    {
        #region Definitions

        public readonly float LENS_FLARE_SUN_SIZE = 130f;
        public readonly float SUN_SIZE = 150f;

        private DirectionalLight _sun;
        private VBOArrayF _attribs;
        private VAO _buffer;
        private bool _postConstructor;
        private SunShader _shader;
        private ITexture _texture1;
        private ITexture _texture2;
        private CollisionQuad _cQuad;
        private bool _isInCameraView;
        private Vector4 _quadLBZ, _quadRTZ;

        public CollisionQuad CQuad { private set { _cQuad = value; } get { return _cQuad; } }
        public bool IsInCameraView { private set { _isInCameraView = value; } get { return _isInCameraView; } }
        
        #endregion

        #region Overrides

        public bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            // set box default values at beginning
            if (Object.Equals(CQuad, null))
            {
                IsInCameraView = true;
                return IsInCameraView;
            }
            // if sun lays in view frustum - then it intersects with it
            return IsInCameraView = FrustumCulling.isQuadIntersection(this.CQuad, viewMatrix, projectionMatrix); 
        }

        #endregion

        #region Renderer

        public void UpdateFrustumView()
        {
            if (Object.Equals(CQuad, null)) CQuad = new CollisionQuad(0, 0, 0, 0, 0, 0);
            var lbn = Vector4.Transform(_quadLBZ, Matrix4.CreateTranslation(_sun.Position));
            var rtf = Vector4.Transform(_quadRTZ, Matrix4.CreateTranslation(_sun.Position));
            _cQuad.synchronizeCoordinates(lbn.X, rtf.X, lbn.Y, rtf.Y, lbn.Z);
        }

        public void RenderWaterReflection(WaterEntity water, LiteCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane, Vector3 scale = new Vector3())
        {
            if (_postConstructor)
                return;

            if (scale.X == 0 || scale.Y == 0) scale = new Vector3(1);
            float translationPositionY = (2 * water.GetTranslation().Y) - _sun.Position.Y;

            Matrix4 modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateScale(scale.X, -scale.Y, scale.Z);
            modelMatrix *= Matrix4.CreateTranslation(_sun.Position.X, translationPositionY, _sun.Position.Z);

            GL.Enable(EnableCap.ClipDistance0);

            _shader.startProgram();
            _texture1.BindTexture(TextureUnit.Texture0);
            _texture2.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValues(ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix, _sun, 0, 1);
            _shader.SetClipPlane(ref clipPlane);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            _shader.stopProgram();

            GL.Disable(EnableCap.ClipDistance0);
        }

        public void renderSun(LiteCamera camera, ref Matrix4 projectionMatrix, Vector3 scale = new Vector3())
        {
            postConstructor();
            if (scale.X == 0 || scale.Y == 0) scale = new Vector3(1);
            Matrix4 modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateScale(scale);
            modelMatrix *= Matrix4.CreateTranslation(_sun.Position);

            GL.Disable(EnableCap.ClipDistance0);
            _shader.startProgram();
            _texture1.BindTexture(TextureUnit.Texture0);
            _texture2.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValues(ref modelMatrix, camera.getViewMatrix(), ref projectionMatrix, _sun, 0, 1);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            _shader.stopProgram();
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (_postConstructor)
            {
                VAOManager.genVAO(_buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);
                _shader = (SunShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "sunVS.glsl",
                    ProjectFolders.ShadersPath + "sunFS.glsl", "", typeof(SunShader));
                _postConstructor = !_postConstructor;
            }
        }

        public SunRenderer(DirectionalLight sun, string sunTexture1, string sunTexture2)
        {
            this._sun = sun;
            this._postConstructor = true;
            this._attribs = new VBOArrayF(new float[6, 3] { { -(SUN_SIZE / 2), SUN_SIZE / 2, 0.0f}, { -(SUN_SIZE / 2), -(SUN_SIZE / 2), 0.0f},
            { SUN_SIZE / 2, -(SUN_SIZE / 2), 0.0f}, { SUN_SIZE / 2, -(SUN_SIZE / 2), 0.0f}, { SUN_SIZE / 2, SUN_SIZE / 2, 0.0f}, { -(SUN_SIZE / 2), SUN_SIZE / 2, 0.0f} },
            new float[6, 3] { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 } },
            new float[6, 2] { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 1, 1 }, { 0, 1 }, { 0, 0 } }, false);
            this._buffer = new VAO(_attribs);
            this._texture1 = ResourcePool.GetTexture(sunTexture1);
            this._texture2 = ResourcePool.GetTexture(sunTexture2);
            _quadLBZ = new Vector4((-SUN_SIZE / 2), (-SUN_SIZE / 2), 0.0f, 1.0f);
            _quadRTZ = new Vector4((SUN_SIZE / 2), (SUN_SIZE / 2), 0.0f, 1.0f);
        }

        public SunRenderer(DirectionalLight sun, string sunTexture1, string sunTexture2, float SunSize, float LensFlareSunSize)
        {
            this._sun = sun;
            this._postConstructor = true;
            this._buffer = new VAO(_attribs);
            this._texture1 = ResourcePool.GetTexture(sunTexture1);
            this._texture2 = ResourcePool.GetTexture(sunTexture2);
            this.SUN_SIZE = SunSize;
            this.LENS_FLARE_SUN_SIZE = LensFlareSunSize;
            this._attribs = new VBOArrayF(new float[6, 3] { { -(SUN_SIZE / 2), SUN_SIZE / 2, 0.0f}, { -(SUN_SIZE / 2), -(SUN_SIZE / 2), 0.0f},
            { SUN_SIZE / 2, -(SUN_SIZE / 2), 0.0f}, { SUN_SIZE / 2, -(SUN_SIZE / 2), 0.0f}, { SUN_SIZE / 2, SUN_SIZE / 2, 0.0f}, { -(SUN_SIZE / 2), SUN_SIZE / 2, 0.0f} },
            new float[6, 3] { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 } },
            new float[6, 2] { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 1, 1 }, { 0, 1 }, { 0, 0 } }, false);
            _quadLBZ = new Vector4((-SUN_SIZE / 2), (-SUN_SIZE / 2), 0.0f, 1.0f);
            _quadRTZ = new Vector4((SUN_SIZE / 2), (SUN_SIZE / 2), 0.0f, 1.0f);
           
        }


        #endregion

        #region Cleaning

        public void cleanUp()
        {
            ResourcePool.ReleaseTexture(_texture1);
            ResourcePool.ReleaseTexture(_texture2);
            ResourcePool.ReleaseShaderProgram(this._shader);
            VAOManager.cleanUp(_buffer);
        }

        #endregion
    }
}
