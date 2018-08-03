using CParser;
using CParser.Assimp;
using GpuGraphics;

namespace OTKWinForm.IOCore
{
    public static class ProxyModelLoader
    {
        public static VBOArrayF LoadModel(string modelPath)
        {
            AssimpModelLoader loader = new AssimpModelLoader(modelPath);
            MeshVertexData meshData = loader.GetMeshData();
            return new VBOArrayF(meshData.Verts, meshData.N_Verts, meshData.T_Verts, false);
        }
    }
}
