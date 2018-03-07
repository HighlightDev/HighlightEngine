using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UIControls
{
    public static class ProjectFolders
    {
        private const string FOLDER_NAME = "MassiveGame";

        public static string getFolderPath()
        {
            string fileFolder = Environment.CurrentDirectory;
            //fileFolder = fileFolder.Remove(fileFolder.IndexOf(FOLDER_NAME)); // trouble with indexing, need to try without index
            return @"D:\Projects\C# Programs\D.O.E";
        }

        private static string _texturesPath = "/MassiveGame/MassiveGame/Textures/";
        private static string _normalMapsPath = "/MassiveGame/MassiveGame/Textures/NormalMaps/";
        private static string _specularMapsPath = "/MassiveGame/MassiveGame/Textures/SpecularMaps/";
        private static string _textureAtlasPath = "/MassiveGame/MassiveGame/Textures/TextureAtlas/";

        public static string TexturesPath { get { return @"" + getFolderPath() + _texturesPath; } }
        public static string NormalMapsPath { get { return @"" + getFolderPath() + _normalMapsPath; } }
        public static string SpecularMapsPath { get { return @"" + getFolderPath() + _specularMapsPath; } }
        public static string TextureAtlasPath { get { return @"" + getFolderPath() + _textureAtlasPath; } }
    }
}
