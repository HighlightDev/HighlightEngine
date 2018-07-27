using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CParser.OBJ_Parser;
using CParser.ASE_Parser;
using CParser.DAE_Parser;

namespace CParser
{
    public class ModelLoader
    {
        public float[,] Verts { get; private set; }
        public float[,] T_Verts { get; private set; }
        public float[,] N_Verts { get; private set; }
        
        public UInt32[] Indices { get; private set; }
        public bool bHasIndices { get; private set; }


        /// <summary>
        /// Загрузчик моделей. Загружает все вершины, нормали и текстурные координаты и преобразовует их для использования в VBO.
        /// Поддерживаемые расширения: .OBJ; .ASE; .DAE;
        /// </summary>
        /// <param name="modelFilePath">Путь к файлу модели.</param>
        public ModelLoader(string modelFilePath)
        {
            switch (modelFilePath.Remove(0, modelFilePath.LastIndexOf(".") + 1).ToLower())
            {
                //case "obj":
                //    OBJ_ModelLoaderEx objModel = new OBJ_ModelLoaderEx(true, modelFilePath);
                //    Verts = objModel.Verts;
                //    T_Verts = objModel.T_Verts;
                //    N_Verts = objModel.N_Verts;
                //    objModel = null;
                //    break;
                //case "ase":
                //    ASE_ModelLoader aseModel = new ASE_ModelLoader(true, modelFilePath);
                //    Verts = aseModel.Verts;
                //    T_Verts = aseModel.T_Verts;
                //    N_Verts = aseModel.N_Verts;
                //    aseModel = null;
                //    break;
                //case "dae":
                //    DAE_Geometries daeModel = new DAE_Geometries(modelFilePath);
                //    Verts = daeModel.Verts;
                //    T_Verts = daeModel.T_Verts;
                //    N_Verts = daeModel.N_Verts;
                //    daeModel = null;
                //    break;
                default:
                    //throw new Exception("wrong / unsupported mesh extension!");
                    AssimpModelLoader mesh = new AssimpModelLoader(modelFilePath);
                    Verts = mesh.Verts;
                    T_Verts = mesh.T_Verts;
                    N_Verts = mesh.N_Verts;
                    Indices = mesh.Indices.ToArray<UInt32>();
                    bHasIndices = mesh.bHasIndices;
                    mesh = null;
                    break;
            }
        }
    }
}
