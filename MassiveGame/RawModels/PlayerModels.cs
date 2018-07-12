using GpuGraphics;
using CParser.OBJ_Parser;
using MassiveGame.Settings;

namespace MassiveGame
{
    public static class PlayerModels
    {
        public static VBOArrayF getPlayerModel1(bool enableNormalMap)
        {
            OBJ_ModelLoaderEx model = new OBJ_ModelLoaderEx(true, ProjectFolders.ModelsPath + "playerCube.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }

        public static VBOArrayF getPlayerModel2(bool enableNormalMap)
        {
            OBJ_ModelLoaderEx model = new OBJ_ModelLoaderEx(true, ProjectFolders.ModelsPath + "testCube.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }
    }
}
