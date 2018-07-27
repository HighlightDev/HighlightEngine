﻿using GpuGraphics;
using CParser;
using MassiveGame.Settings;

namespace MassiveGame
{
    public static class PlantModels
    {
        #region Getter

        public static void LoadCollada()
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "SpaceBoy_Walk.dae");
        }

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
