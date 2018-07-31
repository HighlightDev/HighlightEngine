using System;

namespace CParser.Assimp.Strategy
{
    public abstract class SkeletonPerVertexBoneInfluenceStrategy
    {
        public abstract Int32 GetBonesInfluenceCount();
        public abstract void CollectWeightsAndIndices(Vertex blendVertex, int vertexId, float[,] blendWeights, Int32[,] blendIndices);
    }
}
