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
            VertexArrayObject vao = new VertexArrayObject();
            // Get mesh data 
            using (AssimpModelLoader loader = new AssimpModelLoader(modelPath))
            {
                // explicit null assignment if there is no some of mesh data
                var vertices = loader.MeshData.Verts;
                var normals = loader.MeshData.bHasNormals ? loader.MeshData.N_Verts : null;
                var texCoords = loader.MeshData.T_Verts;
                var tangents = loader.MeshData.bHasTangentVertices ? loader.MeshData.Tangent_Verts : VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords);
                var bitangents = loader.MeshData.bHasTangentVertices ? loader.MeshData.Bitanget_Verts : VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords);

                UInt32[] indices = loader.MeshData.bHasIndices ? loader.MeshData.Indices.ToArray() : null;

                IndexBufferObject ibo = null;
                VertexBufferObjectTwoDimension<float> normalsVBO = null, texCoordsVBO = null, tangentsVBO = null, bitangentsVBO = null;

                if (loader.MeshData.bHasIndices)
                    ibo = new IndexBufferObject(indices);

                VertexBufferObjectTwoDimension<float> vertexVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

                if (loader.MeshData.bHasNormals)
                    normalsVBO = new VertexBufferObjectTwoDimension<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (loader.MeshData.bHasNormals)
                    texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (loader.MeshData.bHasNormals)
                    tangentsVBO = new VertexBufferObjectTwoDimension<float>(tangents, BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (loader.MeshData.bHasNormals)
                    bitangentsVBO = new VertexBufferObjectTwoDimension<float>(bitangents, BufferTarget.ArrayBuffer, 5, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

                vao.AddVBO(vertexVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO);
                vao.AddIndexBuffer(ibo);
                vao.BindBuffersToVao();
            }

            return vao;
        }

        public override void FreeMemory(VertexArrayObject arg)
        {
            arg.CleanUp();
        }
    }
}
