using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GpuGraphics
{
    public static class VAOManager
    {
        // this has to be redone 
        // maybe something using generics...
        

        /*
         Buffer* GenBuffer<T>(T* bufferData)
        */



        #region Create_buffer

        public static void genVAO(VAO buffers)
        {
            if (buffers.isMapped()) return;    //Проверка ,что генерируем буфферы 1 раз 
            GL.GenVertexArrays(1, buffers.Vao); // Gen vao
            GL.GenBuffers(buffers.getBuffersCount(), buffers.Vbo); //Генерируем заданное количество vbo
            if (buffers.getBufferData().Indices != null) // Gen ibo
            {
                GL.GenBuffers(1, buffers.Ibo);
            }
            buffers.mapBuffer();   //Устанавливаем флаг, что буфферы сгенерированы
        }

        #endregion

        #region Put_data_into_buffer

        public static void setBufferData(BufferTarget bufferType, VAO buffer)
        {
            if (buffer.isDataInserted()) return;    //Проверка что заносим информацию в буфферы 1 раз 
            byte bufferCount = 0;

            //***************************************Bind VAO***********************//
            GL.BindVertexArray(buffer.Vao[0]);

            // ************************************************
            // Bind ibo for indexes
            // ************************************************
            if (buffer.getBufferData().Indices != null)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.Ibo[0]);  //Bind IBO
                GL.BufferData(BufferTarget.ElementArrayBuffer, buffer.getBufferData().getIndicesByteSize(),
                    buffer.getBufferData().Indices, BufferUsageHint.StaticDraw);
            }

            // ************************************************
            // Bind vbo for vertices
            // ************************************************
            GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]); //Вершины
            GL.BufferData(bufferType, buffer.getBufferData().getVerticesByteSize(),
                buffer.getBufferData().Vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            
            bufferCount++;

            // ************************************************
            // Bind vbo for normals
            // ************************************************
            if (buffer.getBufferData().Normals != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getNormalsByteSize(),
                    buffer.getBufferData().Normals, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
                
                bufferCount++;
            }

            // ************************************************
            // Bind vbo for texture coordinates
            // ************************************************
            if (buffer.getBufferData().TextureCoordinates != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getTexCoordByteSize(),
                    buffer.getBufferData().TextureCoordinates, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                
                bufferCount++;
            }

            // ************************************************
            // Bind vbo for color values
            // ************************************************
            if (buffer.getBufferData().Color != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getColorByteSize(),
                   buffer.getBufferData().Color, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for tangents
            // ************************************************
            if (buffer.getBufferData().Tangent != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getTangentByteSize(),
                   buffer.getBufferData().Tangent, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(4);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 0, 0);
               
                bufferCount++;
            }

            // ************************************************
            // Bind vbo for bitangents
            // ************************************************
            if (buffer.getBufferData().Bitangent != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getBitangentByteSize(),
                    buffer.getBufferData().Bitangent, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(5);
                GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, 0, 0);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for userSingleAttributes1
            // ************************************************
            if (buffer.getBufferData().SingleAttribute1 != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getSingleAttribute1Size(),
                    buffer.getBufferData().SingleAttribute1, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(6);
                GL.VertexAttribPointer(6, 1, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(6, 1);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for userSingleAttributes2
            // ************************************************
            if (buffer.getBufferData().SingleAttribute2 != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getSingleAttribute2Size(),
                    buffer.getBufferData().SingleAttribute2, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(7);
                GL.VertexAttribPointer(7, 1, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(7, 1);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for userVectorAttributes1
            // ************************************************
            if (buffer.getBufferData().VecAttribute1 != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getVectorAttribute1Size(),
                    buffer.getBufferData().VecAttribute1, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(8);
                GL.VertexAttribPointer(8, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(8, 1);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for userVectorAttributes2
            // ************************************************
            if (buffer.getBufferData().VecAttribute2 != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getVectorAttribute2Size(),
                    buffer.getBufferData().VecAttribute2, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(9);
                GL.VertexAttribPointer(9, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(9, 1);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for userVectorAttributes3
            // ************************************************
            if (buffer.getBufferData().VecAttribute3 != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getVectorAttribute3Size(),
                    buffer.getBufferData().VecAttribute3, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(10);
                GL.VertexAttribPointer(10, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(10, 1);

                bufferCount++;
            }

            // ************************************************
            // Bind vbo for userVectorAttributes4
            // ************************************************
            if (buffer.getBufferData().VecAttribute4 != null)
            {
                GL.BindBuffer(bufferType, buffer.Vbo[bufferCount]);
                GL.BufferData(bufferType, buffer.getBufferData().getVectorAttribute4Size(),
                    buffer.getBufferData().VecAttribute4, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(11);
                GL.VertexAttribPointer(11, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(11, 1);

                bufferCount++;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //Отвязываем VBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);  //Отвязываем IBO
            GL.BindVertexArray(0);      //Отвязываем VAO

            GL.DisableVertexAttribArray(0);
            if (buffer.getBufferData().Normals != null) { GL.DisableVertexAttribArray(1); }
            if (buffer.getBufferData().TextureCoordinates != null) { GL.DisableVertexAttribArray(2); }
            if (buffer.getBufferData().Color != null) { GL.DisableVertexAttribArray(3); }
            if (buffer.getBufferData().Tangent != null) { GL.DisableVertexAttribArray(4); }
            if (buffer.getBufferData().Bitangent != null) { GL.DisableVertexAttribArray(5); }

            // Data is inserted
            buffer.dataInserted(); 
        }

        //public static void setUserAttribsBuffer(BufferTarget bufferType, VAO buffer)
        //{
        //    GL.BindVertexArray(buffer.Vao[0]);
        //    GL.BindBuffer(bufferType, buffer.Vbo[buffer.Vbo.Length - 1]);
        //    GL.BufferData(bufferType, buffer.getBufferData().getUserAttributesSize(), )
        //}

        #endregion

        #region Render_function

        public static void renderBuffers(VAO buffer, PrimitiveType privitiveMode)
        {
            GL.BindVertexArray(buffer.Vao[0]);
            if (buffer.getBufferData().Indices != null)
            {
                GL.DrawElements(privitiveMode, buffer.getBufferData().Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.DrawArrays(privitiveMode, 0, buffer.getBufferData().getCountVertices());
            }
            GL.BindVertexArray(0);
        }

        public static void renderInstanced(VAO buffer, PrimitiveType primitiveMode, Int32 count)
        {
            GL.BindVertexArray(buffer.Vao[0]);
            GL.DrawArraysInstanced(primitiveMode, 0, buffer.getBufferData().getCountVertices(), count);
            GL.BindVertexArray(0);
        }

        #endregion

        #region Update_buffer

        /// <summary>
        /// Function updates buffer with verticies
        /// </summary>
        /// <param name="buffer">Full buffer</param>
        /// <param name="bufferData">Array with data</param>
        /// <param name="bufferIndex">Index of changing buffer</param>
        public static void updateVertexBufferFully(VAO buffer, VBOArrayF bufferData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.Vbo[0]);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(0),
                bufferData.getAtrributeByteSize(0), bufferData.Vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static void AddUserSingleAttribute(VAO buffer, Int32 attributeIndex, float value, IntPtr offset, IntPtr size)
        {
            Int32 bufferIndex = 0;
            for (Int32 i = 0; i < 6 + attributeIndex; i++)
            {
                if (buffer[i]) bufferIndex++;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.Vbo[bufferIndex]);
            GL.BufferSubData(BufferTarget.ArrayBuffer, offset, size, ref value);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static void AddUserVectorAttribute(VAO buffer, Int32 attributeIndex, float[,] value, IntPtr offset, IntPtr size)
        {
            Int32 bufferIndex = 0;
            for (Int32 i = 0; i < (8 + attributeIndex); i++)
            {
                if (buffer[i]) bufferIndex++;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.Vbo[bufferIndex]);
            GL.BufferSubData(BufferTarget.ArrayBuffer, offset, size, value);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        #endregion

        #region Cleanings

        public static void cleanUp(VAO buffer)
        {
            GL.DeleteBuffers(buffer.getBuffersCount(), buffer.Vbo);
            if (buffer.getBufferData().Indices != null) { GL.DeleteBuffers(1, buffer.Ibo); }
            GL.DeleteVertexArrays(1, buffer.Vao);
        }

        #endregion
    }
}
