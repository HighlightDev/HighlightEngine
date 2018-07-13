using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.Sun.DayCycle;
using MassiveGame.Core.GameCore.Terrain;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Skybox;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.GameCore.Sun;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore.Light_visualization;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.RenderCore;

using MassiveGame.API.MouseObjectDetector;
using MassiveGame.Debug.UiPanel;
using MassiveGame.Settings;


namespace MassiveGame
{
    public static class EngineStatics
    {
        public static PrimitiveType Mode = PrimitiveType.Triangles;
       
        public const float FoV = 60.0f;
        public const float MAP_SIZE = 500;
        public const float MAP_HEIGHT = 30;
        public const float DAY_TIMER_HASTE = 10;
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
        public static bool NormalMapTrigger = true;   //temp for normal mapping
        public static bool BorderTrigger = true;     //temp for border setup
        public static bool StateTrigger = true;     //temp for window state setup
        public static bool AltTrigger = false;
        public static bool ControlLeftTrigger = false;
        // ///*Temp triggers*/// //

        public static DateTime ElapsedTime;
        public static Matrix4 ProjectionMatrix;


        public static List<PointLight> PointLight;
        public static DirectionalLight Sun;
        public static DayLightCycle DayCycle;
        public static  Landscape terrain;
        public static  MovableMeshEntity Player;
        public static  MovableMeshEntity Enemy;
        public static  List<Building> City;
        public static  BaseCamera Camera;
        public static  PlantBuilderMaster builder;
        public static  PlantReadyMaster Grass;
        public static  PlantReadyMaster Plant1;
        public static  Skybox Skybox;
        public static  MousePicker Picker;
        public static  WaterPlane Water;
        public static  SunRenderer SunReplica;
        public static  MistComponent Mist;
        public static  PointLightsDebugRenderer pointLightDebugRenderer;
        public static  List<IVisible> RenderableMeshCollection;
        public static  List<ILightHit> LitByLightSourcesMeshCollection;
        public static  Point PrevCursorPosition;

        public static PlayerController playerController;
       
        public static UiFrameMaster uiFrameCreator;
        public static List<IDrawable> shadowList;

        public static GlobalSettings globalSettings = new GlobalSettings();
    }
}
