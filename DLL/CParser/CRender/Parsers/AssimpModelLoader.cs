﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;

namespace CParser
{
    public class AssimpModelLoader
    {
        public AssimpModelLoader()
        {
            imorter = new AssimpImporter();
            scene = null;
            meshes = null;
        }

        public AssimpModelLoader(string modelFilePath) : this()
        {
            LoadModel(modelFilePath);
            CleanUp();
        }


        private AssimpImporter imorter;
        private Scene scene;
        private Mesh[] meshes;

        public bool bHasIndices;
        public float[,] Verts { get; private set; }
        public float[,] T_Verts { get; private set; }
        public float[,] N_Verts { get; private set; }
        public List<UInt32> Indices { get; private set; }


        public void LoadModel(string modelFilePath)
        {
            List<float[]> vert = new List<float[]>(),
                          t_vert = new List<float[]>(),
                          n_vert = new List<float[]>();
            scene = imorter.ImportFile(modelFilePath, PostProcessSteps.FlipUVs);
            meshes = scene.Meshes;
            Indices = new List<UInt32>();


            UInt32 indexCount = 0;
            foreach (Mesh mesh in meshes)
            {
                foreach (var face in mesh.Faces)
                {
                    if (face.Indices.Any( index => index == 0))
                    {
                        indexCount = (uint)Indices.Count;
                    }

                    if (face.IndexCount == 4)
                    {
                        var p1 = face.Indices[0] + indexCount;
                        var p2 = face.Indices[1] + indexCount;
                        var p3 = face.Indices[2] + indexCount;
                        var p4 = face.Indices[2] + indexCount;
                        var p5 = face.Indices[3] + indexCount;
                        var p6 = face.Indices[1] + indexCount;

                        Indices.AddRange(new UInt32[] { p1, p2, p3, p4, p5, p6 });
                    }
                    else if (face.IndexCount == 3)
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

                bHasIndices = Indices.Count != 0;
                
                for (Int32 i = 0; i < mesh.VertexCount; ++i)
                {
                    float[] temp = new float[3];
                    temp[0] = mesh.Vertices[i].X;
                    temp[1] = mesh.Vertices[i].Y;
                    temp[2] = mesh.Vertices[i].Z;
                    vert.Add(temp);
                }

                for (Int32 i = 0; i < mesh.Normals.Length; ++i)
                {
                    float[] temp = new float[3];
                    temp[0] = mesh.Normals[i].X;
                    temp[1] = mesh.Normals[i].Y;
                    temp[2] = mesh.Normals[i].Z;
                    n_vert.Add(temp);
                }

                for (Int32 i = 0; i < mesh.GetTextureCoords(0).Length; ++i)
                {
                    float[] temp = new float[2];
                    temp[0] = mesh.GetTextureCoords(0)[i].X;
                    temp[1] = mesh.GetTextureCoords(0)[i].Y;
                    t_vert.Add(temp);
                }

            }

            //for (Int32 i = 0; i < Indices.Count; i++)
            //{
            //    if (Indices[i] != i)
            //    {
            //        throw new Exception();
            //    }
            //}

            ConvertListToArray(vert, t_vert, n_vert);
        }

        private void ConvertListToArray(List<float[]> vert, List<float[]> t_vert, List<float[]> n_vert)
        {
            Verts = new float[vert.Count, 3];
            N_Verts = new float[n_vert.Count, 3];
            T_Verts = new float[t_vert.Count, 2];

            if ((t_vert.Count == n_vert.Count) && (n_vert.Count == vert.Count))
            {
                for (Int32 i = 0; i < vert.Count; ++i)
                {
                    Verts[i, 0] = vert[i][0];
                    Verts[i, 1] = vert[i][1];
                    Verts[i, 2] = vert[i][2];

                    N_Verts[i, 0] = n_vert[i][0];
                    N_Verts[i, 1] = n_vert[i][1];
                    N_Verts[i, 2] = n_vert[i][2];

                    T_Verts[i, 0] = t_vert[i][0];
                    T_Verts[i, 1] = t_vert[i][1];
                }
            }
            else
            {
                for (Int32 i = 0; i < vert.Count; ++i)
                {
                    Verts[i, 0] = vert[i][0];
                    Verts[i, 1] = vert[i][1];
                    Verts[i, 2] = vert[i][2];
                }

                for (Int32 i = 0; i < n_vert.Count; ++i)
                {
                    N_Verts[i, 0] = n_vert[i][0];
                    N_Verts[i, 1] = n_vert[i][1];
                    N_Verts[i, 2] = n_vert[i][2];
                }

                for (Int32 i = 0; i < t_vert.Count; ++i)
                {
                    T_Verts[i, 0] = t_vert[i][0];
                    T_Verts[i, 1] = t_vert[i][1];
                }
            }
        }

        public void CleanUp()
        {
            imorter = null;
            scene = null;
            meshes = null;
        }
    }
}
