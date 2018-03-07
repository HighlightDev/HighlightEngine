using GpuGraphics;
using CParser;

namespace MassiveGame
{
    public static class BuildingModels
    {
        public static VBOArrayF getBuildingModel1(bool enableNormalMap)
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "wall_7_without_door.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }
        public static VBOArrayF getBuildingModel1_door(bool enableNormalMap)
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "wall_9_door.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }

        public static VBOArrayF getBuildingModel2(bool enableNormalMap)
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "building.ASE");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }
        public static VBOArrayF getBuildingModel3(bool enableNormalMap)
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "MedievalHouse.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }

        public static VBOArrayF getCityModel(bool enableNormalMap)
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "City_House_2_BI.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }

        public static VBOArrayF getBuildingModel(bool enableNormalMap, string modelName)
        {
            ModelLoader model = new ModelLoader(modelName);
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalMap);
        }

        public static VBOArrayF getLampModel(bool enableNormalmap)
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "GothicLamp.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, enableNormalmap);
        }

        public static VBOArrayF getDinoModel()
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "Dino.3ds");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, false);
        }

        public static VBOArrayF getTreeModel()
        {
            ModelLoader model = new ModelLoader(ProjectFolders.ModelsPath + "tree.obj");
            return new VBOArrayF(model.Verts, model.N_Verts, model.T_Verts, false);
        }

    }
}
