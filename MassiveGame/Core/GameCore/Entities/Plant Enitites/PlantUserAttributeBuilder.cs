using System;
using System.Collections.Generic;
using System.Linq;
using GpuGraphics;
using OpenTK;
using VBO;

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{

    public static class PlantUserAttributeBuilder
    {
        public static VBOArrayF BuildReadyUserAttributeBuffer(IEnumerable<PlantUnit> plants, VBOArrayF attribs)
        {
            if (plants.Count() == 0) { return attribs; }

            Int32 size = plants.Count();
            float[] windS = new float[size];
            float[] tSampler = new float[size];

            float[,] matrixColumn1 = new float[size, 4];
            float[,] matrixColumn2 = new float[size, 4];
            float[,] matrixColumn3 = new float[size, 4];
            float[,] matrixColumn4 = new float[size, 4];

            // magic
            unsafe
            {
                fixed (float* matrix1 = matrixColumn1)
                {
                    fixed (float* matrix2 = matrixColumn2)
                    {
                        fixed (float* matrix3 = matrixColumn3)
                        {
                            fixed (float* matrix4 = matrixColumn4)
                            {
                                for (Int32 i = 0, baseDim = 0; i < size; i++, baseDim += 4)
                                {
                                    var item = plants.ElementAt(i);
                                    var modelMatrix = Matrix4.Identity;
                                    modelMatrix *= Matrix4.CreateRotationY(item.Rotation.Y);
                                    modelMatrix *= Matrix4.CreateScale(item.Scale);
                                    modelMatrix *= Matrix4.CreateTranslation(item.Translation);

                                    matrix1[baseDim] = modelMatrix[0, 0];
                                    matrix1[baseDim + 1] = modelMatrix[0, 1];
                                    matrix1[baseDim + 2] = modelMatrix[0, 2];
                                    matrix1[baseDim + 3] = modelMatrix[0, 3];

                                    matrix2[baseDim] = modelMatrix[1, 0];
                                    matrix2[baseDim + 1] = modelMatrix[1, 1];
                                    matrix2[baseDim + 2] = modelMatrix[1, 2];
                                    matrix2[baseDim + 3] = modelMatrix[1, 3];

                                    matrix3[baseDim] = modelMatrix[2, 0];
                                    matrix3[baseDim + 1] = modelMatrix[2, 1];
                                    matrix3[baseDim + 2] = modelMatrix[2, 2];
                                    matrix3[baseDim + 3] = modelMatrix[2, 3];

                                    matrix4[baseDim] = modelMatrix[3, 0];
                                    matrix4[baseDim + 1] = modelMatrix[3, 1];
                                    matrix4[baseDim + 2] = modelMatrix[3, 2];
                                    matrix4[baseDim + 3] = modelMatrix[3, 3];

                                    windS[i] = item.WindLoop;
                                    tSampler[i] = item.textureID;
                                }
                            }
                        }
                    }
                }
            }
            return new VBOArrayF(attribs.Vertices, attribs.Normals, attribs.TextureCoordinates,
                windS, tSampler, matrixColumn1, matrixColumn2, matrixColumn3, matrixColumn4);
        }

        public static Tuple<Tuple<VertexBufferObjectBase, VertexBufferObjectBase>, Tuple<VertexBufferObjectBase, VertexBufferObjectBase>>
            GetInstacedTransformationBuffer(IEnumerable<PlantUnit> plants, Int32 bufferSize)
        {
            if (plants.Count() == 0) { return null; }

            if (plants.Count() > bufferSize)
            {
                throw new ArgumentException();
            }

            Int32 size = plants.Count();

            float[,] matrixColumn1 = new float[bufferSize, 4];
            float[,] matrixColumn2 = new float[bufferSize, 4];
            float[,] matrixColumn3 = new float[bufferSize, 4];
            float[,] matrixColumn4 = new float[bufferSize, 4];
            // magic
            unsafe
            {
                fixed (float* matrix1 = matrixColumn1)
                {
                    fixed (float* matrix2 = matrixColumn2)
                    {
                        fixed (float* matrix3 = matrixColumn3)
                        {
                            fixed (float* matrix4 = matrixColumn4)
                            {
                                for (Int32 i = 0, baseDim = 0; i < size; i++, baseDim += 4)
                                {
                                    var item = plants.ElementAt(i);
                                    var modelMatrix = Matrix4.Identity;
                                    modelMatrix *= Matrix4.CreateRotationY(item.Rotation.Y);
                                    modelMatrix *= Matrix4.CreateScale(item.Scale);
                                    modelMatrix *= Matrix4.CreateTranslation(item.Translation);

                                    matrix1[baseDim] = modelMatrix[0, 0];
                                    matrix1[baseDim + 1] = modelMatrix[0, 1];
                                    matrix1[baseDim + 2] = modelMatrix[0, 2];
                                    matrix1[baseDim + 3] = modelMatrix[0, 3];

                                    matrix2[baseDim] = modelMatrix[1, 0];
                                    matrix2[baseDim + 1] = modelMatrix[1, 1];
                                    matrix2[baseDim + 2] = modelMatrix[1, 2];
                                    matrix2[baseDim + 3] = modelMatrix[1, 3];

                                    matrix3[baseDim] = modelMatrix[2, 0];
                                    matrix3[baseDim + 1] = modelMatrix[2, 1];
                                    matrix3[baseDim + 2] = modelMatrix[2, 2];
                                    matrix3[baseDim + 3] = modelMatrix[2, 3];

                                    matrix4[baseDim] = modelMatrix[3, 0];
                                    matrix4[baseDim + 1] = modelMatrix[3, 1];
                                    matrix4[baseDim + 2] = modelMatrix[3, 2];
                                    matrix4[baseDim + 3] = modelMatrix[3, 3];
                                }
                            }
                        }
                    }
                }
            }
            VertexBufferObjectTwoDimension<float> vector_1 = new VertexBufferObjectTwoDimension<float>(matrixColumn1, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 3, 4, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> vector_2 = new VertexBufferObjectTwoDimension<float>(matrixColumn2, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 4, 4, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> vector_3 = new VertexBufferObjectTwoDimension<float>(matrixColumn3, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 5, 4, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> vector_4 = new VertexBufferObjectTwoDimension<float>(matrixColumn4, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 6, 4, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            return new Tuple<Tuple<VertexBufferObjectBase, VertexBufferObjectBase>, Tuple<VertexBufferObjectBase, VertexBufferObjectBase>>(
                new Tuple<VertexBufferObjectBase, VertexBufferObjectBase>(vector_1, vector_2),
                new Tuple<VertexBufferObjectBase, VertexBufferObjectBase>(vector_3, vector_4));
        }

        public static VertexBufferObjectBase GetInstancedWindBuffer(IEnumerable<PlantUnit> plants, Int32 bufferSize)
        {
            if (plants.Count() == 0) { return null; }

            if (plants.Count() > bufferSize)
            {
                throw new ArgumentException();
            }

            Int32 size = plants.Count();
            float[] windS = new float[bufferSize];
            
            unsafe
            {
                for (Int32 i = 0, baseDim = 0; i < size; i++, baseDim += 4)
                {
                    var item = plants.ElementAt(i);
                    windS[i] = item.WindLoop;
                }
            }

            return new VertexBufferObjectOneDimension<float>(windS, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 7, VertexBufferObjectBase.DataCarryFlag.Invalidate);
        }

        public static VertexBufferObjectBase GetInstanceSamplerBuffer(IEnumerable<PlantUnit> plants, Int32 bufferSize)
        {
            if (plants.Count() == 0) { return null; }

            if (plants.Count() > bufferSize)
            {
                throw new ArgumentException();
            }

            Int32 size = plants.Count();
            float[] tSampler = new float[bufferSize];

            unsafe
            {
                for (Int32 i = 0, baseDim = 0; i < size; i++, baseDim += 4)
                {
                    var item = plants.ElementAt(i);
                    tSampler[i] = item.textureID;
                }
            }

            return new VertexBufferObjectOneDimension<float>(tSampler, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 8, VertexBufferObjectBase.DataCarryFlag.Invalidate);
        }

        public static void AddBuilderUserAttribute(PlantUnit plant, VAO buffer, Int32 plantsCountBeforeAddition)
        {
            float windS = 0.0f, tSampler = 0.0f;

            float[,] matrixColumn1 = new float[1, 4];
            float[,] matrixColumn2 = new float[1, 4];
            float[,] matrixColumn3 = new float[1, 4];
            float[,] matrixColumn4 = new float[1, 4];

            var modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateRotationY(plant.Rotation.Y);
            modelMatrix *= Matrix4.CreateScale(plant.Scale);
            modelMatrix *= Matrix4.CreateTranslation(plant.Translation);

            matrixColumn1[0, 0] = modelMatrix[0, 0];
            matrixColumn1[0, 1] = modelMatrix[0, 1];
            matrixColumn1[0, 2] = modelMatrix[0, 2];
            matrixColumn1[0, 3] = modelMatrix[0, 3];

            matrixColumn2[0, 0] = modelMatrix[1, 0];
            matrixColumn2[0, 1] = modelMatrix[1, 1];
            matrixColumn2[0, 2] = modelMatrix[1, 2];
            matrixColumn2[0, 3] = modelMatrix[1, 3];

            matrixColumn3[0, 0] = modelMatrix[2, 0];
            matrixColumn3[0, 1] = modelMatrix[2, 1];
            matrixColumn3[0, 2] = modelMatrix[2, 2];
            matrixColumn3[0, 3] = modelMatrix[2, 3];

            matrixColumn4[0, 0] = modelMatrix[3, 0];
            matrixColumn4[0, 1] = modelMatrix[3, 1];
            matrixColumn4[0, 2] = modelMatrix[3, 2];
            matrixColumn4[0, 3] = modelMatrix[3, 3];

            windS = plant.WindLoop;
            tSampler = plant.textureID;

            IntPtr singleAttributeOffset = new IntPtr(sizeof(float) * plantsCountBeforeAddition);
            IntPtr singleAttributeValueSize = new IntPtr(sizeof(float));
            IntPtr vectorAttributeOffset = new IntPtr(sizeof(float) * matrixColumn1.Length * plantsCountBeforeAddition);
            IntPtr vectorAttributeValueSize = new IntPtr(sizeof(float) * matrixColumn1.Length);

            VAOManager.AddUserSingleAttribute(buffer, 0, windS, singleAttributeOffset, singleAttributeValueSize);
            VAOManager.AddUserSingleAttribute(buffer, 1, tSampler, singleAttributeOffset, singleAttributeValueSize);
            VAOManager.AddUserVectorAttribute(buffer, 0, matrixColumn1, vectorAttributeOffset, vectorAttributeValueSize);
            VAOManager.AddUserVectorAttribute(buffer, 1, matrixColumn2, vectorAttributeOffset, vectorAttributeValueSize);
            VAOManager.AddUserVectorAttribute(buffer, 2, matrixColumn3, vectorAttributeOffset, vectorAttributeValueSize);
            VAOManager.AddUserVectorAttribute(buffer, 3, matrixColumn4, vectorAttributeOffset, vectorAttributeValueSize);
        }
    }
}
