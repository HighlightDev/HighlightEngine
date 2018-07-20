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
            m_elementsCount = m_data.GetLength(0);
        }

        protected override void BufferData()
        {
            IntPtr size = GetBufferSize();
            GL.BufferData(m_bufferTarget, size, m_data, BufferUsageHint.StaticDraw);

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
            return Marshal.SizeOf(m_data[0, 0]) * 3;
        }
    }
}
