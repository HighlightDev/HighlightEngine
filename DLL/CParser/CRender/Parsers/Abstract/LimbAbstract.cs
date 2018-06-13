using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CParser
{
    public abstract class LimbAbstract
    {
        // массивы для хранения данных (геометрии и текстурных координат)
        public float[,] vert = null;
        public float[,] t_vert = null;
        public Int32[,] face = null;

        protected abstract void MemoryForModel();
    }
}
