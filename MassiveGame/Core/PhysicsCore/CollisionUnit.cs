using OpenTK;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassiveGame.Core.PhysicsCore
{
    public class CollisionUnit
    {
        public Component RootComponent { set; get; }
        public bool bBoundingBoxesTransformDirty;
        public List<BoundBase> allBounds = new List<BoundBase>();

        private AABB framingBoundBox = null;

        public CollisionUnit(Component rootComponent)
        {
            RootComponent = rootComponent;
            bBoundingBoxesTransformDirty = true;
            GetAndTryUpdateFramingBoundingBox();
        }

        public List<BoundBase> GetBoundingBoxes()
        {
            if (RootComponent != null && bBoundingBoxesTransformDirty)
            {
                allBounds.Clear();
                IterateAllBoundBoxes(RootComponent, ref allBounds);
            }
            return allBounds;
        }

        public BoundBase GetFirstBoundingBox()
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

        public AABB GetAndTryUpdateFramingBoundingBox()
        {
            List<BoundBase> boundingBoxes = GetBoundingBoxes();

            // Update framing bounding box only if transformation of component has changed
            if (bBoundingBoxesTransformDirty)
            {
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

                framingBoundBox = AABB.CreateFromMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ), RootComponent);
            }
            
            return framingBoundBox;
        }
    }
}
