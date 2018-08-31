using Assimp;
using CParser.Assimp.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CParser.Assimp
{
    public class MeshVertexData
    {
        private Scene m_scene;
        private Mesh[] m_meshes;
        private SkeletonPerVertexBoneInfluenceStrategy m_strategy;

        public bool bHasIndices = false;
        public bool bHasNormals = false;
        public bool bHasTextureCoordinates = false;
        public bool bHasTangentVertices = false;
        public bool bHasAnimation = false;

        public List<UInt32> Indices { get; private set; }
        public float[,] Verts { get; private set; }
        public float[,] T_Verts { get; private set; }
        public float[,] N_Verts { get; private set; }
        public float[,] Tangent_Verts { get; private set; }
        public float[,] Bitanget_Verts { get; private set; }

        public float[,] BlendWeights { get; private set; }
        public Int32[,] BlendIndices { get; private set; }

        public LoaderSkeletonParentBone SkeletonRoot { get; private set; }

        public void CleanUp()
        {
            m_scene = null;
            m_meshes = null;
            Indices = null;
            Verts = null;
            T_Verts = null;
            N_Verts = null;
            Tangent_Verts = null;
            Bitanget_Verts = null;
            BlendWeights = null;
            BlendIndices = null;
            SkeletonRoot = null;
        }

        public MeshVertexData(Scene scene, SkeletonPerVertexBoneInfluenceStrategy strategy)
        {
            m_strategy = strategy;
            m_scene = scene;
            m_meshes = scene.Meshes;
            GetMeshVertexData();
        }

        private void GetMeshVertexData()
        {
            bHasIndices = m_meshes.Any(mesh => mesh.HasFaces);
            bHasNormals = m_meshes.Any(mesh => mesh.HasNormals);
            bHasTextureCoordinates = m_meshes.Any(mesh => mesh.HasTextureCoords(0));
            bHasTangentVertices = m_meshes.Any(mesh => mesh.HasTangentBasis);
            bHasAnimation = false;
                //m_scene.HasAnimations;

            Indices = bHasIndices ? new List<UInt32>() : null;

            GetSkeleton();
            GetSkin();
        }

        private void GetSkin()
        {
            Int32 countOfVertices = 0;
            foreach (Mesh mesh in m_scene.Meshes)
            {
                countOfVertices += mesh.VertexCount;
            }

            List<UInt32> countOfIndicesPerMesh = new List<UInt32>();

            float[,] local_vert = new float[countOfVertices, 3],
            local_t_vert = bHasTextureCoordinates ? new float[countOfVertices, 2] : null,
            local_n_vert = bHasNormals ? new float[countOfVertices, 3] : null,
            local_tangent_vert = bHasTangentVertices ? new float[countOfVertices, 3] : null,
            local_bitangent_vert = bHasTangentVertices ? new float[countOfVertices, 3] : null;

            for (Int32 i = 0; i < m_meshes.Length; i++)
            {
                Mesh mesh = m_meshes[i];
                countOfIndicesPerMesh.Add((UInt32)Indices.Count);
                CollectIndices(Indices, mesh, (UInt32)Indices.Count);
                TryToCollectSkinInfo((Int32)countOfIndicesPerMesh[i], ref local_vert, ref local_n_vert, ref local_t_vert, ref local_tangent_vert, ref local_bitangent_vert, mesh);
            }

            Verts = local_vert;
            N_Verts = local_n_vert;
            T_Verts = local_t_vert;
            Tangent_Verts = local_tangent_vert;
            Bitanget_Verts = local_bitangent_vert;

            // collect blend weights and blend ids
            if (bHasAnimation)
            {
                List<Vertex> blendData = new List<Vertex>();
                for (Int32 j = 0; j < countOfVertices; j++)
                {
                    for (Int32 i = 0; i < m_meshes.Length; i++)
                    {
                        Mesh mesh = m_meshes[i];
                        CollectBlendables(j, blendData, mesh, countOfIndicesPerMesh);
                    }
                }

                BlendWeights = new float[blendData.Count, m_strategy.GetBonesInfluenceCount()];
                BlendIndices = new int[blendData.Count, m_strategy.GetBonesInfluenceCount()];

                for (Int32 i = 0; i < blendData.Count; i++)
                {
                    Vertex blendVertex = blendData[i];
                    m_strategy.CollectWeightsAndIndices(blendVertex, i, BlendWeights, BlendIndices);
                }
            }
        }

        private void GetSkeleton()
        {
            if (bHasAnimation)
            {
                SkeletonRoot = new LoaderSkeletonParentBone();
                Int32 boneIdCounter = 0;

                Node rootNode = m_scene.RootNode.FindNode(GetSkeletonArmatureNode(m_scene.RootNode));
                if (rootNode != null)
                {
                    foreach (var node in rootNode.Children)
                    {
                        LoaderSkeletonBone skeletonBone = new LoaderSkeletonBone(SkeletonRoot);
                        skeletonBone.SetBoneId(boneIdCounter++);
                        var bone = GetBoneByName(node.Name);
                        skeletonBone.SetBoneInfo(bone);
                        FillHierarchyRecursive(node, skeletonBone, ref boneIdCounter);
                        SkeletonRoot.AddChildBone(skeletonBone);
                    }
                }
            }
        }

        private void FillHierarchyRecursive(Node parentNode, LoaderSkeletonBone parentBone, ref Int32 boneIdCounter)
        {
            if (parentNode.HasChildren)
            {
                foreach (Node child in parentNode.Children)
                {
                    LoaderSkeletonBone childBone = new LoaderSkeletonBone(parentBone);
                    parentBone.AddChildBone(childBone);
                    var boneInfo = GetBoneByName(child.Name);
                    childBone.SetBoneInfo(boneInfo);
                    childBone.SetBoneId(boneIdCounter++);
                    FillHierarchyRecursive(child, childBone, ref boneIdCounter);
                }
            }
        }

        private string GetSkeletonArmatureNode(Node rootNode)
        {
            string name = string.Empty;

            Int32 maxHierarchySize = 0;

            foreach (Node childNode in rootNode.Children)
            {
                Int32 currentChildHierarchySize = 0;
                IterateHierarchy(childNode, ref currentChildHierarchySize);
                if (currentChildHierarchySize > maxHierarchySize)
                {
                    maxHierarchySize = currentChildHierarchySize;
                    name = childNode.Name;
                }
            }

            return name;
        }

        private void IterateHierarchy(Node parentNode, ref Int32 countChildren)
        {
            if (parentNode.HasChildren)
            {
                foreach (Node childNode in parentNode.Children)
                {
                    countChildren++;
                    IterateHierarchy(childNode, ref countChildren);
                }
            }
        }

        private Bone GetBoneByName(string name)
        {
            Bone result = null;

            foreach (var mesh in m_meshes)
            {
                var localBone = mesh.Bones.First(bone => bone.Name == name);
                if (localBone != null)
                {
                    result = localBone;
                    break;
                }
            }

            return result;
        }

        private void CollectIndices(List<UInt32> indices, Mesh meshBeingCollected, UInt32 lastIndexBeenInterrupted)
        {
            foreach (var face in meshBeingCollected.Faces)
            {
                if (face.IndexCount == 3)
                {
                    indices.Add(face.Indices[0] + lastIndexBeenInterrupted);
                    indices.Add(face.Indices[1] + lastIndexBeenInterrupted);
                    indices.Add(face.Indices[2] + lastIndexBeenInterrupted);
                }
                else
                {
                    throw new ArgumentException("Mesh isn't triangulated.");
                }
            }
        }

        private void TryToCollectSkinInfo(Int32 startIndex, ref float[,] vertices, ref float[,] normals, ref float[,] texCoords,
            ref float[,] tangents, ref float[,] bitangents, Mesh meshBeingCollected)
        {
            for (Int32 i = 0; i < meshBeingCollected.VertexCount; ++i)
            {
                Int32 currentVertexIndex = startIndex + i;
                vertices[currentVertexIndex, 0] = meshBeingCollected.Vertices[i][0];
                vertices[currentVertexIndex, 1] = meshBeingCollected.Vertices[i][1];
                vertices[currentVertexIndex, 2] = meshBeingCollected.Vertices[i][2];

                if (bHasNormals)
                {
                    normals[currentVertexIndex, 0] = meshBeingCollected.Normals[i][0];
                    normals[currentVertexIndex, 1] = meshBeingCollected.Normals[i][1];
                    normals[currentVertexIndex, 2] = meshBeingCollected.Normals[i][2];
                }
                if (bHasTextureCoordinates)
                {
                    texCoords[currentVertexIndex, 0] = meshBeingCollected.GetTextureCoords(0)[i][0];
                    texCoords[currentVertexIndex, 1] = meshBeingCollected.GetTextureCoords(0)[i][1];
                }
                if (bHasTangentVertices)
                {
                    tangents[currentVertexIndex, 0] = meshBeingCollected.Tangents[i][0];
                    tangents[currentVertexIndex, 1] = meshBeingCollected.Tangents[i][1];
                    tangents[currentVertexIndex, 2] = meshBeingCollected.Tangents[i][2];

                    bitangents[currentVertexIndex, 0] = meshBeingCollected.BiTangents[i][0];
                    bitangents[currentVertexIndex, 1] = meshBeingCollected.BiTangents[i][1];
                    bitangents[currentVertexIndex, 2] = meshBeingCollected.BiTangents[i][2];
                }
            }
        }

        private void CollectBlendables(Int32 vertexId, List<Vertex> blendData, Mesh meshBeingCollected, List<UInt32> countOfIndicesPerMesh)
        {
            Vertex vertex = new Vertex(vertexId);
            blendData.Add(vertex);

            foreach (var bone in meshBeingCollected.Bones)
            {
                foreach (var weight in bone.VertexWeights)
                {
                    Int32 i = 0;
                    while (meshBeingCollected != m_meshes[i])
                    {
                        i++;
                    }
                    if (weight.VertexID == vertexId + countOfIndicesPerMesh[i])
                    {
                        Int32 boneId = SkeletonRoot.GetIdByBoneInHierarchy(bone);
                        if (boneId < 0)
                            throw new ArgumentException("Such bone doesn't exist in skeleton!");

                        vertex.AddBoneWeight(new KeyValuePair<Tuple<Bone, int>, float>(new Tuple<Bone, Int32>(bone, boneId), weight.Weight));
                    }
                }
            }
        }
    }

}
