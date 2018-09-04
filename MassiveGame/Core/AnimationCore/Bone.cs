using OpenTK;
using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class Bone : ParentBone
    {
        private Bone m_parent;
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
   
        public void SetOffsetMatrix(Matrix4 offsetMatrix)
        {
            m_offsetMatrix = offsetMatrix;
        }

        public void SetOffsetMatrix(ref Matrix4 offsetMatrix)
        {
            m_offsetMatrix = offsetMatrix;
        }

    }
}
