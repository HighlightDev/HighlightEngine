﻿using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace VBO
{
    public class VertexBufferObjectOneDimension<T> : VertexBufferObjectBase where T : struct
    {
        private T[] m_data;

        public VertexBufferObjectOneDimension(T[] data, BufferTarget bufferTarget, Int32 vertexAttribIndex, DataCarryFlag flag)
            : base(bufferTarget, vertexAttribIndex, 1, flag)
        {
            m_data = data;
            m_verticesCount = m_data.GetLength(0);
            m_elementsCount = m_data.Length;
        }

        public override void SendDataToGPU()
        {
            GL.BindBuffer(m_bufferTarget, m_descriptor);
            IntPtr size = GetBufferSize();
            GL.BufferData(m_bufferTarget, size, m_data, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(m_vertexAttribIndex);
            BindVertexAttribPointer(m_vertexAttribIndex, 1, false, 0, 0);

            // If data on CPU is unnecessary - throw it to GC
            if (m_dataCarryFlag == DataCarryFlag.Invalidate)
                m_data = null;
        }

        protected override VertexAttribPointerType GetAttribPointerType()
        {
            return ParseType<T>();
        }

        public override Array GetBufferData()
        {
            return m_data;
        }

        public override Int32 GetElementByteCount()
        {
            return Marshal.SizeOf(typeof(T));
        }
    }
}

