using Assimp;
using System;
using System.Collections.Generic;

namespace CParser.Assimp
{
    public class SkeletonBone
    {
        private SkeletonBone parent;
        private Bone boneInfo;
        private Int32 boneId;
        private List<SkeletonBone> children;

        public SkeletonBone(SkeletonBone parent)
        {
            this.parent = parent;
            children = new List<SkeletonBone>();
        }

        public void SetBoneInfo(Bone info)
        {
            boneInfo = info;
        }

        public void SetBoneId(Int32 id)
        {
            boneId = id;
        }

        public void AddChildBone(SkeletonBone child)
        {
            children.Add(child);
        }

        public Bone GetBoneInfo()
        {
            return boneInfo;
        }

        public Int32 GetBoneId()
        {
            return boneId;
        }

        public SkeletonBone GetParent()
        {
            return parent;
        }

        public List<SkeletonBone> GetChildren()
        {
            return children;
        }

        public Int32 GetIdByBone(Bone seekBone)
        {
            Int32 id = -1;

            if (boneInfo == seekBone)
            {
                id = boneId;
            }

            if (id < 0)
            {
                foreach (var bone in children)
                {
                    id = bone.GetIdByBone(seekBone);
                    if (id > 0)
                        return id;
                }
            }

            return id;
        }
    }
}
