﻿using GpuGraphics;
using Grid;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMath;

namespace MassiveGame
{
    public static class TerrainBuilder
    {
        #region LoadTerrain

        /*   Загружает карту высот с текстуры , работает в связке с 
         * getLandscapeHeight(int,int)                           */
        static public void loadHeightMap(Bitmap bmp, Terrain terrain)
        {
            for (int i = 0; i < terrain.LandscapeMap.TableSize; i++)
            {
                for (int j = 0; j < terrain.LandscapeMap.TableSize; j++)
                {
                    terrain.LandscapeMap.Table[i, j] = Convert.ToInt32(generateMapHeight(i, j, bmp, terrain));
                }
            }
        }

        static public float generateMapHeight(int x, int z, Bitmap bmp, Terrain terrain)
        {
            float height = bmp.GetPixel(x, z).R + bmp.GetPixel(x, z).G + bmp.GetPixel(x, z).B;
            height /= Terrain.MAX_PIXEL_COLOUR / 2;
            height *= terrain.MaximumHeight;
            return height;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Generate smooth normals for terrain
        /// </summary>
        /// <param name="normalVector">2-dimension array of vectors with normals</param>
        /// <param name="x">x - position on heightmap</param>
        /// <param name="z">z - position on heightmap</param>
        /// <param name="smoothLvl">level of normal smooth</param>
        /// <returns></returns>
        static public Vector3 getLandscapeSmoothNormal(TableGrid LandscapeMap, Vector3[,] normalVector, int x, int z, int smoothLvl)
        {
            try
            {
                if (smoothLvl < 0)
                {
                    throw new ArgumentException("Smooth level of normal can't be less then zero!");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                smoothLvl = 0;
                normalVector = new Vector3[1, 3] 
                { 
                    { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) }
                };
            }

            Vector3 normal = new Vector3();
            if (smoothLvl == 0)
            {
                normal = new Vector3(normalVector[x, z]);
            }
            else
            {
                // Multivertex smoothing normals //
                Vector3[] heightL = new Vector3[smoothLvl],
                heightR = new Vector3[smoothLvl],
                heightD = new Vector3[smoothLvl],
                heightU = new Vector3[smoothLvl];
                for (int i = 0; i < smoothLvl; i++)
                {
                    heightL[i] = new Vector3(x - (smoothLvl - i) >= 0 ? normalVector[x - (smoothLvl - i), z] : normalVector[x, z]);
                    heightR[i] = new Vector3(x + (smoothLvl - i) < LandscapeMap.TableSize ? normalVector[x + (smoothLvl - i), z] : normalVector[x, z]);
                    heightD[i] = new Vector3(z - (smoothLvl - i) >= 0 ? normalVector[x, z - (smoothLvl - i)] : normalVector[x, z]);
                    heightU[i] = new Vector3(z + (smoothLvl - i) < LandscapeMap.TableSize ? normalVector[x, z + (smoothLvl - i)] : normalVector[x, z]);
                    normal += heightL[i];
                    normal += heightR[i];
                    normal += heightD[i];
                    normal += heightU[i];
                }
                normal.Normalize();
            }
            return normal;
        }

        static public VBOArrayF getTerrainAttributes(TableGrid LandscapeMap, int normalSmoothLvl)
        {
            int VERTEX_COUNT = LandscapeMap.TableSize - 1;
            float x, z;
            float[,] vertices = new float[(VERTEX_COUNT * VERTEX_COUNT) * 6, 3];
            float[,] texCoords = new float[(VERTEX_COUNT * VERTEX_COUNT) * 6, 2];
            float[,] normals = new float[(VERTEX_COUNT * VERTEX_COUNT) * 6, 3];
            int vertexPointer = 0;
            Vector3 tempNormal;
            Vector3[,] normalMatrix = new Vector3[LandscapeMap.TableSize, LandscapeMap.TableSize];

            for (int i = 0; i < LandscapeMap.TableSize - 1; i++)
            {
                for (int j = 0; j < LandscapeMap.TableSize - 1; j++)
                {
                    x = i * (float)LandscapeMap.GridStep;
                    z = j * (float)LandscapeMap.GridStep;

                    vertices[vertexPointer, 0] = x;
                    vertices[vertexPointer, 1] = LandscapeMap.Table[i, j];
                    vertices[vertexPointer, 2] = z;
                    tempNormal = VectorMath.Normal(new Vector3[3] { new Vector3(x, LandscapeMap.Table[i, j], z),
                        new Vector3(x, LandscapeMap.Table[i, j + 1], z + (float)LandscapeMap.GridStep),
                        new Vector3(x + (float)LandscapeMap.GridStep, LandscapeMap.Table[i + 1, j], z)});
                    normalMatrix[i, j] = tempNormal;
                    //tempNormal = getLandscapeNormal(i, j);
                    normals[vertexPointer, 0] = tempNormal.X;
                    normals[vertexPointer, 1] = tempNormal.Y;
                    normals[vertexPointer, 2] = tempNormal.Z;
                    texCoords[vertexPointer, 0] = vertices[vertexPointer, 0] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    texCoords[vertexPointer, 1] = vertices[vertexPointer, 2] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    vertices[vertexPointer + 1, 0] = x;
                    vertices[vertexPointer + 1, 1] = LandscapeMap.Table[i, j + 1];
                    vertices[vertexPointer + 1, 2] = z + (float)LandscapeMap.GridStep;
                    tempNormal = VectorMath.Normal(new Vector3[3] { new Vector3(x, LandscapeMap.Table[i, j + 1], z + (float)LandscapeMap.GridStep),
                        new Vector3(x + (float)LandscapeMap.GridStep, LandscapeMap.Table[i + 1, j], z),
                        new Vector3(x, LandscapeMap.Table[i, j], z)});
                    normalMatrix[i, j + 1] = tempNormal;
                    //tempNormal = getLandscapeNormal(i, j + 1);
                    normals[vertexPointer + 1, 0] = tempNormal.X;
                    normals[vertexPointer + 1, 1] = tempNormal.Y;
                    normals[vertexPointer + 1, 2] = tempNormal.Z;
                    texCoords[vertexPointer + 1, 0] = vertices[vertexPointer + 1, 0] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    texCoords[vertexPointer + 1, 1] = vertices[vertexPointer + 1, 2] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    vertices[vertexPointer + 2, 0] = x + (float)LandscapeMap.GridStep;
                    vertices[vertexPointer + 2, 1] = LandscapeMap.Table[i + 1, j];
                    vertices[vertexPointer + 2, 2] = z;
                    tempNormal = VectorMath.Normal(new Vector3[3] { new Vector3(x + (float)LandscapeMap.GridStep, LandscapeMap.Table[i + 1, j], z),
                        new Vector3(x, LandscapeMap.Table[i, j], z),
                        new Vector3(x, LandscapeMap.Table[i, j + 1], z + (float)LandscapeMap.GridStep)});
                    normalMatrix[i + 1, j] = tempNormal;
                    //tempNormal = getLandscapeNormal(i + 1, j);
                    normals[vertexPointer + 2, 0] = tempNormal.X;
                    normals[vertexPointer + 2, 1] = tempNormal.Y;
                    normals[vertexPointer + 2, 2] = tempNormal.Z;
                    texCoords[vertexPointer + 2, 0] = vertices[vertexPointer + 2, 0] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    texCoords[vertexPointer + 2, 1] = vertices[vertexPointer + 2, 2] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    vertices[vertexPointer + 3, 0] = x + (float)LandscapeMap.GridStep;
                    vertices[vertexPointer + 3, 1] = LandscapeMap.Table[i + 1, j];
                    vertices[vertexPointer + 3, 2] = z;
                    normals[vertexPointer + 3, 0] = normals[vertexPointer + 2, 0];
                    normals[vertexPointer + 3, 1] = normals[vertexPointer + 2, 1];
                    normals[vertexPointer + 3, 2] = normals[vertexPointer + 2, 2];
                    texCoords[vertexPointer + 3, 0] = vertices[vertexPointer + 3, 0] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    texCoords[vertexPointer + 3, 1] = vertices[vertexPointer + 3, 2] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    vertices[vertexPointer + 4, 0] = x;
                    vertices[vertexPointer + 4, 1] = LandscapeMap.Table[i, j + 1];
                    vertices[vertexPointer + 4, 2] = z + (float)LandscapeMap.GridStep;
                    normals[vertexPointer + 4, 0] = normals[vertexPointer + 1, 0];
                    normals[vertexPointer + 4, 1] = normals[vertexPointer + 1, 1];
                    normals[vertexPointer + 4, 2] = normals[vertexPointer + 1, 2];
                    texCoords[vertexPointer + 4, 0] = vertices[vertexPointer + 4, 0] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    texCoords[vertexPointer + 4, 1] = vertices[vertexPointer + 4, 2] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    vertices[vertexPointer + 5, 0] = x + (float)LandscapeMap.GridStep;
                    vertices[vertexPointer + 5, 1] = LandscapeMap.Table[i + 1, j + 1];
                    vertices[vertexPointer + 5, 2] = z + (float)LandscapeMap.GridStep;
                    tempNormal = VectorMath.Normal(new Vector3[3] { new Vector3(x + (float)LandscapeMap.GridStep, LandscapeMap.Table[i + 1, j + 1], z + (float)LandscapeMap.GridStep),
                        new Vector3(x + (float)LandscapeMap.GridStep, LandscapeMap.Table[i + 1, j], z),
                        new Vector3(x, LandscapeMap.Table[i, j + 1], z + (float)LandscapeMap.GridStep)});
                    normalMatrix[i + 1, j + 1] = tempNormal;
                    //tempNormal = getLandscapeNormal(i + 1, j + 1);
                    normals[vertexPointer + 5, 0] = tempNormal.X;
                    normals[vertexPointer + 5, 1] = tempNormal.Y;
                    normals[vertexPointer + 5, 2] = tempNormal.Z;
                    texCoords[vertexPointer + 5, 0] = vertices[vertexPointer + 5, 0] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    texCoords[vertexPointer + 5, 1] = vertices[vertexPointer + 5, 2] / (float)(LandscapeMap.GridStep * LandscapeMap.TableSize);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    vertexPointer += 6;
                }
            }
            // Making smooth normals
            vertexPointer = 0;
            for (int i = 0; i < LandscapeMap.TableSize - 1; i++)
            {
                for (int j = 0; j < LandscapeMap.TableSize - 1; j++)
                {
                    tempNormal = getLandscapeSmoothNormal(LandscapeMap, normalMatrix, i, j, normalSmoothLvl);
                    normals[vertexPointer, 0] = tempNormal.X;
                    normals[vertexPointer, 1] = tempNormal.Y;
                    normals[vertexPointer, 2] = tempNormal.Z;
                    tempNormal = getLandscapeSmoothNormal(LandscapeMap, normalMatrix, i, j + 1, normalSmoothLvl);
                    normals[vertexPointer + 1, 0] = tempNormal.X;
                    normals[vertexPointer + 1, 1] = tempNormal.Y;
                    normals[vertexPointer + 1, 2] = tempNormal.Z;
                    tempNormal = getLandscapeSmoothNormal(LandscapeMap, normalMatrix, i + 1, j, normalSmoothLvl);
                    normals[vertexPointer + 2, 0] = tempNormal.X;
                    normals[vertexPointer + 2, 1] = tempNormal.Y;
                    normals[vertexPointer + 2, 2] = tempNormal.Z;
                    normals[vertexPointer + 3, 0] = normals[vertexPointer + 2, 0];
                    normals[vertexPointer + 3, 1] = normals[vertexPointer + 2, 1];
                    normals[vertexPointer + 3, 2] = normals[vertexPointer + 2, 2];
                    normals[vertexPointer + 4, 0] = normals[vertexPointer + 1, 0];
                    normals[vertexPointer + 4, 1] = normals[vertexPointer + 1, 1];
                    normals[vertexPointer + 4, 2] = normals[vertexPointer + 1, 2];
                    tempNormal = getLandscapeSmoothNormal(LandscapeMap, normalMatrix, i + 1, j + 1, normalSmoothLvl);
                    normals[vertexPointer + 5, 0] = tempNormal.X;
                    normals[vertexPointer + 5, 1] = tempNormal.Y;
                    normals[vertexPointer + 5, 2] = tempNormal.Z;
                    vertexPointer += 6;
                }
            }
            return new VBOArrayF(vertices, normals, texCoords, true);
        }

        #endregion
    }
}