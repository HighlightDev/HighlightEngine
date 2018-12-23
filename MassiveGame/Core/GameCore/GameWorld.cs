using MassiveGame.API.ObjectFactory;
using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.CollisionEditor.Core.SerializeAPI;
using MassiveGame.Core.DebugCore.UiPanel;
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
using MassiveGame.Core.RenderCore.Shadows;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.SettingsCore;
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
        private const float DEFAULT_LEVEL_SIZE = 300f;

        private static GameWorld m_world = null;

        private Level m_currentLevel = null;

        private UiFrameMaster m_uiFrameCreator;

        [NonSerialized]
        private List<IVisible> m_visibilityCheckCollection;

        [NonSerialized]
        private List<ILightHit> m_litCheckCollection;

        [NonSerialized]
        private List<IDrawable> m_shadowCastCollection;

        public List<IVisible> VisibilityCheckCollection { set { m_visibilityCheckCollection = value; } get { return m_visibilityCheckCollection; } }

        public List<ILightHit> LitCheckCollection { set { m_litCheckCollection = value; } get { return m_litCheckCollection; } }

        public List<IDrawable> ShadowCastCollection { set { m_shadowCastCollection = value; } get { return m_shadowCastCollection; } }

        public CollisionHeadUnit CollisionHeadUnitObject { private set; get; }

        public Level GetLevel()
        {
            return m_currentLevel;
        }

        public void SetLevel(Level lvl)
        {
            m_currentLevel = lvl;
        }

        public UiFrameMaster GetUiFrameCreator()
        {
            return m_uiFrameCreator;
        }

        public Level CreateEmptyLevel()
        {
            Level resultLevel = new Level(new Vector2(DEFAULT_LEVEL_SIZE, DEFAULT_LEVEL_SIZE), "DefaultLevel");
            return resultLevel;
        }

#if ENGINE_EDITOR

        public void CreateEditorDefaultLevel()
        {
            m_currentLevel = CreateEmptyLevel();
            m_currentLevel.Camera = new FirstPersonCamera(new Vector3(0, 0, 1), new Vector3(0, 5, 0));
        }
#endif
        #region TEST

        private void SetLevelTestValues(Level level)
        {
            var globalLightShadowMapParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0,
                PixelInternalFormat.DepthComponent16, EngineStatics.globalSettings.ShadowMapRezolution.X, EngineStatics.globalSettings.ShadowMapRezolution.Y,
                PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat);

            level.DirectionalLight = new DirectionalLightWithShadow(level.Camera, globalLightShadowMapParams, new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));

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
            //level.terrain.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_mid.png");
            //level.terrain.SetMist(level.Mist);

            string modelPath = ProjectFolders.ModelsPath + "house00.obj";
            string texturePath = ProjectFolders.AlbedoTexturePath + "houseDiffuse.jpg";
            string normalMapPath = ProjectFolders.NormalMapPath + "houseNormal.jpg";
            string specularMapPath = ProjectFolders.SpecularMapPath + "brick_sm.png";

            StaticEntityArguments staticMeshArg = new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
              new Vector3(170, 13f + EngineStatics.MAP_HEIGHT, 170), new Vector3(0, 180, 0), new Vector3(20));
            var house = EngineObjectCreator.CreateInstance<Building>(staticMeshArg);
            level.StaticMeshCollection.AddToList(house);

            staticMeshArg = new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
              new Vector3(170, 13f + EngineStatics.MAP_HEIGHT, 210), new Vector3(0, 180, 0), new Vector3(20));
            house = EngineObjectCreator.CreateInstance<Building>(staticMeshArg);
            level.StaticMeshCollection.AddToList(house);

            DeserializeWrapper deserializer = new DeserializeWrapper();
            foreach (var item in level.StaticMeshCollection)
            {
                var inner_wrapper = deserializer.Deserialize<CollisionComponentsWrapper>(ProjectFolders.CollisionPath + "actualHome1.cl");
                item.SetComponents(inner_wrapper.SerializedComponents);
                item.SetCollisionHeadUnit(CollisionHeadUnitObject);

                item.SetMistComponent(level.Mist);
            }

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.LandscapeTexturePath + "path.png";
            normalMapPath = ProjectFolders.NormalMapPath + "brick_nm_mid.png";
            specularMapPath = ProjectFolders.SpecularMapPath + "brick_sm.png";

            MovableEntityArguments movableMeshArg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath, new Vector3(155, 1200, 170), new Vector3(0, 0, 0), new Vector3(4f));

            modelPath = ProjectFolders.ModelsPath + "model.dae";
            texturePath = ProjectFolders.AlbedoTexturePath + "diffuse.png";

            level.SkeletalMesh = new MovableSkeletalMeshEntity(modelPath, texturePath, normalMapPath, specularMapPath, 0.5f, new Vector3(175, 80, 170), new Vector3(-90, 0, 0), new Vector3(1));

            level.Player.AddToWrapper(EngineObjectCreator.CreateInstance<MovableMeshEntity>(movableMeshArg));
            level.Player.GetData().SetMistComponent(level.Mist);

            var wrapper = deserializer.Deserialize<CollisionComponentsWrapper>(ProjectFolders.CollisionPath + "2.cl");
            level.Player.GetData().SetComponents(wrapper.SerializedComponents);

            level.Player.GetData().SetCollisionHeadUnit(CollisionHeadUnitObject);
            level.Player.GetData().Speed = 0.6f;

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.LandscapeTexturePath + "b.png";

            movableMeshArg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath, new Vector3(180, 200, 220), new Vector3(0, 0, 0), new Vector3(10));

            var bot = EngineObjectCreator.CreateInstance<MovableMeshEntity>(movableMeshArg);
            bot.SetMistComponent(level.Mist);

            wrapper = deserializer.Deserialize<CollisionComponentsWrapper>(ProjectFolders.CollisionPath + "2.cl");
            bot.SetComponents(wrapper.SerializedComponents);
            bot.SetCollisionHeadUnit(CollisionHeadUnitObject);
            level.Bots.AddToList(bot);
            movableMeshArg = null;

            level.Skybox = new SkyboxEntity(
                    new string[] { ProjectFolders.CubemapTexturePath + "/Day/" + "right.bmp",
                    ProjectFolders.CubemapTexturePath + "/Day/" + "left.bmp",
                    ProjectFolders.CubemapTexturePath + "/Day/" + "top.bmp",
                    ProjectFolders.CubemapTexturePath + "/Day/" + "bottom.bmp",
                    ProjectFolders.CubemapTexturePath + "/Day/" + "back.bmp",
                    ProjectFolders.CubemapTexturePath + "/Day/" + "front.bmp" },
                    new string[] { ProjectFolders.CubemapTexturePath + "/Night/" + "right.png",
                    ProjectFolders.CubemapTexturePath + "/Night/" + "left.png",
                    ProjectFolders.CubemapTexturePath + "/Night/" + "top.png",
                    ProjectFolders.CubemapTexturePath + "/Night/" + "bottom.png",
                    ProjectFolders.CubemapTexturePath + "/Night/" + "back.png",
                    ProjectFolders.CubemapTexturePath + "/Night/" + "front.png" });
            level.Skybox.setMistComponent(level.Mist);

            level.Water.AddToWrapper(new WaterPlane(ProjectFolders.DistortionTexturePath + "water_dudv.png", ProjectFolders.NormalMapPath + "water_normal.png",
                new Vector3(160, 29, 200), new Vector3(0, 0, 0), new Vector3(200, 1, 200), new WaterQuality(true, true, true), 10));

            level.SunRenderer.AddToWrapper(new SunRenderer(level.DirectionalLight, ProjectFolders.PostprocessTexturePath + "sunC.png",
                    ProjectFolders.PostprocessTexturePath + "sunB.png", 150, 130));

            // Set third person target
            ThirdPersonCamera thirdPersonCamera = (level.Camera as ThirdPersonCamera);
            thirdPersonCamera?.SetThirdPersonTarget(level.Player.GetData());

            //ch = new ComputeShader();
            //ch.Init();
        }


        private void SerializeCurrentLevel()
        {
            void Serialize<T>(T obj, string path)
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(fileStream, obj);
                    fileStream.Close();
                }
            }

            string localPath = "testLevel.save";
            Serialize(m_currentLevel, localPath);
        }

        public void LoadTestLevel()
        {
#if true
            {
                Level level = new Level(new Vector2(500, 500), "Test Level");
                m_currentLevel = level;

                level.Camera = new ThirdPersonCamera(new Vector3(0.5f, -0.8f, 0), 45);
#if DEBUG
                level.PlayerController = new PlayerController(level.Camera as ThirdPersonCamera);
#endif

                SetLevelTestValues(level);

                level.Camera.SetCollisionHeadUnit(CollisionHeadUnitObject);

                SerializeCurrentLevel();
            }
#else
            {
                m_currentLevel = LoadLevel("testLevel.save");
            }
#endif
        }

#endregion

        public Level LoadLevel(string pathToLevel)
        {
            Level result = null;
            DeserializeWrapper deserializer = new DeserializeWrapper();
            result = deserializer.Deserialize<Level>(pathToLevel);
            return result;
        }

        public void PostInit()
        {
#if DEBUG || ENGINE_EDITOR
            if (m_currentLevel != null && m_currentLevel.PointLightCollection.GetCount() > 0)
            {
                m_currentLevel.PointLightDebugRenderer = new PointLightsDebugRenderer(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png",
                    m_currentLevel.PointLightCollection.GetData());
            }
#endif
        }

        public static GameWorld GetWorldInstance()
        {
            if (m_world == null)
                m_world = new GameWorld();

            return m_world;
        }

        private GameWorld()
        {
            m_uiFrameCreator = new UiFrameMaster();
            VisibilityCheckCollection = new List<IVisible>();
            LitCheckCollection = new List<ILightHit>();
            ShadowCastCollection = new List<IDrawable>();
            CollisionHeadUnitObject = new CollisionHeadUnit();
        }
    }
}
