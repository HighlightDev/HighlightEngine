using OpenTK;
using PhysicsBox;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using static MassiveGame.Physics.CollisionHeadUnit;

namespace MassiveGame.Physics
{
    public class CollisionBehaviorManager
    {
        private bool RAYCAST_COLLIDED(float raycastResult)
        {
            if (raycastResult > -0.5f)
                return true;
            return false;
        }

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
            Component characterRootComponent = collisionOutputNoCollided.GetCharacterRootComponent();

            // If character is in free fall - find next his position
            if ((characterRootComponent as Player).ActorState == BEHAVIOR_STATE.FREE_FALLING)
            {
                FRay ray = new FRay(characterRootComponent.ChildrenComponents.First().Bound.GetOrigin(), (characterRootComponent as Player).Velocity);
                float intersectionDistance = TerrainRayIntersaction.Intersection_TerrainRay(DOUEngine.terrain, ray);

                // Character could be elevated on terrain 
                if (intersectionDistance <= (characterRootComponent as Player).Speed)
                {
                    Vector3 CharacterNewPosition = ray.GetPositionInTime(intersectionDistance);
                    (characterRootComponent as Player).collisionOffset(CharacterNewPosition);
                    (characterRootComponent as Player).ActorState = BEHAVIOR_STATE.IDLE;
                    (characterRootComponent as Player).pushPositionStack();
                }
                // Character is still in free fall
                else
                {
                    // Update position during free fall
                    (characterRootComponent as Player).Move = BodyMechanics.UpdateFreeFallPosition((characterRootComponent as Player).Move,
                        (characterRootComponent as Player).Speed, (characterRootComponent as Player).Velocity);

                    (characterRootComponent as Player).pushPositionStack();
                }
            }
            else
            {
                // Character doesn't have actual collision - must to check if character is in air (free fall)
                FRay rayDown = new FRay(characterRootComponent.ChildrenComponents.First().Bound.GetOrigin(), -DOUEngine.Camera.getUpVector());
                float intersectionDistance = TerrainRayIntersaction.Intersection_TerrainRay(DOUEngine.terrain, rayDown);

                // Character could be elevated on terrain 
                if (intersectionDistance <= (characterRootComponent as Player).Speed)
                {
                    Vector3 CharacterNewPosition = rayDown.GetPositionInTime(intersectionDistance);
                    (characterRootComponent as Player).collisionOffset(CharacterNewPosition);
                    (characterRootComponent as Player).ActorState = BEHAVIOR_STATE.IDLE;
                    (characterRootComponent as Player).pushPositionStack();
                }
                // Character is in free fall
                else
                {
                    (characterRootComponent as Player).ActorState = BEHAVIOR_STATE.FREE_FALLING;
                    (characterRootComponent as Player).pushPositionStack();
                }
            }
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
                    // If character collided another character during free fall because of forward velocity
                    if (player.ActorState == BEHAVIOR_STATE.FREE_FALLING)
                    {
                        // Restore previous position and set velocity to fall
                        player.popPositionStack();
                        player.Velocity = BodyMechanics.G;
                    }
                    // If character collided character during move
                    else
                    {
                        player.popPositionStack();
                        (characterRootComponent as Player).ActorState = BEHAVIOR_STATE.IDLE;
                    }
                }
            }
            else // Character can try to do elevation on static mesh
            {
                // NEED TO RESOLVE ALL CASES OF RAY CAST

                // If character is in free falling but has found some collisions
                if ((characterRootComponent as Player).ActorState == BEHAVIOR_STATE.FREE_FALLING)
                {
                    // GO TO RAY CAST DOWN
                    // INTO COLLIDED OBJECTS
                    // AND TERRAIN IF NO COLLISION WITH OBJECTS!
                }
                else
                {
                    BoundBase characterBound = characterRootComponent.ChildrenComponents.First().Bound;
                    Vector3 characterDirection = (characterRootComponent as Player).Velocity;

                    // 1) First of all character has to do ray cast in move direction from center position, edge left position and edge right position before making move

                    Vector3 rayCastEdge1, rayCastEdge2, rayCastCenter;
                    GetRayCastPoints(characterBound, characterDirection, characterRootComponent, out rayCastEdge1, out rayCastEdge2);
                    rayCastCenter = (rayCastEdge1 + rayCastEdge2) / 2;
                    rayCastCenter.Y = rayCastEdge1.Y;

                    FRay rayFromEdge1 = new FRay(rayCastEdge1, characterDirection);
                    FRay rayFromEdge2 = new FRay(rayCastEdge2, characterDirection);
                    FRay rayFromCenter = new FRay(rayCastCenter, characterDirection);

                    float closestDistance = -1.0f;
                    Vector3 closestPosition = rayCastCenter;

                    // DO RAY CAST IN ALL COLLIDED BOUNDING BOXES
                    for (Int32 i = 0; i < collidedBounds.Count; i++)
                    {
                        BoundBase.BoundType boundType = collidedBounds[i].GetBoundType();
                        float intersectionDistance1 = 0.0f, intersectionDistance2 = 0.0f, intersectionDistance3 = 0.0f;
                        if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                        {
                            intersectionDistance1 = GeometricMath.Intersection_RayAABB(rayFromEdge1, collidedBounds[i] as AABB);
                            intersectionDistance2 = GeometricMath.Intersection_RayAABB(rayFromEdge2, collidedBounds[i] as AABB);
                            intersectionDistance3 = GeometricMath.Intersection_RayAABB(rayFromCenter, collidedBounds[i] as AABB);
                        }
                        else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                        {
                            intersectionDistance1 = GeometricMath.Intersection_RayOBB(rayFromEdge1, collidedBounds[i] as OBB);
                            intersectionDistance2 = GeometricMath.Intersection_RayOBB(rayFromEdge2, collidedBounds[i] as OBB);
                            intersectionDistance3 = GeometricMath.Intersection_RayOBB(rayFromCenter, collidedBounds[i] as OBB);
                        }

                        if (closestDistance <= -1.0f)
                        {
                            closestDistance = intersectionDistance1;
                        }

                        if (intersectionDistance1 > 0.0f && intersectionDistance1 < closestDistance)
                        {
                            closestDistance = intersectionDistance1;
                            closestPosition = rayFromEdge1.StartPosition;
                        }
                        if (intersectionDistance2 > 0.0f && intersectionDistance2 < closestDistance)
                        {
                            closestDistance = intersectionDistance2;
                            closestPosition = rayFromEdge2.StartPosition;
                        }
                        if (intersectionDistance3 > 0.0f && intersectionDistance3 < closestDistance)
                        {
                            closestDistance = intersectionDistance3;
                            closestPosition = rayFromCenter.StartPosition;
                        }
                    }

                    // Ray had collision
                    if (RAYCAST_COLLIDED(closestDistance))
                    {
                        // Intersection point is too far from character
                        if (closestDistance > (characterRootComponent as Player).Speed)
                        {
                            // Find height of point where character will be in 'Speed' times
                            Vector3 rayFromNewPosition = closestPosition + characterDirection * (characterRootComponent as Player).Speed;
                            FRay rayDown = new FRay(closestPosition, -DOUEngine.Camera.getUpVector());
                            closestDistance = -1.0f;

                            // DO RAY CAST DOWN IN ALL COLLIDED BOUNDING BOXES
                            for (Int32 i = 0; i < collidedBounds.Count; i++)
                            {
                                BoundBase.BoundType boundType = collidedBounds[i].GetBoundType();
                                float intersectionDistance = -1f;
                                if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                                    intersectionDistance = GeometricMath.Intersection_RayAABB(rayDown, collidedBounds[i] as AABB);
                                else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                                    intersectionDistance = GeometricMath.Intersection_RayOBB(rayFromEdge1, collidedBounds[i] as OBB);

                                if (closestDistance <= -1.0f)
                                {
                                    closestDistance = intersectionDistance;
                                }

                                closestDistance = intersectionDistance > 0.0f && intersectionDistance < closestDistance ? intersectionDistance : closestDistance;
                            }

                            // Character is in free fall
                            if (closestDistance > (characterRootComponent as Player).Speed)
                            {
                                (characterRootComponent as Player).pushPositionStack();
                                (characterRootComponent as Player).ActorState = BEHAVIOR_STATE.FREE_FALLING;
                            }
                            // Character could be elevated on collided mesh
                            else
                            {
                                Vector3 CharacterNewPosition = rayDown.GetPositionInTime(closestDistance);
                                (characterRootComponent as Player).collisionOffset(CharacterNewPosition);
                                (characterRootComponent as Player).pushPositionStack();
                                (characterRootComponent as Player).ActorState = BEHAVIOR_STATE.IDLE;
                            }
                        }
                        // Intersection point is close enough to get there, so character has to get angle of that plane
                        else
                        {

                        }
                    }
                    /*  There were no intersection with ray, this could be one of these reasons :
                     *   Probably case : 
                     *   1) Character is in air;
                     *   2) Character did ray cast higher than object is
                     *   3) Character did ray cast lower than object is */
                    else
                    {


                    }
                }
            }
        }

        private void GetRayCastPoints(BoundBase characterBound, Vector3 characterDirection, Component characterRootComponent, out Vector3 edge1, out Vector3 edge2)
        {
            Vector3 characterBoundOrigin = characterBound.GetOrigin();

            // Find positions for ray casts
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();
            // This is SHIT!
            Vector3 velocityToPreviousPosition = characterDirection * (-(characterRootComponent as Player).Speed);
            boundMax += velocityToPreviousPosition;
            boundMin += velocityToPreviousPosition;

            Vector3 testPoint1 = new Vector3(boundMin.X, characterBoundOrigin.Y, boundMin.Z);
            Vector3 testPoint2 = new Vector3(boundMax.X, characterBoundOrigin.Y, boundMax.Z);
            Vector3 testPoint3 = new Vector3(boundMax.X, characterBoundOrigin.Y, boundMin.Z);
            Vector3 testPoint4 = new Vector3(boundMin.X, characterBoundOrigin.Y, boundMax.Z);

            Dictionary<Vector3, float> points = new Dictionary<Vector3, float>();
            points.Add(testPoint1, GeometricMath.ProjectVectorOnNormalizedVector(testPoint1, characterDirection));
            points.Add(testPoint2, GeometricMath.ProjectVectorOnNormalizedVector(testPoint2, characterDirection));
            points.Add(testPoint3, GeometricMath.ProjectVectorOnNormalizedVector(testPoint3, characterDirection));
            points.Add(testPoint4, GeometricMath.ProjectVectorOnNormalizedVector(testPoint4, characterDirection));

            points = (Dictionary<Vector3, float>)from entry in points orderby entry.Value ascending select entry;
            edge1 = points.Keys.ToList()[0];
            edge2 = points.Keys.ToList()[1];
        }
    }
}
