using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UIControls
{
    public static class ListViewEditor
    {
        static ListViewEditor()
        {
            GroupCount = 0;
            TextureCount = 0;
            PathToTextures = String.Empty;
            GroupNames = new List<string>();
            TexturePaths = new List<string[]>();
            TextureNames = new List<string[]>();
            SmallIconList = new ImageList();
            LargeIconList = new ImageList();
            SmallIconSize = new Size(32, 32);
            LargeIconSize = new Size(96, 96);

            Groups = new List<ListViewGroup>();
        }

        #region Refresh and Connection Methods
        /// <summary>
        /// This method establishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="listView">Specific ListView Control to which connection will be established.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Connect(ListView listView, bool useSmallIconList = false)
        {
            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method establishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="pathToTextures">Path to folder which contains textures.</param>
        /// <param name="listView">Specific ListView Control to which connection will be established.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Connect(string pathToTextures, ListView listView, bool useSmallIconList = false)
        {
            PathToTextures = pathToTextures;

            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method establishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="listView">Specific ListView Control to which connection will be established.</param>
        /// <param name="smallIconSize">Size of small icons in ListView Control.</param>
        /// <param name="largeIconSize">Size of large icons in ListView Control.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Connect(ListView listView, Size smallIconSize, Size largeIconSize, bool useSmallIconList = false)
        {
            SmallIconSize = smallIconSize;
            LargeIconSize = largeIconSize;

            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method establishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="pathToTextures">Path to folder which contains textures.</param>
        /// <param name="listView">Specific ListView Control to which connection will be established.</param>
        /// <param name="smallIconSize">Size of small icons in ListView Control.</param>
        /// <param name="largeIconSize">Size of large icons in ListView Control.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Connect(string pathToTextures, ListView listView, Size smallIconSize, Size largeIconSize, bool useSmallIconList = false)
        {
            PathToTextures = pathToTextures;
            SmallIconSize = smallIconSize;
            LargeIconSize = largeIconSize;

            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method reestablishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="listView">Specific ListView Control to which connection will be reestablished.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Refresh(ListView listView, bool useSmallIconList = false)
        {
            DeleteDataFromListView(listView, useSmallIconList);
            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method reestablishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="pathToTextures">Path to folder which contains textures.</param>
        /// <param name="listView">Specific ListView Control to which connection will be reestablished.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Refresh(string pathToTextures, ListView listView, bool useSmallIconList = false)
        {
            DeleteDataFromListView(listView, useSmallIconList);

            PathToTextures = pathToTextures;

            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method reestablishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="listView">Specific ListView Control to which connection will be reestablished.</param>
        /// <param name="smallIconSize">Size of small icons in ListView Control.</param>
        /// <param name="largeIconSize">Size of large icons in ListView Control.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Refresh(ListView listView, Size smallIconSize, Size largeIconSize, bool useSmallIconList = false)
        {
            DeleteDataFromListView(listView, useSmallIconList);

            SmallIconSize = smallIconSize;
            LargeIconSize = largeIconSize;

            EstablishingConnection(listView, useSmallIconList);
        }

        /// <summary>
        /// This method reestablishing a connection between ListView Control and loaded data.
        /// </summary>
        /// <param name="pathToTextures">Path to folder which contains textures.</param>
        /// <param name="listView">Specific ListView Control to which connection will be reestablished.</param>
        /// <param name="smallIconSize">Size of small icons in ListView Control.</param>
        /// <param name="largeIconSize">Size of large icons in ListView Control.</param>
        /// <param name="useSmallIconList">If true ListView will be allowed to change from Large to Small (and vice versa) view of Icons, else - only Large Icon Size/</param>
        public static void Refresh(string pathToTextures, ListView listView, Size smallIconSize, Size largeIconSize, bool useSmallIconList = false)
        {
            DeleteDataFromListView(listView, useSmallIconList);

            PathToTextures = pathToTextures;
            SmallIconSize = smallIconSize;
            LargeIconSize = largeIconSize;

            EstablishingConnection(listView, useSmallIconList);
        }
        #endregion


        #region Variables
        public static Int32 GroupCount { get; private set; }
        public static Int32 TextureCount { get; private set; }
        public static string PathToTextures { get; set; }
        public static List<string> GroupNames { get; private set; }
        public static List<string[]> TexturePaths { get; private set; }
        public static List<string[]> TextureNames { get; private set; }
        public static ImageList SmallIconList { get; private set; }
        public static ImageList LargeIconList { get; private set; }
        public static Size SmallIconSize { get; set; }
        public static Size LargeIconSize { get; set; }

        private static List<ListViewGroup> Groups { get; set; }
        #endregion


        #region Main Methods
        // group names extracted from path to textures and overall quantity of groups
        private static void GroupNameCreation()
        {
            GroupNames.Add(
                GetGroupNameFromPath(PathToTextures, false, '/')
            );

            string[] subdirectories = Directory.GetDirectories(PathToTextures);
            for (Int32 i = 0; i < subdirectories.Length; ++i)
            {
                GroupNames.Add(
                     GetGroupNameFromPath(subdirectories[i], true, '/')
                );
            }

            GroupCount = subdirectories.Length + 1;
            subdirectories = null;
        }
        // group creation based on names from GroupNameCreation method
        private static void GroupCreation()
        {
            for (Int32 i = 0; i < GroupCount; ++i)
            {
                Groups.Add(
                    new ListViewGroup(GroupNames[i], HorizontalAlignment.Center)
                );
            }
        }
        // texture paths according to (sub)directories and overall quantity of textures
        private static void TexturePathCreation()
        {
            TexturePaths.Add(
                GetTexturesFromDirectory(PathToTextures)
            );
            TextureNames.Add(new string[TexturePaths[0].Length]);

            for (Int32 i = 1; i < GroupCount; ++i)
            {
                TexturePaths.Add(
                    GetTexturesFromDirectory(PathToTextures + GroupNames[i] + "\\")
                );
                TextureNames.Add(new string[TexturePaths[i].Length]);
            }

            foreach (string[] texturePath in TexturePaths)
            {
                TextureCount += texturePath.Length;
            }
        }
        // extraction texture names from texture paths
        private static void TextureNameCreation()
        {
            for (Int32 i = 0; i < TexturePaths.Count; ++i)
            {
                for (Int32 j = 0; j < TexturePaths[i].Length; ++j)
                {
                    if (i == 0)
                        TextureNames[i][j] = TexturePaths[i][j].Replace(PathToTextures, "");
                    else
                        TextureNames[i][j] = TexturePaths[i][j].Replace(PathToTextures, "").Replace(GroupNames[i] + "\\", "");
                }
            }
        }
        // loading textures and creation icons from them, also setting size of icons
        private static void ImageListCreation()
        {
            for (Int32 i = 0; i < GroupCount; ++i)
            {
                for (Int32 j = 0; j < TexturePaths[i].Length; ++j)
                {
                    SmallIconList.Images.Add(
                        TextureNames[i][j], new Bitmap(TexturePaths[i][j])
                    );
                    LargeIconList.Images.Add(
                        TextureNames[i][j], new Bitmap(TexturePaths[i][j])
                    );
                }
            }

            SmallIconList.ImageSize = SmallIconSize;
            LargeIconList.ImageSize = LargeIconSize;
        }
        // setting lists of icons to ListView and creation of ListView items with specific group names
        private static void CreateConnectionToListView(ListView listView, bool useSmallIconList)
        {
            if (useSmallIconList)
            {
                listView.SmallImageList = SmallIconList;
                listView.SmallImageList.ImageSize = SmallIconSize;
            }

            listView.LargeImageList = LargeIconList;
            listView.LargeImageList.ImageSize = LargeIconSize;

            for (Int32 i = 0, imgIndex = 0; i < GroupCount; ++i)
            {
                listView.Groups.Add(Groups[i]);
                for (Int32 j = 0; j < TexturePaths[i].Length; ++j, ++imgIndex)
                {
                    listView.Items.Add(new ListViewItem(TextureNames[i][j], imgIndex, listView.Groups[i]));
                }
            }
        }
        // gathering method
        private static void EstablishingConnection(ListView listView, bool useSmallIconList)
        {
            GroupNameCreation();
            GroupCreation();
            TexturePathCreation();
            TextureNameCreation();
            ImageListCreation();
            CreateConnectionToListView(listView, useSmallIconList);
        }
        // method which deletes all data from ListView Control
        private static void DeleteDataFromListView(ListView listView, bool useSmallIconList)
        {
            Dispose();
            listView.Groups.Clear();
            listView.Items.Clear();
            if (useSmallIconList)
                listView.SmallImageList.Dispose();
            listView.LargeImageList.Dispose();
            listView.Clear();
        }
        #endregion

        #region Encapsulated Methods
        // getter of group names from path
        private static string GetGroupNameFromPath(string path, bool subdirectory = true, char pathSeparator = '\\')
        {
            string groupName = path.Remove(0, path.Remove(path.LastIndexOf(pathSeparator)).LastIndexOf(pathSeparator) + 1).Replace(pathSeparator.ToString(), "");
            if (subdirectory)
                return (groupName.Equals(GroupNames[0])) ? groupName : groupName.Replace(GroupNames[0], "");
            else
                return groupName;
        }
        // getter of texture paths from directory with specific extensions
        private static string[] GetTexturesFromDirectory(string pathToFiles, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.GetFiles(pathToFiles, "*.*", searchOption).Where(
                       s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp")
                   ).ToArray();
        }
        #endregion

        // this method performs change of ListView View property from large to small and vice versa
        public static void IconViewChange(ListView listView, bool isLarge)
        {
            if (isLarge)
            {
                listView.View = View.LargeIcon;
            }
            else
            {
                listView.View = View.SmallIcon;
            }

            listView.Invalidate();
            listView.Update();
        }
        // this method performs change of ListView LargeImageList.ImageSize property between small and large sizes, where 'state' is reference to which size convertion must be peformed
        public static void IconSizeChange(string pathToTextures, ListView listView, Size smallIconSize, Size largeIconSize, bool state)
        {
            if (state)
                Refresh(pathToTextures, listView, largeIconSize, smallIconSize);
            else
                Refresh(pathToTextures, listView, smallIconSize, largeIconSize);
        }

        public static void Dispose()
        {
            GroupCount = 0;
            TextureCount = 0;
            PathToTextures = String.Empty;
            GroupNames = new List<string>();
            TexturePaths = new List<string[]>();
            TextureNames = new List<string[]>();
            SmallIconList = new ImageList();
            LargeIconList = new ImageList();
            SmallIconSize = new Size(32, 32);
            LargeIconSize = new Size(96, 96);

            Groups = new List<ListViewGroup>();
        }

    }
}
