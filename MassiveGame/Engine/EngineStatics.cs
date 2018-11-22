using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MassiveGame.Settings;

namespace MassiveGame
{
    public static class EngineStatics
    {
        public static PrimitiveType Mode = PrimitiveType.Triangles;

        public const float FoV = 60.0f;
        //public const float LevelSize = 500;
        public const float MAP_HEIGHT = 30;
        public const float SCREEN_ASPECT_RATIO = 16f / 9f;
        public const Int32 MAX_LIGHT_COUNT = 5;
        public const float NEAR_CLIPPING_PLANE = 0.1f;
        public const float FAR_CLIPPING_PLANE = 900;

        public static Int32 SCREEN_POSITION_X = 120;
        public static Int32 SCREEN_POSITION_Y = 50;
        public static WindowBorder WINDOW_BORDER = WindowBorder.Resizable;
        public static WindowState WINDOW_STATE = WindowState.Normal;
        public static float RENDER_TIME = 0;

        // ///*Temp triggers*/// //
        public static bool BorderTrigger = true;     //temp for border setup
        public static bool StateTrigger = true;     //temp for window state setup
        public static bool AltTrigger = false;
        public static bool ControlLeftTrigger = false;
        // ///*Temp triggers*/// //

        public static DateTime ElapsedTime;
        public static Matrix4 ProjectionMatrix;

        //public static List<IVisible> RenderableCollection;
        //public static List<ILightHit> LitByLightCollection;
        //public static List<IDrawable> AffectedByShadowCollection;
        //public static List<PointLight> PointLight;
        //public static DirectionalLight Sun;
        //public static DayLightCycle DayCycle;
        //public static Landscape terrain;
        //public static MovableMeshEntity Player;
        //public static ObserverListWrapper<MovableMeshEntity> Bots;
        //public static ObserverListWrapper<Building> City;
        //public static BaseCamera Camera;
        //public static PlantBuilderMaster builder;
        //public static PlantReadyMaster Grass;
        //public static PlantReadyMaster Plant1;
        //public static SkyboxEntity Skybox;
        //public static MousePicker Picker;
        //public static WaterPlane Water;
        //public static SunRenderer SunReplica;
        //public static MistComponent Mist;
        //public static PointLightsDebugRenderer pointLightDebugRenderer;
        public static Point PrevCursorPosition;

        //public static PlayerController playerController;

        //public static UiFrameMaster uiFrameCreator;
        

        public static GlobalSettings globalSettings = new GlobalSettings();

        //public static MovableSkeletalMeshEntity SkeletalMesh = null;
    }
}
