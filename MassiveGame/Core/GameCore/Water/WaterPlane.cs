using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.SettingsCore;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using MassiveGame.Core.MathCore;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore.Water
{
    #region WaterQuality

    [Serializable]
    public class WaterQuality : ISerializable
    {
        public readonly bool EnableBuilding;
        public readonly bool EnableMovableEntities;
        public readonly bool EnableGrassRefraction;

        public WaterQuality(bool enableBuilding, bool enableMovableEntities, bool enableGrassRefraction)
        {
            EnableBuilding = enableBuilding;
            EnableMovableEntities = enableMovableEntities;
            EnableGrassRefraction = enableGrassRefraction;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("EnableBuilding", EnableBuilding);
            info.AddValue("EnableMovableEntities", EnableMovableEntities);
            info.AddValue("EnableGrassRefraction", EnableGrassRefraction);
        }

        protected WaterQuality(SerializationInfo info, StreamingContext context)
        {
            EnableBuilding = info.GetBoolean("EnableBuilding");
            EnableMovableEntities = info.GetBoolean("EnableMovableEntities");
            EnableGrassRefraction = info.GetBoolean("EnableGrassRefraction");
        }
    }

    #endregion

    [Serializable]
    public class WaterPlane : IVisible, IObservable, ISerializable
    {
        #region Definitions

        private float m_transparencyDepth;
        private float m_waveSpeed;
        private float m_waveStrength;
        private float m_moveFactor;
        private ITexture m_waterDistortionMap;
        private ITexture m_waterNormalMap;
        private VertexArrayObject m_buffer;
        private WaterShader m_shader;
        private StencilPassShader m_stencilPassShader;
        public WaterFBO m_fbo;
        private bool m_postConstructor;

        [NonSerialized]
        private MistComponent m_mist;

        private Vector3[] m_collisionCheckPoints;
        private Int32 m_frustumSquares;
        public CollisionQuad Quad { set; get; }

        private Matrix4 m_modelMatrix;

        public WaterQuality Quality { private set; get; }
        public bool IsInCameraView { private set; get; }

        public float TransparencyDepth
        {
            set { m_transparencyDepth = value < 0f ? 0f : value; }
            get { return m_transparencyDepth; }
        }

        public float WaterHeight
        {
            get
            {
                /*TODO - calculate average value of water height*/
                var retValue = 0.0f;
                for (Int32 i = 0; i < m_buffer.GetVertexBufferArray().First().GetBufferVerticesCount(); i++)
                {
                    retValue += ((float[,])(m_buffer.GetVertexBufferArray().First().GetBufferData()))[i, 1];
                }
                return ((retValue / m_buffer.GetVertexBufferArray().First().GetBufferVerticesCount()) + m_translation.Y);
            }
        }

        public float WaveSpeed
        {
            set { m_waveSpeed = value > 1.0f ? 1.0f : value < 0.0f ? 0.0f : value; }
            get { return m_waveSpeed; }
        }

        public float WaveStrength
        {
            set { m_waveStrength = value > 1.0f ? 1.0f : value < 0.0f ? 0.0f : value; }
            get { return m_waveStrength; }
        }

        #region Transformation

        private Vector3 m_translation;
        private Vector3 m_rotation;
        private Vector3 m_scaling;

        public Vector3 GetTranslation()
        {
            return m_translation;
        }

        public Vector3 GetRotation()
        {
            return m_rotation;
        }

        public Vector3 GetScaling()
        {
            return m_scaling;
        }

        #endregion

        #endregion

        #region Constructor

        private void InitPhysics()
        {
            Quad = new CollisionQuad(-1.0f, 1.0f, 0.0f, 0.0f, -1.0f, 1.0f);
            Quad.LBNCoordinates = Vector3.TransformPosition(Quad.LBNCoordinates, m_modelMatrix);
            Quad.RTFCoordinates = Vector3.TransformPosition(Quad.RTFCoordinates, m_modelMatrix);

            // divide water collision box to avoid "bugs" with frustum culling
            m_collisionCheckPoints = FrustumCulling.divideWaterCollisionBox(Quad, m_frustumSquares);
        }

        private void PostConstructor()
        {
            if (this.m_postConstructor)
            {
                float[,] vertices = new float[6, 3] { { -1.0f, 0.0f, 1.0f }, { 1.0f, 0.0f, 1.0f }, { 1.0f, 0.0f, -1.0f }, { 1.0f, 0.0f, -1.0f }, { -1.0f, 0.0f, -1.0f }, { -1.0f, 0.0f, 1.0f } };
                float[,] normals = new float[6, 3] { { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 1.0f, 0.0f } };
                float[,] texCoords = new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } };

                VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Store);
                VertexBufferObjectTwoDimension<float> normalsVBO = new VertexBufferObjectTwoDimension<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                VertexBufferObjectTwoDimension<float> texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                VertexBufferObjectTwoDimension<float> tangentsVBO = new VertexBufferObjectTwoDimension<float>(VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords), BufferTarget.ArrayBuffer, 3, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                VertexBufferObjectTwoDimension<float> bitangentsVBO = new VertexBufferObjectTwoDimension<float>(VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords), BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

                m_buffer = new VertexArrayObject();
                m_buffer.AddVBO(verticesVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO);
                m_buffer.BindBuffersToVao();

                m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<WaterShader>, string, WaterShader>(ProjectFolders.ShadersPath + "waterVS.glsl" + "," + ProjectFolders.ShadersPath + "waterFS.glsl");
                m_stencilPassShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<StencilPassShader>, string, StencilPassShader>(ProjectFolders.ShadersPath + "stencilPassVS.glsl" + "," + ProjectFolders.ShadersPath + "stencilPassFS.glsl");

                this.m_fbo = new WaterFBO();
                this.m_postConstructor = !this.m_postConstructor;
            }
        }

        public WaterPlane(string distortionMap, string normalMap, Vector3 translation, Vector3 rotation, Vector3 scaling,
            WaterQuality quality, Int32 frustumSquares = 0)
        {
            m_waterDistortionMap = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(distortionMap);
            m_waterNormalMap = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(normalMap);
            m_frustumSquares = frustumSquares;

            m_translation = translation;
            m_rotation = rotation;
            m_scaling = scaling;
            /*Nominal water coords*/
           
            m_moveFactor = 0.0f;
            m_waveSpeed = 0.1f;
            m_waveStrength = 0.04f;
            m_transparencyDepth = 10000f;
            /*First pass post constructor*/
            this.m_postConstructor = true;
            this.Quality = quality;
            m_collisionCheckPoints = null;
            m_modelMatrix = Matrix4.Identity;
            m_modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(m_rotation.X));
            m_modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(m_rotation.Y));
            m_modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(m_rotation.Z));
            m_modelMatrix *= Matrix4.CreateScale(m_scaling);
            m_modelMatrix *= Matrix4.CreateTranslation(m_translation);
            InitPhysics();
        }

        #endregion

        #region Serialization

        public void PostDeserializePass(MistComponent mistComponent)
        {
            //m_mist = mistComponent;
        }

        protected WaterPlane(SerializationInfo info, StreamingContext context)
        {
            m_transparencyDepth = info.GetSingle("m_transparencyDepth");
            m_waveSpeed = info.GetSingle("m_waveSpeed");
            m_waveStrength = info.GetSingle("m_waveStrength");
            m_moveFactor = info.GetSingle("m_moveFactor");
            m_modelMatrix = (Matrix4)info.GetValue("m_modelMatrix", typeof(Matrix4));
            m_translation = (Vector3)info.GetValue("m_translation", typeof(Vector3));
            m_rotation = (Vector3)info.GetValue("m_rotation", typeof(Vector3));
            m_scaling = (Vector3)info.GetValue("m_scaling", typeof(Vector3));
            Quality = (WaterQuality)info.GetValue("Quality", typeof(WaterQuality));
            m_frustumSquares = info.GetInt32("m_frustumSquares");

            string distortionMapTexPath = info.GetString("m_waterDistortionMap");
            string normalMapTexPath = info.GetString("m_waterNormalMap");

            m_waterDistortionMap = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(distortionMapTexPath);
            m_waterNormalMap = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(normalMapTexPath);

            m_postConstructor = true;
            InitPhysics();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_transparencyDepth", m_transparencyDepth);
            info.AddValue("m_waveSpeed", m_waveSpeed);
            info.AddValue("m_waveStrength", m_waveStrength);
            info.AddValue("m_moveFactor", m_moveFactor);
            info.AddValue("m_modelMatrix", m_modelMatrix, typeof(Matrix4));
            info.AddValue("m_translation", m_translation, typeof(Vector3));
            info.AddValue("m_rotation", m_rotation, typeof(Vector3));
            info.AddValue("m_scaling", m_scaling, typeof(Vector3));
            info.AddValue("Quality", Quality, typeof(WaterQuality));
            info.AddValue("m_frustumSquares", m_frustumSquares);

            string distortionMapTexPath = PoolProxy.GetResourceKey<GetTexturePool, string, ITexture>(m_waterDistortionMap);
            string normalMapTexPath = PoolProxy.GetResourceKey<GetTexturePool, string, ITexture>(m_waterNormalMap);
            info.AddValue("m_waterDistortionMap", distortionMapTexPath);
            info.AddValue("m_waterNormalMap", normalMapTexPath);
        }

        #endregion

        #region Overrides

        // Implement method of iOptimizable interface
        public bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (Object.Equals(this.m_collisionCheckPoints, null))
                // disable optimization, keep water always in view frustum
                IsInCameraView = true;
            else
                IsInCameraView = this.IsInCameraView = FrustumCulling.isWaterIntersection(this.m_collisionCheckPoints, viewMatrix, ref projectionMatrix);

            return IsInCameraView;
        }

        public void NotifyAdded()
        {
            GameWorld.GetWorldInstance().VisibilityCheckCollection.Add(this);
        }

        public void NotifyRemoved()
        {
            GameWorld.GetWorldInstance().VisibilityCheckCollection.Remove(this);
        }

        #endregion

        #region Renderer

        public void SetReflectionRendertarget()
        {
            PostConstructor();
            /*Select water framebuffer object render to*/
            m_fbo.renderToFBO(1, m_fbo.ReflectionTexture.GetTextureRezolution());
        }

        public void SetRefractionRendertarget()
        {
            PostConstructor();
            /*Select water framebuffer object render to*/
            m_fbo.renderToFBO(2, m_fbo.RefractionTexture.GetTextureRezolution());
        }

        public void StencilPass(BaseCamera camera, ref Matrix4 projectionMatrix)
        {
            PostConstructor();
            m_stencilPassShader.startProgram();
            m_stencilPassShader.SetUniformVariables(ref projectionMatrix, camera.GetViewMatrix(), ref m_modelMatrix);
            m_buffer.RenderVAO(PrimitiveType.Triangles);
            m_stencilPassShader.stopProgram();
        }

        public void Tick(float deltaTime)
        {
            m_moveFactor += m_waveSpeed * deltaTime;
            m_moveFactor %= 1;
        }

        public void RenderWater(BaseCamera camera, ref Matrix4 projectionMatrix, float nearClipPlane, float farClipPlane
            , DirectionalLight sun = null, List<PointLight> lights = null)
        {
            PostConstructor();
            /*Water distortion cycle*/

            m_shader.startProgram();
            m_fbo.ReflectionTexture.BindTexture(TextureUnit.Texture0);
            m_fbo.RefractionTexture.BindTexture(TextureUnit.Texture1);
            m_waterDistortionMap.BindTexture(TextureUnit.Texture2);
            m_waterNormalMap.BindTexture(TextureUnit.Texture3);
            m_fbo.DepthTexture.BindTexture(TextureUnit.Texture4);
            m_shader.setTransformationMatrices(ref m_modelMatrix, camera.GetViewMatrix(), ref projectionMatrix);
            m_shader.setReflectionSampler(0);
            m_shader.setRefractionSampler(1);
            m_shader.setDuDvSampler(2);
            m_shader.setNormalMapSampler(3);
            m_shader.setDepthSampler(4);
            m_shader.setCameraPosition(camera.GetEyeVector());
            m_shader.setDistortionProperties(m_moveFactor, m_waveStrength);
            m_shader.setDirectionalLight(sun);
            m_shader.setPointLight(lights);
            m_shader.setClippingPlanes(ref nearClipPlane, ref farClipPlane);
            m_shader.setMist(m_mist);
            m_shader.setTransparancyDepth(TransparencyDepth);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            m_buffer.RenderVAO(PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            m_shader.stopProgram();
        }

        #endregion

        #region Seter

        public void SetMist(MistComponent mist)
        {
            m_mist = mist;
        }

        #endregion

        #region Cleaning

        public void CleanUp()
        {
            m_buffer?.CleanUp();
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_waterDistortionMap);
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_waterNormalMap);
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<WaterShader>, string, WaterShader>(m_shader);
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<StencilPassShader>, string, StencilPassShader>(m_stencilPassShader);
        }

        #endregion
    }
}
