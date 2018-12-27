using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.ResourceLoader
{
    public class ResourceIO
    {
        private static ResourceIO m_resourceIO = null;
        private string m_resourceAbsolutePath = string.Empty;

        private ResourceIO() { }

        public static ResourceIO GetInstance()
        {
            if (m_resourceIO == null)
                m_resourceIO = new ResourceIO();

            return m_resourceIO;
        }

        public string GetResPath()
        {
            if (string.IsNullOrEmpty(m_resourceAbsolutePath))
            {
                string currentDir = Environment.CurrentDirectory;
                string pathToRes = currentDir.Substring(0, currentDir.IndexOf("HighlightEngine") + "HighlightEngine".Length) + "\\MassiveGame\\res\\";
                m_resourceAbsolutePath = pathToRes;
            }

            return m_resourceAbsolutePath;
        }

        public string GetTexturePath()
        {
            return GetResPath() + "texture\\";
        }
    }
}
