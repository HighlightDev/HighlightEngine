using VBO;

namespace MassiveGame.Core.RenderCore
{
    public static class ScreenQuad
    {
        private static VertexArrayObject quadBuffer;

        public static VertexArrayObject GetScreenQuadBuffer()
        {
            return quadBuffer;
        }

        static ScreenQuad()
        {
            /*Screen fill quad*/
            float[,] vertices = new float[6, 3] { { -1.0f, -1.0f, 0.0f },
                { 1.0f, -1.0f, 0.0f },
                { 1.0f, 1.0f, 0.0f },
                { 1.0f, 1.0f, 0.0f },
                { -1.0f, 1.0f, 0.0f },
                { -1.0f, -1.0f, 0.0f} };
            float[,] texCoords = new float[6, 2] { { 0, 1 },
                { 1, 1 },
                { 1, 0 },
                { 1, 0 },
                { 0, 0 },
                { 0, 1 } };

            VertexBufferObject<float> verticesVBO = new VertexBufferObject<float>(vertices, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> texCoordsVBO = new VertexBufferObject<float>(texCoords, OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, 1, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            quadBuffer = new VertexArrayObject();

            quadBuffer.AddVBO(verticesVBO, texCoordsVBO);
            quadBuffer.BindVbosToVao();
        }
        
    }
}
