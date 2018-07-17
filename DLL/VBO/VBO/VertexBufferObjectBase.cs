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
        protected Int32 m_vertexAttribIndex;
        protected Int32 m_dataVectorSize;

        public VertexBufferObjectBase(BufferTarget bufferTarget, Int32 vertexAttribIndex, Int32 dataVectorSize, DataCarryFlag flag)
        {
            GenVBO();
            m_dataVectorSize = dataVectorSize;
            m_vertexAttribIndex = vertexAttribIndex;
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

        protected abstract VertexAttribPointerType GetAttribPointerType();

        public void SendDataToGPU()
        {
            GL.BindBuffer(m_bufferTarget, m_descriptor);
            BufferData();
            GL.EnableVertexAttribArray(m_vertexAttribIndex);
            GL.VertexAttribPointer(m_vertexAttribIndex, m_dataVectorSize, GetAttribPointerType(), false, 0, 0);
        }

        public void CleanUp()
        {
            GL.DeleteBuffer(m_descriptor);
        }

        private void GenVBO()
        {
            m_descriptor = GL.GenBuffer();
        }

        protected VertexAttribPointerType ParseType<T>()
        {
            VertexAttribPointerType resultType = VertexAttribPointerType.Float;
            var type = typeof(T);
            if (type == typeof(byte)) resultType = VertexAttribPointerType.Byte;
            else if(type == typeof(Int16)) resultType = VertexAttribPointerType.Short;
            else if(type == typeof(Int32)) resultType = VertexAttribPointerType.Int;
            else if (type == typeof(float)) resultType = VertexAttribPointerType.Float;
            else if (type == typeof(double)) resultType = VertexAttribPointerType.Double;
            else if (type == typeof(UInt16)) resultType = VertexAttribPointerType.UnsignedShort;
            else if (type == typeof(UInt32)) resultType = VertexAttribPointerType.UnsignedInt;
            return resultType;
        }
    }
}
