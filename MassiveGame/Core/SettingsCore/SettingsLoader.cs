using System;
using System.IO;
using System.Drawing;
using MassiveGame.Core.DebugCore;

namespace MassiveGame.Core.SettingsCore
{
    public class SettingsLoader
    {
        private string IniContent = null;

        public SettingsLoader()
        {
            string pathToIni = ProjectFolders.IniPath + "settings.ini";

            try
            {
                using (StreamReader reader = new StreamReader(pathToIni))
                    IniContent = reader.ReadToEnd();
            }
            catch (FileLoadException ex)
            {
                Log.AddToFileStreamLog(ex.Message);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #region core

        public Object ReadProperty(IPropertyConverter converter, string propertyName)
        {
            try
            {
                Int32 indexOfProperty = IniContent.IndexOf(propertyName + " = ");

                if (indexOfProperty == -1)
                    throw new ArgumentException();

                Int32 propertyLength = IniContent.IndexOf(";", indexOfProperty) + 1;
                string propertyString = IniContent.Substring(indexOfProperty, propertyLength - indexOfProperty);
                indexOfProperty = propertyString.IndexOf("= ") + 1;
                string property = propertyString.Substring(indexOfProperty, propertyString.IndexOf(";") - indexOfProperty);
                return converter.Convert(property);
            }
            catch (ArgumentException ex)
            {
                Log.AddToFileStreamLog(ex.Message);
                Log.AddToConsoleStreamLog(ex.Message);
                throw;
            }
        }

        #endregion

        public void SetGlobalSettings()
        {
            EngineStatics.globalSettings.DomainFramebufferRezolution = GetScreenRezolution();
            EngineStatics.globalSettings.ShadowMapRezolution = GetDirectionalShadowMapRezolution();
            EngineStatics.globalSettings.bSupported_DepthOfField = GetIsDepthOfFieldSupported();
            EngineStatics.globalSettings.bSupported_Bloom = GetIsBloomSupported();
            EngineStatics.globalSettings.bSupported_LightShafts = GetIsLightShaftsSupported();
            EngineStatics.globalSettings.bSupported_LensFlare = GetIsLensFlaresSupported();
            EngineStatics.globalSettings.bSupported_MipMap = GetIsMipMapSupported();
            EngineStatics.globalSettings.AnisotropicFilterValue = GetAnisotropicFilteringValue();
            EngineStatics.globalSettings.bSeparatedScreen = GetIsSeparatedScreen();

            CleanIniProperty();
        }

        public Point GetDirectionalShadowMapRezolution()
        {
            IPropertyConverter converterToInt32 = new PropertyToInt32();
            var shadowMapWidth = (Int32)ReadProperty(converterToInt32, "directional_shadow_map_width");
            var shadowMapHeight = (Int32)ReadProperty(converterToInt32, "directional_shadow_map_height");
            return new Point(shadowMapWidth, shadowMapHeight);
        }

        public Point GetScreenRezolution()
        {
            IPropertyConverter converterToInt32 = new PropertyToInt32();
            var width = (Int32)ReadProperty(converterToInt32, "screenwidth");
            var height = (Int32)ReadProperty(converterToInt32, "screenheight");
            return new Point(width, height);
        }

        public bool GetIsBloomSupported()
        {
            IPropertyConverter convertToBool = new PropertyToBool();
            var bEnableBloom = (bool)ReadProperty(convertToBool, "bloom");
            return bEnableBloom;
        }

        public bool GetIsLightShaftsSupported()
        {
            IPropertyConverter convertToBool = new PropertyToBool();
            var bEnableLightShafts = (bool)ReadProperty(convertToBool, "lightshafts");
            return bEnableLightShafts;
        }

        public bool GetIsLensFlaresSupported()
        {
            IPropertyConverter convertToBool = new PropertyToBool();
            var bEnableLensFlare = (bool)ReadProperty(convertToBool, "lensflare");
            return bEnableLensFlare;
        }

        public bool GetIsDepthOfFieldSupported()
        {
            IPropertyConverter convertToBoll = new PropertyToBool();
            var bEnableDoF = (bool)ReadProperty(convertToBoll, "depthOfField");
            return bEnableDoF;
        }

        public bool GetIsMipMapSupported()
        {
            IPropertyConverter convertToBool = new PropertyToBool();
            var bEnableMipMap = (bool)ReadProperty(convertToBool, "mipmap");
            return bEnableMipMap;
        }

        public float GetAnisotropicFilteringValue()
        {
            IPropertyConverter convertToFloat = new PropertyToFloat();
            var anisotropicFilteringValue = (float)ReadProperty(convertToFloat, "anisotropic");
            return anisotropicFilteringValue;
        }

        public bool GetIsSeparatedScreen()
        {
            IPropertyConverter convertToBool = new PropertyToBool();
            var bEnableSeparatedScreen = (bool)ReadProperty(convertToBool, "separateScreen");
            return bEnableSeparatedScreen;
        }

        private void CleanIniProperty()
        {
            IniContent = null;
        }
    }
}
