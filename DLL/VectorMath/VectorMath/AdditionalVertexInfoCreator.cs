using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public static class AdditionalVertexInfoCreator
    {
        public static float[,] CreateTangentVertices(float[,] vertices, float[,] textureCoordinates)
        {
            Int32 vertexCount = vertices.Length / 3;
            float[,] resultTangent = new float[vertexCount, 3];

            for (Int32 i = 0; i < vertexCount; i += 3)
            {
                Vector3 v0 = new Vector3(vertices[i, 0], vertices[i, 1], vertices[i, 2]);
                Vector3 v1 = new Vector3(vertices[i + 1, 0], vertices[i + 1, 1], vertices[i + 1, 2]);
                Vector3 v2 = new Vector3(vertices[i + 2, 0], vertices[i + 2, 1], vertices[i + 2, 2]);
                Vector2 uv0 = new Vector2(textureCoordinates[i, 0], textureCoordinates[i, 1]);
                Vector2 uv1 = new Vector2(textureCoordinates[i + 1, 0], textureCoordinates[i + 1, 1]);
                Vector2 uv2 = new Vector2(textureCoordinates[i + 2, 0], textureCoordinates[i + 2, 1]);

                // delta vertex
                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;

                //delta texCoords
                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                Vector3 Tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                resultTangent[i, 0] = Tangent.X;
                resultTangent[i, 1] = Tangent.Y;
                resultTangent[i, 2] = Tangent.Z;
                resultTangent[i + 1, 0] = Tangent.X;
                resultTangent[i + 1, 1] = Tangent.Y;
                resultTangent[i + 1, 2] = Tangent.Z;
                resultTangent[i + 2, 0] = Tangent.X;
                resultTangent[i + 2, 1] = Tangent.Y;
                resultTangent[i + 2, 2] = Tangent.Z;
            }
            return resultTangent;
        }

        public static float[,] CreateBitangentVertices(float[,] vertices, float[,] textureCoordinates)
        {
            Int32 vertexCount = vertices.Length / 3;
            float[,] resultBitangent = new float[vertexCount, 3];

            for (Int32 i = 0; i < vertexCount; i += 3)
            {
                Vector3 v0 = new Vector3(vertices[i, 0], vertices[i, 1], vertices[i, 2]);
                Vector3 v1 = new Vector3(vertices[i + 1, 0], vertices[i + 1, 1], vertices[i + 1, 2]);
                Vector3 v2 = new Vector3(vertices[i + 2, 0], vertices[i + 2, 1], vertices[i + 2, 2]);
                Vector2 uv0 = new Vector2(textureCoordinates[i, 0], textureCoordinates[i, 1]);
                Vector2 uv1 = new Vector2(textureCoordinates[i + 1, 0], textureCoordinates[i + 1, 1]);
                Vector2 uv2 = new Vector2(textureCoordinates[i + 2, 0], textureCoordinates[i + 2, 1]);

                // delta vertex
                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;

                //delta texCoords
                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                Vector3 bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;
                resultBitangent[i, 0] = bitangent.X;
                resultBitangent[i, 1] = bitangent.Y;
                resultBitangent[i, 2] = bitangent.Z;
                resultBitangent[i + 1, 0] = bitangent.X;
                resultBitangent[i + 1, 1] = bitangent.Y;
                resultBitangent[i + 1, 2] = bitangent.Z;
                resultBitangent[i + 2, 0] = bitangent.X;
                resultBitangent[i + 2, 1] = bitangent.Y;
                resultBitangent[i + 2, 2] = bitangent.Z;
            }
            return resultBitangent;
        }
    }
}
