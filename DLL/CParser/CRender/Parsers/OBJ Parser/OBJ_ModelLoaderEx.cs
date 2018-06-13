using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CParser.OBJ_Parser
{
    public class OBJ_ModelLoaderEx : OBJ_ModelLoader
    {
        #region Constructors
        public OBJ_ModelLoaderEx()
        {
            _modelFilePath = "";
            limb = null;
            _objectParameterQuantity = null;
            base.isLoad = false;
        }

        /// <summary>
        /// Загрузчик файлов OBJ. Загружает все вершины, нормали и текстурные координаты в общий Limb.
        /// </summary>
        /// <param name="useConvertionToVBO">Флаг использования преобразования данных для использования буфферами. Если <b>true</b> - преобразование будет выполнено.</param>
        /// <param name="modelFilePath">Путь к файлу модели.</param>
        public OBJ_ModelLoaderEx(bool useConvertionToVBO, string modelFilePath)
        {
            LoadModel(modelFilePath);
            if (useConvertionToVBO)
                ConvertModelData();
        }
        #endregion


        private string _modelFilePath;
        private Int32[] _objectParameterQuantity;
        public OBJ_Limb limb;
                
        
        /// <summary>
        /// Загрузчик файлов OBJ. Загружает все вершины, нормали и текстурные координаты в общий Limb.
        /// </summary>
        /// <param name="modelFilePath">Путь к файлу модели.</param>
        public override Int32 LoadModel(string modelFilePath)
        {
            _modelFilePath = modelFilePath;
            objectParameterCounter();

            string modelFile = "";
            string[] splittedString = null;
            // iterators
            uint vCounter = 0, vtCounter = 0, vnCounter = 0, fCounter = 0;
            StreamReader sr_modelFile = new StreamReader(_modelFilePath, Encoding.Default);

            limb = new OBJ_Limb(_objectParameterQuantity[0], _objectParameterQuantity[3],
                                _objectParameterQuantity[2], _objectParameterQuantity[1]);

            while (!sr_modelFile.EndOfStream)
            {
                modelFile = sr_modelFile.ReadLine();

                #region Parsing file
                if (modelFile.StartsWith("#")) continue;
                if (modelFile.StartsWith("o ")) continue;

                if (modelFile.StartsWith("v ")) // means vertexes of object
                {
                    splittedString = modelFile.Split(' ');
                    limb.vert[vCounter, 0] = Convert.ToSingle(splittedString[1].Replace('.', ','));
                    limb.vert[vCounter, 1] = Convert.ToSingle(splittedString[2].Replace('.', ','));
                    limb.vert[vCounter, 2] = Convert.ToSingle(splittedString[3].Replace('.', ','));
                    splittedString = null;
                    vCounter++;
                    continue;
                }
                else if (modelFile.StartsWith("vt ")) // means texture vertexes of object
                {
                    splittedString = modelFile.Split(' ');
                    limb.t_vert[vtCounter, 0] = Convert.ToSingle(splittedString[1].Replace('.', ','));
                    limb.t_vert[vtCounter, 1] = Convert.ToSingle(splittedString[2].Replace('.', ','));
                    splittedString = null;
                    vtCounter++;
                    continue;
                }
                else if (modelFile.StartsWith("vn "))  // means normal vertexex of object
                {
                    splittedString = modelFile.Split(' ');
                    limb.n_vert[vnCounter, 0] = Convert.ToSingle(splittedString[1].Replace('.', ','));
                    limb.n_vert[vnCounter, 1] = Convert.ToSingle(splittedString[2].Replace('.', ','));
                    limb.n_vert[vnCounter, 2] = Convert.ToSingle(splittedString[3].Replace('.', ','));
                    splittedString = null;
                    vnCounter++;
                    continue;
                }
                else if (modelFile.StartsWith("f "))  // means faces of object (face includes 3 vertexes not 4)
                {
                    string[] splittedItem = null;
                    splittedString = modelFile.Remove(0, 2).Split(' ');

                    foreach (string item in splittedString)
                    {
                        splittedItem = item.Split('/');
                        limb.face[fCounter, 0] = (splittedItem[0] == "") ? 0 : Convert.ToInt32(splittedItem[0]);
                        limb.face[fCounter, 1] = (splittedItem[1] == "") ? 0 : Convert.ToInt32(splittedItem[1]);
                        limb.face[fCounter, 2] = (splittedItem[2] == "") ? 0 : Convert.ToInt32(splittedItem[2]);
                        splittedItem = null;
                        fCounter++;
                    }

                    continue;
                }
                #endregion
            }

            sr_modelFile.Close();

            #region Disposing of local vars
            vCounter = 0;
            vtCounter = 0;
            vnCounter = 0;
            fCounter = 0;
            modelFile = String.Empty;
            #endregion

            isLoad = true;      // загрузка завершена
            return 0;
        }

        #region Counter of object parameters
        protected override void objectParameterCounter() //func calculates quantity of object parameters
        {
            _objectParameterQuantity = new Int32[4];
            string modelFile = "";
            StreamReader sr_modelFile = new StreamReader(_modelFilePath, Encoding.Default);

            while (!sr_modelFile.EndOfStream)
            {
                modelFile = sr_modelFile.ReadLine();

                if (modelFile.StartsWith("#")) continue;
                if (modelFile.StartsWith("o ")) continue;

                if (modelFile.StartsWith("v "))
                {
                    _objectParameterQuantity[0]++;
                }
                else if (modelFile.StartsWith("vt "))
                {
                    _objectParameterQuantity[1]++;
                }
                else if (modelFile.StartsWith("vn "))
                {
                    _objectParameterQuantity[2]++;
                }
                else if (modelFile.StartsWith("f "))
                {
                    _objectParameterQuantity[3] += 3;
                }
            }

            sr_modelFile.Close();
        }
        #endregion

        #region Data converter
        protected override void ConvertModelData()
        {
            uint overallVertexQuantity = Convert.ToUInt32(limb.face.Length / 3);

            verts = new float[overallVertexQuantity, 3];
            n_verts = new float[overallVertexQuantity, 3];
            t_verts = new float[overallVertexQuantity, 2];

            for (uint i = 0; i < overallVertexQuantity; ++i)
            {
                verts[i, 0] = limb.vert[limb.face[i, 0] - 1, 0]; // X pos of vertex
                verts[i, 1] = limb.vert[limb.face[i, 0] - 1, 1]; // Y pos of vertex
                verts[i, 2] = limb.vert[limb.face[i, 0] - 1, 2]; // Z pos of vertex

                n_verts[i, 0] = limb.n_vert[limb.face[i, 2] - 1, 0]; // X pos of normal_vertex
                n_verts[i, 1] = limb.n_vert[limb.face[i, 2] - 1, 1]; // Y pos of normal_vertex
                n_verts[i, 2] = limb.n_vert[limb.face[i, 2] - 1, 2]; // Z pos of normal_vertex

                try // Might not be some of texture coordinates
                {
                    t_verts[i, 0] = limb.t_vert[limb.face[i, 1] - 1, 0]; // U pos of tex_vertex
                    t_verts[i, 1] = 1 - limb.t_vert[limb.face[i, 1] - 1, 1]; // V pos of tex_vertex / (1 - ) - needed 'cause in OGL texture starts in Top-Left corner
                }
                catch // to avoid errors
                {
                    t_verts[i, 0] = 0;
                    t_verts[i, 1] = 0;
                }
            }

            overallVertexQuantity = 0;
            CleanUp();
        }
        #endregion

        protected override void CleanUp()
        {
            base.isLoad = false;
            _modelFilePath = String.Empty;
            _objectParameterQuantity = null;
            limb = null;
        }

    }
}
