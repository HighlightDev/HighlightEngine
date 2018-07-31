using System;

namespace CParser.Assimp.Strategy
{
    public class SkeletonSixBonesInfluenceStrategy : SkeletonPerVertexBoneInfluenceStrategy
    {
        public override void CollectWeightsAndIndices(Vertex blendVertex, int vertexId, float[,] blendWeights, Int32[,] blendIndices)
        {
            throw new NotImplementedException();
        }

        public override int GetBonesInfluenceCount()
        {
            return 6;
        }
    }
}
