using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CParser.ASE_Parser
{
    // класс LIMB отвечает за логические единицы 3D объектов в загружаемой сцене
    public class ASE_Limb : LimbAbstract
    {
        #region Constructors
        private ASE_Limb()
        {
            t_face = null;
            quantityOfV_F_TV_TF = new int[4];
        }

        // при инициализации мы должны указать количество вершин (vertex) и 
        // полигонов (face) которые описывают геометри под-объекта
        public ASE_Limb(int vertex, int face)
            : this()
        {
            // записываем количество вершин и полигонов
            quantityOfV_F_TV_TF[0] = vertex;
            quantityOfV_F_TV_TF[1] = face * 3;

            // выделяем память
            MemoryForModel();
        }
        #endregion

        public new int[] face;
        public int[] t_face;
        // временное хранение информации
        public int[] quantityOfV_F_TV_TF;


        #region Memmory allocation
        // память для геометрии
        protected override void MemoryForModel()
        {
            vert = new float[quantityOfV_F_TV_TF[0], 3];
            face = new int[quantityOfV_F_TV_TF[1]];
        }
        
        // массивы для текстурных координат
        public void CreateTextureVertexMemory(int t_vertex)
        {
            quantityOfV_F_TV_TF[2] = t_vertex;
            t_vert = new float[quantityOfV_F_TV_TF[2], 2];
        }

        // привязка значений текстурных координат к полигонам 
        public void CreateTextureFaceMemory(int t_face_local)
        {
            quantityOfV_F_TV_TF[3] = t_face_local * 3;
            t_face = new int[quantityOfV_F_TV_TF[3]];
        }
        #endregion

    }

}
