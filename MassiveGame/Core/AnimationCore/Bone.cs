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

        // here are all necessary info regarding bone transformation and it's slerp
        private BoneTransformation localSpaceBoneTransformation;

        public Bone(Int32 boneId, string boneName, Bone parentBone = null)
        {
            m_name = boneName;
            m_children = new List<Bone>();
            this.m_parent = parentBone;
            localSpaceBoneTransformation = new BoneTransformation();
            m_id = boneId;
        }

        public void SetBoneTransformation(BoneTransformation boneTransformation)
        {
            localSpaceBoneTransformation = boneTransformation;
        }

        public void SetOffsetMatrix(Matrix4 offsetMatrix)
        {
            localSpaceBoneTransformation.SetLocalOffsetMatrix(offsetMatrix);
        }

        public Matrix4 GetOffsetMatrix()
        {
            return localSpaceBoneTransformation.GetLocalOffsetMatrix();
        }

        public void SetInverseOffsetMatrix(Matrix4 inverseOffsetMatrix)
        {
            localSpaceBoneTransformation.SetLocalInverseOffsetMatrix(inverseOffsetMatrix);
        }

        public Matrix4 GetInverseOffsetMatrix()
        {
            return localSpaceBoneTransformation.GetLocalInverseOffsetMatrix();
        }

        public bool IsRootBone()
        {
            return m_parent == null;
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

        public void ClearChilderBones()
        {
            m_children.Clear();
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

        public bool SetSkeletonUpdatedTransforms(List<BoneTransformation> alignedWithIdListOfBoneTransformations)
        {
            Bone rootBone = GetRootBone();
            return SetEachBoneItsMatrix(alignedWithIdListOfBoneTransformations, rootBone);
        }

        private bool SetEachBoneItsMatrix(List<BoneTransformation> alignedWithIdListOfBoneTransformations, Bone parentBone)
        {
            bool bResult = false;

            // make check if matrix with id of current parent bone exists
            if (alignedWithIdListOfBoneTransformations.Count >= parentBone.m_id + 1)
            {
                bResult = true;
                parentBone.SetBoneTransformation(alignedWithIdListOfBoneTransformations[parentBone.m_id]);
                foreach (var childBone in parentBone.m_children)
                {
                    bResult = SetEachBoneItsMatrix(alignedWithIdListOfBoneTransformations, childBone);
                    if (!bResult)
                        break;
                }
            }

            return bResult;
        }

        public void CalcInverseOffsetMatrices()
        {
            UpdateInverseOffsetMatrixRecursive(m_parent);
        }

        private void UpdateInverseOffsetMatrixRecursive(Bone parentBone)
        {
            Matrix4 parentOffsetMatrix = parentBone != null ? parentBone.localSpaceBoneTransformation.GetLocalOffsetMatrix() : Matrix4.Identity;
            localSpaceBoneTransformation.SetLocalInverseOffsetMatrix(Matrix4.Mult(localSpaceBoneTransformation.GetLocalOffsetMatrix(), parentOffsetMatrix).Inverted());

            foreach (var bone in m_children)
            {
                if (bone != null)
                {
                    bone.UpdateInverseOffsetMatrixRecursive(this);
                }
            }
        }

        public List<Matrix4> GetAlignedWithIdListOffsetMatrices()
        {
            Bone rootBone = GetRootBone();

            List<Matrix4> collectionOfOffsetMatrices = new List<Matrix4>();
            CollectOffsetMatricesRecursive(Matrix4.CreateRotationX(MathHelper.DegreesToRadians(270)), ref collectionOfOffsetMatrices, rootBone);
            return collectionOfOffsetMatrices;
        }

        private void CollectOffsetMatricesRecursive(Matrix4 toParentSpaceMatrix, ref List<Matrix4> collectionOfOffsetMatrices, Bone parentBone)
        {
            if (parentBone != null)
            {
               
                Matrix4 ToLocalBoneSpace = parentBone.localSpaceBoneTransformation.GetLocalOffsetMatrix() * toParentSpaceMatrix;

                collectionOfOffsetMatrices.Add(ToLocalBoneSpace);
                foreach (var childBone in parentBone.m_children)
                {
                    CollectOffsetMatricesRecursive(ToLocalBoneSpace, ref collectionOfOffsetMatrices, childBone);
                }
            }
        }

        public List<Matrix4> GetAlignedWithIdListOfInvertedOffsetMatrices()
        {
            Bone rootBone = GetRootBone();

            List<Matrix4> collectionOfOffsetMatrices = new List<Matrix4>();
            CollectInverseOffsetMatricesRecursive(ref collectionOfOffsetMatrices, rootBone);
            return collectionOfOffsetMatrices;
        }

        private void CollectInverseOffsetMatricesRecursive(ref List<Matrix4> collectionOfOffsetMatrices, Bone parentBone)
        {
            if (parentBone != null)
            {
                collectionOfOffsetMatrices.Add(parentBone.localSpaceBoneTransformation.GetLocalInverseOffsetMatrix());
                foreach (var childBone in parentBone.m_children)
                {
                    CollectInverseOffsetMatricesRecursive(ref collectionOfOffsetMatrices, childBone);
                }
            }
        }

        public void CleanUp()
        {
            m_parent = null;
            foreach (var child in m_children)
            {
                child.CleanUp();
            }
            ClearChilderBones();
        }
    }
}
