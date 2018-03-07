using CParser;
using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTKWinForm.IOCore
{
    public static class ProxyModelLoader
    {
        public static VBOArrayF LoadModel(string modelPath)
        {
            ModelLoader loader = new ModelLoader(modelPath);
            return new VBOArrayF(loader.Verts, loader.N_Verts, loader.T_Verts, false);
        }
    }
}
