using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CParser.OBJ_Parser
{
    public class OBJ_Limb : LimbAbstract
    {
        #region Constructors
        private OBJ_Limb()
        {
            objectName = String.Empty;
            quantityOf_V_VT_VN_Face = new int[4];
        }

        public OBJ_Limb(int vertexQuantity, int faceQuantity, int normalVertexQuantity = 0, int textureVertexQuantity = 0)
            : this()
        {
            // записываем количество вершин и полигонов
            quantityOf_V_VT_VN_Face[0] = vertexQuantity;
            quantityOf_V_VT_VN_Face[1] = textureVertexQuantity;
            quantityOf_V_VT_VN_Face[2] = normalVertexQuantity;
            quantityOf_V_VT_VN_Face[3] = faceQuantity;

            // выделяем память
            MemoryForModel();
        }
        #endregion


        public float[,] n_vert;
        public string objectName;               // name of obj mesh
        public int[] quantityOf_V_VT_VN_Face;   // временное хранение информации
        public bool ModelHasTexture { get; set; }
                
        // память для геометрии
        protected override void MemoryForModel()
        {
            vert = new float[quantityOf_V_VT_VN_Face[0], 3];
            t_vert = new float[quantityOf_V_VT_VN_Face[1], 2];
            n_vert = new float[quantityOf_V_VT_VN_Face[2], 3];
            face = new int[quantityOf_V_VT_VN_Face[3], 3];
        }

    }
}
