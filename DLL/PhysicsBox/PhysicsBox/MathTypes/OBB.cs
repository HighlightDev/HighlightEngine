using OpenTK;
using PhysicsBox.ComponentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.MathTypes
{
    [Serializable]
    public class OBB : BoundBase
    {
        public Matrix4 TransformationMatrix;

        public override Vector3 GetExtent()
        {
            return Vector3.TransformVector(Extent, Matrix4.CreateScale(TransformationMatrix.ExtractScale()));
        }

        public override Vector3 GetTangetX()
        {
            return Vector3.TransformNormal(base.GetTangetX(), TransformationMatrix).Normalized();
        }

        public override Vector3 GetTangetY()
        {
            return Vector3.TransformNormal(base.GetTangetY(), TransformationMatrix).Normalized();
        }

        public override Vector3 GetTangetZ()
        {
            return Vector3.TransformNormal(base.GetTangetZ(), TransformationMatrix).Normalized();
        }

        public override BoundType GetBoundType()
        {
            return BoundType.OBB;
        }

        public override Vector3 GetOrigin()
        {
            return Vector3.TransformPosition(base.GetOrigin(), TransformationMatrix);
        }

        public override Vector3 GetMax()
        {
            return Vector3.TransformPosition(base.GetLocalSpaceMax(), TransformationMatrix);
        }

        public override Vector3 GetMin()
        {
            return Vector3.TransformPosition(base.GetLocalSpaceMin(), TransformationMatrix);
        }

        public OBB(Vector3 Origin, Vector3 Extent, Matrix4 TransformationMatrix, Component parentComponent) : base(Origin, Extent, parentComponent)
        {
            this.TransformationMatrix = TransformationMatrix;
        }
    }
}
