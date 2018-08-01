using System;
using OpenTK.Graphics.OpenGL;
using CParser;
using VBO;
using CParser.Assimp;
using MassiveGame.API.Mesh;
using MassiveGame.Core.AnimationCore;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class ModelAllocationPolicy : AllocationPolicy<string, Skin>
    {
        public override Skin AllocateMemory(string arg)
        {
            Skin resultSkin = null;
            resultSkin = SendDataToGpu(arg);
            return resultSkin;
        }

        private Skin SendDataToGpu(string modelPath)
        {
            Skin resultSkin = null;
            
            // Get mesh data 
            using (AssimpModelLoader loader = new AssimpModelLoader(modelPath))
            {
                VertexArrayObject vao = new VertexArrayObject();
                MeshVertexData meshData = loader.GetMeshData();
                // explicit null assignment if there is no some of mesh data
                var vertices = meshData.Verts;
                var normals = meshData.bHasNormals ? meshData.N_Verts : null;
                var texCoords = meshData.T_Verts;
                var tangents = meshData.bHasTangentVertices ? meshData.Tangent_Verts : VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords);
                var bitangents = meshData.bHasTangentVertices ? meshData.Bitanget_Verts : VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords);

                UInt32[] indices = meshData.bHasIndices ? meshData.Indices.ToArray() : null;

                IndexBufferObject ibo = null;
                VertexBufferObjectTwoDimension<float> normalsVBO = null, texCoordsVBO = null, tangentsVBO = null, bitangentsVBO = null;

                if (meshData.bHasIndices)
                    ibo = new IndexBufferObject(indices);

                VertexBufferObjectTwoDimension<float> vertexVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

                if (meshData.bHasNormals)
                    normalsVBO = new VertexBufferObjectTwoDimension<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasNormals)
                    texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasNormals)
                    tangentsVBO = new VertexBufferObjectTwoDimension<float>(tangents, BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasNormals)
                    bitangentsVBO = new VertexBufferObjectTwoDimension<float>(bitangents, BufferTarget.ArrayBuffer, 5, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

                vao.AddVBO(vertexVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO);
                vao.AddIndexBuffer(ibo);
                vao.BindBuffersToVao();

                if (meshData.bHasAnimation)
                {
                    Bone rootBone = AssimpConverter.Converter.ConvertAssimpBoneToEngineBone(meshData.SkeletonRoot);
                    resultSkin = new AnimatedSkin(vao, rootBone);
                }
                else
                {
                    resultSkin = new Skin(vao);
                }
            }

            return resultSkin;
        }

        public override void FreeMemory(Skin arg)
        {
            arg.CleanUp();
        }
    }
}
