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
                var tangents = meshData.bHasTangentVertices ? meshData.Tangent_Verts : meshData.bHasTextureCoordinates ? VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords) : null;
                var bitangents = meshData.bHasTangentVertices ? meshData.Bitanget_Verts : meshData.bHasTextureCoordinates ? VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords) : null;
                var blendWeights = meshData.bHasAnimation ? meshData.BlendWeights : null;
                var blendIndices = meshData.bHasAnimation ? meshData.BlendIndices : null;

                UInt32[] indices = meshData.bHasIndices ? meshData.Indices.ToArray() : null;

                IndexBufferObject ibo = null;
                VertexBufferObjectTwoDimension<float> normalsVBO = null, texCoordsVBO = null, tangentsVBO = null, bitangentsVBO = null, blendWeightsVBO = null;
                VertexBufferObjectTwoDimension<Int32> blendIndicesVBO = null;

                if (meshData.bHasIndices)
                    ibo = new IndexBufferObject(indices);

                VertexBufferObjectTwoDimension<float> vertexVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

                if (meshData.bHasNormals)
                    normalsVBO = new VertexBufferObjectTwoDimension<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasTextureCoordinates)
                    texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasTangentVertices)
                    tangentsVBO = new VertexBufferObjectTwoDimension<float>(tangents, BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasTangentVertices)
                    bitangentsVBO = new VertexBufferObjectTwoDimension<float>(bitangents, BufferTarget.ArrayBuffer, 5, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                if (meshData.bHasAnimation)
                {
                    Int32 skeletonWeightsPerVertexCount = (Int32)loader.m_skeletonType;
                    if (skeletonWeightsPerVertexCount > 4)
                        throw new NotImplementedException("There is no implementation yet for cases when there are more than four weights influencing on vertex.");

                    Int32 vectorSize = skeletonWeightsPerVertexCount;
                    blendWeightsVBO = new VertexBufferObjectTwoDimension<float>(blendWeights, BufferTarget.ArrayBuffer, 6, vectorSize, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                    blendIndicesVBO = new VertexBufferObjectTwoDimension<int>(blendIndices, BufferTarget.ArrayBuffer, 7, vectorSize, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                }

                vao.AddVBO(vertexVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO, blendWeightsVBO, blendIndicesVBO);
                vao.AddIndexBuffer(ibo);
                vao.BindBuffersToVao();

                if (meshData.bHasAnimation)
                {
                    ParentBone rootBone = AssimpConverter.Converter.ConvertAssimpBoneToEngineBone(meshData.SkeletonRoot);
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
