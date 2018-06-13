using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CParser.OBJ_Parser
{
    class OBJ_TextureDataLoader : DataLoaderAbstract
    {
        #region Constructors
        public OBJ_TextureDataLoader(string modelName, string textureDataFilePath)
        {
            // names of obj file and txr file must be same
            if (equalityCheckOfModelName(modelName, textureDataFilePath))
                LoadData(textureDataFilePath);
        }

        public OBJ_TextureDataLoader(string textureDataFilePath)
        {
            LoadData(textureDataFilePath);
        }
        #endregion


        public string[] materialPath = null;

        public override void LoadData(string dataFilePath)
        {
            FileInfo file = new FileInfo(dataFilePath);
            if (file.Exists)
            {
                string txrFile = "";
                Int32 index = 0;
                char[] paramsToTrim = { ' ', '\t', '\'', '\"' };
                StreamReader sr_txrFile = new StreamReader(dataFilePath, Encoding.UTF8);

                while (!sr_txrFile.EndOfStream)
                {
                    txrFile = sr_txrFile.ReadLine();
                    txrFile = txrFile.Trim(paramsToTrim);

                    #region Parsing of file
                    if (txrFile.Equals("[TEXTURES]")) continue;
                    if (txrFile.Equals("[MODEL_START_SECTION]")) continue;
                    if (txrFile.Equals("[MATERIAL_START_SECTION]")) continue;
                    if (txrFile.Equals("[MATERIAL_END_SECTION]")) continue;

                    if (txrFile.StartsWith("*MODEL_NAME"))
                    {
                        txrFile = txrFile.Replace("*MODEL_NAME", "");
                        txrFile = txrFile.Trim(paramsToTrim);
                        _modelName = txrFile;
                        continue;
                    }
                    else if (txrFile.StartsWith("*MATERIAL_COUNT"))
                    {
                        txrFile = txrFile.Replace("*MATERIAL_COUNT", "");
                        txrFile = txrFile.Trim(paramsToTrim);
                        materialCount = Convert.ToInt32(txrFile);
                        materialNumber = new Int32[materialCount];
                        materialPath = new string[materialCount];
                        continue;
                    }
                    else if (txrFile.StartsWith("*MATERIAL_NUMBER"))
                    {
                        txrFile = txrFile.Replace("*MATERIAL_NUMBER", "");
                        txrFile = txrFile.Trim(paramsToTrim);
                        materialNumber[index] = Convert.ToInt32(txrFile);
                        continue;
                    }
                    else if (txrFile.StartsWith("*MATERIAL_PATH"))
                    {
                        txrFile = txrFile.Replace("*MATERIAL_PATH", "");
                        txrFile = txrFile.Trim(paramsToTrim);
                        materialPath[index] = txrFile;
                        index++;
                        continue;
                    }

                    if (txrFile.Equals("[MODEL_END_SECTION]")) break;
                    #endregion
                }

                sr_txrFile.Close();
            }
        }

    }
}
