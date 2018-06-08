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
            return ParentComponent.GetTotalScale();
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
            ScalePlusTranslation = Matrix4.Identity;
        }

        public static AABB CreateFromMinMax(Vector3 min, Vector3 max, Component parentComponent)
        {
            return new AABB((min + max) * 0.5f, (max - min) * 0.5f, parentComponent);
        }

        public override Vector3 GetTangetX()
        {
            Vector3 scale = ParentComponent.GetTotalScale();
            return (base.GetTangetX() * scale).Normalized();
        }

        public override Vector3 GetTangetY()
        {
            Vector3 scale = ParentComponent.GetTotalScale();
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

        public override Vector3[] GetWorldSpaceVertices()
        {
            Vector3[] resultVertices = null;

            Vector3 extent = GetExtent();
            Vector3 position = GetOrigin();

            // find all vertices of axis aligned bounding box
            resultVertices = new Vector3[8];
            resultVertices[0] = position + extent;
            resultVertices[1] = position + new Vector3(-extent.X, extent.Y, extent.Z);
            resultVertices[2] = position + new Vector3(extent.X, -extent.Y, extent.Z);
            resultVertices[3] = position + new Vector3(extent.X, extent.Y, -extent.Z);
            resultVertices[4] = position - new Vector3(extent.X, extent.Y, extent.Z);
            resultVertices[5] = position + new Vector3(extent.X, -extent.Y, -extent.Z);
            resultVertices[6] = position + new Vector3(-extent.X, extent.Y, -extent.Z);
            resultVertices[7] = position + new Vector3(-extent.X, -extent.Y, extent.Z);

            return resultVertices;
        }

        public override Vector3 GetNormalToIntersectedPosition(Vector3 position)
        {
            // Gather six planes, find the closest plane to incoming position
            // As we know plane equation Ax + By + Cz + D = 0
            // we must substitute in place of xyz incoming position and check when D is near a zero value

            Vector3 tangentSpaceExtent = GetExtent();
            Vector3 tangentSpacePosition = GetOrigin();
            Vector3 tangentX = GetTangetX();
            Vector3 tangentY = GetTangetY();
            Vector3 tangentZ = GetTangetZ();

            // find edge vertices for each plane of bounding box
            Vector3 edgeRight = tangentSpacePosition + (tangentX * tangentSpaceExtent.X);
            Vector3 edgeLeft = tangentSpacePosition - (tangentX * tangentSpaceExtent.X);
            Vector3 edgeUp = tangentSpacePosition + (tangentY * tangentSpaceExtent.Y);
            Vector3 edgeDown = tangentSpacePosition - (tangentY * tangentSpaceExtent.Y);
            Vector3 edgeForward = tangentSpacePosition + (tangentZ * tangentSpaceExtent.Z);
            Vector3 edgeBack = tangentSpacePosition - (tangentZ * tangentSpaceExtent.Z);

            FPlane[] boundPlanes = new FPlane[6]
            {
                new FPlane(edgeLeft, -tangentX), new FPlane(edgeDown, -tangentY), new FPlane(edgeBack, -tangentZ),
                new FPlane(edgeRight, tangentX), new FPlane(edgeUp, tangentY), new FPlane(edgeForward, tangentZ)
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
