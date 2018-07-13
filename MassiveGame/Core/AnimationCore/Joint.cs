using OpenTK;
using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class Joint 
    {
        private Joint parentJoint;
        private List<Joint> childrenJoints;
        private Int32 id;
        private Matrix4 skinningMatrix;

        public Joint(Int32 jointId, Joint parentJoint = null)
        {
            childrenJoints = new List<AnimationCore.Joint>();
            this.parentJoint = parentJoint;
            skinningMatrix = Matrix4.Identity;
            id = jointId;
        }

        public bool IsRootJoint()
        {
            return parentJoint != null;
        }

        public void AddChildJoint(Joint childJoint)
        {
            childrenJoints.Add(childJoint);
        }

        public void AddRangeChildJoints(List<Joint> childJointRange)
        {
            childrenJoints.AddRange(childJointRange);
        }

        public void RemoveChildJoint(Joint childJoint)
        {
            childrenJoints.Remove(childJoint);
        }

        public void RemoveRagneChildJoints(Int32 index, Int32 count)
        {
            childrenJoints.RemoveRange(index, count);
        }

        public void ClearChilderJoints()
        {
            childrenJoints.Clear();
        }

        public Joint GetRootJoint()
        {
            Joint this_joint = this;
            while (this_joint != null)
            {
                this_joint = parentJoint;
            }
            return this_joint;
        }
    }
}
