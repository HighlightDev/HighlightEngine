using GpuGraphics;
using CParser;
using MassiveGame.Settings;

namespace MassiveGame
{
    public static class PlantModels
    {
        #region Getter

        public static VBOArrayF getPlantModel2()
        {
            AssimpModelLoader loader = new AssimpModelLoader(ProjectFolders.ModelsPath + "Plant1.obj");
            var meshData = loader.GetMeshData();
            return new VBOArrayF(meshData.Verts, meshData.N_Verts, meshData.T_Verts, false);
        }

        public static VBOArrayF getBillboardModel1()
        {
            AssimpModelLoader loader = new AssimpModelLoader(ProjectFolders.ModelsPath + "grassBillboard.obj");
            var meshData = loader.GetMeshData();
            return new VBOArrayF(meshData.Verts, meshData.N_Verts, meshData.T_Verts, false);
        }

        #endregion
    }
}
