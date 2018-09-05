using MassiveGame.API.MouseObjectDetector;
using MassiveGame.API.ObjectFactory;
using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.Skeletal_Entities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.GameCore.Skybox;
using MassiveGame.Core.GameCore.Sun;
using MassiveGame.Core.GameCore.Sun.DayCycle;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Light_visualization;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Debug.UiPanel;
using MassiveGame.Settings;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PhysicsBox.ComponentCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TextureLoader;
using static MassiveGame.Core.GameCore.Sun.DayCycle.DayPhases;

namespace MassiveGame.Engine
{
    public class EngineCore
    {
        // here has to be created and held game or editor window
        private Form m_UiWindow = null;
        private Stopwatch m_renderTickTime;
        
        private CollisionHeadUnit m_collisionHeadUnit;

        private bool bPostConstructor = true;

        private RenderThread m_renderThread;
        private GameThread m_gameThread;

        #region Init

        public EngineCore()
        {
            Point startScreenRezoluion = new Point(1400, 800);
            Action preConstructorFunction = new Action(preConstructor), renderQueueFunction = new Action(RenderQueue), cleanUpFunction = new Action(cleanEverythingUp);

#if DESIGN_EDITOR
            m_UiWindow = new UI.EditorWindow(startScreenRezoluion.X, startScreenRezoluion.Y, preConstructorFunction, renderQueueFunction, cleanUpFunction);
#else
            m_UiWindow = new UI.GameWindow(startScreenRezoluion.X, startScreenRezoluion.Y, preConstructorFunction, renderQueueFunction, cleanUpFunction);
#endif
            m_UiWindow.Left = EngineStatics.SCREEN_POSITION_X;
            m_UiWindow.Top = EngineStatics.SCREEN_POSITION_Y;
            preConstructor();
        }

        public void ShowUiWindow()
        {
            m_UiWindow.ShowDialog();
        }

        public void CloseUiWindow()
        {
            m_UiWindow.Close();
        }

        private void defaultMatrixSettings()
        {
            // create projection matrix
            EngineStatics.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(EngineStatics.FoV), EngineStatics.SCREEN_ASPECT_RATIO,
                EngineStatics.NEAR_CLIPPING_PLANE, EngineStatics.FAR_CLIPPING_PLANE);
        }

        private void LoadIniSettings()
        {
            SettingsLoader settingsLoader = new SettingsLoader();
            settingsLoader.SetGlobalSettings();
        }

        private void preConstructor() //Start initialize values
        {
            EngineStatics.Camera = new ThirdPersonCamera(new Vector3(0.5f, -0.8f, 0), 45);
            EngineStatics.PrevCursorPosition = new System.Drawing.Point(-1, -1);
            EngineStatics.ElapsedTime = DateTime.Now;
            EngineStatics.playerController = new PlayerController(EngineStatics.Camera as ThirdPersonCamera);

            LoadIniSettings();

            m_renderTickTime = new Stopwatch();
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                m_collisionHeadUnit = new CollisionHeadUnit();
                EngineStatics.ProjectionMatrix = Matrix4.Identity;
                EngineStatics.City = new List<Building>();
                // need to delete NewMesh.msh if it exists
                if (File.Exists(@"NewModel.msh"))
                    File.Delete(@"NewModel.msh");
                defaultMatrixSettings();


                setTestValues();
                KeyboardBindingsLoader bindingsLoader = new KeyboardBindingsLoader();
                bindingsLoader.SetKeyboardBindings();

                // add objects to optimization list
                EngineStatics.RenderableMeshCollection = new List<IVisible> { EngineStatics.SunReplica, EngineStatics.Water,
                    EngineStatics.Player, EngineStatics.Enemy };
                EngineStatics.RenderableMeshCollection.AddRange(EngineStatics.City);

                EngineStatics.LitByLightSourcesMeshCollection = new List<ILightHit> { EngineStatics.Player, EngineStatics.Enemy };
                EngineStatics.LitByLightSourcesMeshCollection.AddRange(EngineStatics.City);

                // Start game and render thread execution
                m_renderThread = new RenderThread();

                // Every frame capture time of draw call execution
                m_renderTickTime.Start();
                m_gameThread = new GameThread(100, 1);
            }
        }

        #endregion

        private void setTestValues()
        {
            var rtParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.DepthComponent16, EngineStatics.globalSettings.ShadowMapRezolution.X, EngineStatics.globalSettings.ShadowMapRezolution.Y, PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat);
            EngineStatics.Sun = new DirectionalLight(rtParams, new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));
            EngineStatics.Sun.GetShadow().CreateShadowMapCache();

            var dayPhases = new DayPhases(new Morning(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(.7f)),
                    new DayPhases.Day(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.9f, 0.79f, 0.79f), new Vector3(1.0f)),
                new Evening(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.30f, 0.30f), new Vector3(0.9f)),
                new Night(new Vector3(0.09f, 0.09f, 0.09f), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.0f)));

            EngineStatics.DayCycle = new DayLightCycle(EngineStatics.Sun,
                EngineStatics.MAP_SIZE, dayPhases);
            EngineStatics.DayCycle.SetTime(25);
            EngineStatics.DayCycle.TimeFlow = 0.001f;

            EngineStatics.PointLight = new List<PointLight>();

            /*Create mist component*/
            EngineStatics.Mist = new MistComponent(0.003f, 1f, new Vector3(0.7f, 0.75f, 0.8f));

            // temporary

            //EngineStatics.terrain = new Landscape(EngineStatics.MAP_SIZE, EngineStatics.MAP_HEIGHT, 3,
            //       ProjectFolders.HeightMapsTexturesPath + "heightmap2.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01.jpg",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/tundra02.jpg",
            //       ProjectFolders.MultitexturesPath + "b.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/snow01.jpg",
            //       ProjectFolders.MultitexturesPath + "blendMap.png");

            //EngineStatics.terrain.SetNormalMapR(ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01_n.png");
            //EngineStatics.terrain.SetNormalMapG(ProjectFolders.MultitexturesPath + "NewLandscape/tundra02_n.png");
            ////EngineStatics.Map.SetNormalMapBlack(ProjectFolders.MultitexturesPath + "NewLandscape/snow01_n.png");
            //EngineStatics.terrain.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_high.png");
            //EngineStatics.terrain.SetMist(EngineStatics.Mist);

            string modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            string texturePath = ProjectFolders.MultitexturesPath + "b.png";
            string normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            string specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //    new Vector3(230, 0 + EngineStatics.MAP_HEIGHT, 310), new Vector3(20, 0, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 2.5f + EngineStatics.MAP_HEIGHT, 248), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 5f + EngineStatics.MAP_HEIGHT, 180), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //  new Vector3(230, 7.5f + EngineStatics.MAP_HEIGHT, 115), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 10 + EngineStatics.MAP_HEIGHT, 48), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
              new Vector3(170, 13f + EngineStatics.MAP_HEIGHT, 170), new Vector3(0, 180, 0), new Vector3(10, 10, 10))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(280, 10, 350), new Vector3(0, 180, 0), new Vector3(10))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //    new Vector3(230, 10, 410), new Vector3(0, 180, 0), new Vector3(10))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(260, 10, 400), new Vector3(0, 180, 0), new Vector3(10))));

            // TEST components
            ComponentSerializer serializer = new ComponentSerializer();
            SerializedComponentsContainer container;
            Component parent = new Component();
            Component component;
            foreach (var item in EngineStatics.City)
            {
                // FIXME
                container = serializer.DeserializeComponents("12345.cl");
                parent.ChildrenComponents = container.SerializedComponents;
                component = convertToSceneComponent(parent);
                item.SetComponents(component.ChildrenComponents);
                item.SetCollisionHeadUnit(m_collisionHeadUnit);

                item.SetMistComponent(EngineStatics.Mist);
            }

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "path.png";
            normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            MovableEntityArguments arg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath, new Vector3(175, 1200, 170), new Vector3(0, 0, 0), new Vector3(4f));

            modelPath = ProjectFolders.ModelsPath + "model.dae";
            texturePath = ProjectFolders.MultitexturesPath + "diffuse.png";

            EngineStatics.SkeletalMesh = new MovableSkeletalMeshEntity(modelPath, texturePath, normalMapPath, specularMapPath, 0.5f, new Vector3(175, 60, 170), new Vector3(-90, 0 , 0), new Vector3(1));

            EngineStatics.Player = (MovableMeshEntity)EngineObjectCreator.CreateInstance(arg);
            EngineStatics.Player.SetMistComponent(EngineStatics.Mist);

            // TEST components
            container = serializer.DeserializeComponents("123.cl");
            parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            component = convertToSceneComponent(parent);
            // TEST
            EngineStatics.Player.SetComponents(component.ChildrenComponents);

            EngineStatics.Player.SetCollisionHeadUnit(m_collisionHeadUnit);
            EngineStatics.Player.Speed = 0.6f;

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "b.png";

            arg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath, new Vector3(180, 200, 220), new Vector3(0, 0, 0), new Vector3(10));

            EngineStatics.Enemy = (MovableMeshEntity)EngineObjectCreator.CreateInstance(arg);
            EngineStatics.Enemy.SetMistComponent(EngineStatics.Mist);

            container = serializer.DeserializeComponents("123.cl");
            parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            component = convertToSceneComponent(parent);
            EngineStatics.Enemy.SetComponents(component.ChildrenComponents);
            EngineStatics.Enemy.SetCollisionHeadUnit(m_collisionHeadUnit);
            arg = null;

            //EngineStatics.Grass = new PlantReadyMaster(
            //    4000, EngineStatics.MAP_SIZE, PlantModels.getBillboardModel1(), new Vector3(1),
            //    new string[] { ProjectFolders.GrassTexturesPath + "grass1.png",
            //        ProjectFolders.GrassTexturesPath + "grass2.png",
            //        ProjectFolders.GrassTexturesPath + "grass3.png"}, new WindComponent(2.35f, 1.1f, 0.6f, new Vector3(0.6f, 0, 0.3f)), EngineStatics.Mist);    //Добавление травы

            //EngineStatics.Plant1 = new PlantReadyMaster(13, EngineStatics.MAP_SIZE, PlantModels.getPlantModel2(), new Vector3(1),
            //    new string[] { ProjectFolders.GrassTexturesPath + "fern.png" },
            //  new WindComponent(0.95f, 0.35f, 0.5f, new Vector3(0.5f, 0, 0.5f)), EngineStatics.Mist);

            EngineStatics.Skybox = new Skybox(
                    new string[] { ProjectFolders.SkyboxTexturesPath + "/Day/" + "right.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "left.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "top.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "bottom.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "back.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "front.bmp" },
                    new string[] { ProjectFolders.SkyboxTexturesPath + "/Night/" + "right.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "left.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "top.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "bottom.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "back.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "front.png" });
            EngineStatics.Skybox.setMistComponent(EngineStatics.Mist);

            EngineStatics.Water = new WaterPlane(ProjectFolders.WaterTexturePath + "DUDV.png", ProjectFolders.WaterTexturePath + "normal.png",
                new Vector3(160, 29, 200), new Vector3(0, 0, 0), new Vector3(200, 1, 200), new WaterQuality(true, true, true), 10);
            //EngineStatics.Water.setMist(EngineStatics.Mist);

            EngineStatics.SunReplica = new SunRenderer(EngineStatics.Sun, ProjectFolders.SunTexturePath + "sunC.png",
                    ProjectFolders.SunTexturePath + "sunB.png");

            EngineStatics.Picker = new MousePicker(EngineStatics.ProjectionMatrix, EngineStatics.Camera);

            //EngineSingleton.EnvObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(true), TextureSet.PlayerTextureSet2,
            //    TextureSet.SkyboxDayCubemapTexture, new Vector3(180, 0, 220), new Vector3(0, 0, 0), new Vector3(10));

            EngineStatics.pointLightDebugRenderer = new PointLightsDebugRenderer(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png"
                , EngineStatics.PointLight);

            //gras = new Grass(new Vector3(1, 0, 1), new Vector3(1), new Vector3(0), new Vector3(0.2f, 0.8f, 0.3f));
            //envObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(false), TextureSet.PlayerTextureSet2, TextureSet.SkyboxDayCubemapTexture,
            //    new Vector3(40, 70, 40), new Vector3(0, 0, 0), new Vector3(0.5f));

            if (EngineStatics.Camera as ThirdPersonCamera != null)
            {
                (EngineStatics.Camera as ThirdPersonCamera).SetThirdPersonTarget(EngineStatics.Player);
            }
            EngineStatics.Camera.SetCollisionHeadUnit(m_collisionHeadUnit);
            //EngineStatics.Camera.SetFirstPerson();

            EngineStatics.shadowList = new List<IDrawable>();
            EngineStatics.City.ForEach(new Action<Building>((house) => { EngineStatics.shadowList.Add(house); }));
            EngineStatics.shadowList.Add(EngineStatics.Player);
            EngineStatics.shadowList.Add(EngineStatics.Enemy);
            EngineStatics.shadowList.Add(EngineStatics.terrain);


            EngineStatics.uiFrameCreator = new UiFrameMaster();

            //ch = new ComputeShader();
            //ch.Init();
        }

        private void RenderQueue()
        {
            postConstructor();
            m_renderTickTime.Restart();
            m_renderThread.ThreadExecution(EngineStatics.globalSettings.ActualScreenRezolution, bPostConstructor);
            EngineStatics.RENDER_TIME = (float)m_renderTickTime.Elapsed.TotalSeconds;
            bPostConstructor = false;
        }

        #region TEST

        private void convertToSceneComponentRecursive(Component existing, Component duplicate)
        {
            for (Int32 i = 0; i < existing.ChildrenComponents.Count; i++)
            {
                Component existingChild = existing.ChildrenComponents[i];
                duplicate.ChildrenComponents.Add(new SceneComponent(existingChild));
                Component duplicateChild = duplicate.ChildrenComponents[i];
                duplicate.ChildrenComponents[i].ParentComponent = duplicate;
                convertToSceneComponentRecursive(existingChild, duplicateChild);
            }
        }

        private Component convertToSceneComponent(Component existing)
        {
            Component duplicate = new Component();
            convertToSceneComponentRecursive(existing, duplicate);
            return duplicate;
        }

        #endregion

        #region Cleaning

        private void cleanEverythingUp()
        {
            if (EngineStatics.Water != null) EngineStatics.Water.cleanUp();
            if (EngineStatics.SunReplica != null) EngineStatics.SunReplica.cleanUp();
            if (EngineStatics.terrain != null) EngineStatics.terrain.cleanUp();
            if (EngineStatics.Player != null) EngineStatics.Player.CleanUp();
            if (EngineStatics.Enemy != null) EngineStatics.Enemy.CleanUp();
            if (EngineStatics.Grass != null) EngineStatics.Grass.cleanUp();
            if (EngineStatics.Plant1 != null) EngineStatics.Plant1.cleanUp();
            if (EngineStatics.City != null) foreach (Building house in EngineStatics.City) { house.CleanUp(); }
            if (EngineStatics.Skybox != null) EngineStatics.Skybox.cleanUp();
        }

        #endregion
    }
}
