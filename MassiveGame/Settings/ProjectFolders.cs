using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MassiveGame
{
    static public class ProjectFolders
    {
        private const string FOLDER_NAME = "bin";

        /// <summary>
        /// Retrieves folder path to system files
        /// </summary>
        /// <returns> Folder path </returns>
        public static string getFolderPath()
        {
            string FolderPath = "";
            string fileFolder = System.Environment.CurrentDirectory;
            Regex folderEx = new Regex(FOLDER_NAME);
            if (folderEx.IsMatch(fileFolder))
            {
                int index = folderEx.Match(fileFolder).Index;
                FolderPath = fileFolder.Substring(0, index);
            }
            return FolderPath;
        }

        
        private static string _texturesPath = "/Textures/";
        private static string _shadersPath = "/Shaders/";
        private static string _grassTexturesPath = "/Textures/GrassTextures/";
        private static string _heightMapsTexturesPath = "/Textures/HeightMaps/";
        private static string _multitexturesPath = "/Textures/Multitextures/";
        private static string _simpleTexturesPath = "/Textures/SimpleTextures/";
        private static string _skyboxTexturesPath = "/Textures/Skybox/";
        private static string _modelsPath = "/ModelFiles/";
        private static string _texturesDataFilePath = "/ModelFiles/";
        private static string _normalMapsPath = "/Textures/NormalMaps/";
        private static string _specularMapsPath = "/Textures/SpecularMaps/";
        private static string _textureAtlasPath = "/Textures/TextureAtlas/";
        private static string _waterTexturePath = "/Textures/WaterTextures/";
        private static string _sunTexturePath = "/Textures/SunTextures/";
        private static string _lensFlareTexturePath = "/Textures/LensFlareTextures/";


        public static string TexturesPath { get { return @"" + getFolderPath() + _texturesPath; } }
        public static string ShadersPath { get { return @"" + getFolderPath() + _shadersPath; } }
        public static string GrassTexturesPath { get { return @"" + getFolderPath() + _grassTexturesPath; } }
        public static string HeightMapsTexturesPath { get { return @"" + getFolderPath() + _heightMapsTexturesPath; } }
        public static string MultitexturesPath { get { return @"" + getFolderPath() + _multitexturesPath; } }
        public static string SimpleTexturesPath { get { return @"" + getFolderPath() + _simpleTexturesPath; } }
        public static string SkyboxTexturesPath { get { return @"" + getFolderPath() + _skyboxTexturesPath; } }
        public static string ModelsPath { get { return @"" + getFolderPath() + _modelsPath; } }
        public static string TexturesDataFilePath { get { return @"" + getFolderPath() + _texturesDataFilePath; } }
        public static string NormalMapsPath { get { return @"" + getFolderPath() + _normalMapsPath; } }
        public static string SpecularMapsPath { get { return @"" + getFolderPath() + _specularMapsPath; } }
        public static string TextureAtlasPath { get { return @"" + getFolderPath() + _textureAtlasPath; } }
        public static string WaterTexturePath { get { return @"" + getFolderPath() + _waterTexturePath; } }
        public static string SunTexturePath { get { return @"" + getFolderPath() + _sunTexturePath; } }
        public static string LensFlareTexturePath { get { return @"" + getFolderPath() + _lensFlareTexturePath; } }

        static public class AudioFolders
        {
            private static string _audioPath = "/Audio/";
            private static string _audioMaterialPath = "/Audio/material/";
            private static string _audioActorPath = "/Audio/material/actor/";
            private static string _audioActorStepPath = "/Audio/material/actor/step/";
            private static string _audioActorCollidePath = "/Audio/material/actor/collide/";
            private static string _audioAmbientPath = "/Audio/ambient/";


            public static string AudioPath { get { return @"" + getFolderPath() + _audioPath; } }
            public static string AudioMaterialPath { get { return @"" + getFolderPath() + _audioMaterialPath; } }
            public static string AudioActorPath { get { return @"" + getFolderPath() + _audioActorPath; } }
            public static string AudioActorStepPath { get { return @"" + getFolderPath() + _audioActorStepPath; } }
            public static string AudioActorCollidePath { get { return @"" + getFolderPath() + _audioActorCollidePath; } }
            public static string AudioAmbientPath { get { return @"" + getFolderPath() + _audioAmbientPath; } }
        }

    }
}