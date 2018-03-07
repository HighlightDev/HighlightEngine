using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CParser.DAE_Parser
{
    public class DAE_Limb
    {
        public DAE_Limb()
        {
            verts = null;
            n_verts = null;
            t_verts = null;
            face = null;
            lines = null;
            objectID = String.Empty;
            objectName = String.Empty;
        }


        public float[,] verts;
        public float[,] n_verts;
        public float[,] t_verts;
        public int[,] face;
        // additional data, could be deleted
        public int[,] lines;
        public string objectID;
        public string objectName;
    }
}
