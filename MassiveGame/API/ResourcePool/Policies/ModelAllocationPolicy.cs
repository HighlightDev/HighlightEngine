using System;
using OpenTK.Graphics.OpenGL;
using CParser;
using VBO;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class ModelAllocationPolicy : AllocationPolicy<string, VertexArrayObject>
    {
        public override VertexArrayObject AllocateMemory(string arg)
        {
            VertexArrayObject resultBufferArray = null;
            resultBufferArray = SendDataToGpu(arg);
            return resultBufferArray;
        }

        private VertexArrayObject SendDataToGpu(string modelPath)
        {
            // Get mesh data 
            ModelLoader model = new ModelLoader(modelPath);
            var vertices = model.Verts;
            var normals = model.N_Verts;
            var texCoords = model.T_Verts;
            var tangents = VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords);
            var bitangents = VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords);

            UInt32[] indices = model.Indices;

            VertexArrayObject vao = new VertexArrayObject();

            IndexBufferObject ibo = null;
            if (model.bHasIndices)
            {
                ibo = new IndexBufferObject(indices);
            }
            VertexBufferObjectTwoDimension<float> vertexVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> normalsVBO = new VertexBufferObjectTwoDimension<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> tangentsVBO = new VertexBufferObjectTwoDimension<float>(tangents, BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> bitangentsVBO = new VertexBufferObjectTwoDimension<float>(bitangents, BufferTarget.ArrayBuffer, 5, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            vao.AddVBO(vertexVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO);
            vao.AddIndexBuffer(ibo);
            vao.BindBuffersToVao();

            return vao;
        }

        public override void FreeMemory(VertexArrayObject arg)
        {
            arg.CleanUp();
        }
    }
}
