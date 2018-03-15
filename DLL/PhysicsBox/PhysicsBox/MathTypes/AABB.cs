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

        public AABB(Vector3 Origin, Vector3 Extent) : base(Origin, Extent)
        {
            type = BoundType.AABB;
        }

        public static AABB CreateFromMinMax(Vector3 min, Vector3 max)
        {
            return new AABB((min + max) * 0.5f, (max - min) * 0.5f);
        }

        public AABB TransformBound(Matrix4 TransformMatrixWithoutRotation)
        {
            Vector3 transformedMin = Vector3.TransformPosition(GetMin(), TransformMatrixWithoutRotation);
            Vector3 transformedMax = Vector3.TransformPosition(GetMax(), TransformMatrixWithoutRotation);
            AABB transformedAABB = AABB.CreateFromMinMax(transformedMin, transformedMax);
            Origin = transformedAABB.Origin;
            Extent = transformedAABB.Extent;
            return transformedAABB;
        }
    }
}
