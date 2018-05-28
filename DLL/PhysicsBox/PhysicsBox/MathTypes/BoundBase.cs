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
    public abstract class BoundBase
    {
        [Flags]
        public enum BoundType
        {
            AABB = 0x0001,
            OBB = 0x010
        }

        public Component ParentComponent { set; get; }

        protected Vector3 Origin { set; get; }
        protected Vector3 Extent { set; get; }

        public BoundBase(Component parentComponent)
        {
            this.ParentComponent = parentComponent;
            Extent = new Vector3(1);
        }

        public BoundBase(Vector3 origin, Vector3 extent, Component parentComponent)
        {
            this.ParentComponent = parentComponent;
            Origin = origin;
            Extent = extent;
        }

        public Vector3 GetLocalSpaceOrigin()
        {
            return Origin;
        }

        public Vector3 GetLocalSpaceMin()
        {
            Vector3 p1 = Origin + Extent;
            Vector3 p2 = Origin - Extent;
            return new Vector3(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
        }

        public Vector3 GetLocalSpaceMax()
        {
            Vector3 p1 = Origin + Extent;
            Vector3 p2 = Origin - Extent;
            return new Vector3(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public Vector3 GetLocalSpaceExtent()
        {
            return Extent;
        }

        public virtual Vector3 GetMax()
        {
            Vector3 p1 = Origin + Extent;
            Vector3 p2 = Origin - Extent;
            return new Vector3(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public virtual Vector3 GetMin()
        {
            Vector3 p1 = Origin + Extent;
            Vector3 p2 = Origin - Extent;
            return new Vector3(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
        }

        public abstract BoundType GetBoundType();

        public virtual Vector3 GetTangetX()
        {
            return new Vector3(1, 0, 0);
        }

        public virtual Vector3 GetTangetY()
        {
            return new Vector3(0, 1, 0);
        }

        public virtual Vector3 GetTangetZ()
        {
            return new Vector3(0, 0, 1);
        }

        public virtual Vector3 GetOrigin()
        {
            return GetLocalSpaceOrigin();
        }

        public virtual Vector3 GetExtent()
        {
            return GetLocalSpaceExtent();
        }

        public Vector3 GetNormalToIntersectedPosition(Vector3 position)
        {
            // Gather six planes, find the closest plane to incoming position
            // As we know plane equation Ax + By + Cz + D = 0
            // we must substitute in place of xyz incoming position and check when D is near a zero value

            Vector3 min = GetMin();
            Vector3 max = GetMax();
            Vector3 tangentX = GetTangetX();
            Vector3 tangentY = GetTangetY();
            Vector3 tangentZ = GetTangetZ();

            List<FPlane> boundPlanes = new List<FPlane>(6)
            {
                new FPlane(min, -tangentX), new FPlane(min, -tangentY), new FPlane(min, -tangentZ),
                new FPlane(max, tangentX), new FPlane(max, tangentY), new FPlane(max, tangentZ)
            };

            float d = float.MaxValue;
            FPlane closestPlane = null;
            foreach (FPlane plane in boundPlanes)
            {
                float distance = Vector3.Dot((Vector3)plane, position) - plane.D;
                if (Math.Abs(distance) < Math.Abs(d))
                {
                    closestPlane = plane;
                    d = Math.Abs(distance);
                }
            }

            return (Vector3)closestPlane;
        }
    }
}
