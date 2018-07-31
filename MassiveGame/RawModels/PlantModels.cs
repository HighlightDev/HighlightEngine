using GpuGraphics;
using CParser;
using MassiveGame.Settings;

namespace MassiveGame
{
    public static class PlantModels
    {
        #region Getter

        public static void LoadCollada()
        {
            AssimpModelLoader loader = new AssimpModelLoader(ProjectFolders.ModelsPath + "SpaceBoy_Walk.dae");
        }

        public static VBOArrayF getPlantModel2()
        {
            AssimpModelLoader loader = new AssimpModelLoader(ProjectFolders.ModelsPath + "Plant1.obj");
            return new VBOArrayF(loader.MeshData.Verts, loader.MeshData.N_Verts, loader.MeshData.T_Verts, false);
        }

        public static VBOArrayF getBillboardModel1()
        {
            AssimpModelLoader loader = new AssimpModelLoader(ProjectFolders.ModelsPath + "grassBillboard.obj");
            return new VBOArrayF(loader.MeshData.Verts, loader.MeshData.N_Verts, loader.MeshData.T_Verts, false);
        }

        #endregion
    }
}
