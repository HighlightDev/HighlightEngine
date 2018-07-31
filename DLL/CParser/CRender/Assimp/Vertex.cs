using Assimp;
using System;
using System.Collections.Generic;

namespace CParser.Assimp
{
    public class Vertex
    {
        public Int32 m_index;
        public List<KeyValuePair<Tuple<Bone, Int32>, float>> BoneWeightMap { set; get; }

        public Vertex(Int32 index)
        {
            m_index = index;
            BoneWeightMap = new List<KeyValuePair<Tuple<Bone, int>, float>>();
        }

        public void AddBoneWeight(KeyValuePair<Tuple<Bone, int>, float> boneInfo)
        {
            BoneWeightMap.Add(boneInfo);
        }
    }
}
