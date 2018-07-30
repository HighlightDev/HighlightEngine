using Assimp;
using System;
using System.Collections.Generic;

namespace CParser.Assimp
{
    public class Vertex
    {
        public Int32 m_index;
        public Dictionary<Tuple<Bone, Int32>, float> BoneWeightMap { set; get; }

        public Vertex(Int32 index)
        {
            m_index = index;
            BoneWeightMap = new Dictionary<Tuple<Bone, Int32>, float>();
        }

        public void AddBoneWeight(Tuple<Bone, Int32> bone, float weight)
        {
            BoneWeightMap.Add(bone, weight);
        }
    }
}
