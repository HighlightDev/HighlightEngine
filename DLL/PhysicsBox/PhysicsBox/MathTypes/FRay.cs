using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.MathTypes
{
    public class FRay
    {
        public Vector3 StartPosition { set; get; }
        public Vector3 Direction { set; get; }

        static public FRay operator*(FRay ray, Matrix4 rotationMatrix)
        {
            return new FRay(Vector3.TransformPosition(ray.StartPosition, rotationMatrix), Vector3.TransformNormal(ray.Direction, rotationMatrix));
        }

        public FRay(Vector3 startPosition, Vector3 direction)
        {
            StartPosition = startPosition;
            Direction = direction;
        }
    }
}
