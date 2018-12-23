using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TextureLoader;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.SettingsCore;
using MassiveGame.Core.GameCore.Water;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using MassiveGame.Core.MathCore;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore.Sun
{
    [Serializable]
    public class SunRenderer 
        : IVisible, IObservable, ISerializable
    {
        #region Definitions

        public readonly float LENS_FLARE_SUN_SIZE;
        public readonly float SUN_SIZE;
        public readonly float LENS_FLARE_SIZE_TO_SUN_SIZE;

        [NonSerialized]
        private DirectionalLight m_lightSource;

        private VertexArrayObject m_buffer;
        
        private SunShader m_shader;
        
        private Vector4 m_quadLBZ, m_quadRTZ;
        private ITexture m_texture1;
        private ITexture m_texture2;
        private bool m_postConstructor;

        public CollisionQuad CQuad { private set; get; }
        public bool IsInCameraView { private set; get; }

        #endregion

        #region Serialization

        public void PostDeserializePass(DirectionalLight globalLight)
        {
            m_lightSource = globalLight;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string pathToTexture1 = PoolProxy.GetResourceKey<GetTexturePool, string, ITexture>(m_texture1);
            string pathToTexture2 = PoolProxy.GetResourceKey<GetTexturePool, string, ITexture>(m_texture2);

            info.AddValue("pathToTexture1", pathToTexture1);
            info.AddValue("pathToTexture2", pathToTexture2);
            info.AddValue("lensFlareSunSize", LENS_FLARE_SUN_SIZE);
            info.AddValue("sunSize", SUN_SIZE);
            info.AddValue("lensFlareSizeToSunSize", LENS_FLARE_SIZE_TO_SUN_SIZE);
        }

        protected SunRenderer(SerializationInfo info, StreamingContext context)
        {
            string pathToTexture1 = info.GetString("pathToTexture1");
            string pathToTexture2 = info.GetString("pathToTexture2");


            LENS_FLARE_SUN_SIZE = info.GetSingle("lensFlareSunSize");
            SUN_SIZE = info.GetSingle("sunSize");
            LENS_FLARE_SIZE_TO_SUN_SIZE = info.GetSingle("lensFlareSizeToSunSize");
            InitResources(pathToTexture1, pathToTexture2);
            m_postConstructor = true;
        }

        #endregion

        #region Overrides

        public bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            // set quad default value at beginning
            if (CQuad == null)
            {
                IsInCameraView = true;
            }
            else
            {
                IsInCameraView = FrustumCulling.isQuadIntersection(CQuad, viewMatrix, projectionMatrix);
            }
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

        public void Tick(float deltaTime)
        {
            if (CQuad == null) CQuad = new CollisionQuad(0, 0, 0, 0, 0, 0);
            var lbn = Vector4.Transform(m_quadLBZ, Matrix4.CreateTranslation(m_lightSource.Position));
            var rtf = Vector4.Transform(m_quadRTZ, Matrix4.CreateTranslation(m_lightSource.Position));
            CQuad.synchronizeCoordinates(lbn.X, rtf.X, lbn.Y, rtf.Y, lbn.Z, lbn.Z);
        }

        public void RenderWaterReflection(WaterPlane water, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane, bool bApplyLensFlareScale = false)
        {
            if (m_postConstructor)
                return;

            Matrix4 modelMatrix = Matrix4.Identity;
            if (bApplyLensFlareScale)
            {
                modelMatrix *= Matrix4.CreateScale(LENS_FLARE_SIZE_TO_SUN_SIZE, -LENS_FLARE_SIZE_TO_SUN_SIZE, LENS_FLARE_SIZE_TO_SUN_SIZE);
            }
            float translationPositionY = (2 * water.GetTranslation().Y) - m_lightSource.Position.Y;
            
            modelMatrix *= Matrix4.CreateTranslation(m_lightSource.Position.X, translationPositionY, m_lightSource.Position.Z);

            GL.Enable(EnableCap.ClipDistance0);

            m_shader.startProgram();
            m_texture1.BindTexture(TextureUnit.Texture0);
            m_texture2.BindTexture(TextureUnit.Texture1);
            m_shader.setUniformValues(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix, m_lightSource, 0, 1);
            m_shader.SetClipPlane(ref clipPlane);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            m_buffer.RenderVAO(PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            m_shader.stopProgram();

            GL.Disable(EnableCap.ClipDistance0);
        }

        public void Render(BaseCamera camera, ref Matrix4 projectionMatrix, bool bApplyLensFlareScale = false)
        {
            postConstructor();
            Matrix4 modelMatrix = Matrix4.Identity;
            if (bApplyLensFlareScale)
            {
                modelMatrix *= Matrix4.CreateScale(LENS_FLARE_SIZE_TO_SUN_SIZE);
            }
            modelMatrix *= Matrix4.CreateTranslation(m_lightSource.Position);

            GL.Disable(EnableCap.ClipDistance0);
            m_shader.startProgram();
            m_texture1.BindTexture(TextureUnit.Texture0);
            m_texture2.BindTexture(TextureUnit.Texture1);
            m_shader.setUniformValues(ref modelMatrix, camera.GetViewMatrix(), ref projectionMatrix, m_lightSource, 0, 1);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            m_buffer.RenderVAO(PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            m_shader.stopProgram();
        }

        #endregion

        #region Constructor

        private void InitResources(string texture1, string texture2)
        {
            float[,] vertices = new float[6, 3] { { -(SUN_SIZE / 2), SUN_SIZE / 2, 0.0f }, { -(SUN_SIZE / 2), -(SUN_SIZE / 2), 0.0f }, { SUN_SIZE / 2, -(SUN_SIZE / 2), 0.0f }, { SUN_SIZE / 2, -(SUN_SIZE / 2), 0.0f }, { SUN_SIZE / 2, SUN_SIZE / 2, 0.0f }, { -(SUN_SIZE / 2), SUN_SIZE / 2, 0.0f } };
            float[,] normals = new float[6, 3] { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 } };
            float[,] texCoords = new float[6, 2] { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 1, 1 }, { 0, 1 }, { 0, 0 } };

            VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> normalsVBO = new VertexBufferObjectTwoDimension<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            m_buffer = new VertexArrayObject();
            m_buffer.AddVBO(verticesVBO, normalsVBO, texCoordsVBO);
            m_buffer.BindBuffersToVao();

            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<SunShader>, string, SunShader>(ProjectFolders.ShadersPath + "sunVS.glsl" + "," + ProjectFolders.ShadersPath + "sunFS.glsl");

            m_texture1 = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(texture1);
            m_texture2 = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(texture2);

            m_quadLBZ = new Vector4((-SUN_SIZE / 2), (-SUN_SIZE / 2), 0.0f, 1.0f);
            m_quadRTZ = new Vector4((SUN_SIZE / 2), (SUN_SIZE / 2), 0.0f, 1.0f);
        }

        private void postConstructor()
        {
            if (m_postConstructor)
            { 
                m_postConstructor = !m_postConstructor;
            }
        }

        public SunRenderer(DirectionalLight directionalLight, string pathToTexture1, string pathToTexture2, float SunSize, float LensFlareSunSize)
        {
            m_lightSource = directionalLight;
            m_postConstructor = true;
            SUN_SIZE = SunSize;
            LENS_FLARE_SUN_SIZE = LensFlareSunSize;
            LENS_FLARE_SIZE_TO_SUN_SIZE = LENS_FLARE_SUN_SIZE / SUN_SIZE;
            InitResources(pathToTexture1, pathToTexture2);
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_texture1);
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_texture2);
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<SunShader>, string, SunShader>(this.m_shader);
            m_buffer.CleanUp();
        }

        #endregion
    }
}
