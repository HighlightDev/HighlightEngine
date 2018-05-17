using OpenTK;
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
            return Vector3.TransformPosition(Extent, Matrix4.CreateScale(TransformationMatrix.ExtractScale()));
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
            Vector3 p1 = Vector3.TransformPosition(Origin + Extent, TransformationMatrix);
            Vector3 p2 = Vector3.TransformPosition(Origin - Extent, TransformationMatrix);
            return new Vector3(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public override Vector3 GetMin()
        {
            Vector3 p1 = Vector3.TransformPosition(Origin + Extent, TransformationMatrix);
            Vector3 p2 = Vector3.TransformPosition(Origin - Extent, TransformationMatrix);
            return new Vector3(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
        }

        public OBB(Vector3 Origin, Vector3 Extent, Matrix4 TransformationMatrix) : base(Origin, Extent)
        {
            this.TransformationMatrix = TransformationMatrix;
        }
    }
}
