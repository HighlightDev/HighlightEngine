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

        public VertexArrayObject()
        {
            m_vboList = new List<VertexBufferObjectBase>();
            GenVAO();
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
            GL.DrawArrays(privitiveMode, 0, m_vboList.First<VertexBufferObjectBase>().GetBufferElementsCount());
            GL.BindVertexArray(0);
        }

        public void AddVBO(params VertexBufferObjectBase[] vbos)
        {
            m_vboList.AddRange(vbos);
        }

        public void AddVBO(VertexBufferObjectBase vbo)
        {
            m_vboList.Add(vbo);
        }

        public void BindVbosToVao()
        {
            GL.BindVertexArray(m_descriptor);
            m_vboList.ForEach(vbo => vbo.SendDataToGPU());
            GL.BindVertexArray(0);
            DisableVertexAttribArrays();
        }

        private void DisableVertexAttribArrays()
        {
            m_vboList.ForEach(vbo =>
           {
               vbo.UnbindVBO();
               GL.BindVertexArray(0);
               GL.DisableVertexAttribArray(vbo.GetVertexAttribIndex());
           });
        }

        public void CleanUp()
        {
            m_vboList.ForEach(vbo => vbo.CleanUp());
            GL.DeleteVertexArray(m_descriptor);
            m_vboList.Clear();
            m_vboList = null;
        }
    }
}
