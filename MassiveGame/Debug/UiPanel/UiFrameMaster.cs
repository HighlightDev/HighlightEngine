using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.Debug.UiPanel
{
    public static class UiFrameMaster
    {
        public static bool bEnabledDebug = false;

        private static UiFrame DebugFrame;
        private static ITexture CurrentDebugTexture;

        static UiFrameMaster()
        {
            DebugFrame = null;
            CurrentDebugTexture = null;
        }

        public static void CreateUiFrame(float posX, float posY, float width, float height)
        {
            // TODO : add condition for already existing ui frame with same properties
            DebugFrame = new UiFrame(posX, posY, width, height);
            CheckAvailability();
        }

        public static void RemoveUiFrame()
        {
            DebugFrame = null;
            CheckAvailability();
        }
        
        public static void SetForDebugTexture(Int32 texHandler, Int32 texWidth, Int32 texHeight)
        {
            CurrentDebugTexture = new Texture2Dlite(texHandler, new RectParams(texWidth, texHeight));
            CheckAvailability();
        }

        private static void CheckAvailability()
        {
            if (DebugFrame != null && CurrentDebugTexture != null)
                bEnabledDebug = true;
            else bEnabledDebug = false;
        }

        public static void ShowDebugFrame()
        {
            DebugFrame.Render(CurrentDebugTexture);
        }

    }
}
