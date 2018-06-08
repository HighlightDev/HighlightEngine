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
    public class OBB : BoundBase
    {
        public Matrix4 TransformationMatrix;

        public override Vector3 GetExtent()
        {
            return ParentComponent.GetTotalScale();
        }

        public override Vector3 GetTangetX()
        {
            return Vector3.TransformNormal(base.GetTangetX(), TransformationMatrix).Normalized();
        }

        public override Vector3 GetTangetY()
        {
            return Vector3.TransformNormal(base.GetTangetY(), TransformationMatrix).Normalized();
        }

        public override Vector3 GetTangetZ()
        {
            return Vector3.TransformNormal(base.GetTangetZ(), TransformationMatrix).Normalized();
        }

        public override BoundType GetBoundType()
        {
            return BoundType.OBB;
        }

        public override Vector3 GetOrigin()
        {
            return Vector3.TransformPosition(base.GetOrigin(), TransformationMatrix);
        }

        public override Vector3[] GetWorldSpaceVertices()
        {
            Vector3[] resultVertices = null;

            Vector3 extent = GetLocalSpaceExtent();
            Vector3 position = GetLocalSpaceOrigin();

            // find all vertices of rotated bounding box
            resultVertices = new Vector3[8];
            resultVertices[0] = position + extent;
            resultVertices[1] = position + new Vector3(-extent.X, extent.Y, extent.Z);
            resultVertices[2] = position + new Vector3(extent.X, -extent.Y, extent.Z);
            resultVertices[3] = position + new Vector3(extent.X, extent.Y, -extent.Z);
            resultVertices[4] = position - new Vector3(extent.X, extent.Y, extent.Z);
            resultVertices[5] = position + new Vector3(extent.X, -extent.Y, -extent.Z);
            resultVertices[6] = position + new Vector3(-extent.X, extent.Y, -extent.Z);
            resultVertices[7] = position + new Vector3(-extent.X, -extent.Y, extent.Z);

            for (Int32 i = 0; i < resultVertices.Length; i++)
                resultVertices[i] = Vector3.TransformPosition(resultVertices[i], TransformationMatrix);


            return resultVertices;
        }

        public override Vector3 GetMax()
        {
            var vertices = GetWorldSpaceVertices();
            return new Vector3
                  (Math.Max(Math.Max(Math.Max(vertices[0].X, vertices[1].X), Math.Max(vertices[2].X, vertices[3].X)), Math.Max(Math.Max(vertices[4].X, vertices[5].X), Math.Max(vertices[6].X, vertices[7].X)))
                , (Math.Max(Math.Max(Math.Max(vertices[0].Y, vertices[1].Y), Math.Max(vertices[2].Y, vertices[3].Y)), Math.Max(Math.Max(vertices[4].Y, vertices[5].Y), Math.Max(vertices[6].Y, vertices[7].Y)))),
                  (Math.Max(Math.Max(Math.Max(vertices[0].Z, vertices[1].Z), Math.Max(vertices[2].Z, vertices[3].Z)), Math.Max(Math.Max(vertices[4].Z, vertices[5].Z), Math.Max(vertices[6].Z, vertices[7].Z)))));
        }

        public override Vector3 GetMin()
        {
            var vertices = GetWorldSpaceVertices();
            return new Vector3
                  (Math.Min(Math.Min(Math.Min(vertices[0].X, vertices[1].X), Math.Min(vertices[2].X, vertices[3].X)), Math.Min(Math.Min(vertices[4].X, vertices[5].X), Math.Min(vertices[6].X, vertices[7].X)))
                , (Math.Min(Math.Min(Math.Min(vertices[0].Y, vertices[1].Y), Math.Min(vertices[2].Y, vertices[3].Y)), Math.Min(Math.Min(vertices[4].Y, vertices[5].Y), Math.Min(vertices[6].Y, vertices[7].Y)))),
                  (Math.Min(Math.Min(Math.Min(vertices[0].Z, vertices[1].Z), Math.Min(vertices[2].Z, vertices[3].Z)), Math.Min(Math.Min(vertices[4].Z, vertices[5].Z), Math.Min(vertices[6].Z, vertices[7].Z)))));
        }

        public override Vector3 GetNormalToIntersectedPosition(Vector3 position)
        {
            // Gather six planes, find the closest plane to incoming position
            // As we know plane equation Ax + By + Cz + D = 0
            // we must substitute in place of xyz incoming position and check when D is near a zero value

            Vector3 localSpaceExtent = GetLocalSpaceExtent();
            Vector3 localSpacePosition = GetLocalSpaceOrigin();
            Vector3 tangentX = GetTangetX();
            Vector3 tangentY = GetTangetY();
            Vector3 tangentZ = GetTangetZ();

            // find edge vertices for each plane of bounding box
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(localSpacePosition.X + localSpaceExtent.X, localSpacePosition.Y, localSpacePosition.Z),
                new Vector3(localSpacePosition.X - localSpaceExtent.X, localSpacePosition.Y, localSpacePosition.Z),
                new Vector3(localSpacePosition.X, localSpacePosition.Y + localSpaceExtent.Y, localSpacePosition.Z),
                new Vector3(localSpacePosition.X, localSpacePosition.Y - localSpaceExtent.Y, localSpacePosition.Z),
                new Vector3(localSpacePosition.X, localSpacePosition.Y, localSpacePosition.Z + localSpaceExtent.Z),
                new Vector3(localSpacePosition.X, localSpacePosition.Y, localSpacePosition.Z - localSpaceExtent.Z)
            };

            for (Int32 i = 0; i < vertices.Length; i++)
                vertices[i] = Vector3.TransformPosition(vertices[i], TransformationMatrix);

            FPlane[] boundPlanes = new FPlane[6]
            {
                new FPlane(vertices[1], -tangentX), new FPlane(vertices[3], -tangentY), new FPlane(vertices[5], -tangentZ),
                new FPlane(vertices[0], tangentX), new FPlane(vertices[2], tangentY), new FPlane(vertices[4], tangentZ)
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

        public OBB(Vector3 Origin, Vector3 Extent, Matrix4 TransformationMatrix, Component parentComponent) : base(Origin, Extent, parentComponent)
        {
            this.TransformationMatrix = TransformationMatrix;
        }
    }
}
