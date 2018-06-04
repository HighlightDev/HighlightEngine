using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;
using PhysicsBox;
using MassiveGame.Optimization;
using AudioEngine;
using MassiveGame.Sun.DayCycle;
using MassiveGame.UI;
using System.Drawing;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.Light_visualization;
using System.Threading;
using MassiveGame.Debug.UiPanel;

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

        public static int SCREEN_POSITION_X = 120;
        public static int SCREEN_POSITION_Y = 50;
        public static int SCREEN_WIDTH = 1020;
        public static int SCREEN_HEIGHT = 800;
        public static int TIMER_CALLBACK = 0;
        public static WindowBorder WINDOW_BORDER = WindowBorder.Resizable;
        public static WindowState WINDOW_STATE = WindowState.Normal;
        public static int MAX_LIGHT_COUNT = 5;  //Максимальное количество источников света присутствующих на сцене
        public static float RenderTime = 0;

        // ///*Temp triggers*/// //
        public static bool ShowLightSource = false;       //temp for rendering light sources
        public static bool CollisionBoxRender = false;    //temp for rendering collision boxes
        public static bool NormalMapTrigger = true;   //temp for normal mapping
        public static bool BorderTrigger = true;     //temp for border setup
        public static bool StateTrigger = true;     //temp for window state setup
        public static bool FirstPersonCameraTrigger = false;     //temp for camera setup
        public static bool AltTrigger = false;
        public static bool ControlLeftTrigger = false;
        public static bool RedrawScene = false;
        // ///*Temp triggers*/// //

        public static DateTime ElapsedTime;
        public static Matrix4 ProjectionMatrix;
        public static bool PostConstructor;
        public static bool PostConstructorExecuted = false;
        public static List<PointLight> PointLight;
        public static DirectionalLight Sun;
        public static DayLightCycle DayCycle;

        public static  CollisionDetector Collision;
        public static  Terrain terrain;
        public static  Player Player;
        public static  Player Enemy;
        public static  List<Building> Buildings;
        public static  List<Building> City;
        public static  Camera Camera;
        public static PlantBuilderMaster builder;
        public static  PlantReadyMaster Grass;
        public static  PlantReadyMaster Plant1;
        public static  Skybox Skybox;
        public static  MousePicker Picker;
        public static  PostprocessRenderer PostProc;
        public static  WaterEntity Water;
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

        public static  int[] SB_step;
        public static  int[] SB_collide;
        public static  int SB_ambient;
        public static  Source SourceAmbient;

        public static bool[] keyboardMaskArray;
        public static Timer EngineTickTimer;

        public static Point ScreenRezolution;

        public static Point ShadowMapRezolution;
        public static UiFrameMaster uiFrameCreator;
    }
}
