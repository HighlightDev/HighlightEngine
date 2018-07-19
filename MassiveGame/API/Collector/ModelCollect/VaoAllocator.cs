using OpenTK.Graphics.OpenGL;
using CParser;
using VBO;

namespace MassiveGame.API.Collector.ModelCollect
{
    public static class VaoAllocator
    {
        public static VertexArrayObject LoadModelFromFile(string modelPath)
        {
            VertexArrayObject resultBufferArray = null;
            resultBufferArray = SendDataToGpu(modelPath);
            return resultBufferArray;
        }

        private static VertexArrayObject SendDataToGpu(string modelPath)
        {
            // Get data from mesh
            ModelLoader model = new ModelLoader(modelPath);
            var vertices = model.Verts;
            var normals = model.N_Verts;
            var texCoords = model.T_Verts;
            var tangents = VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords);
            var bitangents = VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords);

            VertexArrayObject vao = new VertexArrayObject();

            VertexBufferObject<float> vertexVBO = new VertexBufferObject<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> normalsVBO = new VertexBufferObject<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> texCoordsVBO = new VertexBufferObject<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> tangentsVBO = new VertexBufferObject<float>(tangents, BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> bitangentsVBO = new VertexBufferObject<float>(bitangents, BufferTarget.ArrayBuffer, 5, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            vao.AddVBO(vertexVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO);
            vao.BindVbosToVao();

            return vao;
        }

        public static void ReleaseVAO(VertexArrayObject bufferArray)
        {
            bufferArray.CleanUp();
        }
    }
}
