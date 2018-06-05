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
            return Vector3.TransformVector(Extent, Matrix4.CreateScale(TransformationMatrix.ExtractScale()));
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
        
        private Vector3[] GetVerticesOfOBB()
        {
            Vector3[] resultVertices = null;
            // extract tangent vectors of bounding box
            Vector3 obbTangentX = GetTangetX();
            Vector3 obbTangentY = GetTangetY();
            Vector3 obbTangentZ = GetTangetZ();

            // extent and origin of bounding box
            Vector3 extent = GetExtent();
            Vector3 position = GetOrigin();

            // find all vertices of rotated bounding box
            resultVertices = new Vector3[8];
            resultVertices[0] = position + (obbTangentX * extent.X) + (obbTangentY * extent.Y) + (obbTangentZ * extent.Z);
            resultVertices[1] = position - (obbTangentX * extent.X) + (obbTangentY * extent.Y) + (obbTangentZ * extent.Z);
            resultVertices[2] = position + (obbTangentX * extent.X) - (obbTangentY * extent.Y) + (obbTangentZ * extent.Z);
            resultVertices[3] = position + (obbTangentX * extent.X) + (obbTangentY * extent.Y) - (obbTangentZ * extent.Z);
            resultVertices[4] = position - (obbTangentX * extent.X) - (obbTangentY * extent.Y) - (obbTangentZ * extent.Z);
            resultVertices[5] = position + (obbTangentX * extent.X) - (obbTangentY * extent.Y) - (obbTangentZ * extent.Z);
            resultVertices[6] = position - (obbTangentX * extent.X) + (obbTangentY * extent.Y) - (obbTangentZ * extent.Z);
            resultVertices[7] = position - (obbTangentX * extent.X) - (obbTangentY * extent.Y) + (obbTangentZ * extent.Z);

            return resultVertices;
        }

        public override Vector3 GetMax()
        {
            var vertices = GetVerticesOfOBB();
            return new Vector3
                  (Math.Max(Math.Max(Math.Max(vertices[0].X, vertices[1].X), Math.Max(vertices[2].X, vertices[3].X)), Math.Max(Math.Max(vertices[4].X, vertices[5].X), Math.Max(vertices[6].X, vertices[7].X)))
                , (Math.Max(Math.Max(Math.Max(vertices[0].Y, vertices[1].Y), Math.Max(vertices[2].Y, vertices[3].Y)), Math.Max(Math.Max(vertices[4].Y, vertices[5].Y), Math.Max(vertices[6].Y, vertices[7].Y)))),
                  (Math.Max(Math.Max(Math.Max(vertices[0].Z, vertices[1].Z), Math.Max(vertices[2].Z, vertices[3].Z)), Math.Max(Math.Max(vertices[4].Z, vertices[5].Z), Math.Max(vertices[6].Z, vertices[7].Z)))));
        }

        public override Vector3 GetMin()
        {
            var vertices = GetVerticesOfOBB();
            return new Vector3
                  (Math.Min(Math.Min(Math.Min(vertices[0].X, vertices[1].X), Math.Min(vertices[2].X, vertices[3].X)), Math.Min(Math.Min(vertices[4].X, vertices[5].X), Math.Min(vertices[6].X, vertices[7].X)))
                , (Math.Min(Math.Min(Math.Min(vertices[0].Y, vertices[1].Y), Math.Min(vertices[2].Y, vertices[3].Y)), Math.Min(Math.Min(vertices[4].Y, vertices[5].Y), Math.Min(vertices[6].Y, vertices[7].Y)))),
                  (Math.Min(Math.Min(Math.Min(vertices[0].Z, vertices[1].Z), Math.Min(vertices[2].Z, vertices[3].Z)), Math.Min(Math.Min(vertices[4].Z, vertices[5].Z), Math.Min(vertices[6].Z, vertices[7].Z)))));
        }

        public OBB(Vector3 Origin, Vector3 Extent, Matrix4 TransformationMatrix, Component parentComponent) : base(Origin, Extent, parentComponent)
        {
            this.TransformationMatrix = TransformationMatrix;
        }
    }
}
