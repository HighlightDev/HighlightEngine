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
        private class RayCastOutputData
        {
            public BoundBase collidedBound { set; get; }
            public float shortestDistance { set; get; }
            public Vector3 intersectionPosition { set; get; }
            
            public RayCastOutputData(BoundBase boundBase, float shortestDistance, Vector3 intersectionPosition)
            {
                collidedBound = boundBase;
                this.shortestDistance = shortestDistance;
                this.intersectionPosition = intersectionPosition;
            }
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

        #region core

        private bool RAYCAST_COLLIDED(float raycastResult)
        {
            if (raycastResult > -1f)
                return true;
            return false;
        }

        private bool RAYCAST_INTERSECTION_FAR(float characterSpeed, float intersectionDistance)
        {
            if (intersectionDistance > characterSpeed)
                return true;

            return false;
        }

        private bool REACHABLE_INCLINE(Vector3 normal)
        {
            float cosIncline = normal.Y;
            if (cosIncline <= BodyMechanics.COS_MAX_REACHABLE_INCLINE)
                return false;
            return true;
        }

        private EntityType GetEntityType(Component entityComponent)
        {
            if (entityComponent as MovableEntity != null)
                return EntityType.MOVABLE_ENTITY;
            else if (entityComponent as StaticEntity != null)
                return EntityType.STATIC_ENTITY;

            return EntityType.UNDEFINED;
        }

        private RayCastOutputData GetClosestRayCastResult(FRay ray, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            float resultShortestDistance = -1.0f;
            BoundBase resultBound = null;

            // DO RAY CAST IN ALL COLLIDED BOUNDING BOXES
            for (Int32 i = 0; i < collidedBounds.Count; i++)
            {
                float localIntersectionDistance = 0.0f;
                BoundBase.BoundType boundType = collidedBounds[i].GetBoundType();
                if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                    localIntersectionDistance = GeometricMath.Intersection_RayAABB(ray, collidedBounds[i] as AABB);
                else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                    localIntersectionDistance = GeometricMath.Intersection_RayOBB(ray, collidedBounds[i] as OBB);

                if (resultShortestDistance <= -1.0f || (localIntersectionDistance > 0.0f && localIntersectionDistance < resultShortestDistance))
                {
                    resultShortestDistance = localIntersectionDistance;
                    resultBound = collidedBounds[i];
                }
            }
           
            Vector3 intersectionPosition = ray.GetPositionInTime(resultShortestDistance);

            return new RayCastOutputData(resultBound, resultShortestDistance, intersectionPosition);
        }

        private void GetEgePositionsForRayCast(ref Vector3 boundMax, ref Vector3 boundMin, float Yheight, MovableEntity characterEntity, out Vector3 edge1, out Vector3 edge2)
        {
            Vector3 testPoint1 = new Vector3(boundMin.X, Yheight, boundMin.Z);
            Vector3 testPoint2 = new Vector3(boundMax.X, Yheight, boundMax.Z);
            Vector3 testPoint3 = new Vector3(boundMax.X, Yheight, boundMin.Z);
            Vector3 testPoint4 = new Vector3(boundMin.X, Yheight, boundMax.Z);

            Dictionary<Vector3, float> points = new Dictionary<Vector3, float>();
            points.Add(testPoint1, GeometricMath.ProjectVectorOnNormalizedVector(testPoint1, characterEntity.Velocity));
            points.Add(testPoint2, GeometricMath.ProjectVectorOnNormalizedVector(testPoint2, characterEntity.Velocity));
            points.Add(testPoint3, GeometricMath.ProjectVectorOnNormalizedVector(testPoint3, characterEntity.Velocity));
            points.Add(testPoint4, GeometricMath.ProjectVectorOnNormalizedVector(testPoint4, characterEntity.Velocity));

            var listOfKeyPair = (from entry in points orderby entry.Value ascending select entry).ToList();
            edge1 = listOfKeyPair[0].Key;
            edge2 = listOfKeyPair[1].Key;
        }

        private Vector3 GetPreviosClosestPositionForRayCast(BoundBase characterBound, float timeInterval, MovableEntity characterEntity)
        {
            Vector3 characterOrigin = characterBound.GetOrigin();
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();

            Vector3 velocityToPreviousPosition = characterEntity.Velocity * (-timeInterval);
            boundMax += velocityToPreviousPosition;
            boundMin += velocityToPreviousPosition;
            characterOrigin += velocityToPreviousPosition;


            FRay rayToBoundsEdge = new FRay(characterOrigin, characterEntity.Velocity);

            float localIntersectionDistance = 0.0f;
            BoundBase.BoundType boundType = characterBound.GetBoundType();
            if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                localIntersectionDistance = GeometricMath.Intersection_RayAABBExt(rayToBoundsEdge, boundMax, boundMin);
            else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                localIntersectionDistance = GeometricMath.Intersection_RayOBBExt(rayToBoundsEdge, (characterBound as OBB).GetTangetX(),  (characterBound as OBB).GetTangetY(),
                    (characterBound as OBB).GetTangetZ(), characterOrigin, (characterBound as OBB).GetExtent());

            return rayToBoundsEdge.GetPositionInTime(localIntersectionDistance);
        }

        private Vector3 GetPreviousBottomPositionsForRayCast(BoundBase characterBound, float timeInterval, MovableEntity characterEntity)
        {
            Vector3 characterOrigin = characterBound.GetOrigin();
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();

            Vector3 velocityToPreviousPosition = characterEntity.Velocity * (-timeInterval);
            boundMax += velocityToPreviousPosition;
            boundMin += velocityToPreviousPosition;
            characterOrigin += velocityToPreviousPosition;


            FRay rayToBoundsEdge = new FRay(characterOrigin, characterEntity.Velocity);

            float localIntersectionDistance = 0.0f;
            BoundBase.BoundType boundType = characterBound.GetBoundType();
            if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                localIntersectionDistance = GeometricMath.Intersection_RayAABBExt(rayToBoundsEdge, boundMax, boundMin);
            else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                localIntersectionDistance = GeometricMath.Intersection_RayOBBExt(rayToBoundsEdge, (characterBound as OBB).GetTangetX(), (characterBound as OBB).GetTangetY(),
                    (characterBound as OBB).GetTangetZ(), characterOrigin, (characterBound as OBB).GetExtent());

            Vector3 intersectionPosition = rayToBoundsEdge.GetPositionInTime(localIntersectionDistance);

            return new Vector3(intersectionPosition.X, boundMin.Y, intersectionPosition.Z);
        }

        private void ProcessNoCollisionAtState_FreeFalling(MovableEntity character)
        {
            if (character.Velocity.LengthSquared > 0)
            {
                Vector3 boundMin = character.GetCharacterCollisionBound().GetMin();
                // Raycast from bottom height point
                Vector3 rayCastStartPosition = new Vector3(character.GetCharacterCollisionBound().GetOrigin());
                rayCastStartPosition.Y = boundMin.Y;

                FRay ray = new FRay(rayCastStartPosition, character.Velocity);
                float intersectionDistance = TerrainRayIntersection.Intersection_TerrainRay(DOUEngine.terrain, ray);

                // Character is still in free fall, just update position
                if (intersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), intersectionDistance))
                    character.ComponentTranslation = BodyMechanics.UpdateFreeFallPosition(character.ComponentTranslation, character.Velocity);

                // Character could be elevated on terrain 
                else
                {
                    Vector3 CharacterNewPosition = ray.GetPositionInTime(intersectionDistance);
                    CharacterNewPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                    character.collisionOffset(CharacterNewPosition);
                    character.ActorState = BEHAVIOR_STATE.IDLE;
                }
            }

            character.pushPositionStack();
        }

        private void ProcessNoCollisionAtState_Move(MovableEntity character)
        {
            Vector3 boundMin = character.GetCharacterCollisionBound().GetMin();
            Vector3 origin = character.GetCharacterCollisionBound().GetOrigin();

            // Ray cast from middle height position to avoid miss ray casting
            Vector3 rayCastStartPosition = new Vector3(origin);

            FRay rayDown = new FRay(rayCastStartPosition, -DOUEngine.Camera.getUpVector());
            float intersectionDistance = TerrainRayIntersection.Intersection_TerrainRay(DOUEngine.terrain, rayDown);

            // Subtract length of bound extent from middle height position
            float boundExtent = origin.Y - boundMin.Y;
            float actualIntersectionDistance = intersectionDistance - boundExtent;

            // Character is in free fall, next position will be calculated in next tick
            if (intersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), actualIntersectionDistance))
                character.ActorState = BEHAVIOR_STATE.FREE_FALLING;

            // Check if character can reach that height
            else
            {  // Character could be elevated on terrain 
                Vector3 CharacterNewPosition = rayDown.GetPositionInTime(intersectionDistance);
                CharacterNewPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                character.collisionOffset(CharacterNewPosition);
                character.ActorState = BEHAVIOR_STATE.IDLE;
            }
            

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

                        // 1) First of all character has to do ray cast in move direction

                        Vector3 previousClosestPositionToVelocityDirection = GetPreviosClosestPositionForRayCast(characterBound, characterEntity.Speed, characterEntity);

                        FRay rayFromCenter = new FRay(previousClosestPositionToVelocityDirection, characterEntity.Velocity);

                        // Necessary data for subsequent calculations
                        float closestDistance = -1.0f;
                        Vector3 previousClosestPosition = new Vector3(0);
                        RayCastOutputData outputData = null;

                        var rayCastResult = GetClosestRayCastResult(rayFromCenter, collidedBounds, characterEntity);
                        closestDistance = rayCastResult.shortestDistance;
                        previousClosestPosition = rayFromCenter.StartPosition;
                        outputData = rayCastResult;

                        // Ray intersected with one of collided bounds
                        if (RAYCAST_COLLIDED(closestDistance))
                        {
                            // Distance to collided bound is too large and character can't step so far
                            // In this case do ray cast down from current character position
                            if (RAYCAST_INTERSECTION_FAR((characterEntity.Velocity * characterEntity.Speed).Length, closestDistance))
                            {
                                // Do ray cast down from middle height current position
                                Vector3 CurrentClosestPosition = previousClosestPositionToVelocityDirection + characterEntity.Velocity * characterEntity.Speed;

                                FRay rayDown = new FRay(CurrentClosestPosition, -DOUEngine.Camera.getUpVector());
                                RayCastOutputData rayDownOutput = GetClosestRayCastResult(rayDown, collidedBounds, characterEntity);
                                float closestDistanceBottom = rayDownOutput.shortestDistance;

                                // CHECK IF rayDown result HAS COLLISION!!!!!
                                if (RAYCAST_COLLIDED(closestDistanceBottom))
                                {
                                    // Character could be elevated on collided mesh
                                    if (closestDistanceBottom <= characterBound.GetExtent().Y)
                                    {
                                        float distanceToStep = (characterBound.GetExtent().Y) - closestDistanceBottom;
                                        Vector3 elevationPosition = CurrentClosestPosition;
                                        elevationPosition.Y += distanceToStep;
                                        characterEntity.collisionOffset(elevationPosition);
                                        characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                                    }
                                    // Character now goes to free fall
                                    else
                                        characterEntity.ActorState = BEHAVIOR_STATE.FREE_FALLING;

                                    characterEntity.pushPositionStack();
                                }
                                else
                                {

                                    characterEntity.popPositionStack();
                                    characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                                }
                            }
                            // Distance to collided bound is small enough to step there
                            // In this case acquire normal to that plane to find out can character step on that surface, if no - pop previous position and set to idle
                            else
                            {
                                BoundBase bound = outputData.collidedBound;
                                Vector3 normalToCollidedPlane = bound.GetNormalToIntersectedPosition(outputData.intersectionPosition);
                               
                                // Character can step on this surface
                                if (REACHABLE_INCLINE(normalToCollidedPlane))
                                {
                                    Vector3 NewCharacterPosition  = outputData.intersectionPosition + new Vector3(0, characterEntity.GetCharacterCollisionBound().GetExtent().Y, 0);
                                    characterEntity.collisionOffset(NewCharacterPosition);
                                    characterEntity.pushPositionStack();
                                }
                                // If normal is down directed or too up directed - character can't step on this surface, return to previous position
                                else
                                    characterEntity.popPositionStack();

                                characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                            }
                        }
                        /*  There was no intersection with ray, this could be one of these reasons :
                         *   Probably case : 
                         *   1) Character did ray cast higher than object is;
                         *   2) Character did ray cast lower than object is;
                         *   3) Character is on terrain;
                         *   4) Character is in free falling.
                         */
                        else
                        {
                            // Firstly test if collided object is higher than ray was
                            Vector3 boundMax = characterBound.GetMax();
                            FRay rayForwardFromUpperPoint = new FRay(new Vector3(previousClosestPosition.X, boundMax.Y, previousClosestPosition.Z), characterEntity.Velocity);
                            float closestDistanceUpper = GetClosestRayCastResult(rayForwardFromUpperPoint, collidedBounds, characterEntity).shortestDistance;
                            
                            // If static object collided with "head" of character - we can't stand there, restore previous position
                            if (RAYCAST_COLLIDED(closestDistanceUpper))
                            {
                                characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                                characterEntity.popPositionStack();
                            }
                            // No collision with "head" of character
                            else
                            {
                                // Do ray cast from current middle height position down
                                Vector3 localBoundOrigin = characterBound.GetOrigin();
                                Vector3 CurrentClosestPosition = previousClosestPosition + characterEntity.Velocity * characterEntity.Speed;
                                CurrentClosestPosition = new Vector3(CurrentClosestPosition.X, localBoundOrigin.Y, CurrentClosestPosition.Z);
                                FRay rayDown = new FRay(CurrentClosestPosition, -DOUEngine.Camera.getUpVector());
                                RayCastOutputData rayDownOutput = GetClosestRayCastResult(rayDown, collidedBounds, characterEntity);
                                float closestDistanceDown = rayDownOutput.shortestDistance;

                                // Character can step on it, so find out intersection position and elevate him on it
                                if (RAYCAST_COLLIDED(closestDistanceDown))
                                {
                                    Vector3 NewCharacterPosition = rayDownOutput.intersectionPosition + new Vector3(0, characterEntity.GetCharacterCollisionBound().GetExtent().Y, 0);
                                    characterEntity.collisionOffset(NewCharacterPosition);
                                    characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                                    
                                }
                                // No collision with bounds, so just make sure that character is standing on terrain or is in free falling
                                else 
                                {
                                    // Do ray cast  from current middle height position down
                                    Vector3 boundMin = characterBound.GetMin();
                                    rayDown = new FRay(CurrentClosestPosition, -DOUEngine.Camera.getUpVector());

                                    float intersectionDistance = TerrainRayIntersection.Intersection_TerrainRay(DOUEngine.terrain, rayDown);

                                    // Subtract length of bound extent from middle height position
                                    float boundExtent = localBoundOrigin.Y - boundMin.Y;
                                    float actualIntersectionDistance = intersectionDistance - boundExtent;

                                    // Character is in free fall, next position will be calculated in next tick
                                    if (intersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR((characterEntity.Velocity * BodyMechanics.FallTime).Length, actualIntersectionDistance))
                                        characterEntity.ActorState = BEHAVIOR_STATE.FREE_FALLING;

                                    // Check if character can reach that height
                                    else
                                    {  // Character could be elevated on terrain 
                                        Vector3 CharacterNewPosition = rayDown.GetPositionInTime(intersectionDistance);
                                        CharacterNewPosition.Y += characterEntity.GetCharacterCollisionBound().GetExtent().Y;
                                        characterEntity.collisionOffset(CharacterNewPosition);
                                        characterEntity.ActorState = BEHAVIOR_STATE.IDLE;
                                    }
                                }

                                // Doesn't matter what result obtained(elevated on static mesh; elevated on terrain; free falling) we push current position to stack
                                characterEntity.pushPositionStack();
                            }
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
                        character.Velocity = -DOUEngine.Camera.getUpVector();
                        break;
                    }

                // If character is in free falling but has encountered some collisions with bounds
                case EntityType.STATIC_ENTITY:
                    {
                        Vector3 previousBottomPosition = GetPreviousBottomPositionsForRayCast(character.GetCharacterCollisionBound(), BodyMechanics.FallTime, character);
                      
                        // Necessary data for subsequent calculations
                        RayCastOutputData outputData = null;
                        FRay rayFromBottom = new FRay(previousBottomPosition, character.Velocity);

                        float terrainIntersectionDistance = TerrainRayIntersection.Intersection_TerrainRay(DOUEngine.terrain, rayFromBottom);
                        bool bTerrainIntersection = !(terrainIntersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), terrainIntersectionDistance));

                        // No terrain intersection - check intersection with bounds
                        if (!bTerrainIntersection)
                        {
                            var rayCastResult = GetClosestRayCastResult(rayFromBottom, collidedBounds, character);

                            outputData = rayCastResult;

                            // Ray collided with one of the bounds, check angle of that plane, if can elevate on it - do it
                            if (RAYCAST_COLLIDED(outputData.shortestDistance))
                            {
                                Vector3 normalToCollidedPlane = outputData.collidedBound.GetNormalToIntersectedPosition(outputData.intersectionPosition);
                                // Character can step on this surface
                                if (REACHABLE_INCLINE(normalToCollidedPlane))
                                {
                                    RayCastOutputData finalData = outputData;

                                    // True - distance to intersected position is far, else - distance to intersected position is close
                                    if (RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), outputData.shortestDistance))
                                    {
                                        FRay rayDown = new FRay(previousBottomPosition + character.Velocity * character.Speed, -DOUEngine.Camera.getUpVector());
                                        var rayDownOutputData = GetClosestRayCastResult(rayDown, collidedBounds, character);
                                        finalData = rayDownOutputData;
                                    }

                                    Vector3 NewCharacterPosition = finalData.intersectionPosition;
                                    NewCharacterPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                                    character.collisionOffset(NewCharacterPosition);
                                    character.pushPositionStack();
                                    character.ActorState = BEHAVIOR_STATE.IDLE;
                                }
                                // If normal is down directed or too up directed - character can't step on this surface - return to previous position and set velocity to down
                                else
                                {
                                    character.popPositionStack();
                                    character.Velocity = -DOUEngine.Camera.getUpVector();
                                }
                            }
                            // No ray collision, but bound collision exists, bound position is unknown - return to previous position and set velocity to down
                            else
                            {
                                character.popPositionStack();
                                character.Velocity = -DOUEngine.Camera.getUpVector();
                            }
                        }
                        // Character could be elevated on terrain 
                        else
                        {
                            Vector3 CharacterNewPosition = rayFromBottom.GetPositionInTime(terrainIntersectionDistance);
                            CharacterNewPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                            character.collisionOffset(CharacterNewPosition);
                            character.ActorState = BEHAVIOR_STATE.IDLE;
                            character.pushPositionStack();
                        }

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
            Entity collidedEntity = data.GetCollidedRootComponent() as Entity;
            List<BoundBase> collidedBounds = data.GetCollidedBoundingBoxes();

            switch (character.ActorState)
            {
                case BEHAVIOR_STATE.FREE_FALLING: ProcessCollisionAtState_FreeFalling(character, collidedEntity, collidedBounds); break;
                case BEHAVIOR_STATE.MOVE: ProcessCollisionAtState_Move(character, collidedEntity, collidedBounds); break;
            }
        }
    }
}
