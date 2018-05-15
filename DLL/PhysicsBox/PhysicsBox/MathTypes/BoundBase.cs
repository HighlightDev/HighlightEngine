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
    public class BoundBase
    {

        [Flags]
        public enum BoundType
        {
            AABB = 0x0001,
            OBB = 0x001
        }

        public Component ParentComponent { set; get; }
        public Vector3 Origin { set; get; }
        public Vector3 Extent { set; get; }

        protected Vector3 tangentX = new Vector3(1, 0, 0);
        protected Vector3 tangentY = new Vector3(0, 1, 0);
        protected Vector3 tangentZ = new Vector3(0, 0, 1);

        protected BoundType type;

        public BoundBase()
        {
            Extent = new Vector3(1);
        }

        public BoundBase(Vector3 origin, Vector3 extent)
        {
            Origin = origin;
            Extent = extent;
        }

        public Vector3 GetMax()
        {
            Vector3 p1 = Origin + Extent;
            Vector3 p2 = Origin - Extent;
            return new Vector3(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public Vector3 GetMin()
        {
            Vector3 p1 = Origin + Extent;
            Vector3 p2 = Origin - Extent;
            return new Vector3(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
        }

        public BoundType GetBoundType()
        {
            return type;
        }

        public virtual Vector3 GetTangetX()
        {
            return tangentX;
        }

        public virtual Vector3 GetTangetY()
        {
            return tangentY;
        }

        public virtual Vector3 GetTangetZ()
        {
            return tangentZ;
        }
    }
}
