using System;
using System.Collections.Generic;
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

        private void GenVAO()
        {
            m_descriptor = GL.GenVertexArray();
        }

        public void AddVBO(VertexBufferObjectBase vbo)
        {
            m_vboList.Add(vbo);
        }

        public void BindVboWithVao()
        {
            GL.BindVertexArray(m_descriptor);
            m_vboList.ForEach(vbo => vbo.BindVBO());
            GL.BindVertexArray(0);
        }

        public void CleanUp()
        {
            m_vboList.ForEach(vbo => vbo.CleanUp());
            GL.DeleteVertexArray(m_descriptor);
        }

        public VertexArrayObject CreateVAO()
        {
            VertexArrayObject vao = new VertexArrayObject();
            VertexBufferObject<float> positionVBO = new VertexBufferObject<float>(new float[1, 1], BufferTarget.ArrayBuffer, 0, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            vao.AddVBO(positionVBO);

            return vao;
        }
    }
}
