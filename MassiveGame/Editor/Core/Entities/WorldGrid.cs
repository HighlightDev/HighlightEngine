using MassiveGame.Core.GameCore;
using System;
using OpenTK;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.Core.SettingsCore;
using VBO;
using OpenTK.Graphics.OpenGL;
using TextureLoader;

namespace MassiveGame.EngineEditor.Core.Entities
{
    public class WorldGrid
    {
        private float m_farPlaneDistance;
        private WorldGridShader m_shader;
        private VertexArrayObject m_vao;
        private ITexture m_gridTexture;

        public WorldGrid(float farPlaneDistance)
        {
            m_farPlaneDistance = farPlaneDistance;
            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<WorldGridShader>, string, WorldGridShader>(string.Format("{0}gridVS.glsl,{0}gridFS.glsl", ProjectFolders.ShadersPath));
            m_gridTexture = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(ProjectFolders.EditorTexturePath + "grid.png");

            m_vao = new VertexArrayObject();
            GenQuadAttributes(ref m_vao);
            m_vao.BindBuffersToVao();
        }

        private void GenQuadAttributes(ref VertexArrayObject vao)
        {
            VertexBufferObjectBase verticesVBO = null, texCoordsVBO = null;

            float halfQuad = m_farPlaneDistance / 2;

            /*Screen fill quad*/
            float[,] vertices = new float[6, 3] { { -halfQuad, 0,.0f -halfQuad },
                { halfQuad, 0f, -halfQuad },
                { halfQuad, 0f, halfQuad },
                { halfQuad, 0f, halfQuad },
                { -halfQuad, 0f, halfQuad },
                { -halfQuad, 0f, -halfQuad} };
            float[,] texCoords = new float[6, 2] { { 0, 1 },
                { 1, 1 },
                { 1, 0 },
                { 1, 0 },
                { 0, 0 },
                { 0, 1 } };

            verticesVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            texCoordsVBO = new VertexBufferObjectTwoDimension<float>(texCoords, BufferTarget.ArrayBuffer, 1, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            m_vao.AddVBO(verticesVBO, texCoordsVBO);
        }
        public void Render(BaseCamera baseCamera, ref Matrix4 projectionMatrix)
        {
            Matrix4 viewMatrix = baseCamera.GetViewMatrix();
            viewMatrix[3, 0] = 0;
            viewMatrix[3, 1] = -3;
            viewMatrix[3, 2] = 0;

            m_gridTexture.BindTexture(0);
            m_shader.startProgram();
            m_shader.SetTransformationMatrices(ref viewMatrix, ref projectionMatrix);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            m_vao.RenderVAO(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            m_shader.stopProgram();
        }
    }
}
