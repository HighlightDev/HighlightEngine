using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MassiveGame.Core.SettingsCore;

namespace MassiveGame
{
    public static class EngineStatics
    {
        public static PrimitiveType Mode = PrimitiveType.Triangles;

        public const float FoV = 60.0f;
        public const float MAP_HEIGHT = 30;
        public const float SCREEN_ASPECT_RATIO = 16f / 9f;
        public const Int32 MAX_LIGHT_COUNT = 5;
        public const float NEAR_CLIPPING_PLANE = 0.1f;
        public const float FAR_CLIPPING_PLANE = 900;

        public static Point ScreenLocation = new Point(120, 50);
        public static WindowBorder WINDOW_BORDER = WindowBorder.Resizable;
        public static WindowState WINDOW_STATE = WindowState.Normal;
        public static float RENDER_TIME = 0;

        public static DateTime ElapsedTime;
        public static Matrix4 ProjectionMatrix;
        
        public static Point PrevCursorPosition;
        public static GlobalSettings globalSettings = new GlobalSettings();
    }
}
