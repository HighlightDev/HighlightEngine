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

        public Point GetDirectionalShadowMapRezolution()
        {
            try
            {
                if (IniContent == null)
                    throw new FileLoadException("Could not load ini file");

                Int32 indexOfScreenWidth = IniContent.IndexOf("directional_shadow_map_width = ");
                Int32 indexOfScreenHeight = IniContent.IndexOf("directional_shadow_map_height = ");
                if (indexOfScreenWidth == -1 && indexOfScreenHeight == -1)
                    throw new ArgumentException();

                Int32 lengthWidth = IniContent.IndexOf(";", indexOfScreenWidth) + 1;
                string widthString = IniContent.Substring(indexOfScreenWidth, lengthWidth - indexOfScreenWidth);
                Int32 indexOfWidth = widthString.IndexOf("= ") + 1;
                string width = widthString.Substring(indexOfWidth, widthString.IndexOf(";") - indexOfWidth);
                Int32 widthValue = Int32.Parse(width);

                Int32 lengthHeight = IniContent.IndexOf(";", indexOfScreenHeight) + 1;
                string heightString = IniContent.Substring(indexOfScreenHeight, lengthHeight - indexOfScreenHeight);
                Int32 indexOfHeight = heightString.IndexOf("= ") + 1;
                string height = heightString.Substring(indexOfHeight, heightString.IndexOf(";") - indexOfHeight);
                Int32 heightValue = Int32.Parse(height);

                return new Point(widthValue, heightValue);
            }
            catch (FileLoadException ex)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                throw;
            }
        }

        public Point GetScreenRezolution()
        {
            try
            {
                if (IniContent == null)
                    throw new FileLoadException("Could not load ini file");

                Int32 indexOfScreenWidth = IniContent.IndexOf("screenwidth = ");
                Int32 indexOfScreenHeight = IniContent.IndexOf("screenheight = ");
                if (indexOfScreenWidth == -1 && indexOfScreenHeight == -1)
                    throw new ArgumentException();

                Int32 lengthWidth = IniContent.IndexOf(";", indexOfScreenWidth) + 1;
                string widthString = IniContent.Substring(indexOfScreenWidth, lengthWidth - indexOfScreenWidth);
                Int32 indexOfWidth = widthString.IndexOf("= ") + 1;
                string width = widthString.Substring(indexOfWidth, widthString.IndexOf(";") - indexOfWidth);
                Int32 widthValue = Int32.Parse(width);

                Int32 lengthHeight = IniContent.IndexOf(";", indexOfScreenHeight) + 1;
                string heightString = IniContent.Substring(indexOfScreenHeight, lengthHeight - indexOfScreenHeight);
                Int32 indexOfHeight = heightString.IndexOf("= ") + 1;
                string height = heightString.Substring(indexOfHeight, heightString.IndexOf(";") - indexOfHeight);
                Int32 heightValue = Int32.Parse(height);

                return new Point(widthValue, heightValue);
            }
            catch(FileLoadException ex)
            {
                throw;
            }
            catch(ArgumentException ex)
            {
                throw;
            }
        }
    }
}
