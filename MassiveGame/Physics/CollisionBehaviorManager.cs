using MassiveGame.API.Factory;
using MassiveGame.Physics.OutputData;
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

        #region core

        private bool RAYCAST_COLLIDED(float raycastResult)
        {
            if (raycastResult > -1f)
                return true;
            return false;
        }

        private EntityType GetEntityType(Component entityComponent)
        {
            if (entityComponent as MovableEntity != null)
                return EntityType.MOVABLE_ENTITY;
            else if (entityComponent as StaticEntity != null)
                return EntityType.STATIC_ENTITY;

            return EntityType.UNDEFINED;
        }

        private float GetClosestRayCastResult(FRay ray, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            float shortestDistance = -1.0f;

            // DO RAY CAST IN ALL COLLIDED BOUNDING BOXES
            for (Int32 i = 0; i < collidedBounds.Count; i++)
            {
                float localIntersectionDistance = 0.0f;
                BoundBase.BoundType boundType = collidedBounds[i].GetBoundType();
                if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                    localIntersectionDistance = GeometricMath.Intersection_RayAABB(ray, collidedBounds[i] as AABB);
                else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                    localIntersectionDistance = GeometricMath.Intersection_RayOBB(ray, collidedBounds[i] as OBB);

                if (shortestDistance <= -1.0f)
                    shortestDistance = localIntersectionDistance;

                if (localIntersectionDistance > 0.0f && localIntersectionDistance < shortestDistance)
                    shortestDistance = localIntersectionDistance;
            }
            return shortestDistance;
        }

        private void GetPreviosPositionsForRayCast(BoundBase characterBound, Vector3 characterDirection, MovableEntity characterEntity, out Vector3 edge1, out Vector3 edge2)
        {
            Vector3 characterBoundOrigin = characterBound.GetOrigin();

            // Find positions for ray casts
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();
            // This is SHIT!
            Vector3 velocityToPreviousPosition = characterDirection * (-characterEntity.Speed);
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

        private void ProcessNoCollisionAtState_FreeFalling(MovableEntity character)
        {
            Vector3 boundMin = character.GetCharacterCollisionBound().GetMin();

            // Raycast from bottom point
            Vector3 rayCastStartPosition = new Vector3(character.GetCharacterCollisionBound().GetOrigin());
            rayCastStartPosition.Y = boundMin.Y;

            FRay ray = new FRay(rayCastStartPosition, character.Velocity);
            float intersectionDistance = TerrainRayIntersaction.Intersection_TerrainRay(DOUEngine.terrain, ray);

            // Character could be elevated on terrain 
            if (intersectionDistance <= character.Speed)
            {
                Vector3 CharacterNewPosition = ray.GetPositionInTime(intersectionDistance);
                character.collisionOffset(CharacterNewPosition);
                character.ActorState = BEHAVIOR_STATE.IDLE;
            }
            // Character is still in free fall, just uppdate position
            else
                character.Move = BodyMechanics.UpdateFreeFallPosition(character.Move, character.Speed, character.Velocity);

            character.pushPositionStack();
        }

        private void ProcessNoCollisionAtState_Move(MovableEntity character)
        {
            Vector3 boundMax = character.GetCharacterCollisionBound().GetMax();
            Vector3 boundMin = character.GetCharacterCollisionBound().GetMin();

            // Ray cast from top point to avoid miss ray casting
            Vector3 rayCastStartPosition = new Vector3(character.GetCharacterCollisionBound().GetOrigin());
            rayCastStartPosition.Y = boundMax.Y;

            FRay rayDown = new FRay(rayCastStartPosition, -DOUEngine.Camera.getUpVector());
            float intersectionDistance = TerrainRayIntersaction.Intersection_TerrainRay(DOUEngine.terrain, rayDown);

            float boundExtent = boundMax.Y - boundMin.Y;
            float actualIntersectionDistance = intersectionDistance - boundExtent;

            // Check if character can reach that height
            if (actualIntersectionDistance <= character.Speed)
            {
                // Character could be elevated on terrain 
                Vector3 CharacterNewPosition = rayDown.GetPositionInTime(actualIntersectionDistance);
                character.collisionOffset(CharacterNewPosition);
                character.ActorState = BEHAVIOR_STATE.IDLE;
            }
            // Character is in free fall, next position will be calculated in next tick
            else
                character.ActorState = BEHAVIOR_STATE.FREE_FALLING;

            // Push current position to stack
            character.pushPositionStack();
        }

        private void ProcessCollisionAtState_Move(MovableEntity characterEntity, Entity collidedEntity, List<BoundBase> collidedBounds)
        {
            switch (GetEntityType(collidedEntity))
            {
                // If character collided character during move
                case EntityType.MOVABLE_ENTITY:
                    {
                        characterEntity.popPositionStack();
                        characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                        break;
                    }

                case EntityType.STATIC_ENTITY:
                    {
                        BoundBase characterBound = characterEntity.GetCharacterCollisionBound();
                        Vector3 characterDirection = characterEntity.Velocity;

                        // 1) First of all character has to do ray cast in move direction from edge middle position, edge left position and edge right position before making move

                        Vector3 rayCastEdge1, rayCastEdge2, rayCastCenter;
                        GetPreviosPositionsForRayCast(characterBound, characterDirection, characterEntity, out rayCastEdge1, out rayCastEdge2);
                        rayCastCenter = (rayCastEdge1 + rayCastEdge2) / 2; // interpolate between two edge points to get middle
                        rayCastCenter.Y = rayCastEdge1.Y;

                        FRay rayFromEdge1 = new FRay(rayCastEdge1, characterDirection);
                        FRay rayFromEdge2 = new FRay(rayCastEdge2, characterDirection);
                        FRay rayFromCenter = new FRay(rayCastCenter, characterDirection);

                        float closestDistance = -1.0f;
                        Vector3 previousClosestPosition = new Vector3(0);

                        float intersectionDistanceFromEdge1 = GetClosestRayCastResult(rayFromEdge1, collidedBounds, characterEntity);
                        float intersectionDistanceFromEdge2 = GetClosestRayCastResult(rayFromEdge2, collidedBounds, characterEntity);
                        float intersectionDistanceFromCenter = GetClosestRayCastResult(rayFromCenter, collidedBounds, characterEntity);

                        // Get closest position to collided bound
                        if (intersectionDistanceFromEdge1 > intersectionDistanceFromEdge2 && intersectionDistanceFromEdge1 > intersectionDistanceFromCenter)
                        {
                            closestDistance = intersectionDistanceFromEdge1;
                            previousClosestPosition = rayFromEdge1.StartPosition;
                        }
                        if (intersectionDistanceFromEdge2 > intersectionDistanceFromEdge1 && intersectionDistanceFromEdge2 > intersectionDistanceFromCenter)
                        {
                            closestDistance = intersectionDistanceFromEdge2;
                            previousClosestPosition = rayFromEdge2.StartPosition;
                        }
                        if (intersectionDistanceFromCenter > intersectionDistanceFromEdge1 && intersectionDistanceFromCenter > intersectionDistanceFromEdge2)
                        {
                            closestDistance = intersectionDistanceFromCenter;
                            previousClosestPosition = rayFromCenter.StartPosition;
                        }

                        // Ray intersected with one of collided bounds
                        if (RAYCAST_COLLIDED(closestDistance))
                        {
                            // Distance to collided bound is too large and character can't step so far
                            // In this case do ray cast down from current character position
                            if (closestDistance > characterEntity.Speed)
                            {
                                // Do ray cast from upper current position
                                Vector3 CurrentClosestPosition = previousClosestPosition + characterEntity.Velocity * characterEntity.Speed; 
                                FRay rayDown = new FRay(CurrentClosestPosition, -DOUEngine.Camera.getUpVector());
                                closestDistance = GetClosestRayCastResult(rayDown, collidedBounds, characterEntity);

                                float boundExtent = characterEntity.GetCharacterCollisionBound().GetMax().Y - characterEntity.GetCharacterCollisionBound().GetMin().Y;
                                float actualIntersectionDistance = closestDistance - boundExtent;

                                // Character could be elevated on collided mesh
                                if (actualIntersectionDistance <= characterEntity.Speed)
                                {
                                    Vector3 CharacterNewPosition = rayDown.GetPositionInTime(closestDistance);
                                    characterEntity.collisionOffset(CharacterNewPosition);
                                    characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                                }
                                // Character now goes to free fall
                                else
                                    characterEntity.ActorState = BEHAVIOR_STATE.FREE_FALLING;

                                characterEntity.pushPositionStack();
                            }
                            // Distance to collided bound is small enough to step there
                            // In this case acquire angle of that plane to find out can character step on that surface, if no - pop previous position and set to idle
                            else
                            {

                            }
                        }
                        /*  There was no intersection with ray, this could be one of these reasons :
                         *   Probably case : 
                         *   1) Character is in air;
                         *   2) Character did ray cast higher than object is
                         *   3) Character did ray cast lower than object is */
                        else
                        {


                        }
                        break;
                    }
            }
        }

        private void ProcessCollisionAtState_FreeFalling(MovableEntity character, Entity collidedEntity, List<BoundBase> collidedBounds)
        {
            switch (GetEntityType(collidedEntity))
            {
                // If character collided another character during free fall because of forward velocity
                case EntityType.MOVABLE_ENTITY:
                    {
                        // Restore previous position and set velocity to fall
                        character.popPositionStack();
                        character.Velocity = new Vector3(0, character.Velocity.Y, 0);
                        break;
                    }

                // If character is in free falling but has found some collisions
                case EntityType.STATIC_ENTITY:
                    {
                        // GO TO RAY CAST Velocity
                        // INTO COLLIDED OBJECTS
                        // AND TERRAIN IF NO COLLISION WITH OBJECTS!

                        // if there are collisions - and can't get on mesh - just change velocity vector to down;
                        break;
                    }
            }
        }

        #endregion

        private void Process_NoCollision(CollisionOutputNoCollided collisionOutputNoCollided)
        {
            MovableEntity character = collisionOutputNoCollided.GetCharacterRootComponent() as MovableEntity;

            switch (character.ActorState)
            {
                // Character is in free fall and has no collision with bounds
                // need to find his next position
                case BEHAVIOR_STATE.FREE_FALLING: ProcessNoCollisionAtState_FreeFalling(character); break;
                // Character is moving and has no collision with bound
                // he may be colliding with terrain, if yes - elevate him on it, else - set free falling state
                case BEHAVIOR_STATE.MOVE: ProcessNoCollisionAtState_Move(character); break;
            }
        }

        private void Process_FramingAABB(CollisionOutputFramingAABB data)
        {
            Component characterRootComponent = data.GetCharacterRootComponent();
            Component collidedRootComponent = data.GetCollidedRootComponent();
        }

        private void Process_RegularOBB(CollisionOutputRegularOBB data)
        {
            MovableEntity character = data.GetCharacterRootComponent() as MovableEntity;
            Entity collidedEntity = data.GetCharacterRootComponent() as Entity;
            List<BoundBase> collidedBounds = data.GetCollidedBoundingBoxes();

            switch (character.ActorState)
            {
                case BEHAVIOR_STATE.FREE_FALLING: ProcessCollisionAtState_FreeFalling(character, collidedEntity, collidedBounds); break;
                case BEHAVIOR_STATE.MOVE: ProcessCollisionAtState_Move(character, collidedEntity, collidedBounds); break;
            }
        }
    }
}
