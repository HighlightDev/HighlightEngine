using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CParser.OBJ_Parser
{
    public class OBJ_SimpleAnimationLoader
    {
        #region Constructors
        public OBJ_SimpleAnimationLoader()
        {
            _isLoad = false;
            animatedModel = null;
        }

        public OBJ_SimpleAnimationLoader(string animationFilesPath)
            : this()
        {
            LoadAnimation(animationFilesPath);
        }
        #endregion


        private bool _isLoad;
        OBJ_ModelLoaderEx[] animatedModel;

        
        public void LoadAnimation(string animationFilesPath)
        {
            string[] objFiles = GetAnimationFiles(animationFilesPath);
            animatedModel = new OBJ_ModelLoaderEx[objFiles.Length];
            for (Int32 i = 0; i < objFiles.Length; ++i)
            {
                animatedModel[i] = new OBJ_ModelLoaderEx(false, objFiles[i]);
            }
            _isLoad = true;
        }

        private string[] GetAnimationFiles(string storageDirectory)
        {
            string[] objFiles = Directory.GetFiles(storageDirectory, "*.obj");
            return objFiles;
        }

    }
}
