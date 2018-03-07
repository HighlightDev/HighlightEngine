using GpuGraphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CParser.OBJ_Parser;

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
