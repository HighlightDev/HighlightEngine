using Assimp;
using System;
using System.Collections.Generic;

namespace CParser.Assimp
{
    public class LoaderSkeletonParentBone
    {
        protected List<LoaderSkeletonBone> m_children;

        public LoaderSkeletonParentBone()
        {
            m_children = new List<LoaderSkeletonBone>();
        }

        public void AddChildBone(LoaderSkeletonBone child)
        {
            m_children.Add(child);
        }

        public List<LoaderSkeletonBone> GetChildren()
        {
            return m_children;
        }

        public Int32 GetIdByBoneInHierarchy(Bone seekBone)
        {
            Int32 id = -1;

            foreach (var child in m_children)
            {
                id = child.GetIdByBone(seekBone, child);
                if (id >= 0)
                    break;
            }

            return id;
        }

        public Int32 GetIdByBone(Bone seekBone, LoaderSkeletonBone currentSkeletonBone)
        {
            Int32 id = -1;

            if (currentSkeletonBone.GetBoneInfo() == seekBone)
            {
                id = currentSkeletonBone.GetBoneId();
            }

            if (id < 0)
            {
                foreach (var skeletonBone in currentSkeletonBone.GetChildren())
                {
                    id = skeletonBone.GetIdByBone(seekBone, skeletonBone);
                    if (id > 0)
                        return id;
                }
            }

            return id;
        }
    }

    public class LoaderSkeletonBone : LoaderSkeletonParentBone
    {
        private LoaderSkeletonParentBone parent;
        private Bone boneInfo;
        private Int32 boneId;

        public LoaderSkeletonBone(LoaderSkeletonParentBone parent)
        {
            this.parent = parent;
            m_children = new List<LoaderSkeletonBone>();
        }

        public void SetBoneInfo(Bone info)
        {
            boneInfo = info;
        }

        public void SetBoneId(Int32 id)
        {
            boneId = id;
        }

        public Bone GetBoneInfo()
        {
            return boneInfo;
        }

        public Int32 GetBoneId()
        {
            return boneId;
        }

        public LoaderSkeletonParentBone GetParent()
        {
            return parent;
        }

        public void CleanUp()
        {
            foreach (var child in m_children)
            {
                boneInfo = null;
                child.CleanUp();
            }
            m_children = null;
        }
    }
}
