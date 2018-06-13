using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;
using MassiveGame.Optimization;
using MassiveGame.Sun.DayCycle;
using System.Drawing;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.Light_visualization;
using MassiveGame.Debug.UiPanel;
using MassiveGame.API.EventHandlers;
using MassiveGame.Settings;
using MassiveGame.RenderCore;

namespace MassiveGame
{
    public static class DOUEngine
    {
        /*  Модель примитива */
        public static PrimitiveType Mode = PrimitiveType.Triangles;

        public static float NEAR_CLIPPING_PLANE = 0.1f;
        public static float FAR_CLIPPING_PLANE = 1000;
        public const float FoV = 60.0f;
        public const float MAP_SIZE = 500;
        public const float MAP_HEIGHT = 30;
        public const float DAY_TIMER_HASTE = 10;
        public const float SCREEN_ASPECT_RATIO = 16f / 9f;

        public static Int32 SCREEN_POSITION_X = 120;
        public static Int32 SCREEN_POSITION_Y = 50;
        public static Int32 SCREEN_WIDTH = 1020;
        public static Int32 SCREEN_HEIGHT = 800;
        public static Int32 TIMER_CALLBACK = 0;
        public static WindowBorder WINDOW_BORDER = WindowBorder.Resizable;
        public static WindowState WINDOW_STATE = WindowState.Normal;
        public static Int32 MAX_LIGHT_COUNT = 5;  //Максимальное количество источников света присутствующих на сцене
        public static float RenderTime = 0;

        // ///*Temp triggers*/// //
        public static bool ShowLightSource = false;       //temp for rendering light sources
        public static bool CollisionBoxRender = false;    //temp for rendering collision boxes
        public static bool NormalMapTrigger = true;   //temp for normal mapping
        public static bool BorderTrigger = true;     //temp for border setup
        public static bool StateTrigger = true;     //temp for window state setup
        public static bool AltTrigger = false;
        public static bool ControlLeftTrigger = false;
        public static bool RedrawScene = false;
        // ///*Temp triggers*/// //

        public static DateTime ElapsedTime;
        public static Matrix4 ProjectionMatrix;
        public static bool PostConstructorExecuted = false;
        public static List<PointLight> PointLight;
        public static DirectionalLight Sun;
        public static DayLightCycle DayCycle;

        public static  Terrain terrain;
        public static  Player Player;
        public static  Player Enemy;
        public static  List<Building> Buildings;
        public static  List<Building> City;
        public static  Camera Camera;
        public static  PlantBuilderMaster builder;
        public static  PlantReadyMaster Grass;
        public static  PlantReadyMaster Plant1;
        public static  Skybox Skybox;
        public static  MousePicker Picker;
        public static  PostprocessRenderer PostProc;
        public static  WaterPlane Water;
        public static  SunRenderer SunReplica;
        public static  LensFlareRenderer Lens;
        public static  GodRaysRenderer Ray;
        public static  MistComponent Mist;
        public static  VisualizeLight Lights;
        public static  List<IVisible> RenderedPrimitives;
        public static  List<ILightAffection> AffectedByLightPrimitives;
        public static  EnvironmentEntities EnvObj;
        public static  PostProcessFlag Settings;
        public static  Point PrevCursorPosition;

        public static KeyboardHandler keyboardMask;

        public static Point ScreenRezolution;

        public static Point ShadowMapRezolution;
        public static UiFrameMaster uiFrameCreator;
        public static List<IDrawable> shadowList;
    }
}
