using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Core.AnimationCore
{
    public class ParentBone
    {
        protected List<Bone> m_children;

        public ParentBone()
        {
            m_children = new List<Bone>();
        }

        public List<Bone> GetChildren()
        {
            return m_children;
        }

        public void AddChildBone(Bone childBone)
        {
            m_children.Add(childBone);
        }

        public void AddRangeChildBones(List<Bone> childBoneRange)
        {
            m_children.AddRange(childBoneRange);
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

        public void CleanUp()
        {
            foreach (var child in m_children)
            {
                child.CleanUp();
            }
            ClearChildrenBones();
        }

        public List<Matrix4> GetToBoneSpaceMatricesList()
        {
            List<Matrix4> collectionOfOffsetMatrices = new List<Matrix4>();

            foreach (Bone child in m_children)
            {
                collectionOfOffsetMatrices.Add(child.GetOffsetMatrix());
                IterateToBoneHierarchy(child, ref collectionOfOffsetMatrices);
            }
            return collectionOfOffsetMatrices;
        }

        private void IterateToBoneHierarchy(Bone parentBone, ref List<Matrix4> collectionOfOffsetMatrices)
        {
            foreach (var child in parentBone.m_children)
            {
                collectionOfOffsetMatrices.Add(child.GetOffsetMatrix());
                IterateToBoneHierarchy(child, ref collectionOfOffsetMatrices);
            }
        }

        public List<Matrix4> GetToParentSpaceMatricesList(List<Matrix4> toBoneSpaceMatrices)
        {
            List<Matrix4> resultToParentSpaceMatrices = new List<Matrix4>();

            for (Int32 i = 0; i < m_children.Count; i++)
            {
                // iterate through whole hierarchy and collect inverted ToBoneSpace matrices
                Matrix4 toParentSpaceMatrix = toBoneSpaceMatrices[i].Inverted();
                resultToParentSpaceMatrices.Add(toParentSpaceMatrix);
                IterateToParentHierarchy(m_children[i], ref toParentSpaceMatrix, toBoneSpaceMatrices, ref resultToParentSpaceMatrices);
            }
            return resultToParentSpaceMatrices;
        }

        private void IterateToParentHierarchy(Bone parent, ref Matrix4 toParentSpaceMatrix, List<Matrix4> toBoneSpaceMatrices, ref List<Matrix4> toParentSpaceMatrices)
        {
            foreach (var child in parent.GetChildren())
            {
                Matrix4 fromBoneSpaceMatrix = toBoneSpaceMatrices[child.GetId()].Inverted();
                Matrix4 childToParentSpaceMatrix = fromBoneSpaceMatrix * toParentSpaceMatrix;
                toParentSpaceMatrices.Add(childToParentSpaceMatrix);
                IterateToParentHierarchy(child, ref toParentSpaceMatrix, toBoneSpaceMatrices, ref toParentSpaceMatrices);
            }
        }
    }
}
