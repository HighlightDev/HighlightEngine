using OpenTK;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMath;

namespace MassiveGame.Physics
{
    public class CollisionUnit
    {
        public Component RootComponent { set; get; }
        public bool bBoundingBoxesTransformDirty;
        public List<BoundBase> allBounds = new List<BoundBase>();

        public List<BoundBase> GetBoundingBoxes()
        {
            if (RootComponent != null && bBoundingBoxesTransformDirty)
            {
                IterateAllBoundBoxes(RootComponent, ref allBounds);
            }
            return allBounds;
        }

        public BoundBase GetFirstBoundBox()
        {
            return allBounds.First();
        }

        private void IterateAllBoundBoxes(Component parent, ref List<BoundBase> bounds)
        {
            foreach (var component in parent.ChildrenComponents)
            {
                IterateAllBoundBoxes(component, ref bounds);
                bounds.Add(component.Bound);
            }
        }

        public AABB GetFramingBoundBox()
        {
            List<BoundBase> boundingBoxes = GetBoundingBoxes();
            var max = boundingBoxes[0].GetMax();
            var min = boundingBoxes[0].GetMin();
            float minX = min.X, minY = min.Y, minZ = min.Z, maxX = max.X, maxY = max.Y, maxZ = max.Z;
            for (Int32 i = 1; i < boundingBoxes.Count; i++)
            {
                var localMax = boundingBoxes[i].GetMax();
                var localMin = boundingBoxes[i].GetMin();
                maxX = Math.Max(maxX, localMax.X);
                maxY = Math.Max(maxY, localMax.Y);
                maxZ = Math.Max(maxZ, localMax.Z);
                minX = Math.Min(minX, localMin.X);
                minY = Math.Min(minY, localMin.Y);
                minZ = Math.Min(minZ, localMin.Z);
            }
            return AABB.CreateFromMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }

        public CollisionUnit(Component rootComponent)
        {
            RootComponent = rootComponent;
            bBoundingBoxesTransformDirty = true;
        }

    }
}
