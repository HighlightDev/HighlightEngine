using OpenTK;
using PhysicsBox;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MassiveGame.Physics.CollisionHeadUnit;

namespace MassiveGame.Physics
{
    public class CollisionBehaviorManager
    {
        public CollisionBehaviorManager()
        {
        }

        public void ProcessCollision(List<CollisionOutputData> data)
        {
            foreach (var data_item in data)
            {
                if (data_item.GetDataType() == CollisionOutputData.OutDataType.NoCollided || data.Count == 1)
                    Process_NoCollision(data_item as CollisionOutputNoCollided);

                if (data_item.GetDataType() == CollisionOutputData.OutDataType.FramingAABB)
                    Process_FramingAABB(data_item as CollisionOutputFramingAABB);

                else if (data_item.GetDataType() == CollisionOutputData.OutDataType.RegularOBB)
                    Process_RegularOBB(data_item as CollisionOutputRegularOBB);

            }
        }

        private void Process_NoCollision(CollisionOutputNoCollided collisionOutputNoCollided)
        {
            // If we didn't have actual collision - we have to check if character is in air (free fall)

        }

        private void Process_FramingAABB(CollisionOutputFramingAABB data)
        {
            Component characterRootComponent = data.GetCharacterRootComponent();
            Component collidedRootComponent = data.GetCollidedRootComponent();
        }

        private void Process_RegularOBB(CollisionOutputRegularOBB data)
        {
            Component characterRootComponent = data.GetCharacterRootComponent();
            Component collidedRootComponent = data.GetCollidedRootComponent();
            List<BoundBase> collidedBounds = data.GetCollidedBoundingBoxes();

            if (collidedRootComponent.GetType() == typeof(Player)) // Find better way for checking that, this was a fast solution
            {
                if (characterRootComponent as Player != null) // this too
                {
                    Player player = characterRootComponent as Player;
                    player.popPositionStack();
                }
            }
            else
            {
                // NEED TO RESOLVE ALL CASES OF RAY CAST

                // We can try to do elevation on static mesh
                BoundBase characterBound = characterRootComponent.ChildrenComponents.First().Bound;

                Vector3 characterDirection = new Vector3(0);
                FRay ray = new FRay(characterBound.GetOrigin(), characterDirection);
                float closestDistance = -1.0f; // set back direction distance as the smallest

                for (Int32 i = 0; i < collidedBounds.Count; i++)
                {
                    BoundBase.BoundType boundType = collidedBounds[i].GetBoundType();
                    float intersectionDistance = 0.0f;
                    if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                        intersectionDistance = GeometricMath.Intersection_RayAABB(ray, collidedBounds[i] as AABB);
                    else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                        intersectionDistance = GeometricMath.Intersection_RayOBB(ray, collidedBounds[i] as OBB);

                    closestDistance = intersectionDistance > 0.0f && intersectionDistance < closestDistance ? intersectionDistance : closestDistance;
                }

                // Get current distance 
                //closestDistance -= Extent;




            }

        }
    }
}
