using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.MathCore.MathTypes;
using OpenTK;
using System;

namespace MassiveGame.CollisionEditor.Core
{
    public static class CollisionComponentBoundBuilder
    {
        private static BoundBase UpdateCollisionBound(ref SceneComponent component, float[,] vertices)
        {
            BoundBase resultBound = null;

            AABB aabb = AABB.CreateFromMinMax(
                FindEdgeInMesh((lhv, rhv) => { return Math.Min(lhv, rhv); }, vertices),
                FindEdgeInMesh((lhv, rhv) => { return Math.Max(lhv, rhv); }, vertices),
                component);
            Matrix4 TransformMatrix = component.GetWorldMatrix();
            Quaternion rotation = TransformMatrix.ExtractRotation();
            if (rotation.Xyz.LengthSquared > 0.01f)
                resultBound = new OBB(aabb.GetLocalSpaceOrigin(), aabb.GetLocalSpaceExtent(), TransformMatrix, component);
            else
            {
                aabb.ScalePlusTranslation = TransformMatrix;
                resultBound = aabb;
            }

            return resultBound;
        }

        private static Vector3 FindEdgeInMesh(Func<float, float, float> func, float[,] vertices)
        {
            float edge1 = vertices[0, 0], edge2 = vertices[0, 1], edge3 = vertices[0, 2];

            var iterationCount = vertices.Length / 3;

            for (Int32 i = 0; i < iterationCount; i++)
            {
                edge1 = func(edge1, vertices[i, 0]);
                edge2 = func(edge2, vertices[i, 1]);
                edge3 = func(edge3, vertices[i, 2]);
            }

            return new Vector3(edge1, edge2, edge3);
        }

        public static void UpdateCollisionBoundHierarchy(Component component, float[,] vertices)
        {
            if (component.ChildrenComponents != null)
            {
                foreach (var child in component.ChildrenComponents)
                {
                    UpdateCollisionBoundHierarchy(child, vertices);
                    SceneComponent childComponent = child as SceneComponent;
                    if (childComponent != null)
                        childComponent.Bound = UpdateCollisionBound(ref childComponent, vertices);
                }
            }
        }
    }
}
