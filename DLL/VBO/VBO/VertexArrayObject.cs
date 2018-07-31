using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace VBO
{
    public class VertexArrayObject
    {
        private Int32 m_descriptor;
        private List<VertexBufferObjectBase> m_vboList;
        private IndexBufferObject m_ibo;

        public VertexArrayObject()
        {
            m_vboList = new List<VertexBufferObjectBase>();
            GenVAO();
        }

        public IndexBufferObject GetIndexBufferObject()
        {
            return m_ibo;
        }

        public bool HasIBO()
        {
            return m_ibo != null;
        }

        public List<VertexBufferObjectBase> GetVertexBufferArray()
        {
            return m_vboList;
        }

        private void GenVAO()
        {
            m_descriptor = GL.GenVertexArray();
        }

        public void RenderVAO(PrimitiveType privitiveMode)
        {
            GL.BindVertexArray(m_descriptor);
            if (HasIBO())
            {
                GL.DrawElements(privitiveMode, m_ibo.GetIndicesCount(), DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.DrawArrays(privitiveMode, 0, m_vboList.First<VertexBufferObjectBase>().GetBufferVerticesCount());
            }
            GL.BindVertexArray(0);
        }

        public void AddVBO(params VertexBufferObjectBase[] vbos)
        {
            foreach (var vbo in vbos)
            {
                AddVBO(vbo);
            }
        }

        public void AddVBO(VertexBufferObjectBase vbo)
        {
            if (vbo != null)
                m_vboList.Add(vbo);
        }

        public void AddIndexBuffer(IndexBufferObject ibo)
        {
            if (ibo != null)
                m_ibo = ibo;
        }

        public void BindBuffersToVao()
        {
            GL.BindVertexArray(m_descriptor);
            m_ibo?.SendDataToGPU();
            m_vboList.ForEach(vbo => vbo.SendDataToGPU());
            GL.BindVertexArray(0);
            DisableVertexAttribArrays();
        }

        private void DisableVertexAttribArrays()
        {
            GL.BindVertexArray(0);
            IndexBufferObject.UnbindIndexBuffer();
            VertexBufferObjectBase.UnbindVBO();
            m_vboList.ForEach(vbo =>
           {
               GL.DisableVertexAttribArray(vbo.GetVertexAttribIndex());
           });
        }

        public void CleanUp()
        {
            m_ibo?.CleanUp();
            m_ibo = null;
            m_vboList.ForEach(vbo => vbo.CleanUp());
            GL.DeleteVertexArray(m_descriptor);
            m_vboList.Clear();
            m_vboList = null;
        }
    }
}
