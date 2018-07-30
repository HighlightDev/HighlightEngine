using System;
using System.Collections.Generic;
using System.Linq;
using Assimp;
using CParser.Assimp;

namespace CParser
{
    public class AssimpModelLoader
    {
        public enum SkeletonPerVertexBoneType
        {
            ThreeBones = 3,
            FourBones = 4,
            FiveBones = 5,
            SixBones = 6
        }

        public const SkeletonPerVertexBoneType skeletonType = SkeletonPerVertexBoneType.ThreeBones;

        public AssimpModelLoader()
        {
            importer = new AssimpImporter();
            scene = null;
            meshes = null;
        }

        public AssimpModelLoader(string modelFilePath) : this()
        {
            LoadModel(modelFilePath);
            CleanUp();
        }

        private AssimpImporter importer;
        private Scene scene;
        private Mesh[] meshes;

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

        public SkeletonBone RootBone { get; private set; }

        private Bone GetBoneByName(string name)
        {
            Bone result = null;

            foreach (var mesh in meshes)
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

        private SkeletonBone GetSkeleton()
        {
            SkeletonBone root = new SkeletonBone(null);
            if (bHasAnimation)
            {
                Int32 boneIdCounter = 0;

                Node rootNode = scene.RootNode.FindNode("Armature")?.Children.First();
                if (rootNode != null)
                {
                    root.SetBoneId(boneIdCounter++);
                    var bone = GetBoneByName(rootNode.Name);
                    root.SetBoneInfo(bone);
                    FillHierarchyRecursive(rootNode, root, ref boneIdCounter);
                }
            }
            return root;
        }

        private void FillHierarchyRecursive(Node parentNode, SkeletonBone parentBone, ref Int32 boneIdCounter)
        {
            if (parentNode.HasChildren)
            {
                foreach (Node child in parentNode.Children)
                {
                    SkeletonBone childBone = new SkeletonBone(parentBone);
                    parentBone.AddChildBone(childBone);
                    var boneInfo = GetBoneByName(child.Name);
                    childBone.SetBoneInfo(boneInfo);
                    childBone.SetBoneId(boneIdCounter++);
                    FillHierarchyRecursive(child, childBone, ref boneIdCounter);
                }
            }
        }

        private void GetSkin()
        {
            List<Vector3D> vert = new List<Vector3D>(),
            t_vert = bHasTextureCoordinates ? new List<Vector3D>() : null,
            n_vert = bHasNormals ? new List<Vector3D>() : null,
            tangent_vert = bHasTangentVertices ? new List<Vector3D>() : null,
            bitangent_vert = bHasTangentVertices ? new List<Vector3D>() : null;
            Indices = bHasIndices ? new List<UInt32>() : null;

            UInt32 indexCount = 0;
            for (Int32 i = 0; i < meshes.Length; i++)
            {
                Mesh mesh = meshes[i];
                foreach (var face in mesh.Faces)
                {
                    if (face.Indices.Any(index => index == 0))
                    {
                        indexCount = (uint)Indices.Count;
                    }

                    if (face.IndexCount == 3)
                    {
                        Indices.Add(face.Indices[0] + indexCount);
                        Indices.Add(face.Indices[1] + indexCount);
                        Indices.Add(face.Indices[2] + indexCount);
                    }
                    else
                    {
                        throw new Exception("Unusual staff!");
                    }
                }

                for (Int32 j = 0; j < mesh.VertexCount; ++j)
                {
                    vert.Add(mesh.Vertices[j]);
                    if (bHasNormals)
                    {
                        n_vert.Add(mesh.Normals[j]);
                    }
                    if (bHasTextureCoordinates)
                    {
                        t_vert.Add(mesh.GetTextureCoords(0)[j]);
                    }
                    if (bHasTangentVertices)
                    {
                        tangent_vert.Add(mesh.Tangents[j]);
                        bitangent_vert.Add(mesh.Tangents[j]);
                    }
                }

            }

            Verts = new float[vert.Count, 3];
            if (bHasNormals)
                N_Verts = new float[n_vert.Count, 3];

            if (bHasTextureCoordinates)
                T_Verts = new float[t_vert.Count, 2];

            if (bHasTangentVertices)
            {
                Tangent_Verts = new float[tangent_vert.Count, 3];
                Bitanget_Verts = new float[tangent_vert.Count, 3];
            }

            for (Int32 i = 0; i < vert.Count; ++i)
            {
                Verts[i, 0] = vert[i][0]; Verts[i, 1] = vert[i][1]; Verts[i, 2] = vert[i][2];
                if (bHasNormals)
                {
                    N_Verts[i, 0] = n_vert[i][0];
                    N_Verts[i, 1] = n_vert[i][1];
                    N_Verts[i, 2] = n_vert[i][2];
                }
                if (bHasTextureCoordinates)
                {
                    T_Verts[i, 0] = t_vert[i][0];
                    T_Verts[i, 1] = t_vert[i][1];
                }
                if (bHasTangentVertices)
                {
                    Tangent_Verts[i, 0] = tangent_vert[i][0];
                    Tangent_Verts[i, 1] = tangent_vert[i][1];
                    Tangent_Verts[i, 2] = tangent_vert[i][2];
                    Bitanget_Verts[i, 0] = bitangent_vert[i][0];
                    Bitanget_Verts[i, 1] = bitangent_vert[i][1];
                    Bitanget_Verts[i, 2] = bitangent_vert[i][2];
                }
            }

            if (bHasAnimation)
            {
                List<Vertex> blend_vertices = new List<Vertex>();
                for (Int32 j = 0; j < vert.Count; j++)
                {
                    for (Int32 i = 0; i < meshes.Length; i++)
                    {
                        Mesh mesh = meshes[i];
                        Vertex vertex = null;

                        vertex = new Vertex(j);
                        blend_vertices.Add(vertex);
                        foreach (var bone in mesh.Bones)
                        {
                            foreach (var weight in bone.VertexWeights)
                            {
                                Int32 meshOffset = 0;
                                for (Int32 k = 0; k < i; k++)
                                {
                                    meshOffset += meshes[k].Vertices.Length;
                                }
                                if (weight.VertexID == j + meshOffset)
                                {
                                    Int32 boneId = RootBone.GetIdByBone(bone);
                                    if (boneId < 0)
                                        throw new Exception("such bone doesn't exist in skeleton!");

                                    vertex.AddBoneWeight(new KeyValuePair<Tuple<Bone, int>, float>(new Tuple<Bone, Int32>(bone, boneId), weight.Weight));
                                }
                            }
                        }

                    }
                }

                blend_vertices.ToList();

                BlendWeights = new float[blend_vertices.Count, (Int32)skeletonType];
                BlendIndices = new int[blend_vertices.Count, (Int32)skeletonType];

                for (Int32 i = 0; i < blend_vertices.Count; i++)
                {
                    Vertex blendVertex = blend_vertices[i];
                    Int32 actualInfluenceCount = blendVertex.BoneWeightMap.Count;
                    if (skeletonType == SkeletonPerVertexBoneType.ThreeBones)
                    {
                        if (actualInfluenceCount == 1)
                        {
                            BlendWeights[i, 0] = blendVertex.BoneWeightMap[0].Value;
                            BlendIndices[i, 0] = blendVertex.BoneWeightMap[0].Key.Item2;

                            BlendWeights[i, 1] = 0.0f; // this provides assurance that skin matrix will not do anything in shader.
                            BlendIndices[i, 1] = 0; // take root matrix

                            BlendWeights[i, 2] = 0.0f; // this provides assurance that skin matrix will not do anything in shader.
                            BlendIndices[i, 2] = 0; // take root matrix
                        }
                        else if (actualInfluenceCount == 2)
                        {
                            BlendWeights[i, 0] = blendVertex.BoneWeightMap[0].Value;
                            BlendIndices[i, 0] = blendVertex.BoneWeightMap[0].Key.Item2;

                            BlendWeights[i, 1] = blendVertex.BoneWeightMap[1].Value;
                            BlendIndices[i, 1] = blendVertex.BoneWeightMap[1].Key.Item2;

                            BlendWeights[i, 2] = 0.0f; // this provides assurance that skin matrix will not do anything in shader.
                            BlendIndices[i, 2] = 0; // take root matrix
                        }
                        else if (actualInfluenceCount == 3)
                        {
                            BlendWeights[i, 0] = blendVertex.BoneWeightMap[0].Value;
                            BlendIndices[i, 0] = blendVertex.BoneWeightMap[0].Key.Item2;

                            BlendWeights[i, 1] = blendVertex.BoneWeightMap[1].Value;
                            BlendIndices[i, 1] = blendVertex.BoneWeightMap[1].Key.Item2;

                            BlendWeights[i, 2] = blendVertex.BoneWeightMap[2].Value;
                            BlendIndices[i, 2] = blendVertex.BoneWeightMap[2].Key.Item2;
                        }
                    }

                }
            }
        }

        public void LoadModel(string modelFilePath)
        {
            importer.XAxisRotation = 270.0f;
            scene = importer.ImportFile(modelFilePath, PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            meshes = scene.Meshes;

            bHasIndices = meshes.Any(mesh => mesh.HasFaces);
            bHasNormals = meshes.Any(mesh => mesh.HasNormals);
            bHasTextureCoordinates = meshes.Any(mesh => mesh.HasTextureCoords(0));
            bHasTangentVertices = meshes.Any(mesh => mesh.HasTangentBasis);
            bHasAnimation = scene.HasAnimations;

            RootBone = GetSkeleton();
            GetSkin();
        }

        public void CleanUp()
        {
            importer = null;
            scene = null;
            meshes = null;
        }
    }
}
