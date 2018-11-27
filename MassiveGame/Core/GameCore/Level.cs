using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.GameCore.Sun;
using MassiveGame.Core.GameCore.Sun.DayCycle;
using MassiveGame.Core.GameCore.Terrain;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.GameCore.Skybox;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore.Visibility;
using OpenTK;
using System.Collections.Generic;
using MassiveGame.Core.RenderCore.Light_visualization;
using MassiveGame.Core.GameCore.Entities.Skeletal_Entities;
using MassiveGame.Core.ioCore;
using System;
using System.Runtime.Serialization;
using MassiveGame.Core.RenderCore.Shadows;
using MassiveGame.Core.PhysicsCore;

namespace MassiveGame.Core.GameCore
{
    [Serializable]
    public class Level : IPostDeserializable, ISerializable
    {
        #region private_fields

        private string m_levelName = string.Empty;

        private Vector2 m_levelSize;

        private ObserverListWrapper<PointLight> m_pointLightCollection;

#if DEBUG || DESIGN_EDITOR
        [NonSerialized]
        private PointLightsDebugRenderer m_pointLightDebugRenderer;
#endif

        private ObserverWrapper<MovableMeshEntity> m_player;

        private ObserverListWrapper<MovableMeshEntity> m_bots;

        private ObserverListWrapper<Building> m_staticMeshCollection;

        private BaseCamera m_camera;

        private SkyboxEntity m_skybox;

        private DirectionalLight m_directionalLight;

        private DayLightCycle m_dayLightCycle;

        private ObserverWrapper<Landscape> m_terrain;

        [NonSerialized]
        private MovableSkeletalMeshEntity m_skeletalMesh;

        private ObserverWrapper<WaterPlane> m_waterPlane;

        private ObserverWrapper<SunRenderer> m_sunRenderer;

        private MistComponent m_mistComponent;

        [NonSerialized]
        private PlayerController m_playerController;

        [NonSerialized]
        private PlantReadyMaster m_grass;

        [NonSerialized]
        private PlantReadyMaster m_plant;

        #endregion  

        #region public_properties

        public string LevelName { set { m_levelName = value; } get { return m_levelName; } }

        public Vector2 LevelSize { set { m_levelSize = value; } get { return m_levelSize; } }

        public ObserverListWrapper<PointLight> PointLightCollection { set { m_pointLightCollection = value; }  get { return m_pointLightCollection; } }

#if DEBUG || DESIGN_EDITOR
        public PointLightsDebugRenderer PointLightDebugRenderer { set { m_pointLightDebugRenderer = value; } get { return m_pointLightDebugRenderer; } }
#endif

        public ObserverWrapper<MovableMeshEntity> Player { set { m_player = value; }  get { return m_player; } }

        public ObserverListWrapper<MovableMeshEntity> Bots { set { m_bots = value; } get { return m_bots; } }

        public ObserverListWrapper<Building> StaticMeshCollection { set { m_staticMeshCollection = value; } get { return m_staticMeshCollection; } }

        public BaseCamera Camera { set { m_camera = value; } get { return m_camera; } }

        public SkyboxEntity Skybox { set { m_skybox = value; } get { return m_skybox; } }

        public DirectionalLight DirectionalLight { set { m_directionalLight = value; } get { return m_directionalLight; } }

        public DayLightCycle DayCycle { set { m_dayLightCycle = value; } get { return m_dayLightCycle; } }

        public ObserverWrapper<Landscape> Terrain { set { m_terrain = value; } get { return m_terrain; } }

        public MovableSkeletalMeshEntity SkeletalMesh { set { m_skeletalMesh = value; } get { return m_skeletalMesh; } }

        public ObserverWrapper<WaterPlane> Water { set { m_waterPlane = value; } get { return m_waterPlane; } }

        public ObserverWrapper<SunRenderer> SunRenderer { set { m_sunRenderer = value; } get { return m_sunRenderer; } }

        public MistComponent Mist { set { m_mistComponent = value; } get { return m_mistComponent; } }

        public PlayerController PlayerController { set { m_playerController = value; } get { return m_playerController; } }

#if DESIGN_EDITOR
        public MousePicker Picker { set; get; }
#endif

        // TODO -> not yet ready for serialization
        public PlantReadyMaster Grass { set { m_grass = value; } get { return m_grass; } }

        public PlantReadyMaster Plant { set { m_plant = value; } get { return m_plant; } }

        #endregion

        #region Constructor

        public Level(Vector2 levelSize, string levelName)
        {
            LevelSize = levelSize;
            m_levelName = levelName;
            StaticMeshCollection = new ObserverListWrapper<Building>();
            Player = new ObserverWrapper<MovableMeshEntity>();
            Bots = new ObserverListWrapper<MovableMeshEntity>();
            SunRenderer = new ObserverWrapper<SunRenderer>();
            Water = new ObserverWrapper<WaterPlane>();
            Terrain = new ObserverWrapper<Landscape>();
            PointLightCollection = new ObserverListWrapper<PointLight>();
        }

        #endregion

        #region Serialization

        void IPostDeserializable.PostDeserializeInit()
        {
            var collisionHeadUnit = GameWorld.GetWorldInstance().CollisionHeadUnitObject;

            if (m_camera is ThirdPersonCamera)
            {
                ThirdPersonCamera camera = m_camera as ThirdPersonCamera;
                camera.SetThirdPersonTarget(m_player.GetData());
                m_playerController = new PlayerController(camera);
            }
            m_camera.SetCollisionHeadUnit(collisionHeadUnit);

            if (m_directionalLight is DirectionalLightWithShadow)
            {
                (m_directionalLight as DirectionalLightWithShadow).PostDeserializePass(m_camera);
            }

            m_dayLightCycle.PostDeserializePass(m_directionalLight);

            m_terrain.GetData()?.PostDeserializePass(m_mistComponent);
            m_terrain.PostDeserializeInit();

            foreach (var building in m_staticMeshCollection)
            {
                building.PostDeserializePass(m_mistComponent, collisionHeadUnit);
            }
            m_staticMeshCollection.PostDeserializeInit();

            m_player.GetData().PostDeserializePass(m_mistComponent, collisionHeadUnit);
            m_player.PostDeserializeInit();

            foreach (var bot in m_bots)
            {
                bot.PostDeserializePass(m_mistComponent, collisionHeadUnit);
                bot.PostDeserializeInit();
            }

            m_skybox?.PostDeserializePass(m_mistComponent);

            m_waterPlane.GetData()?.PostDeserializePass(m_mistComponent);
            m_waterPlane.PostDeserializeInit();

            m_sunRenderer.GetData()?.PostDeserializePass(m_directionalLight);
            m_sunRenderer.PostDeserializeInit();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_levelSize", m_levelSize);
            info.AddValue("m_levelName", m_levelName);
            info.AddValue("m_pointLightCollection", m_pointLightCollection, typeof(ObserverListWrapper<PointLight>));
            info.AddValue("m_directionalLight", m_directionalLight, typeof(DirectionalLight));
            info.AddValue("m_camera", m_camera, typeof(BaseCamera));
            info.AddValue("m_dayLightCycle", m_dayLightCycle, typeof(DayLightCycle));
            info.AddValue("m_mistComponent", m_mistComponent, typeof(MistComponent));
            info.AddValue("m_terrain", m_terrain, typeof(ObserverWrapper<Landscape>));
            info.AddValue("m_staticMeshCollection", m_staticMeshCollection, typeof(ObserverListWrapper<Building>));
            info.AddValue("m_player", m_player, typeof(ObserverWrapper<MovableMeshEntity>));
            info.AddValue("m_bots", m_bots, typeof(ObserverListWrapper<MovableMeshEntity>));
            info.AddValue("m_skybox", m_skybox, typeof(SkyboxEntity));
            info.AddValue("m_waterPlane", m_waterPlane, typeof(ObserverWrapper<WaterPlane>));
            info.AddValue("m_sunRenderer", m_sunRenderer, typeof(ObserverWrapper<SunRenderer>));
        }

        protected Level(SerializationInfo info, StreamingContext context)
        {
            m_levelSize = (Vector2)info.GetValue("m_levelSize", typeof(Vector2));
            m_levelName = info.GetString("m_levelName");
            m_pointLightCollection = info.GetValue("m_pointLightCollection", typeof(ObserverListWrapper<PointLight>)) as ObserverListWrapper<PointLight>;
            m_directionalLight = info.GetValue("m_directionalLight", typeof(DirectionalLight)) as DirectionalLight;
            m_camera = info.GetValue("m_camera", typeof(BaseCamera)) as BaseCamera;
            m_dayLightCycle = info.GetValue ("m_dayLightCycle", typeof(DayLightCycle)) as DayLightCycle;
            m_mistComponent = info.GetValue("m_mistComponent", typeof(MistComponent)) as MistComponent;
            m_terrain = info.GetValue("m_terrain", typeof(ObserverWrapper<Landscape>)) as ObserverWrapper<Landscape>;
            m_staticMeshCollection = info.GetValue("m_staticMeshCollection", typeof(ObserverListWrapper<Building>)) as ObserverListWrapper<Building>;
            m_player = info.GetValue("m_player", typeof(ObserverWrapper<MovableMeshEntity>)) as ObserverWrapper<MovableMeshEntity>;
            m_bots = info.GetValue("m_bots", typeof(ObserverListWrapper<MovableMeshEntity>)) as ObserverListWrapper<MovableMeshEntity>;
            m_skybox = info.GetValue("m_skybox", typeof(SkyboxEntity)) as SkyboxEntity;
            m_waterPlane = info.GetValue("m_waterPlane", typeof(ObserverWrapper<WaterPlane>)) as ObserverWrapper<WaterPlane>;
            m_sunRenderer= info.GetValue("m_sunRenderer", typeof(ObserverWrapper<SunRenderer>)) as ObserverWrapper<SunRenderer>;
        }

        #endregion
    }
}
