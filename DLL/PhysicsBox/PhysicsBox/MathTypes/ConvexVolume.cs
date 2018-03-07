using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.MathTypes
{
    public class ConvexVolume
    {
        public enum RelativePosition
        {
            BEYOND_PLANE,
            BEFORE_PLANE,
            ON_PLANE
        }

        public FPlane LeftPlane { set; get; }
        public FPlane RightPlane { set; get; }
        public FPlane TopPlane { set; get; }
        public FPlane BottomPlane { set; get; }
        public FPlane NearPlane { set; get; }
        public FPlane FarPlane { set; get; }

        public RelativePosition relativePosition { set; get; }

        public FPlane this[Int32 index]
        {
            set
            {
                switch (index)
                {
                    case 0: LeftPlane = value; break;
                    case 1: RightPlane = value; break;
                    case 2: TopPlane = value; break;
                    case 3: BottomPlane = value; break;
                    case 4: NearPlane = value; break;
                    case 5: FarPlane = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            get
            {
                switch (index)
                {
                    case 0: return LeftPlane;
                    case 1: return RightPlane;
                    case 2: return TopPlane;
                    case 3: return BottomPlane;
                    case 4: return NearPlane;
                    case 5: return FarPlane;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ConvexVolume(FPlane leftPlane, FPlane rightPlane, FPlane topPlane, FPlane bottomPlane, FPlane nearPlane, FPlane farPlane)
        {
            LeftPlane = leftPlane;
            RightPlane = rightPlane;
            TopPlane = topPlane;
            BottomPlane = bottomPlane;
            NearPlane = nearPlane;
            FarPlane = farPlane;
            relativePosition = RelativePosition.BEYOND_PLANE;
        }

        public ConvexVolume(Vector3 leftPoint, Vector3 leftNormal,
            Vector3 rightPoint, Vector3 rightNormal,
            Vector3 topPoint, Vector3 topNormal,
            Vector3 bottomPoint, Vector3 bottomNormal,
            Vector3 nearPoint, Vector3 nearNormal,
            Vector3 farPoint, Vector3 farNormal)
        {
            LeftPlane = new FPlane(leftPoint, leftNormal);
            RightPlane = new FPlane(rightPoint, rightNormal);
            TopPlane = new FPlane(topPoint, topNormal);
            BottomPlane = new FPlane(bottomPoint, bottomNormal);
            NearPlane = new FPlane(nearPoint, nearNormal);
            FarPlane = new FPlane(farPoint, farNormal);
            relativePosition = RelativePosition.BEYOND_PLANE;
        }

        public ConvexVolume(Matrix4 ViewProjectionMatrix)
        {
            // Right clipping plane.
            RightPlane = new FPlane(new Vector4(ViewProjectionMatrix.M14 - ViewProjectionMatrix.M11, ViewProjectionMatrix.M24 - ViewProjectionMatrix.M21, ViewProjectionMatrix.M34 - ViewProjectionMatrix.M31, ViewProjectionMatrix.M44 - ViewProjectionMatrix.M41));
            // Left clipping plane.
            LeftPlane = new FPlane(new Vector4(ViewProjectionMatrix.M14 + ViewProjectionMatrix.M11, ViewProjectionMatrix.M24 + ViewProjectionMatrix.M21, ViewProjectionMatrix.M34 + ViewProjectionMatrix.M31, ViewProjectionMatrix.M44 + ViewProjectionMatrix.M41));
            // Bottom clipping plane.
            BottomPlane = new FPlane(new Vector4(ViewProjectionMatrix.M14 + ViewProjectionMatrix.M12, ViewProjectionMatrix.M24 + ViewProjectionMatrix.M22, ViewProjectionMatrix.M34 + ViewProjectionMatrix.M32, ViewProjectionMatrix.M44 + ViewProjectionMatrix.M42));
            // Top clipping plane.
            TopPlane = new FPlane(new Vector4(ViewProjectionMatrix.M14 - ViewProjectionMatrix.M12, ViewProjectionMatrix.M24 - ViewProjectionMatrix.M22, ViewProjectionMatrix.M34 - ViewProjectionMatrix.M32, ViewProjectionMatrix.M44 - ViewProjectionMatrix.M42));
            // Far clipping plane.
            FarPlane = new FPlane(new Vector4(ViewProjectionMatrix.M14 - ViewProjectionMatrix.M13, ViewProjectionMatrix.M24 - ViewProjectionMatrix.M23, ViewProjectionMatrix.M34 - ViewProjectionMatrix.M33, ViewProjectionMatrix.M44 - ViewProjectionMatrix.M43));
            // Near clipping plane.
            NearPlane = new FPlane(new Vector4(ViewProjectionMatrix.M13, ViewProjectionMatrix.M23, ViewProjectionMatrix.M33, ViewProjectionMatrix.M43));

            // This is not always necessary...
            for (Int32 i = 0; i < 6; i++)
            {
                Vector3 Normalized = new Vector3(this[i].X, this[i].Y, this[i].Z).Normalized();
                this[i] = new FPlane(new Vector4(Normalized, this[i].D));
            }
        }

    }
}