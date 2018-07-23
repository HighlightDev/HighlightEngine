using OpenTK;
using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class Bone 
    {
        private Bone parentBone;
        private List<Bone> childrenBones;
        private Int32 id;
        private string name;

        // here is all necessary info regarding bone transformation and it's slerp
        private BoneTransformation localSpaceBoneTransformation;

        public Bone(Int32 boneId, string boneName, Bone parentBone = null)
        {
            name = boneName;
            childrenBones = new List<Bone>();
            this.parentBone = parentBone;
            localSpaceBoneTransformation = new BoneTransformation();
             id = boneId;
        }

        public void SetSkinningMatrix(Matrix4 skinMatrix)
        {
            localSpaceBoneTransformation.SetLocalSkinningMatrix(skinMatrix);
        }

        public Matrix4 GetSkinMatrix()
        {
            return localSpaceBoneTransformation.GetLocalSkinningMatrix();
        }

        public void SetInverseSkinMatrix(Matrix4 inverseSkinMatrix)
        {
            localSpaceBoneTransformation.SetLocalInverseSkinningMatrix(inverseSkinMatrix);
        }

        public Matrix4 GetInverseSkinMatrix()
        {
            return localSpaceBoneTransformation.GetLocalInverseSkinningMatrix();
        }

        public bool IsRootBone()
        {
            return parentBone == null;
        }

        public void AddChildBone(Bone childBone)
        {
            childrenBones.Add(childBone);
        }

        public void AddRangeChildBones(List<Bone> childBoneRange)
        {
            childrenBones.AddRange(childBoneRange);
        }

        public void RemoveChildBone(Bone childBone)
        {
            childrenBones.Remove(childBone);
        }

        public void RemoveRangeChildBones(Int32 index, Int32 count)
        {
            childrenBones.RemoveRange(index, count);
        }

        public void ClearChilderBones()
        {
            childrenBones.Clear();
        }

        public Bone GetRootBone()
        {
            Bone this_bone = this;
            while (this_bone != null)
            {
                this_bone = parentBone;
            }
            return this_bone;
        }

        public void CalcInverseSkinMatrices()
        {
            UpdateInverseSkinMatrixRecursive(parentBone);
        }

        private void UpdateInverseSkinMatrixRecursive(Bone parentBone)
        {
            Matrix4 parentSkinMatrix = parentBone != null ? parentBone.localSpaceBoneTransformation.GetLocalSkinningMatrix() : Matrix4.Identity;
            localSpaceBoneTransformation.SetLocalInverseSkinningMatrix(Matrix4.Mult(localSpaceBoneTransformation.GetLocalSkinningMatrix(), parentSkinMatrix).Inverted());

            foreach (var bone in childrenBones)
            {
                if (bone != null)
                {
                    bone.UpdateInverseSkinMatrixRecursive(this);
                }
            }
        }

        public List<Matrix4> GetAlignedWithIdSkinningMatrices()
        {
            List<Matrix4> collectionOfSkinningMatrices = new List<Matrix4>();
            CollectSkinningMatricesRecursive(ref collectionOfSkinningMatrices, this);
            return collectionOfSkinningMatrices;
        }

        private void CollectSkinningMatricesRecursive(ref List<Matrix4> collectionOfSkinningMatrices, Bone parentBone)
        {
            if (parentBone != null)
            {
                collectionOfSkinningMatrices.Add(localSpaceBoneTransformation.GetLocalSkinningMatrix());
                foreach(var childBone in parentBone.childrenBones)
                {
                    CollectSkinningMatricesRecursive(ref collectionOfSkinningMatrices, childBone);
                }
            }
        }
    }
}
