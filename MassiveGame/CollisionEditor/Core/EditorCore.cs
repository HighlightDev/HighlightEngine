using OpenTK;
using TextureLoader;
using PhysicsBox.ComponentCore;
using CollisionEditor.RenderCore;
using CollisionEditor.IOCore;
using MassiveGame.Settings;

namespace CollisionEditor.Core
{
    public class EditorCore
    {
        public  Actor actor { set; get; }
        private Skybox skybox;
        private Matrix4 projectionMatrix;

        public FirstPersonCamera EditorCamera { set; get; }

        public ITexture DefaultTexture;
        public BasicShader DefaultShader;

        private bool PostConstructor = true;

        private string CollisionBoxPath;

        public EditorCore()
        {
            actor = null;
            skybox = new Skybox();

            CollisionBoxPath = ProjectFolders.ModelsPath + "\\playerCube.obj";

            BuildProjectionMatrix();
            EditorCamera = new FirstPersonCamera(0, 0, 10, 0, 0, 0, 0, 1, 0);
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
            {
                Matrix4 viewMatrix = EditorCamera.ViewMatrix;
                actor.Tick(0);
            }
        }

        private void RenderBasePass()
        {
            if (PostConstructor)
            {
                DefaultTexture = ProxyTextureLoader.LoadSingleTexture(ProjectFolders.TextureAtlasPath + "\\default.jpg");
                DefaultShader = new BasicShader(ProjectFolders.ShadersPath + "\\basicVS.glsl", ProjectFolders.ShadersPath + "\\basicFS.glsl");
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
            actor = new Actor(new RawModel(ProxyModelLoader.LoadModel(modelPath)), DefaultTexture, DefaultShader);
            ComponentCreator.AddComponentToRoot(actor);
            return actor;
        }

        public Component CreateCollisionComponent()
        {
            var component = new SceneComponent(new RawModel(ProxyModelLoader.LoadModel(CollisionBoxPath)), DefaultTexture, DefaultShader);
            ComponentCreator.AddComponentToRoot(component);
            return component;
        }

        public void SetTexture(string texturePath)
        {
            actor.SetTexture(ProxyTextureLoader.LoadSingleTexture(texturePath));
        }
    }
}
