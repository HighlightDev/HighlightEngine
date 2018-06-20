using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;

namespace MassiveGame.Settings
{
    public class SettingsLoader
    {
        string IniContent = null;

        public SettingsLoader()
        {
            string pathToIni = ProjectFolders.getFolderPath() + "/Settings/settings.ini";
            using (StreamReader reader = new StreamReader(pathToIni))
                IniContent = reader.ReadToEnd();
        }

        #region core

        public Object ReadProperty(IPropertyConverter converter, string propertyName)
        {
            try
            {
                if (IniContent == null)
                    throw new FileLoadException("Could not load ini file");

                Int32 indexOfProperty = IniContent.IndexOf(propertyName + " = ");

                if (indexOfProperty == -1)
                    throw new ArgumentException();


                Int32 propertyLength = IniContent.IndexOf(";", indexOfProperty) + 1;
                string propertyString = IniContent.Substring(indexOfProperty, propertyLength - indexOfProperty);
                indexOfProperty = propertyString.IndexOf("= ") + 1;
                string property = propertyString.Substring(indexOfProperty, propertyString.IndexOf(";") - indexOfProperty);
                return converter.Convert(property);
            }
            catch (FileLoadException ex)
            {
                Debug.Log.addToLog(ex.Message);
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                Debug.Log.addToLog(ex.Message);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

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
    }
}
