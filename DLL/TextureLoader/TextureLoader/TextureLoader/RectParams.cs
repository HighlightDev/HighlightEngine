using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureLoader
{
    public class RectParams
    {
        public Int32 Width { set; get; }
        public Int32 Height { set; get; }

        public RectParams(Int32 width, Int32 height)
        {
            Width = width;
            Height = height;
        }
    }
}
