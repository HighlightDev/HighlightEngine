﻿using System;
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

    public class WaterPlane : IVisible
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
        public CollisionQuad quad { set; get; }

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
                for (Int32 i = 0; i < _attribs.Vertices.Length / 3; i++)
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

        // Implement method of iOptimizable interface
        public bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (Object.Equals(this._collisionCheckPoints, null))
                // disable optimization, keep water always in view frustum
                IsInCameraView = true;
            else
                IsInCameraView = this.IsInCameraView = FrustumCulling.isWaterIntersection(this._collisionCheckPoints, viewMatrix, ref projectionMatrix);

            return IsInCameraView;
        }

        #endregion

        #region Renderer

        public void SetReflectionRendertarget()
        {
            postConstructor();
            /*Select water framebuffer object render to*/
            _fbo.renderToFBO(1, _fbo.ReflectionTexture.GetTextureRezolution());
        }

        public void SetRefractionRendertarget()
        {
            postConstructor();
            /*Select water framebuffer object render to*/
            _fbo.renderToFBO(2, _fbo.RefractionTexture.GetTextureRezolution());
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

            _moveFactor += _waveSpeed * frameTimeSec;
            _moveFactor %= 1;

            this._shader.startProgram();
            this._fbo.ReflectionTexture.BindTexture(TextureUnit.Texture0);
            this._fbo.RefractionTexture.BindTexture(TextureUnit.Texture1);
            this._waterDistortionMap.BindTexture(TextureUnit.Texture2);
            this._waterNormalMap.BindTexture(TextureUnit.Texture3);
            this._fbo.DepthTexture.BindTexture(TextureUnit.Texture4);
            _shader.setTransformationMatrices(ref modelMatrix, camera.getViewMatrix(), ref projectionMatrix);
            _shader.setReflectionSampler(0);
            _shader.setRefractionSampler(1);
            _shader.setDuDvSampler(2);
            _shader.setNormalMapSampler(3);
            _shader.setDepthSampler(4);
            _shader.setCameraPosition(camera.getPositionVector());
            _shader.setDistortionProperties(_moveFactor, _waveStrength);
            _shader.setDirectionalLight(sun);
            _shader.setPointLight(lights);
            _shader.setClippingPlanes(ref nearClipPlane, ref farClipPlane);
            _shader.setMist(_mist);
            _shader.setTransparancyDepth(TransparencyDepth);

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

                DOUEngine.uiFrameCreator.PushFrame(new Texture2Dlite((Int32)DOUEngine.Water._fbo.ReflectionTexture.GetTextureDescriptor(), _fbo.ReflectionTexture.GetTextureRezolution()));
                DOUEngine.uiFrameCreator.PushFrame(new Texture2Dlite((Int32)DOUEngine.Water._fbo.RefractionTexture.GetTextureDescriptor(), _fbo.RefractionTexture.GetTextureRezolution()));
            }
        }

        public WaterPlane(string distortionMap, string normalMap, Vector3 translation, Vector3 rotation, Vector3 scaling,
            WaterQuality quality, Int32 frustumSquares = 0)
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
            this.quad = new CollisionQuad(-1.0f, 1.0f, 0.0f, 0.0f, -1.0f, 1.0f);
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

            quad.LBNCoordinates = Vector3.TransformPosition(quad.LBNCoordinates, modelMatrix);
            quad.RTFCoordinates = Vector3.TransformPosition(quad.RTFCoordinates, modelMatrix);

            // divide water collision box to avoid "bugs" with frustum culling
            this._collisionCheckPoints = FrustumCulling.divideWaterCollisionBox(quad, frustumSquares);      
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
