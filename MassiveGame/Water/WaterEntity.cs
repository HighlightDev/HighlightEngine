using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GpuGraphics;
using MassiveGame.RenderCore.Lights;
using TextureLoader;
using PhysicsBox;

using MassiveGame.Optimization;
using MassiveGame.API.Collector;
using PhysicsBox.MathTypes;
using MassiveGame.RenderCore;
using System.Drawing;

namespace MassiveGame
{
    #region WaterQuality

    public struct WaterQuality
    {
        public readonly bool EnableBuilding;
        public readonly bool EnablePlayer;
        public readonly bool EnableGrassRefraction;

        public WaterQuality(bool enableBuilding, bool enablePlayer, bool enableGrassRefraction)
        {
            EnableBuilding = enableBuilding;
            EnablePlayer = enablePlayer;
            EnableGrassRefraction = enableGrassRefraction;
        }
    }

    #endregion

    public class WaterEntity : IVisible
    {
        private StencilPassShader stencilPassShader;


        #region Definitions

        private float _transparencyDepth;
        private float _waveSpeed;
        private float _waveStrength;
        private float _moveFactor;
        private ITexture _waterDistortionMap;
        private ITexture _waterNormalMap;
        private VBOArrayF _attribs;
        private VAO _buffer;
        private WaterShader _shader;
        public WaterFBO _fbo;
        private bool _postConstructor;
        private MistComponent _mist;
        private Vector3[] _collisionCheckPoints;
        public CollisionSphereBox Box { set; get; }

        private Matrix4 modelMatrix;

        public WaterQuality Quality { private set; get; }

        private bool _isInCameraView;
        public bool IsInCameraView { private set { _isInCameraView = value; } get { return _isInCameraView; } }

        public float TransparencyDepth
        {
            set { this._transparencyDepth = value < 0f ? 0f : value; }
            get { return this._transparencyDepth; }
        }

        public float WaterHeight
        {
            get
            {
                /*TODO - calculate average value of water height*/
                var retValue = 0.0f;
                for (int i = 0; i < _attribs.Vertices.Length / 3; i++)
                {
                    retValue += _attribs.Vertices[i, 1];
                }
                return ((retValue / (_attribs.Vertices.Length / 3)) + _translation.Y);
            }
        }

        public float WaveSpeed
        {
            set { this._waveSpeed = value > 1.0f ? 1.0f : value < 0.0f ? 0.0f : value; }
            get { return this._waveSpeed; }
        }

        public float WaveStrength
        {
            set { this._waveStrength = value > 1.0f ? 1.0f : value < 0.0f ? 0.0f : value; }
            get { return this._waveStrength; }
        }

        #region Transformation

        private Vector3 _translation;
        private Vector3 _rotation;
        private Vector3 _scaling;

        public Vector3 GetTranslation()
        {
            return _translation;
        }

        public Vector3 GetRotation()
        {
            return _rotation;
        }

        public Vector3 GetScaling()
        {
            return _scaling;
        }

        #endregion

        #endregion

        #region Overrides

        // realize method of iOptimizable interface
        public bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (Object.Equals(this._collisionCheckPoints, null))
            {
                // disable optimization, keep water always in view frustum
                return this.IsInCameraView = true; 
            }
            return this.IsInCameraView = FrustumCulling.isWaterIntersection(this._collisionCheckPoints, viewMatrix, ref projectionMatrix);
        }

        #endregion

        #region Renderer

        public void SetReflectionRendertarget()
        {
            postConstructor();
            /*Select water framebuffer object render to*/
            _fbo.renderToFBO(1, _fbo.Texture.Rezolution[0].widthRezolution, _fbo.Texture.Rezolution[0].heightRezolution);
        }

        public void SetRefractionRendertarget()
        {
            postConstructor();
            /*Select water framebuffer object render to*/
            _fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
        }

        public void StencilPass(LiteCamera camera, ref Matrix4 projectionMatrix)
        {
            postConstructor();
            stencilPassShader.startProgram();
            stencilPassShader.SetUniformVariables(ref projectionMatrix, camera.getViewMatrix(), ref modelMatrix);
            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
            stencilPassShader.stopProgram();
        }

        public void renderWater(Camera camera, ref Matrix4 projectionMatrix, float frameTimeSec, float nearClipPlane, float farClipPlane
            , DirectionalLight sun = null, List<PointLight> lights = null)
        {
            postConstructor();
            /*Water distortion cycle*/

            Console.WriteLine(frameTimeSec);

            _moveFactor += _waveSpeed * frameTimeSec;
            _moveFactor %= 1;

            this._shader.startProgram();
            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
            this._fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[1]);
            this._waterDistortionMap.BindTexture(TextureUnit.Texture2);
            this._waterNormalMap.BindTexture(TextureUnit.Texture3);
            this._fbo.Texture.bindTexture2D(TextureUnit.Texture4, _fbo.Texture.TextureID[2]);
            this._shader.setUniformValues(ref modelMatrix, camera.getViewMatrix(), ref projectionMatrix, 0, 1, 2, 3, 4,
                camera.getPositionVector(), _moveFactor, _waveStrength, sun, lights, ref nearClipPlane, ref farClipPlane, this.TransparencyDepth,
                _mist == null ? false : _mist.EnableMist, _mist == null ? 0 : _mist.MistDensity, _mist == null ? 0 : _mist.MistGradient,
                _mist == null ? new Vector3() : _mist.MistColour);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            this._shader.stopProgram();
        }

        #endregion

        #region Seter

        public void setMist(MistComponent mist)
        {
            this._mist = mist;
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (this._postConstructor)
            {
                VAOManager.genVAO(this._buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, this._buffer);

                this._shader = (WaterShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterVS.glsl",
                    ProjectFolders.ShadersPath + "waterFS.glsl", "", typeof(WaterShader));

                stencilPassShader = (StencilPassShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "stencilPassVS.glsl",
                    ProjectFolders.ShadersPath + "stencilPassFS.glsl", "", typeof(StencilPassShader));

                this._fbo = new WaterFBO();
                this._postConstructor = !this._postConstructor;

                DOUEngine.uiFrameCreator.PushFrame(new Texture2Dlite((Int32)DOUEngine.Water._fbo.Texture.TextureID[0], new Point(DOUEngine.Water._fbo.Texture.Rezolution[0].widthRezolution, DOUEngine.Water._fbo.Texture.Rezolution[0].heightRezolution)));
                DOUEngine.uiFrameCreator.PushFrame(new Texture2Dlite((Int32)DOUEngine.Water._fbo.Texture.TextureID[1], new Point(DOUEngine.Water._fbo.Texture.Rezolution[1].widthRezolution, DOUEngine.Water._fbo.Texture.Rezolution[1].heightRezolution)));
            }
        }

        public WaterEntity(string distortionMap, string normalMap, Vector3 translation, Vector3 rotation, Vector3 scaling,
            WaterQuality quality, int frustumSquares = 0)
        {
            this._waterDistortionMap = ResourcePool.GetTexture(distortionMap);
            this._waterNormalMap = ResourcePool.GetTexture(normalMap);

            this._translation = translation;
            this._rotation = rotation;
            this._scaling = scaling;
            /*Nominal water coords*/
            this._attribs = new VBOArrayF(new float[6, 3] { { -1.0f, 0.0f, 1.0f }, { 1.0f, 0.0f, 1.0f }, { 1.0f, 0.0f, -1.0f }, { 1.0f, 0.0f, -1.0f }, { -1.0f, 0.0f, -1.0f }, { -1.0f, 0.0f, 1.0f } },
                new float[6, 3] { { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f } },
                new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, true);
            this.Box = new CollisionSphereBox(-1.0f, 1.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0);
            _moveFactor = 0.0f;
            _waveSpeed = 0.3f;
            _waveStrength = 0.02f;
            _transparencyDepth = 10000f;
            this._buffer = new VAO(this._attribs);
            /*First pass postconstructor*/
            this._postConstructor = true;
            this.Quality = quality;
            _collisionCheckPoints = null;

            modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this._rotation.X));
            modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this._rotation.Y));
            modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this._rotation.Z));
            modelMatrix *= Matrix4.CreateScale(this._scaling);
            modelMatrix *= Matrix4.CreateTranslation(this._translation);

            Box.LBNCoordinates = new Vector3(Vector4.Transform(new Vector4(Box.LBNCoordinates, 1.0f), modelMatrix));
            Box.RTFCoordinates = new Vector3(Vector4.Transform(new Vector4(Box.RTFCoordinates, 1.0f), modelMatrix));

            // divide water collision box to avoid "bugs" with frustum culling
            this._collisionCheckPoints = FrustumCulling.divideWaterCollisionBox(Box, frustumSquares);      
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            VAOManager.cleanUp(this._buffer);
            ResourcePool.ReleaseTexture(this._waterDistortionMap);
            ResourcePool.ReleaseTexture(this._waterNormalMap);
            ResourcePool.ReleaseShaderProgram(this._shader);
        }

        #endregion 
    }
}
