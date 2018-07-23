using MassiveGame.API.ObjectFactory;
using MassiveGame.Core.GameCore.Entities;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore.Terrain;
using MassiveGame.Core.PhysicsCore.OutputData;
using OpenTK;
using PhysicsBox;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassiveGame.Core.PhysicsCore
{
    public class CollisionBehaviorManager
    {
        private class RayCastOutputData
        {
            public BoundBase collidedBound { set; get; }
            public float shortestDistance { set; get; }
            public Vector3 intersectionPosition { set; get; }
            public FRay parentRay { set; get; }
            
            public RayCastOutputData(FRay ray, BoundBase boundBase, float shortestDistance, Vector3 intersectionPosition)
            {
                parentRay = ray;
                collidedBound = boundBase;
                this.shortestDistance = shortestDistance;
                this.intersectionPosition = intersectionPosition;
            }
        }

        public CollisionBehaviorManager() { }

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

        private RayCastOutputData GetClosestRayCastResult(FRay ray, List<BoundBase> collidedBounds)
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

            return new RayCastOutputData(ray, resultBound, resultShortestDistance, intersectionPosition);
        }

        private RayCastOutputData GetClosestRayCastResultFromMultipleRayCastExt(List<FRay> Rays, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            // Get closest ray cast result with existing priority - (high > middle > bottom)

            RayCastOutputData result = null;

            var boundOriginRayCastResult = GetClosestRayCastResult(Rays.Last(), collidedBounds);

            // Highest position has higher priority than origin
            var closestHighest = GetClosestAndHighestRay(GetRayCastResultsFromMultipleRayCast(Rays, collidedBounds, characterEntity), characterEntity.GetCharacterCollisionBound());

            if (closestHighest != null && closestHighest.shortestDistance > -1.0f && closestHighest.shortestDistance <= boundOriginRayCastResult.shortestDistance)
                result = closestHighest;

            else if (boundOriginRayCastResult.shortestDistance > -1)
                result = boundOriginRayCastResult;

            else
                result = GetClosestRayCastResultFromMultipleRayCast(Rays, collidedBounds, characterEntity);

            return result;
        }

        private RayCastOutputData GetClosestRayCastResultFromMultipleRayCast(List<FRay> Rays, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            RayCastOutputData resultOutput = null;
            float resultShortestDistance = -1.0f;

            foreach (FRay ray in Rays)
            {
                var localRayCastResult = GetClosestRayCastResult(ray, collidedBounds);

                if (resultShortestDistance <= -1)
                {
                    resultShortestDistance = localRayCastResult.shortestDistance;
                    resultOutput = localRayCastResult;
                }

                if (localRayCastResult.shortestDistance > -1 && resultShortestDistance >= localRayCastResult.shortestDistance)
                {
                    resultShortestDistance = localRayCastResult.shortestDistance;
                    resultOutput = localRayCastResult;
                }
            }

            return resultOutput;
        }

        private List<RayCastOutputData> GetRayCastResultsFromMultipleRayCast(List<FRay> Rays, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            List<RayCastOutputData> resultOutput = new List<RayCastOutputData>();

            foreach (FRay ray in Rays)
            {
                resultOutput.Add(GetClosestRayCastResult(ray, collidedBounds));
            }

            return resultOutput;
        }

        private RayCastOutputData GetClosestAndHighestRay(List<RayCastOutputData> rayCastResults, BoundBase characterBound)
        {
            float boundMaxY = characterBound.GetMax().Y;
            RayCastOutputData rayCastOutputData = null;

            List<RayCastOutputData> mostHighRayCastPositionResults = new List<RayCastOutputData>();

            // find rays with the most high start position
            foreach (var result in rayCastResults)
            {
                if (result.shortestDistance > -1 && GeometricMath.CMP(result.parentRay.StartPosition.Y, boundMaxY) > 0)
                {
                    mostHighRayCastPositionResults.Add(result);
                }
            }

            if (mostHighRayCastPositionResults.Count > 1)
            {
                // find ray with the most close intersection position
                float intersectionDistance = mostHighRayCastPositionResults.First().shortestDistance;
                rayCastOutputData = mostHighRayCastPositionResults.First();

                for (Int32 i = 1; i < mostHighRayCastPositionResults.Count; i++)
                {
                    if (mostHighRayCastPositionResults[i].shortestDistance <= intersectionDistance)
                    {
                        rayCastOutputData = mostHighRayCastPositionResults[i];
                        intersectionDistance = rayCastOutputData.shortestDistance;
                    }
                }
            }

            return rayCastOutputData;
        }

        private List<Vector3> GetRayCastPositionsOnCharacterBoundByVelocity(FRay[] insideCastRays, BoundBase characterBound, Vector3 boundMax, Vector3 boundMin, Vector3 boundOrigin)
        {
            List<Vector3> rayCastPositions = new List<Vector3>();

            for (Int32 i = 0; i < insideCastRays.Length; i++)
            {
                FRay ray = insideCastRays[i];
                float localIntersectionDistance = -1.0f;
                BoundBase.BoundType boundType = characterBound.GetBoundType();
                if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                    localIntersectionDistance = GeometricMath.Intersection_RayAABBExt(ray, boundMax, boundMin);
                else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                    localIntersectionDistance = GeometricMath.Intersection_RayOBBExt(ray, (characterBound as OBB).GetTangetX(), (characterBound as OBB).GetTangetY(),
                        (characterBound as OBB).GetTangetZ(), boundOrigin, (characterBound as OBB).GetExtent());

                if (localIntersectionDistance > -1.0f)
                {
                    Vector3 intersectionPosition = ray.GetPositionInTime(localIntersectionDistance);
                    rayCastPositions.Add(new Vector3(intersectionPosition));
                }
            }
            return rayCastPositions;
        }

        private List<Vector3> GetPreviousClosestPositionsForRayCast(BoundBase characterBound, float timeInterval, MovableEntity characterEntity)
        {
            Vector3 velocityToPreviousPosition = characterEntity.Velocity * (-timeInterval);

            Vector3 boundOrigin = characterBound.GetOrigin();
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();

            boundMax += velocityToPreviousPosition;
            boundMin += velocityToPreviousPosition;
            boundOrigin += velocityToPreviousPosition;


            FRay[] rays = new FRay[9]
           {
               new FRay(new Vector3(boundMax.X, boundMax.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMax.X, boundMax.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMax.X, boundMin.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMax.X, boundMin.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMax.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMax.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMin.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMin.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(boundOrigin, characterEntity.Velocity)
           };

            return GetRayCastPositionsOnCharacterBoundByVelocity(rays, characterBound, boundMax, boundMin, boundOrigin);
        }

        private List<Vector3> GetClosestPositionsForRayCast(FRay[] insideCastRays, BoundBase characterBound, Vector3 boundMax, Vector3 boundMin, Vector3 boundOrigin, float ResultHeight)
        {
            List<Vector3> rayCastPositions = new List<Vector3>();

            for (Int32 i = 0; i < insideCastRays.Length; i++)
            {
                FRay ray = insideCastRays[i];
                float localIntersectionDistance = 0.0f;
                BoundBase.BoundType boundType = characterBound.GetBoundType();
                if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                    localIntersectionDistance = GeometricMath.Intersection_RayAABBExt(ray, boundMax, boundMin);
                else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                    localIntersectionDistance = GeometricMath.Intersection_RayOBBExt(ray, (characterBound as OBB).GetTangetX(), (characterBound as OBB).GetTangetY(),
                        (characterBound as OBB).GetTangetZ(), boundOrigin, (characterBound as OBB).GetExtent());

                Vector3 intersectionPosition = ray.GetPositionInTime(localIntersectionDistance);
                rayCastPositions.Add(new Vector3(intersectionPosition.X, ResultHeight, intersectionPosition.Z));
            }
            return rayCastPositions;
        }

        private List<Vector3> GetCurrentMiddlePositionsForRayCast(BoundBase characterBound, MovableEntity characterEntity)
        {
            Vector3 boundOrigin = characterBound.GetOrigin();
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();

            Vector3 tangentY = -characterBound.GetTangetY();

            FRay[] rays = new FRay[5]
            {
                new FRay(boundOrigin, tangentY),
                new FRay(new Vector3(boundMax.X, boundOrigin.Y, boundMax.Z), tangentY),
                new FRay(new Vector3(boundMax.X, boundOrigin.Y, boundMin.Z), tangentY),
                new FRay(new Vector3(boundMin.X, boundOrigin.Y, boundMin.Z), tangentY),
                new FRay(new Vector3(boundMin.X, boundOrigin.Y, boundMax.Z), tangentY),
            };

            return GetClosestPositionsForRayCast(rays, characterBound, boundMax, boundMin, boundOrigin, boundOrigin.Y);
        }

        private List<Vector3> GetPreviousFreeFallBottomPositionsForRayCast(BoundBase characterBound, MovableEntity characterEntity)
        {
            Vector3 boundOrigin = characterBound.GetOrigin();
            Vector3 boundMax = characterBound.GetMax();
            Vector3 boundMin = characterBound.GetMin();

            Vector3 velocityToPreviousPosition = BodyMechanics.GetFreeFallVelocity(characterEntity.Velocity);
            boundMax -= velocityToPreviousPosition;
            boundMin -= velocityToPreviousPosition;
            boundOrigin -= velocityToPreviousPosition;


            FRay[] rays = new FRay[9]
           {
               new FRay(new Vector3(boundMax.X, boundMax.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMax.X, boundMax.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMax.X, boundMin.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMax.X, boundMin.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMax.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMax.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMin.Y, boundMax.Z), characterEntity.Velocity),
               new FRay(new Vector3(boundMin.X, boundMin.Y, boundMin.Z), characterEntity.Velocity),
               new FRay(boundOrigin, characterEntity.Velocity)
           };

            return GetRayCastPositionsOnCharacterBoundByVelocity(rays, characterBound, boundMax, boundMin, boundOrigin);
        }

        private void InterProcessDownRayCastCollision(MovableEntity character, BoundBase characterBound, List<BoundBase> collidedBounds)
        {
            List<Vector3> currentPositionsForRayCast = GetCurrentMiddlePositionsForRayCast(characterBound, character);
            List<FRay> listOfRays = new List<FRay>();
            for (Int32 i = 0; i < currentPositionsForRayCast.Count; i++)
                listOfRays.Add(new FRay(currentPositionsForRayCast[i], -EngineStatics.Camera.GetLocalSpaceUpVector()));

            RayCastOutputData closestRayCastDown = GetClosestRayCastResultFromMultipleRayCast(listOfRays, collidedBounds, character);

            if (RAYCAST_COLLIDED(closestRayCastDown.shortestDistance))
            {
                // Character could be elevated on collided mesh

                if (closestRayCastDown.shortestDistance <= (characterBound.GetExtent().Y * 2))
                {
                    float distanceToStep = (characterBound.GetExtent().Y - closestRayCastDown.shortestDistance);
                    Vector3 elevationPosition = characterBound.GetOrigin();
                    elevationPosition.Y += distanceToStep;
                    character.SetPosition(elevationPosition);
                    character.ActorState = BehaviorState.IDLE;
                }
                // Character now goes to free fall
                else
                    character.ActorState = BehaviorState.FREE_FALLING;

                character.pushPosition();
            }
            else
            {

                character.popPosition();
                character.ActorState = BehaviorState.IDLE;
            }
        }

        #endregion

        private void ProcessNoCollisionAtState_FreeFalling(MovableEntity character)
        {
            if (character.Velocity.LengthSquared > 0)
            {
                Vector3 boundMin = character.GetCharacterCollisionBound().GetMin();
                // Raycast from bottom height point
                Vector3 rayCastStartPosition = new Vector3(character.GetCharacterCollisionBound().GetOrigin());
                rayCastStartPosition.Y = boundMin.Y;

                FRay ray = new FRay(rayCastStartPosition, character.Velocity);
                float intersectionDistance = LandscapeRayIntersection.Intersection_TerrainRay(EngineStatics.terrain, ray);

                // Character is still in free fall, just update position
                if (intersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), intersectionDistance))
                    character.SetPosition(BodyMechanics.UpdateFreeFallPosition(character.ComponentTranslation, character.Velocity));

                // Character could be elevated on terrain 
                else
                {
                    Vector3 CharacterNewPosition = ray.GetPositionInTime(intersectionDistance);
                    CharacterNewPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                    character.SetPosition(CharacterNewPosition);
                    character.ActorState = BehaviorState.IDLE;
                }
            }

            character.pushPosition();
        }

        private void ProcessNoCollisionAtState_Move(MovableEntity character)
        {
            Vector3 boundMin = character.GetCharacterCollisionBound().GetMin();
            Vector3 origin = character.GetCharacterCollisionBound().GetOrigin();

            // Ray cast from middle height position to avoid miss ray casting
            Vector3 rayCastStartPosition = new Vector3(origin);

            FRay rayDown = new FRay(rayCastStartPosition, -EngineStatics.Camera.GetLocalSpaceUpVector());
            float intersectionDistance = LandscapeRayIntersection.Intersection_TerrainRay(EngineStatics.terrain, rayDown);

            // Subtract length of bound extent from middle height position
            float boundExtent = origin.Y - boundMin.Y;
            float actualIntersectionDistance = intersectionDistance - boundExtent;

            // Character is in free fall, next position will be calculated in next tick
            if (intersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), actualIntersectionDistance))
                character.ActorState = BehaviorState.FREE_FALLING;

            // Check if character can reach that height
            else
            {  // Character could be elevated on terrain 
                Vector3 CharacterNewPosition = rayDown.GetPositionInTime(intersectionDistance);
                CharacterNewPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                character.SetPosition(CharacterNewPosition);
                character.ActorState = BehaviorState.IDLE;
            }
            

            // Push current position to stack
            character.pushPosition();
        }

        private void ProcessCollisionAtState_Move(MovableEntity characterEntity, Entity collidedEntity, List<BoundBase> collidedBounds)
        {
            switch (GetEntityType(collidedEntity))
            {
                // If character collided character during move
                case EntityType.MOVABLE_ENTITY:
                    {
                        characterEntity.popPosition();
                        characterEntity.ActorState = BehaviorState.IDLE;
                        break;
                    }

                case EntityType.STATIC_ENTITY:
                    {
                        BoundBase characterBound = characterEntity.GetCharacterCollisionBound();

                        // 1) First of all character has to do ray cast in move direction
                        List<Vector3> previousPositionsForRayCast = GetPreviousClosestPositionsForRayCast(characterBound, characterEntity.Speed, characterEntity);
                        List<FRay> listOfRays = new List<FRay>();
                        for (Int32 i = 0; i < previousPositionsForRayCast.Count; i++)
                            listOfRays.Add(new FRay(previousPositionsForRayCast[i], characterEntity.Velocity));
                        RayCastOutputData outputData = GetClosestRayCastResultFromMultipleRayCastExt(listOfRays, collidedBounds, characterEntity);

                        // Ray intersected with one of collided bounds
                        if (RAYCAST_COLLIDED(outputData.shortestDistance))
                        {
                            // Distance to collided bound is too large and character can't step so far
                            // In this case do ray cast down from current character position
                            if (RAYCAST_INTERSECTION_FAR((characterEntity.Velocity * characterEntity.Speed).Length, outputData.shortestDistance))
                            {
                                InterProcessDownRayCastCollision(characterEntity, characterBound, collidedBounds);
                            }
                            // Distance to collided bound is small enough to step there
                            // In this case acquire normal to that plane to find out can character step on that surface, if no - pop previous position and set to idle
                            else
                            {
                                var rayCastResults = GetRayCastResultsFromMultipleRayCast(listOfRays, collidedBounds, characterEntity);
                                var upperRayResult = GetClosestAndHighestRay(rayCastResults, characterBound);

                                bool bCanElevateOnMesh = true;

                                if (upperRayResult != null)
                                {
                                    // check normal of collided plane
                                    BoundBase bound = outputData.collidedBound;
                                    Vector3 normalToCollidedPlane = bound.GetNormalToIntersectedPosition(outputData.intersectionPosition);
                                    if (!REACHABLE_INCLINE(normalToCollidedPlane)) bCanElevateOnMesh = false;
                                }

                                if (bCanElevateOnMesh)
                                {
                                    Vector3 NewCharacterPosition = characterBound.GetOrigin();
                                    NewCharacterPosition.Y = outputData.intersectionPosition.Y + characterBound.GetExtent().Y;
                                    characterEntity.SetPosition(NewCharacterPosition);
                                    characterEntity.pushPosition();
                                }
                                else
                                    characterEntity.popPosition();

                                characterEntity.ActorState = BehaviorState.IDLE;
                            }
                        }
                        /*  There was no intersection with ray, this could be one of these reasons :
                         *   Probably case : 
                         *   1) Character is on terrain;
                         *   2) Character is in free falling.
                         */
                        else
                        {
                            InterProcessDownRayCastCollision(characterEntity, characterBound, collidedBounds);
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
                        character.popPosition();
                        character.Velocity = -EngineStatics.Camera.GetLocalSpaceUpVector();
                        break;
                    }

                // If character is in free falling but has encountered some collisions with bounds
                case EntityType.STATIC_ENTITY:
                    {
                        List<Vector3> previousRayCastPositions = GetPreviousFreeFallBottomPositionsForRayCast(character.GetCharacterCollisionBound(), character);
                        List<FRay> listOfRays = new List<FRay>();
                        for (Int32 i = 0; i < previousRayCastPositions.Count; i++)
                            listOfRays.Add(new FRay(previousRayCastPositions[i], character.Velocity));

                        FRay rayFromMiddleBottom = listOfRays.First();

                        // Necessary data for subsequent calculations
                        RayCastOutputData rayCastOutputData = null;
                        float terrainIntersectionDistance = LandscapeRayIntersection.Intersection_TerrainRay(EngineStatics.terrain, rayFromMiddleBottom);

                        bool bTerrainIntersection = !(terrainIntersectionDistance < 0.0f ||
                            RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(character.Velocity), terrainIntersectionDistance));

                        // No terrain intersection - check intersection with bounds
                        if (!bTerrainIntersection)
                        {
                            rayCastOutputData = GetClosestRayCastResultFromMultipleRayCast(listOfRays, collidedBounds, character);

                            // Ray collided with one of the bounds, check angle of that plane, if can elevate on it - do it
                            if (RAYCAST_COLLIDED(rayCastOutputData.shortestDistance))
                            {
                                Vector3 normalToCollidedPlane = rayCastOutputData.collidedBound.GetNormalToIntersectedPosition(rayCastOutputData.intersectionPosition);
                                // Character can step on this surface
                                if (REACHABLE_INCLINE(normalToCollidedPlane))
                                {
                                    Vector3 BoundOrigin = character.GetCharacterCollisionBound().GetOrigin();
                                    Vector3 NewCharacterPosition = new Vector3(BoundOrigin.X, rayCastOutputData.intersectionPosition.Y + character.GetCharacterCollisionBound().GetExtent().Y,
                                        BoundOrigin.Z);
                                    character.SetPosition(NewCharacterPosition);
                                    character.pushPosition();
                                    character.ActorState = BehaviorState.IDLE;
                                }
                                // If normal is down directed or too up directed - character can't step on this surface - return to previous position and set velocity to down
                                else
                                {
                                    // This is quick fix
                                    character.ActorState = BehaviorState.MOVE;
                                    character.popPosition();
                                    character.Velocity = -EngineStatics.Camera.GetLocalSpaceUpVector();
                                }
                            }
                            // No ray collision, but bound collision exists, bound position is unknown - return to previous position and set velocity to down
                            else
                            {
                                character.popPosition();
                                character.Velocity = -EngineStatics.Camera.GetLocalSpaceUpVector();
                            }
                        }
                        // Character could be elevated on terrain 
                        else
                        {
                            Vector3 CharacterNewPosition = rayFromMiddleBottom.GetPositionInTime(terrainIntersectionDistance);
                            CharacterNewPosition.Y += character.GetCharacterCollisionBound().GetExtent().Y;
                            character.SetPosition(CharacterNewPosition);
                            character.ActorState = BehaviorState.IDLE;
                            character.pushPosition();
                        }

                        break;
                    }
            }
        }

        private void Process_NoCollision(CollisionOutputNoCollided collisionOutputNoCollided)
        {
            MovableEntity character = collisionOutputNoCollided.GetCharacterRootComponent() as MovableEntity;

            switch (character.ActorState)
            {
                // Character is in free fall and has no collision with bounds
                // need to find his next position
                case BehaviorState.FREE_FALLING: ProcessNoCollisionAtState_FreeFalling(character); break;
                // Character is moving and has no collision with bound
                // he may be colliding with terrain, if yes - elevate him on it, else - set free falling state
                case BehaviorState.MOVE: ProcessNoCollisionAtState_Move(character); break;
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
                case BehaviorState.FREE_FALLING: ProcessCollisionAtState_FreeFalling(character, collidedEntity, collidedBounds); break;
                case BehaviorState.MOVE: ProcessCollisionAtState_Move(character, collidedEntity, collidedBounds); break;
            }
        }
    }
}
