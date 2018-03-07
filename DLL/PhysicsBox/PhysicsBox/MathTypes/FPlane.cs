using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace PhysicsBox.MathTypes
{
    public class FPlane
    {
        public float X { set; get; }
        public float Y { set; get; }
        public float Z { set; get; }
        public float D { set; get; }

        public FPlane()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
            this.Z = 0.0f;
            this.D = 0.0f;
        }

        public FPlane(Vector4 Plane)
        {
            this.X = Plane.X;
            this.Y = Plane.Y;
            this.Z = Plane.Z;
            this.D = Plane.W;
        }

        public FPlane(FPlane Plane)
        {
            this.X = Plane.X;
            this.Y = Plane.Y;
            this.Z = Plane.Z;
            this.D = Plane.D;
        }

        public FPlane(Vector3 InBase, Vector3 InNormal)
        {
            this.X = InBase.X;
            this.Y = InBase.Y;
            this.Z = InBase.Z;
            this.D = Vector3.Dot(InBase, InNormal);
        }

        public static explicit operator Vector4(FPlane plane)
        {
            return new Vector4(plane.X, plane.Y, plane.Z, plane.D);
        }
    }
}
