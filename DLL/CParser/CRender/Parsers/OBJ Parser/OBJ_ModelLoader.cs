using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CParser.OBJ_Parser
{
    public class OBJ_ModelLoader : ModelLoaderAbstract
    {
        #region Constructors
        public OBJ_ModelLoader()
        {
            _modelFilePath = "";
            _objectQuantity = 0;
            _objectParameterQuantity = null;
            base.isLoad = false;
            limbs = null;
            verts = null;
            t_verts = null;
            n_verts = null;
        }

        /// <summary>
        /// Загрузчик файлов OBJ. Загружает все вершины, нормали и текстурные координаты в Limbs, где каждый limb - 1 под-объект.
        /// </summary>
        /// <param name="useConvertionToVBO">Флаг использования преобразования данных для использования буфферами. Если <b>true</b> - преобразование будет выполнено.</param>
        /// <param name="modelFilePath">Путь к файлу модели.</param>
        public OBJ_ModelLoader(bool useConvertionToVBO, string modelFilePath)
            : this()
        {
            LoadModel(modelFilePath);
            if (useConvertionToVBO)
                ConvertModelData();
        }
        #endregion


        private string _modelFilePath;
        private int _objectQuantity;
        private int[,] _objectParameterQuantity;
        protected float[,] verts;
        protected float[,] t_verts;
        protected float[,] n_verts;
        private OBJ_Limb[] limbs;   // need to clean up data after convertion

        public float[,] Verts { get { return verts; } }
        public float[,] T_Verts { get { return t_verts; } }
        public float[,] N_Verts { get { return n_verts; } }
        

        /// <summary>
        /// Загрузчик файлов OBJ. Загружает все вершины, нормали и текстурные координаты в Limbs, где каждый limb - 1 под-объект.
        /// </summary>
        /// <param name="modelFilePath">Путь к файлу модели.</param>
        public override int LoadModel(string modelFilePath)
        {
            _modelFilePath = modelFilePath;
            objectCounter();
            limbs = new OBJ_Limb[_objectQuantity];
            objectParameterCounter();

            string modelFile = "";
            bool objectCounterTrigger = false;
            StreamReader sr_modelFile = new StreamReader(_modelFilePath, Encoding.Default);

            for (int i = 0; i < _objectQuantity; ++i)
            {
                limbs[i] = new OBJ_Limb(_objectParameterQuantity[i, 0], 
                                       _objectParameterQuantity[i, 3], 
                                       _objectParameterQuantity[i, 2], 
                                       _objectParameterQuantity[i, 1]);
                int j = 0; //iterator
                //trigger for correct work of file reader
                bool fileTrigger = false;
                //triggers for correct work of iterator j
                bool vCounterTrigger = false;
                bool vtCounterTrigger = false;
                bool vnCounterTrigger = false;
                bool fCounterTrigger = false;

                while (!sr_modelFile.EndOfStream)
                {
                    #region Parsing file
                    if (objectCounterTrigger == false) modelFile = sr_modelFile.ReadLine();
                    else objectCounterTrigger = false;
                    if (modelFile.StartsWith("#")) continue;
                    if (modelFile.StartsWith("o ")) // means object name
                    {
                        if (fileTrigger == false)
                        {
                            limbs[i].objectName = modelFile.Remove(0, 2);
                            fileTrigger = true;
                            continue;
                        }
                        else break;
                    }

                    if (modelFile.StartsWith("v ")) // means vertexes of object
                    {
                        if (vCounterTrigger == false)
                        {
                            j = 0;
                            vCounterTrigger = true;
                        }
                        string[] splittedString = modelFile.Split(' ');
                        limbs[i].vert[j, 0] = Convert.ToSingle(splittedString[1].Replace('.', ','));
                        limbs[i].vert[j, 1] = Convert.ToSingle(splittedString[2].Replace('.', ','));
                        limbs[i].vert[j, 2] = Convert.ToSingle(splittedString[3].Replace('.', ','));
                        if (j == _objectParameterQuantity[i, 0] - 1)
                        {
                            vnCounterTrigger = false;
                            vtCounterTrigger = false;
                            fCounterTrigger = false;
                        }
                    }
                    else if (modelFile.StartsWith("vt ")) // means texture vertexes of object
                    {
                        if (vtCounterTrigger == false)
                        {
                            j = 0;
                            vtCounterTrigger = true;
                        }
                        string[] splittedString = modelFile.Split(' ');
                        limbs[i].t_vert[j, 0] = Convert.ToSingle(splittedString[1].Replace('.', ','));
                        limbs[i].t_vert[j, 1] = Convert.ToSingle(splittedString[2].Replace('.', ','));
                        if (j == _objectParameterQuantity[i, 1] - 1)
                        {
                            vnCounterTrigger = false;
                            vCounterTrigger = false;
                            fCounterTrigger = false;
                        }
                    }
                    else if (modelFile.StartsWith("vn "))  // means normal vertexex of object
                    {
                        if (vnCounterTrigger == false)
                        {
                            j = 0;
                            vnCounterTrigger = true;
                        }
                        string[] splittedString = modelFile.Split(' ');
                        limbs[i].n_vert[j, 0] = Convert.ToSingle(splittedString[1].Replace('.', ','));
                        limbs[i].n_vert[j, 1] = Convert.ToSingle(splittedString[2].Replace('.', ','));
                        limbs[i].n_vert[j, 2] = Convert.ToSingle(splittedString[3].Replace('.', ','));
                        if (j == _objectParameterQuantity[i, 2] - 1)
                        {
                            vtCounterTrigger = false;
                            vCounterTrigger = false;
                            fCounterTrigger = false;
                        }
                    }
                    else if (modelFile.StartsWith("f "))  // means faces of object (face includes 3 vertexes not 4)
                    {
                        if (fCounterTrigger == false)
                        {
                            j = 0;
                            fCounterTrigger = true;
                        }
                        string face = modelFile.Remove(0, 2);
                        string[] splittedString = face.Split(' ');
                        int sumOfPrevValsOfVertexes = 0;        //for de-globalization of vertex counter in faces
                        int sumOfPrevValsOfTexVertexes = 0;     //for de-globalization of texture vertex counter in faces
                        int sumOfPrevValsOfNormalVertexes = 0;  //for de-globalization of normal vertex counter in faces

                        if (i > 0)
                            for (int l = 0; l < i; l++)
                            {
                                sumOfPrevValsOfVertexes += _objectParameterQuantity[l, 0];
                                sumOfPrevValsOfTexVertexes += _objectParameterQuantity[l, 1];
                                sumOfPrevValsOfNormalVertexes += _objectParameterQuantity[l, 2];
                            }  

                        foreach (string item in splittedString)
                        {
                            string[] splittedItem = item.Split('/');
                            limbs[i].face[j, 0] = (splittedItem[0] == "") ? 0 : Convert.ToInt32(splittedItem[0]) - sumOfPrevValsOfVertexes;
                            limbs[i].face[j, 1] = (splittedItem[1] == "") ? 0 : Convert.ToInt32(splittedItem[1]) - sumOfPrevValsOfTexVertexes;
                            limbs[i].face[j, 2] = (splittedItem[2] == "") ? 0 : Convert.ToInt32(splittedItem[2]) - sumOfPrevValsOfNormalVertexes;
                            j++;
                        }
                        if (j == _objectParameterQuantity[i, 3] - 1)
                        {
                            vnCounterTrigger = false;
                            vtCounterTrigger = false;
                            vCounterTrigger = false;
                        }
                        continue;
                    }

                    j++;
                    #endregion
                }
                objectCounterTrigger = true;
            }

            sr_modelFile.Close();

            isLoad = true;      // загрузка завершена
            return 0;
        }

        #region Counters of objects and parameters
        private void objectCounter() //func calculates quantity of objects
        {
            string modelFile = "";
            StreamReader sr_modelFile = new StreamReader(_modelFilePath, Encoding.Default);

            while (!sr_modelFile.EndOfStream)
            {
                modelFile = sr_modelFile.ReadLine();
                if (modelFile.StartsWith("o "))
                {
                    _objectQuantity++;
                }
            }

            sr_modelFile.Close();
        }

        protected virtual void objectParameterCounter() //func calculates quantity of object parameters
        {
            _objectParameterQuantity = new int[_objectQuantity, 4];
            string modelFile = "";
            bool objectCounterTrigger = false;
            StreamReader sr_modelFile = new StreamReader(_modelFilePath, Encoding.Default);

            for (int i = 0; i < _objectQuantity; ++i)
            {
                //temporary vault of parameter quantity
                int vertexQuantity = 0;
                int vertexTextureQuantity = 0;
                int vertexNormalQuantity = 0;
                int faceQuantity = 0;
                bool fileTrigger = false;

                while (!sr_modelFile.EndOfStream)
                {
                    if (objectCounterTrigger == false) modelFile = sr_modelFile.ReadLine();
                    else objectCounterTrigger = false;
                    if (modelFile.StartsWith("#")) continue;
                    if (modelFile.StartsWith("o "))
                    {
                        if (fileTrigger == false)
                        {
                            fileTrigger = true;
                            continue;
                        }
                        else break;
                    }

                    if (modelFile.StartsWith("v "))
                    {
                        vertexQuantity++;
                    }
                    else if (modelFile.StartsWith("vt "))
                    {
                        vertexTextureQuantity++;
                    }
                    else if (modelFile.StartsWith("vn "))
                    {
                        vertexNormalQuantity++;
                    }
                    else if (modelFile.StartsWith("f "))
                    {
                        faceQuantity++;
                    }
                }

                _objectParameterQuantity[i, 0] = vertexQuantity;
                _objectParameterQuantity[i, 1] = vertexTextureQuantity;
                _objectParameterQuantity[i, 2] = vertexNormalQuantity;
                _objectParameterQuantity[i, 3] = faceQuantity * 3;
                objectCounterTrigger = true;
            }

            sr_modelFile.Close();
        }
        #endregion

        #region Data converter
        protected override void ConvertModelData()
        {
            uint overallVertexQuantity = 0;

            foreach (OBJ_Limb limb in this.limbs)
            {
                overallVertexQuantity += Convert.ToUInt32(limb.quantityOf_V_VT_VN_Face[3]);
            }

            verts = new float[overallVertexQuantity, 3];
            n_verts = new float[overallVertexQuantity, 3];
            t_verts = new float[overallVertexQuantity, 2];

            for (int iGlobal = 0, iLocal = 0, objectCounter = 0; (objectCounter < this.limbs.Length)
                && (iLocal < this.limbs[objectCounter].quantityOf_V_VT_VN_Face[3])
                && (iGlobal < overallVertexQuantity); ++iLocal, ++iGlobal)
            {
                verts[iGlobal, 0] = this.limbs[objectCounter].vert[this.limbs[objectCounter].face[iLocal, 0] - 1, 0]; // X pos of vertex
                verts[iGlobal, 1] = this.limbs[objectCounter].vert[this.limbs[objectCounter].face[iLocal, 0] - 1, 1]; // Y pos of vertex
                verts[iGlobal, 2] = this.limbs[objectCounter].vert[this.limbs[objectCounter].face[iLocal, 0] - 1, 2]; // Z pos of vertex

                n_verts[iGlobal, 0] = this.limbs[objectCounter].n_vert[this.limbs[objectCounter].face[iLocal, 2] - 1, 0]; // X pos of normal_vertex
                n_verts[iGlobal, 1] = this.limbs[objectCounter].n_vert[this.limbs[objectCounter].face[iLocal, 2] - 1, 1]; // Y pos of normal_vertex
                n_verts[iGlobal, 2] = this.limbs[objectCounter].n_vert[this.limbs[objectCounter].face[iLocal, 2] - 1, 2]; // Z pos of normal_vertex

                try // Might not be some of texture coordinates
                {
                    t_verts[iGlobal, 0] = this.limbs[objectCounter].t_vert[this.limbs[objectCounter].face[iLocal, 1] - 1, 0]; // U pos of tex_vertex
                    t_verts[iGlobal, 1] = this.limbs[objectCounter].t_vert[this.limbs[objectCounter].face[iLocal, 1] - 1, 1]; // V pos of tex_vertex
                }
                catch // to avoid errors
                {
                    t_verts[iGlobal, 0] = 0;
                    t_verts[iGlobal, 1] = 0;
                }

                if ((iLocal + 1) == this.limbs[objectCounter].quantityOf_V_VT_VN_Face[3])
                {
                    iLocal = -1;
                    ++objectCounter;
                }
            }

            CleanUp();
        }
        #endregion

        protected override void CleanUp()
        {
            base.isLoad = false;
            _modelFilePath = String.Empty;
            _objectQuantity = 0;
            _objectParameterQuantity = null;
            limbs = null;
        }

    }
}
