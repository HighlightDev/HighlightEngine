using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GpuGraphics;
using TextureLoader;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public class Grass
    {
        private SingleTexture2D _texture;
        private Vector3 translation;
        private Vector3 scale;
        private Vector3 rotation;
        private bool postConstructor;
        private GrassShader shader;
        private Vector3 color;
        //private float time;
        private float rotate = 0.0f;
        private float windSpeed = 110f;
        private float rotateSpeed = 20;

        private VBOArrayF attributes;
        private VAO buffer;

        private Matrix4[] translations = new Matrix4[2000];
        private float[] time = new float[2000];

        public Vector3 createRandom(ref int position)
        {
            Random random = new Random(position);
            return new Vector3((float)random.NextDouble() * (0.01f * position), 0.0f, (float)random.NextDouble() * (0.01f * position)); 
        }

        public void render(Camera camera, ref Matrix4 projectionMatrix, Terrain landscape, float renderTime, DirectionalLight sun = null)
        {
            postConstruction(landscape);
            // make pause at climax points
            _texture.bindTexture2D(TextureUnit.Texture0, _texture.TextureID);
            for (int i = 0; i < translations.Length; i++)
            {
                if ((time[i] > 160 && time[i] < 200) || (time[i] > 340 || time[i] < 20))
                {
                    time[i] += ((windSpeed * renderTime) * 0.55f);
                }
                else time[i] += (windSpeed + (createRandom(ref i).X)) * renderTime;
                time[i] %= 360;
                float angle = MathHelper.DegreesToRadians(time[i]);

                //rotate += rotateSpeed * renderTime;
                //rotate %= 360;
                //float rotateAngle = MathHelper.DegreesToRadians(rotate);

                //Matrix4 modelMatrix = Matrix4.Identity;
                //modelMatrix *= Matrix4.CreateScale(scale);
                //modelMatrix *= Matrix4.CreateRotationX(rotation.X);
                //modelMatrix *= Matrix4.CreateRotationY(rotateAngle);
                //modelMatrix *= Matrix4.CreateRotationZ(rotation.Z);

                shader.startProgram();
                shader.setUniformValues(ref translations[i], camera.getViewMatrix(), ref projectionMatrix, sun, ref color, ref angle, 0);
                VAOManager.renderBuffers(buffer, PrimitiveType.Points);
                shader.stopProgram();
            }
            
        }
        
        public void postConstruction(Terrain landscape = null)
        {
            if (postConstructor)
            {
                shader = new GrassShader(ProjectFolders.ShadersPath + "grassVS.glsl", ProjectFolders.ShadersPath + "grassFS.glsl",
                    ProjectFolders.ShadersPath + "grassGS.glsl");

                attributes = new VBOArrayF(new float[1, 3] { { 0.0f, 0.0f, 0.0f } });
                buffer = new VAO(attributes);
                VAOManager.genVAO(buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, buffer);
                _texture = new SingleTexture2D(ProjectFolders.GrassTexturesPath + "grass1(mono).png", false, false);

                try
                {
                    for (int i = 0; i < translations.Length; i++)
                    {
                        var currentLocation = createRandom(ref i);
                        translations[i] = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(currentLocation.Z * (i + 1)));
                        translations[i] *= Matrix4.CreateTranslation(new Vector3(currentLocation.X, landscape.getLandscapeHeight(currentLocation.X, currentLocation.Z), currentLocation.Z));
                    }
                }
                catch (NullReferenceException)
                {
                    // error
                    return;
                }

                

                postConstructor = false;
            }
        }

        public Grass(Vector3 position, Vector3 scale, Vector3 rotation, Vector3 color)
        {
            this.translation = position;
            this.scale = scale;
            this.rotation = rotation;
            postConstructor = true;
            this.color = color;
        }
    }
}
