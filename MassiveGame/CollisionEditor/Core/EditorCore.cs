using OpenTK;
using TextureLoader;
using MassiveGame.Core.ComponentCore;
using CollisionEditor.RenderCore;
using CollisionEditor.IOCore;
using MassiveGame.CollisionEditor.Core;
using MassiveGame.Core.SettingsCore;

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
        private RawModel CollisionBoxModel;

        private object locker = new object();

        public bool bComponentHierarchyIsDirty { set; get; } = false;

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
            lock (locker)
            {
                if (actor != null && bComponentHierarchyIsDirty)
                {
                    Matrix4 viewMatrix = EditorCamera.ViewMatrix;
                    CollisionComponentBoundBuilder.UpdateCollisionBoundHierarchy(actor, CollisionBoxModel.Buffer.getBufferData().Vertices);
                    bComponentHierarchyIsDirty = false;
                }
            }
        }

        private void RenderBasePass()
        {
            if (PostConstructor)
            {
                CollisionBoxModel = new RawModel(ProxyModelLoader.LoadModel(CollisionBoxPath));
                DefaultTexture = ProxyTextureLoader.LoadSingleTexture(ProjectFolders.EditorTexturePath + "\\default.jpg");
                DefaultShader = new BasicShader(ProjectFolders.ShadersPath + "\\basicVS.glsl", ProjectFolders.ShadersPath + "\\basicFS.glsl");
                PostConstructor = false;
            }

            if (actor != null && !bComponentHierarchyIsDirty)
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
            var component = new SceneComponent();
            ComponentCreator.AddComponentToRoot(component);
            return component;
        }

        public void SetTexture(string texturePath)
        {
            actor.SetTexture(ProxyTextureLoader.LoadSingleTexture(texturePath));
        }
    }
}
