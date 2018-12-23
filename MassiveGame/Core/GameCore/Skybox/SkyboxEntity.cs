using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.SettingsCore;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using System;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore.Skybox
{
    [Serializable]
    public class SkyboxEntity : ISerializable
    {
        #region Definitions

        private const float SKYBOX_SIZE = 500.0f;
        public float FloatSpeed { set; get; }
        private float m_moveFactor;
        private bool m_bPostConstructor;
        private ITexture m_skyboxDayTexture;
        private ITexture m_skyboxNightTexture;
        private SkyboxShader m_shader;
        private VertexArrayObject m_buffer;

        [NonSerialized]
        private MistComponent m_mist;

        #endregion

        #region Constructor

        private void PostConstructor()
        {
            if (m_bPostConstructor)
            {
                m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<SkyboxShader>, string, SkyboxShader>(ProjectFolders.ShadersPath + "skyboxVS.glsl" + "," +
                    ProjectFolders.ShadersPath + "skyboxFS.glsl");

                float[,] vertices = new float[6 * 6, 3] { { -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE }, { -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE }, { SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },
                { SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
                { -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
                { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
                { SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
                { -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
                { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
                { SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },
                { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
                { SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE } };

                VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                m_buffer = new VertexArrayObject();
                m_buffer.AddVBO(verticesVBO);
                m_buffer.BindBuffersToVao();

                m_bPostConstructor = !m_bPostConstructor;
            }
        }

        public SkyboxEntity(string[] dayTextures, string[] nightTextures)
        {
            FloatSpeed = 0.3f;
            Init(dayTextures, nightTextures);
        }

        private void Init(string[] dayTextures, string[] nightTextures)
        {
            m_moveFactor = 0.0f;
            m_skyboxDayTexture = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(dayTextures[0] + "," + dayTextures[1] + "," + dayTextures[2] + "," +
               dayTextures[3] + "," + dayTextures[4] + "," + dayTextures[5]);
            m_skyboxNightTexture = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(nightTextures[0] + "," + nightTextures[1] + "," + nightTextures[2] + "," +
                nightTextures[3] + "," + nightTextures[4] + "," + nightTextures[5]);
            m_bPostConstructor = true;
        }

        #endregion

        #region Serialization

        public void PostDeserializePass(MistComponent mistComponent)
        {
            m_mist = mistComponent;
        }

        protected SkyboxEntity(SerializationInfo info, StreamingContext context)
        {
            FloatSpeed = info.GetSingle("FloatSpeed");
            string[] dayTexturesPath = info.GetString("m_skyboxDayTexture").Split(',');
            string[] nightTexturesPath = info.GetString("m_skyboxNightTexture").Split(',');
            Init(dayTexturesPath, nightTexturesPath);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string dayTexturesPath = PoolProxy.GetResourceKey<GetTexturePool, string, ITexture>(m_skyboxDayTexture);
            string nightTexturesPath = PoolProxy.GetResourceKey<GetTexturePool, string, ITexture>(m_skyboxNightTexture);

            info.AddValue("m_skyboxDayTexture", dayTexturesPath);
            info.AddValue("m_skyboxNightTexture", nightTexturesPath);
            info.AddValue("FloatSpeed", FloatSpeed);
        }

        #endregion

        #region Renderer

        private Matrix4 GetMirrorMatrix(WaterPlane water)
        {
            Matrix4 mirrorMatrix = Matrix4.Identity;
            mirrorMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(m_moveFactor));
            mirrorMatrix *= Matrix4.CreateScale(1, -1, 1);
            return mirrorMatrix;
        }

        public void Tick(float deltaTime)
        {
            m_moveFactor += deltaTime * FloatSpeed;
            m_moveFactor %= 360.0f;
        }

        public void RenderWaterReflection(WaterPlane water, BaseCamera camera, Vector3 sunDirection, Matrix4 projectionMatrix, Vector4 clipPlane)
        {
            if (m_bPostConstructor)
                return;

            GL.Enable(EnableCap.ClipDistance0);

            Matrix4 mirrorMatrix;
            mirrorMatrix = GetMirrorMatrix(water);

            m_shader.startProgram();
            m_skyboxDayTexture.BindTexture(TextureUnit.Texture0);
            m_skyboxNightTexture.BindTexture(TextureUnit.Texture1);
            m_shader.SetTransformationMatrices(ref projectionMatrix, camera.GetViewMatrix(), ref mirrorMatrix);
            m_shader.SetDayCubeTexture(0);
            m_shader.SetNightCubeTexture(1);
            m_shader.SetDayCycleValue(sunDirection.Normalized().Y);
            m_shader.SetMist(m_mist);
            m_shader.SetClipPlane(ref clipPlane);
            m_buffer.RenderVAO(PrimitiveType.Triangles);
            m_shader.stopProgram();

            GL.Disable(EnableCap.ClipDistance0);
        }

        public void renderSkybox(BaseCamera camera, Vector3 sunDirection, Matrix4 projectionMatrix)
        {
            PostConstructor();

            GL.Disable(EnableCap.ClipDistance0);

            Matrix4 modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(m_moveFactor));

            m_shader.startProgram();
            m_skyboxDayTexture.BindTexture(TextureUnit.Texture0);
            m_skyboxNightTexture.BindTexture(TextureUnit.Texture1);
            m_shader.SetTransformationMatrices(ref projectionMatrix, camera.GetViewMatrix(), ref modelMatrix);
            m_shader.SetDayCubeTexture(0);
            m_shader.SetNightCubeTexture(1);
            m_shader.SetDayCycleValue(sunDirection.Normalized().Y);
            m_shader.SetMist(m_mist);
            m_buffer.RenderVAO(PrimitiveType.Triangles);
            m_shader.stopProgram();
        }

        #endregion

        #region Seter

        public void setMistComponent(MistComponent mist)
        {
            this.m_mist = mist;
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<SkyboxShader>, string, SkyboxShader>(m_shader);
            m_buffer.CleanUp();
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_skyboxDayTexture);
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_skyboxNightTexture);
        }

        #endregion
    }
}
