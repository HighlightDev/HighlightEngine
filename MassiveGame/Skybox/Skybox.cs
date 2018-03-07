using GpuGraphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.API.Collector;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
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
        private VBOArrayF attribs;
        private VAO buffer;
        private MistComponent _mist;

        #endregion

        #region Renderer

        private Matrix4 GetMirrorMatrix(WaterEntity water)
        {
            Matrix4 mirrorMatrix = Matrix4.Identity;
            mirrorMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_moveFactor));
            mirrorMatrix *= Matrix4.CreateScale(1, -1, 1);
            return mirrorMatrix;
        }

        public void AnimationTick(float frameElapseTime)
        {
            _moveFactor += frameElapseTime * FloatSpeed;
            _moveFactor %= 360.0f;
        }

        public void RenderWaterReflection(WaterEntity water, LiteCamera camera, DirectionalLight sun, Matrix4 projectionMatrix, Vector4 clipPlane)
        {
            if (_postConstructor)
                return;

            GL.Enable(EnableCap.ClipDistance0);

            Matrix4 mirrorMatrix;
            mirrorMatrix = GetMirrorMatrix(water);

            shader.startProgram();
            skyboxDayTexture.BindTexture(TextureUnit.Texture0);
            skyboxNightTexture.BindTexture(TextureUnit.Texture1);
            shader.setAllUniforms(mirrorMatrix, camera.getViewMatrix(), projectionMatrix, 0, 1, sun, _mist == null ? false : _mist.EnableMist,
                _mist == null ? new Vector3() : _mist.MistColour);
            shader.SetClipPlane(ref clipPlane);
            VAOManager.renderBuffers(buffer, PrimitiveType.Triangles);
            shader.stopProgram();

            GL.Disable(EnableCap.ClipDistance0);
        }

        public void renderSkybox(LiteCamera camera, DirectionalLight sun, Matrix4 projectionMatrix)
        {
            postConstructor();

            GL.Disable(EnableCap.ClipDistance0);

            Matrix4 modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_moveFactor));

            shader.startProgram();
            skyboxDayTexture.BindTexture(TextureUnit.Texture0);
            skyboxNightTexture.BindTexture(TextureUnit.Texture1);
            shader.setAllUniforms(modelMatrix, camera.getViewMatrix(), projectionMatrix, 0, 1, sun, _mist == null ? false : _mist.EnableMist,
                _mist == null ? new Vector3() : _mist.MistColour);
            VAOManager.renderBuffers(buffer, PrimitiveType.Triangles);
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
                shader = (SkyboxShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "skyboxVertexShader.glsl",
                    ProjectFolders.ShadersPath + "skyboxFragmentShader.glsl", "", typeof(SkyboxShader));
                buffer = new VAO(attribs);
                VAOManager.genVAO(buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, buffer);
                this._postConstructor = !this._postConstructor;
            }
        }

        public Skybox(string[] dayTextures, string[] nightTextures)
        {
            _moveFactor = 0.0f;
            FloatSpeed = 0.3f;
            this.skyboxDayTexture = ResourcePool.GetTexture(dayTextures);
            this.skyboxNightTexture = ResourcePool.GetTexture(nightTextures);
            _postConstructor = true;
            attribs = new VBOArrayF(new float[6 * 6, 3] { { -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE }, { -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE }, { SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },
            { SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
            { SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
            { SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE }});
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            ResourcePool.ReleaseShaderProgram(shader);
            VAOManager.cleanUp(buffer);
            ResourcePool.ReleaseTexture(skyboxDayTexture);
            ResourcePool.ReleaseTexture(skyboxNightTexture);
        }

        #endregion
    }
}
