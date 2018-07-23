using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace VBO
{
    public class VertexBufferObjectTwoDimension<T> : VertexBufferObjectBase where T : struct
    {
        private T[,] m_data;

        public VertexBufferObjectTwoDimension(T[,] data, BufferTarget bufferTarget, Int32 vertexAttribIndex, Int32 dataVectorSize, DataCarryFlag flag)
            : base(bufferTarget, vertexAttribIndex, dataVectorSize, flag)
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
            BindVertexAttribPointer(m_vertexAttribIndex, m_dataVectorSize, false, 0, 0);

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
