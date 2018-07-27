using System;
using OpenTK.Graphics.OpenGL;
using static VBO.VertexBufferObjectBase;

namespace VBO
{
    public class IndexBufferObject
    {
        private Int32 m_descriptor;
        private UInt32[] m_data;
        private DataCarryFlag m_dataCarryFlag;
        private Int32 m_indicesCount;

        public IndexBufferObject(UInt32[] indicesData, DataCarryFlag dataCarryFlag = DataCarryFlag.Invalidate)
        {
            m_data = indicesData;
            m_dataCarryFlag = dataCarryFlag;
            m_indicesCount = indicesData.Length;
            GenIndexBuffer();
        }

        private void GenIndexBuffer()
        {
            m_descriptor = GL.GenBuffer();
        }

        public void BindIndexBuffer()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_descriptor);
        }

        public void SendDataToGPU()
        {
            BindIndexBuffer();
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(UInt32) * m_data.Length), m_data, BufferUsageHint.StaticDraw);

            if (m_dataCarryFlag == DataCarryFlag.Invalidate)
                m_data = null;
        }

        public int GetIndicesCount()
        {
            return m_indicesCount;
        }

        public UInt32[] GetIndicesData()
        {
            return m_data;
        }

        public static void UnbindIndexBuffer()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void CleanUp()
        {
            GL.DeleteBuffer(m_descriptor);
        }
    }
}
