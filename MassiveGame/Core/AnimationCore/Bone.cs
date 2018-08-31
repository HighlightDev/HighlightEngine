using OpenTK;
using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class Bone 
    {
        private Bone m_parent;
        private List<Bone> m_children;
        private Int32 m_id;
        private string m_name;

        // here is necessary info regarding bone transformation and it's slerp
        private Matrix4 m_offsetMatrix;

        public Bone(Int32 boneId, string boneName, Bone parentBone = null)
        {
            m_name = boneName;
            m_children = new List<Bone>();
            this.m_parent = parentBone;
            m_offsetMatrix = Matrix4.Identity;
            m_id = boneId;
        }

        public Bone GetParent()
        {
            return m_parent;
        }

        public List<Bone> GetChildren()
        {
            return m_children;
        }

        public Int32 GetId()
        {
            return m_id;
        }

        public string GetName()
        {
            return m_name;
        }

        public Matrix4 GetOffsetMatrix()
        {
            return m_offsetMatrix;
        }

        public List<Matrix4> GetToParentSpaceMatricesList(List<Matrix4> toBoneSpaceMatrices)
        {
            Bone rootBone = GetRootBone();
            List<Matrix4> resultToParentSpaceMatrices = new List<Matrix4>();

            // iterate through whole hierarchy and collect inverted ToBoneSpace matrices
            Matrix4 toParentSpaceMatrix = toBoneSpaceMatrices[0].Inverted();
            resultToParentSpaceMatrices.Add(toParentSpaceMatrix);
            IterateBoneHierarchy(rootBone, ref toParentSpaceMatrix, toBoneSpaceMatrices, ref resultToParentSpaceMatrices);

            return resultToParentSpaceMatrices;
        }

        private void IterateBoneHierarchy(Bone parent, ref Matrix4 toParentSpaceMatrix, List<Matrix4> toBoneSpaceMatrices, ref List<Matrix4> toParentSpaceMatrices)
        {
            foreach (var child in parent.GetChildren())
            {
                Matrix4 fromBoneSpaceMatrix = toBoneSpaceMatrices[child.GetId()].Inverted();
                Matrix4 childToParentSpaceMatrix = fromBoneSpaceMatrix * toParentSpaceMatrix;
                toParentSpaceMatrices.Add(childToParentSpaceMatrix);
                IterateBoneHierarchy(child, ref toParentSpaceMatrix, toBoneSpaceMatrices, ref toParentSpaceMatrices);
            }
        }

        public Bone GetRootBone()
        {
            Bone this_bone = this;
            while (this_bone.m_parent != null)
            {
                this_bone = this_bone.m_parent;
            }
            return this_bone;
        }

        public bool IsRootBone()
        {
            return m_parent == null;
        }

        public void SetOffsetMatrix(Matrix4 offsetMatrix)
        {
            m_offsetMatrix = offsetMatrix;
        }

        public void SetOffsetMatrix(ref Matrix4 offsetMatrix)
        {
            m_offsetMatrix = offsetMatrix;
        }

        public void AddChildBone(Bone childBone)
        {
            m_children.Add(childBone);
        }

        public void AddRangeChildBones(List<Bone> childBoneRange)
        {
            m_children.AddRange(childBoneRange);
        }
     
        public List<Matrix4> GetSortedWithIdOffsetMatrices()
        {
            Bone rootBone = GetRootBone();

            List<Matrix4> collectionOfOffsetMatrices = new List<Matrix4>();
            collectionOfOffsetMatrices.Add(rootBone.GetOffsetMatrix());
            CollectOffsetMatricesRecursive(rootBone, ref collectionOfOffsetMatrices);
            return collectionOfOffsetMatrices;
        }

        private void CollectOffsetMatricesRecursive(Bone parentBone, ref List<Matrix4> collectionOfOffsetMatrices)
        {
            foreach (var child in parentBone.m_children)
            {
                collectionOfOffsetMatrices.Add(child.m_offsetMatrix);
                CollectOffsetMatricesRecursive(child, ref collectionOfOffsetMatrices);
            }
        }
       
        public void CleanUp()
        {
            m_parent = null;
            foreach (var child in m_children)
            {
                child.CleanUp();
            }
            ClearChildrenBones();
        }

        public void RemoveChildBone(Bone childBone)
        {
            m_children.Remove(childBone);
        }

        public void RemoveRangeChildBones(Int32 index, Int32 count)
        {
            m_children.RemoveRange(index, count);
        }

        public void ClearChildrenBones()
        {
            m_children.Clear();
        }
    }
}
