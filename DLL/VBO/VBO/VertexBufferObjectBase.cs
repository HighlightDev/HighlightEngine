using OpenTK.Graphics.OpenGL;
using System;

namespace VBO
{
    public abstract class VertexBufferObjectBase
    {
        public enum DataCarryFlag
        {
            //buffer data becomes unavailable after it is send on a gpu
            Invalidate,
            //buffer data is available
            Store
        }

        protected Int32 m_descriptor;
        protected BufferTarget m_bufferTarget;
        protected DataCarryFlag m_dataCarryFlag;
        protected Int32 m_vertexAttribArray;

        public VertexBufferObjectBase(BufferTarget bufferTarget, Int32 vertexAttribArray, DataCarryFlag flag)
        {
            GenVBO();
            m_vertexAttribArray = vertexAttribArray;
            m_bufferTarget = bufferTarget;
            m_dataCarryFlag = flag;
        }

        public abstract Int32 GetBufferElementsCount();
        public abstract Int32 GetElementByteCount();
        public abstract Array GetBufferData();
        protected abstract void BufferData();

        public void BindVBO()
        {
            GL.BindBuffer(m_bufferTarget, m_descriptor);
        }

        public BufferTarget GetBufferTarget() { return m_bufferTarget; }

        public IntPtr GetBufferSize() { return new IntPtr(GetBufferElementsCount() * GetElementByteCount()); }

        public DataCarryFlag GetDataCarryFlag() { return m_dataCarryFlag; }

        public void SendDataToGPU()
        {
            GL.BindBuffer(m_bufferTarget, m_descriptor);
            BufferData();
            GL.EnableVertexAttribArray(m_vertexAttribArray);
            GL.VertexAttribPointer(m_vertexAttribArray, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void CleanUp()
        {
            GL.DeleteBuffer(m_descriptor);
        }

        private void GenVBO()
        {
            m_descriptor = GL.GenBuffer();
        }
    }
}
