using MassiveGame.API.ObjectFactory;
using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.CollisionEditor.Core.SerializeAPI;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.Skeletal_Entities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.GameCore.Skybox;
using MassiveGame.Core.GameCore.Sun;
using MassiveGame.Core.GameCore.Sun.DayCycle;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.ioCore;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Light_visualization;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Debug.UiPanel;
using MassiveGame.Settings;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TextureLoader;
using static MassiveGame.Core.GameCore.Sun.DayCycle.DayPhases;

namespace MassiveGame.Core.GameCore
{
    public class GameWorld
    {
        private static GameWorld m_world = null;

        private Level m_currentLevel = null;
        private CollisionHeadUnit m_collisionHeadUnit;
        private UiFrameMaster m_uiFrameCreator;

        public Level GetLevel()
        {
            return m_currentLevel;
        }

        public void SetLevel(Level lvl)
        {
            m_currentLevel = lvl;
        }

        public CollisionHeadUnit GetCollisionHeadUnit()
        {
            return m_collisionHeadUnit;
        }

        public UiFrameMaster GetUiFrameCreator()
        {
            return m_uiFrameCreator;
        }

        // TODO ->> Here path to level supposed to be file with serialized level 
        public void LoadLevel(string pathToLevel)
        {
            throw new NotImplementedException();
        }

        private void SetLevelTestValues(Level level)
        {
            var rtParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0,
                PixelInternalFormat.DepthComponent16, EngineStatics.globalSettings.ShadowMapRezolution.X, EngineStatics.globalSettings.ShadowMapRezolution.Y,
                PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat);

            level.DirectionalLight = new DirectionalLight(rtParams, new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));
            level.DirectionalLight.GetShadowHolder().CreateShadowMapCache();

            var dayPhases = new DayPhases(new Morning(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(.7f)),
                    new DayPhases.Day(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.9f, 0.79f, 0.79f), new Vector3(1.0f)),
                new Evening(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.30f, 0.30f), new Vector3(0.9f)),
                new Night(new Vector3(0.09f, 0.09f, 0.09f), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.0f)));

            level.DayCycle = new DayLightCycle(level.DirectionalLight,
                Math.Max(level.LevelSize.X, level.LevelSize.Y), dayPhases);
            level.DayCycle.SetTime(25);
            level.DayCycle.TimeFlow = 0.001f;

            level.Mist = new MistComponent(0.003f, 1f, new Vector3(0.7f, 0.75f, 0.8f));

            // temporary
            //level.terrain = new Landscape(level.MAP_SIZE, level.MAP_HEIGHT, 3,
            //       ProjectFolders.HeightMapsTexturesPath + "heightmap2.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01.jpg",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/tundra02.jpg",
            //       ProjectFolders.MultitexturesPath + "b.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/snow01.jpg",
            //       ProjectFolders.MultitexturesPath + "blendMap.png");

            //level.terrain.SetNormalMapR(ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01_n.png");
            //level.terrain.SetNormalMapG(ProjectFolders.MultitexturesPath + "NewLandscape/tundra02_n.png");
            ////level.Map.SetNormalMapBlack(ProjectFolders.MultitexturesPath + "NewLandscape/snow01_n.png");
            //level.terrain.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_high.png");
            //level.terrain.SetMist(level.Mist);

            string modelPath = ProjectFolders.ModelsPath + "house00.obj";
            string texturePath = ProjectFolders.TextureAtlasPath + "houseDiffuse.jpg";
            string normalMapPath = ProjectFolders.NormalMapsPath + "houseNormal.jpg";
            string specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            StaticEntityArguments staticMeshArg = new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
              new Vector3(170, 13f + EngineStatics.MAP_HEIGHT, 170), new Vector3(0, 180, 0), new Vector3(20));
            var house1 = EngineObjectCreator.CreateInstance<Building>(staticMeshArg);
            level.StaticMeshCollection.AddToList(house1);

            DeserializeWrapper deserializer = new DeserializeWrapper();
            foreach (var item in level.StaticMeshCollection)
            {
                var inner_wrapper = deserializer.Deserialize<CollisionComponentsWrapper>("house.cl");
                item.SetComponents(inner_wrapper.SerializedComponents);
                item.SetCollisionHeadUnit(m_collisionHeadUnit);

                item.SetMistComponent(level.Mist);
            }

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "path.png";
            normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            MovableEntityArguments movableMeshArg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath, new Vector3(175, 1200, 170), new Vector3(0, 0, 0), new Vector3(4f));

            modelPath = ProjectFolders.ModelsPath + "model.dae";
            texturePath = ProjectFolders.MultitexturesPath + "diffuse.png";

            level.SkeletalMesh = new MovableSkeletalMeshEntity(modelPath, texturePath, normalMapPath, specularMapPath, 0.5f, new Vector3(175, 60, 170), new Vector3(-90, 0, 0), new Vector3(1));

            level.Player = EngineObjectCreator.CreateInstance<MovableMeshEntity>(movableMeshArg);
            level.Player.SetMistComponent(level.Mist);

            var wrapper = deserializer.Deserialize<CollisionComponentsWrapper>("2.cl");
            level.Player.SetComponents(wrapper.SerializedComponents);

            level.Player.SetCollisionHeadUnit(m_collisionHeadUnit);
            level.Player.Speed = 0.6f;

            // TODO: test io
            //TestingIO(level);

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "b.png";

            movableMeshArg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath, new Vector3(180, 200, 220), new Vector3(0, 0, 0), new Vector3(10));

            var bot = EngineObjectCreator.CreateInstance<MovableMeshEntity>(movableMeshArg);
            bot.SetMistComponent(level.Mist);

            wrapper = deserializer.Deserialize<CollisionComponentsWrapper>("2.cl");
            bot.SetComponents(wrapper.SerializedComponents);
            bot.SetCollisionHeadUnit(m_collisionHeadUnit);
            level.Bots.AddToList(bot);
            movableMeshArg = null;

            level.Skybox = new SkyboxEntity(
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
            level.Skybox.setMistComponent(level.Mist);

            level.Water = new WaterPlane(ProjectFolders.WaterTexturePath + "DUDV.png", ProjectFolders.WaterTexturePath + "normal.png",
                new Vector3(160, 29, 200), new Vector3(0, 0, 0), new Vector3(200, 1, 200), new WaterQuality(true, true, true), 10);

            level.SunRenderer = new SunRenderer(level.DirectionalLight, ProjectFolders.SunTexturePath + "sunC.png",
                    ProjectFolders.SunTexturePath + "sunB.png");

#if DEBUG || DESIGN_EDITOR
            level.PointLightDebugRenderer = new PointLightsDebugRenderer(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png",
                level.PointLightCollection);
#endif

            if (level.Camera as ThirdPersonCamera != null)
            {
                (level.Camera as ThirdPersonCamera).SetThirdPersonTarget(level.Player);
            }
            level.Camera.SetCollisionHeadUnit(m_collisionHeadUnit);

            //ch = new ComputeShader();
            //ch.Init();
        }

        private void TestingIO(Level level)
        {
            /***********************************************************/
            var serialization = new Action<MovableMeshEntity>((player) =>
            {
                BinaryFormatter serializer = new BinaryFormatter();


                using (FileStream fileStream = new FileStream("entity.bn", FileMode.OpenOrCreate))
                {
                    serializer.Serialize(fileStream, player);
                }
            });

            var deserialization = new Func<MovableMeshEntity>(() =>
            {
                BinaryFormatter serializer = new BinaryFormatter();
                MovableMeshEntity deserializedComponent = null;
                using (FileStream fileStream = new FileStream("entity.bn", FileMode.Open))
                {
                    deserializedComponent = serializer.Deserialize(fileStream) as MovableMeshEntity;
                }
                return deserializedComponent as MovableMeshEntity;
            });

            serialization(level.Player);
            level.Player = deserialization();
            level.Player.SetCollisionHeadUnit(m_collisionHeadUnit);
            /***********************************************************/
        }

        // TODO ->> Load test values for current level
        public void LoadTestLevel()
        {
            Level level = new Level(new Vector2(500, 500), "Test Level");
            m_currentLevel = level;

            level.PointLightCollection = new List<PointLight>();
            level.StaticMeshCollection = new ObserverListWrapper<Building>();
            level.Bots = new ObserverListWrapper<MovableMeshEntity>();
            level.VisibilityCheckCollection = new List<IVisible>();
            level.LitCheckCollection = new List<ILightHit>();
            level.ShadowCastCollection = new List<IDrawable>();

            level.Camera = new ThirdPersonCamera(new Vector3(0.5f, -0.8f, 0), 45);
            level.PlayerController = new PlayerController(level.Camera as ThirdPersonCamera);

            SetLevelTestValues(level);

            level.VisibilityCheckCollection.Add(m_currentLevel.SunRenderer);
            level.VisibilityCheckCollection.Add(m_currentLevel.Water);
            level.VisibilityCheckCollection.Add(m_currentLevel.Player);
            level.LitCheckCollection.Add(m_currentLevel.Player);
            level.ShadowCastCollection.Add(m_currentLevel.Player);
            level.ShadowCastCollection.Add(m_currentLevel.Terrain);
        }

        public static GameWorld GetWorldInstance()
        {
            if (m_world == null)
                m_world = new GameWorld();

            return m_world;
        }

        private GameWorld()
        {
            m_collisionHeadUnit = new CollisionHeadUnit();
            m_uiFrameCreator = new UiFrameMaster();
        }
    }
}
