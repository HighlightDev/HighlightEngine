using System;

namespace CParser.Assimp.Strategy
{
    public class SkeletonThreeBonesInfluenceStrategy : SkeletonPerVertexBoneInfluenceStrategy
    {
        public override void CollectWeightsAndIndices(Vertex blendVertex, int vertexId, float[,] blendWeights, Int32[,] blendIndices)
        {
            Int32 currentVertexInfluenceCount = blendVertex.BoneWeightMap.Count;

            if (currentVertexInfluenceCount == 1)
            {
                blendWeights[vertexId, 0] = blendVertex.BoneWeightMap[0].Value;
                blendIndices[vertexId, 0] = blendVertex.BoneWeightMap[0].Key.Item2;

                blendWeights[vertexId, 1] = 0.0f;
                blendIndices[vertexId, 1] = -1;  // this provides assurance that skin matrix will not do anything in shader.

                blendWeights[vertexId, 2] = 0.0f;
                blendIndices[vertexId, 2] = -1;   // this provides assurance that skin matrix will not do anything in shader.
            }
            else if (currentVertexInfluenceCount == 2)
            {
                blendWeights[vertexId, 0] = blendVertex.BoneWeightMap[0].Value;
                blendIndices[vertexId, 0] = blendVertex.BoneWeightMap[0].Key.Item2;

                blendWeights[vertexId, 1] = blendVertex.BoneWeightMap[1].Value;
                blendIndices[vertexId, 1] = blendVertex.BoneWeightMap[1].Key.Item2;

                blendWeights[vertexId, 2] = 0.0f; 
                blendIndices[vertexId, 2] = -1; // this provides assurance that skin matrix will not do anything in shader.
            }
            else if (currentVertexInfluenceCount > 2)
            {
                blendWeights[vertexId, 0] = blendVertex.BoneWeightMap[0].Value;
                blendIndices[vertexId, 0] = blendVertex.BoneWeightMap[0].Key.Item2;

                blendWeights[vertexId, 1] = blendVertex.BoneWeightMap[1].Value;
                blendIndices[vertexId, 1] = blendVertex.BoneWeightMap[1].Key.Item2;

                blendWeights[vertexId, 2] = blendVertex.BoneWeightMap[2].Value;
                blendIndices[vertexId, 2] = blendVertex.BoneWeightMap[2].Key.Item2;
            }
        }

        public override Int32 GetBonesInfluenceCount()
        {
            return 3;
        }
    }
}
