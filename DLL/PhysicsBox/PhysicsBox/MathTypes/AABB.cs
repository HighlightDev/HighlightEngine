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
    public class AABB : BoundBase
    {
        public Matrix4 ScalePlusTranslation;

        public override Vector3 GetExtent()
        {
            return Vector3.TransformPosition(Extent, Matrix4.CreateScale(ScalePlusTranslation[0, 0], ScalePlusTranslation[1, 1], ScalePlusTranslation[2, 2]));
        }

        public override Vector3 GetOrigin()
        {
            return Vector3.TransformPosition(Origin, ScalePlusTranslation);
        }

        public override Vector3 GetMin()
        {
            Vector3 p1 = Vector3.TransformPosition(Origin + Extent, ScalePlusTranslation);
            Vector3 p2 = Vector3.TransformPosition(Origin - Extent, ScalePlusTranslation);
            return new Vector3(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
        }

        public override Vector3 GetMax()
        {
            Vector3 p1 = Vector3.TransformPosition(Origin + Extent, ScalePlusTranslation);
            Vector3 p2 = Vector3.TransformPosition(Origin - Extent, ScalePlusTranslation);
            return new Vector3(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public AABB(Vector3 Origin, Vector3 Extent, Component parentComponent) : base(Origin, Extent, parentComponent)
        {
        }

        public static AABB CreateFromMinMax(Vector3 min, Vector3 max, Component parentComponent)
        {
            return new AABB((min + max) * 0.5f, (max - min) * 0.5f, parentComponent);
        }

        public override Vector3 GetTangetX()
        {
            Vector3 scale = ScalePlusTranslation.ExtractScale();
            return (base.GetTangetX() * scale).Normalized();
        }

        public override Vector3 GetTangetY()
        {
            Vector3 scale = ScalePlusTranslation.ExtractScale();
            return (base.GetTangetY() * scale).Normalized();
        }

        public override Vector3 GetTangetZ()
        {
            Vector3 scale = ScalePlusTranslation.ExtractScale();
            return (base.GetTangetZ() * scale).Normalized();
        }

        public override BoundType GetBoundType()
        {
            return BoundType.AABB;
        }
    }
}
