using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace MassiveGame.Core.SettingsCore
{
    static public class ProjectFolders
    {
        private const string BINARY_FOLDER_NAME = "bin";

        private static string m_rootFolder = String.Empty;

        public static string GetRootFolder()
        {
            if (string.IsNullOrEmpty(m_rootFolder))
                BuildRootFolder();

            return m_rootFolder;
        }

        private static string ConcatDirectoryBack(Int32 countChangeDirectoryBack)
        {
            string resultStr = string.Empty;
            while (countChangeDirectoryBack-- > 0)
            {
                resultStr += "..\\";
            }
            return resultStr;
        }

        private static void BuildRootFolder()
        {
            string currentDirPath = System.Environment.CurrentDirectory;
            Regex folderEx = new Regex(BINARY_FOLDER_NAME);
            if (folderEx.IsMatch(currentDirPath))
            {
                Int32 indexOfRootEntrance = folderEx.Match(currentDirPath).Index - 1;
                string pathFromRootToCurrentDir = currentDirPath.Substring(indexOfRootEntrance);
                Int32 countChangeDirectoryBack = pathFromRootToCurrentDir.Count<char>(separator => separator == '\\');
                m_rootFolder = ConcatDirectoryBack(countChangeDirectoryBack);
            }
        }

        private static string resPath = "res\\";
        private static string modelsPath = resPath + "model\\";
        private static string shadersPath = resPath + "shaders\\";
        private static string collisionPath = resPath + "collision\\";
        private static string texturesPath = resPath + "texture\\";
        private static string iniPath = resPath + "ini\\";

        private static string grassTexPath = texturesPath + "grass\\";
        private static string heightmapsPath = texturesPath + "heightmaps\\";
        private static string landscapeTexPath = texturesPath + "landscape\\";
        private static string cubemapTexPath = texturesPath + "cubemap\\";
        private static string normalmapsPath = texturesPath + "normalmap\\";
        private static string specularmapsPath = texturesPath + "specularmap\\";
        private static string albedoTexPath = texturesPath + "albedo\\";
        private static string distortionTexPath = texturesPath + "distortion\\";
        private static string postprocessTexPath = texturesPath + "postprocess\\";
        private static string editorTexturePath = texturesPath + "editor\\";

        public static string ResPath { get { return GetRootFolder() + resPath; } }
        public static string ModelsPath { get { return GetRootFolder() + modelsPath; } }
        public static string ShadersPath { get { return GetRootFolder() + shadersPath; } }
        public static string CollisionPath { get { return GetRootFolder() + collisionPath; } }
        public static string TexturesPath { get { return GetRootFolder() + texturesPath; } }
        public static string IniPath { get { return GetRootFolder() + iniPath; } }
        
        public static string GrassTexturePath { get { return GetRootFolder() + grassTexPath; } }
        public static string HeightMapsTexturePath { get { return GetRootFolder() + heightmapsPath; } }
        public static string LandscapeTexturePath { get { return GetRootFolder() + landscapeTexPath; } }
        public static string CubemapTexturePath { get { return GetRootFolder() + cubemapTexPath; } }
        public static string NormalMapPath { get { return GetRootFolder() + normalmapsPath; } }
        public static string SpecularMapPath { get { return GetRootFolder() + specularmapsPath; } }
        public static string AlbedoTexturePath { get { return GetRootFolder() + albedoTexPath; } }
        public static string DistortionTexturePath { get { return GetRootFolder() + distortionTexPath; } }
        public static string PostprocessTexturePath { get { return GetRootFolder() + postprocessTexPath; } }
        public static string EditorTexturePath { get { return GetRootFolder() + editorTexturePath; } }
    }
}