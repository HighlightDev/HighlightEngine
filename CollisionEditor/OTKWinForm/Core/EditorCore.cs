using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using TextureLoader;
using OTKWinForm.IOCore;
using OTKWinForm.RenderCore;
using PhysicsBox.ComponentCore;

namespace OTKWinForm.Core
{
    public class EditorCore
    {
        public Actor actor { set; get; }
        private Skybox skybox;
        private Matrix4 projectionMatrix;

        public Camera EditorCamera { set; get; }

        public ITexture DefaultTexture;
        public BasicShader DefaultShader;

        private bool PostConstructor = true;

        private string CollisionBoxPath;

        public EditorCore()
        {
            actor = null;
            skybox = new Skybox();

            CollisionBoxPath = System.Environment.CurrentDirectory + "/../../Mesh/playerCube.obj";

            BuildProjectionMatrix();
            EditorCamera = new Camera(0, 0, 10, 0, 0, 0, 0, 1, 0);
        }

        private void BuildProjectionMatrix()
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75), 16 / 9, 1, 400);
        }

        public void DisplayEditor()
        {
            RenderBasePass();
        }

        public void TickEditor()
        {
            if (actor != null)
                actor.Tick(EditorCamera.ViewMatrix, projectionMatrix);
        }

        private void RenderBasePass()
        {
            if (PostConstructor)
            {
                DefaultTexture = ProxyTextureLoader.LoadSingleTexture(System.Environment.CurrentDirectory + "\\..\\..\\Textures\\default.jpg");
                DefaultShader = new BasicShader(System.Environment.CurrentDirectory + "/../../Shaders/basicVS.glsl", "../../Shaders/basicFS.glsl");
                PostConstructor = false;
            }

            if (actor != null)
                actor.Render(EditorCamera.ViewMatrix, projectionMatrix);

            skybox.Render(EditorCamera.ViewMatrix, projectionMatrix);
        }

        public Actor CreateActor(string modelPath)
        {
            if (actor != null)
            {
                actor.CleanUp();
                ComponentCreator.RemoveComponentFromRoot(actor);
            }
            actor = new Actor(new RenderCore.RawModel(ProxyModelLoader.LoadModel(modelPath)), DefaultTexture, DefaultShader);
            ComponentCreator.AddComponentToRoot(actor);
            return actor;
        }

        public Component CreateCollisionComponent()
        {
            var component =  new SceneComponent(new RawModel(ProxyModelLoader.LoadModel(CollisionBoxPath)), DefaultTexture, DefaultShader);
            ComponentCreator.AddComponentToRoot(component);
            return component;
        }

        public void SetTexture(string texturePath)
        {
            actor.SetTexture(ProxyTextureLoader.LoadSingleTexture(texturePath));
        }
    }
}
