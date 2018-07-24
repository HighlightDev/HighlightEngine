using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Settings;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.GameCore.Skybox
{
    public class Skybox
    {
        #region Definitions

        private const float SKYBOX_SIZE = 500.0f;
        public float FloatSpeed { set; get; }
        private float _moveFactor;
        private bool _postConstructor;
        private ITexture skyboxDayTexture;
        private ITexture skyboxNightTexture;
        private SkyboxShader shader;
        private VertexArrayObject buffer;
        private MistComponent _mist;

        #endregion

        #region Renderer

        private Matrix4 GetMirrorMatrix(WaterPlane water)
        {
            Matrix4 mirrorMatrix = Matrix4.Identity;
            mirrorMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_moveFactor));
            mirrorMatrix *= Matrix4.CreateScale(1, -1, 1);
            return mirrorMatrix;
        }

        public void UpdateAnimation(float frameElapseTime)
        {
            _moveFactor += frameElapseTime * FloatSpeed;
            _moveFactor %= 360.0f;
        }

        public void RenderWaterReflection(WaterPlane water, BaseCamera camera, DirectionalLight sun, Matrix4 projectionMatrix, Vector4 clipPlane)
        {
            if (_postConstructor)
                return;

            GL.Enable(EnableCap.ClipDistance0);

            Matrix4 mirrorMatrix;
            mirrorMatrix = GetMirrorMatrix(water);

            shader.startProgram();
            skyboxDayTexture.BindTexture(TextureUnit.Texture0);
            skyboxNightTexture.BindTexture(TextureUnit.Texture1);
            shader.SetTransformationMatrices(ref projectionMatrix, camera.GetViewMatrix(), ref mirrorMatrix);
            shader.SetDayCubeTexture(0);
            shader.SetNightCubeTexture(1);
            shader.SetDirectionalLight(sun);
            shader.SetMist(_mist);
            shader.SetClipPlane(ref clipPlane);
            buffer.RenderVAO(PrimitiveType.Triangles);
            shader.stopProgram();

            GL.Disable(EnableCap.ClipDistance0);
        }

        public void renderSkybox(BaseCamera camera, DirectionalLight sun, Matrix4 projectionMatrix)
        {
            postConstructor();

            GL.Disable(EnableCap.ClipDistance0);

            Matrix4 modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_moveFactor));

            shader.startProgram();
            skyboxDayTexture.BindTexture(TextureUnit.Texture0);
            skyboxNightTexture.BindTexture(TextureUnit.Texture1);
            shader.SetTransformationMatrices(ref projectionMatrix, camera.GetViewMatrix(), ref modelMatrix);
            shader.SetDayCubeTexture(0);
            shader.SetNightCubeTexture(1);
            shader.SetDirectionalLight(sun);
            shader.SetMist(_mist);
            buffer.RenderVAO(PrimitiveType.Triangles);
            shader.stopProgram();
        }

        #endregion

        #region Seter

        public void setMistComponent(MistComponent mist)
        {
            this._mist = mist;
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (this._postConstructor)
            {
                shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<SkyboxShader>, string, SkyboxShader>(ProjectFolders.ShadersPath + "skyboxVertexShader.glsl" + "," +
                    ProjectFolders.ShadersPath + "skyboxFragmentShader.glsl");

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
                buffer = new VertexArrayObject();
                buffer.AddVBO(verticesVBO);
                buffer.BindVbosToVao();

                this._postConstructor = !this._postConstructor;
            }
        }

        public Skybox(string[] dayTextures, string[] nightTextures)
        {
            _moveFactor = 0.0f;
            FloatSpeed = 0.3f;
            skyboxDayTexture = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(dayTextures[0] + "," + dayTextures[1] + "," + dayTextures[2] + "," +
                dayTextures[3] + "," + dayTextures[4] + "," + dayTextures[5]);
            skyboxNightTexture = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(nightTextures[0] + "," + nightTextures[1] + "," + nightTextures[2] + "," +
                nightTextures[3] + "," + nightTextures[4] + "," + nightTextures[5]);
            _postConstructor = true;
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<SkyboxShader>, string, SkyboxShader>(shader);
            buffer.CleanUp();
            PoolProxy.FreeResourceMemoryByValue<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(skyboxDayTexture);
            PoolProxy.FreeResourceMemoryByValue<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(skyboxNightTexture);
        }

        #endregion
    }
}
