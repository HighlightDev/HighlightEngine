using OpenTK.Graphics.OpenGL;
using System;

namespace VBO
{
    public abstract class VertexBufferObjectBase
    {
        public enum DataCarryFlag
        {
            //buffer data becomes unavailable after it is sent on a gpu
            Invalidate,
            //buffer data is available
            Store
        }

        protected Int32 m_descriptor;
        protected BufferTarget m_bufferTarget;
        protected DataCarryFlag m_dataCarryFlag;
        protected Int32 m_vertexAttribIndex;
        protected Int32 m_dataVectorSize;

        protected Int32 m_verticesCount;
        protected Int32 m_elementsCount;

        public VertexBufferObjectBase(BufferTarget bufferTarget, Int32 vertexAttribIndex, Int32 dataVectorSize, DataCarryFlag flag)
        {
            GenVBO();
            m_dataVectorSize = dataVectorSize;
            m_vertexAttribIndex = vertexAttribIndex;
            m_bufferTarget = bufferTarget;
            m_dataCarryFlag = flag;
        }

        protected void BindVertexAttribPointer(Int32 index, Int32 size, bool normalized, Int32 stride, Int32 offset)
        {
            var pointerType = GetAttribPointerType();
            if (pointerType == VertexAttribPointerType.Int ||
                pointerType == VertexAttribPointerType.UnsignedByte ||
                pointerType == VertexAttribPointerType.UnsignedInt ||
                pointerType == VertexAttribPointerType.UnsignedByte)
            {
                GL.VertexAttribIPointer(index, size, VertexAttribIntegerType.Int, stride, new IntPtr(0));
            }
            else if (pointerType == VertexAttribPointerType.Double)
            {
                GL.VertexAttribLPointer(index, size, VertexAttribDoubleType.Double, stride, new IntPtr(0));
            }
            else
            {
                GL.VertexAttribPointer(index, size, pointerType, normalized, stride, offset);
            }
        }
    
        public void BindVBO()
        {
            GL.BindBuffer(m_bufferTarget, m_descriptor);
        }

        public abstract Int32 GetElementByteCount();
        public abstract Array GetBufferData();

        public Int32 GetVertexAttribIndex()
        {
            return m_vertexAttribIndex;
        }

        public Int32 GetBufferVerticesCount()
        {
            return m_verticesCount;
        }

        public Int32 GetBufferTotalElementsCount()
        {
            return m_elementsCount;
        }

        public BufferTarget GetBufferTarget() { return m_bufferTarget; }

        public IntPtr GetBufferSize() { return new IntPtr(GetBufferTotalElementsCount() * GetElementByteCount()); }

        public DataCarryFlag GetDataCarryFlag() { return m_dataCarryFlag; }

        protected abstract VertexAttribPointerType GetAttribPointerType();

        public abstract void SendDataToGPU();

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
            else if (type == typeof(Int16)) resultType = VertexAttribPointerType.Short;
            else if (type == typeof(Int32)) resultType = VertexAttribPointerType.Int;
            else if (type == typeof(float)) resultType = VertexAttribPointerType.Float;
            else if (type == typeof(double)) resultType = VertexAttribPointerType.Double;
            else if (type == typeof(UInt16)) resultType = VertexAttribPointerType.UnsignedShort;
            else if (type == typeof(UInt32)) resultType = VertexAttribPointerType.UnsignedInt;
            return resultType;
        }

        public static void UnbindVBO()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
