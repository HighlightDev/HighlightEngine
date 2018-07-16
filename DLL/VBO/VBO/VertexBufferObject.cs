using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace VBO
{
    public class VertexBufferObject<T> : VertexBufferObjectBase where T : struct
    {
        private T[,] m_data;

        public VertexBufferObject(T[,] data, BufferTarget bufferTarget, Int32 vertexAttribIndex, DataCarryFlag flag)
            : base(bufferTarget, vertexAttribIndex, flag)
        {
            m_data = data;
        }

        protected override void BufferData()
        {
            GL.BufferData(m_bufferTarget, GetBufferSize(),
               m_data, BufferUsageHint.StaticDraw);

            // If data on CPU in unnecessary - throw it to GC
            if (m_dataCarryFlag == DataCarryFlag.Invalidate)
                m_data = null;
        }

        public override Array GetBufferData()
        {
            return m_data;
        }

        public override Int32 GetBufferElementsCount()
        {
            return m_data.GetLength(0);
        }

        public override Int32 GetElementByteCount()
        {
            return Marshal.SizeOf(m_data[0, 0]);
        }

        protected override VertexAttribPointerType GetAttribPointerType()
        {
            return ParseType<T>();
        }
    }
}
