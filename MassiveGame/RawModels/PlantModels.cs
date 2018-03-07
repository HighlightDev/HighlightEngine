using GpuGraphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CParser;

namespace MassiveGame
{
    public static class PlantModels
    {
        #region Getter

        public static VBOArrayF getPlantModel2()
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "Plant1.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, false);
        }

        public static VBOArrayF getBillboardModel1()
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "grassBillboard.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, false);
        }

        #endregion
    }
}
