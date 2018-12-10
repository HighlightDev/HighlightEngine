using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace MassiveGame.Settings
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

        private static string _texturesPath = "Textures\\";
        private static string _shadersPath = "Shaders\\";
        private static string _grassTexturesPath = "Textures\\GrassTextures\\";
        private static string _heightMapsTexturesPath = "Textures\\HeightMaps\\";
        private static string _multitexturesPath = "Textures\\Multitextures\\";
        private static string _simpleTexturesPath = "Textures\\SimpleTextures\\";
        private static string _skyboxTexturesPath = "Textures\\Skybox\\";
        private static string _modelsPath = "ModelFiles\\";
        private static string _normalMapsPath = "Textures\\NormalMaps\\";
        private static string _specularMapsPath = "Textures\\SpecularMaps\\";
        private static string _textureAtlasPath = "Textures\\TextureAtlas\\";
        private static string _waterTexturePath = "Textures\\WaterTextures\\";
        private static string _sunTexturePath = "Textures\\SunTextures\\";
        private static string _lensFlareTexturePath = "Textures\\LensFlareTextures\\";
        private static string _editorTexturePath = "Textures\\EditorTextures\\";

        public static string TexturesPath { get { return GetRootFolder() + _texturesPath; } }
        public static string ShadersPath { get { return GetRootFolder() + _shadersPath; } }
        public static string GrassTexturesPath { get { return GetRootFolder() + _grassTexturesPath; } }
        public static string HeightMapsTexturesPath { get { return GetRootFolder() + _heightMapsTexturesPath; } }
        public static string MultitexturesPath { get { return GetRootFolder() + _multitexturesPath; } }
        public static string SimpleTexturesPath { get { return GetRootFolder() + _simpleTexturesPath; } }
        public static string SkyboxTexturesPath { get { return GetRootFolder() + _skyboxTexturesPath; } }
        public static string ModelsPath { get { return GetRootFolder() + _modelsPath; } }
        public static string NormalMapsPath { get { return GetRootFolder() + _normalMapsPath; } }
        public static string SpecularMapsPath { get { return GetRootFolder() + _specularMapsPath; } }
        public static string TextureAtlasPath { get { return GetRootFolder() + _textureAtlasPath; } }
        public static string WaterTexturePath { get { return GetRootFolder() + _waterTexturePath; } }
        public static string SunTexturePath { get { return GetRootFolder() + _sunTexturePath; } }
        public static string LensFlareTexturePath { get { return GetRootFolder() + _lensFlareTexturePath; } }
        public static string EditorTexturePath { get { return GetRootFolder() + _editorTexturePath; } }
    }
}