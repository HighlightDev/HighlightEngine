using MassiveGame.Core.GameCore;
using System;
using OpenTK;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.Settings;
using VBO;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.EngineEditor.Core.Entities
{
    public class WorldGrid
    {
        private const float m_gridSquareSize = 1f;

        private float m_farPlaneDistance;
        private WorldGridShader m_shader;
        private VertexArrayObject m_vao;

        public WorldGrid(float farPlaneDistance)
        {
            m_farPlaneDistance = farPlaneDistance;
            m_shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WorldGridShader>, string, WorldGridShader>(string.Format("{0}gridVS.glsl,{0}gridFS.glsl", ProjectFolders.ShadersPath));

            m_vao = new VertexArrayObject();
            VertexBufferObjectBase verticesVBO = null;
            GenVertices(ref verticesVBO);
            m_vao.AddVBO(verticesVBO);
            m_vao.BindBuffersToVao();
        }

        private void GenVertices(ref VertexBufferObjectBase vbo)
        {
            float splitGridSize = m_farPlaneDistance / 2;
            float xLeft = -splitGridSize, xRight = splitGridSize, zNear = -splitGridSize, zFar = splitGridSize;
            Int32 gridSquareCount = (Int32)Math.Ceiling(m_farPlaneDistance / m_gridSquareSize);
            Int32 nodesCount = gridSquareCount * 8; 
            float[,] vertices = new float[nodesCount , 3];

            float localXLeft = xLeft;
            // Draw Grid at X-axis
            for (Int32 i = 0, j = 0; i < gridSquareCount; i++, j+= 4, localXLeft += m_gridSquareSize)
            {
                vertices[j, 0] = localXLeft;
                vertices[j, 1] = 0.0f;
                vertices[j, 2] = zNear;

                vertices[j + 1, 0] = localXLeft;
                vertices[j + 1, 1] = 0.0f;
                vertices[j + 1, 2] = zFar;

                vertices[j + 2, 0] = localXLeft + m_gridSquareSize;
                vertices[j + 2, 1] = 0.0f;
                vertices[j + 2, 2] = zFar;

                vertices[j + 3, 0] = localXLeft + m_gridSquareSize;
                vertices[j + 3, 1] = 0.0f;
                vertices[j + 3, 2] = zNear;
            }

            Int32 verticesContinue = nodesCount / 2;
            // Draw Grid at Z-axis
            for (Int32 i = 0, j = verticesContinue; i < gridSquareCount; i++, j += 4, zNear += m_gridSquareSize)
            {
                vertices[j, 0] = xLeft;
                vertices[j, 1] = 0.0f;
                vertices[j, 2] = zNear;

                vertices[j + 1, 0] = xRight;
                vertices[j + 1, 1] = 0.0f;
                vertices[j + 1, 2] = zNear;

                vertices[j + 2, 0] = xRight;
                vertices[j + 2, 1] = 0.0f;
                vertices[j + 2, 2] = zNear + m_gridSquareSize;

                vertices[j + 3, 0] = xLeft;
                vertices[j + 3, 1] = 0.0f;
                vertices[j + 3, 2] = zNear + m_gridSquareSize;
            }

            vbo = new VertexBufferObjectTwoDimension<float>(vertices, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
        }

        public void Render(BaseCamera baseCamera, ref Matrix4 projectionMatrix)
        {
            Matrix4 viewMatrix = baseCamera.GetViewMatrix();
            viewMatrix[3, 0] = 0;
            viewMatrix[3, 1] = -3;
            viewMatrix[3, 2] = 0;

            m_shader.startProgram();
            m_shader.SetTransformationMatrices(ref viewMatrix, ref projectionMatrix);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            m_vao.RenderVAO(OpenTK.Graphics.OpenGL.PrimitiveType.LineStrip);
            GL.Disable(EnableCap.Blend);
            m_shader.stopProgram();
        }
    }
}
