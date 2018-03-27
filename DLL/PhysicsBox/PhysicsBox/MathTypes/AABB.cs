using OpenTK;
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

        public Vector3 GetTransformedMin()
        {
            Vector3 min = GetMin();
            return Vector3.TransformPosition(min, ScalePlusTranslation);
        }

        public Vector3 GetTransformedMax()
        {
            Vector3 max = GetMax();
            return Vector3.TransformPosition(max, ScalePlusTranslation);
        }

        public AABB(Vector3 Origin, Vector3 Extent) : base(Origin, Extent)
        {
            type = BoundType.AABB;
        }

        public static AABB CreateFromMinMax(Vector3 min, Vector3 max)
        {
            return new AABB((min + max) * 0.5f, (max - min) * 0.5f);
        }

        public override Vector3 GetTangetX()
        {
            Vector3 scale = ScalePlusTranslation.ExtractScale();
            return (base.GetTangetX() * scale);
        }

        public override Vector3 GetTangetY()
        {
            Vector3 scale = ScalePlusTranslation.ExtractScale();
            return (base.GetTangetY() * scale);
        }

        public override Vector3 GetTangetZ()
        {
            Vector3 scale = ScalePlusTranslation.ExtractScale();
            return (base.GetTangetZ() * scale);
        }
    }
}
