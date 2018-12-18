using MassiveGame.API.ObjectFactory;
using MassiveGame.Core.GameCore.Entities;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.GameCore.Terrain;
using MassiveGame.Core.PhysicsCore.OutputData;
using OpenTK;
using MassiveGame.Core.ComponentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using MassiveGame.Core.MathCore.MathTypes;
using MassiveGame.Core.MathCore;
using MassiveGame.Core.GameCore;
using System.Diagnostics.Contracts;

namespace MassiveGame.Core.PhysicsCore
{
    public class CollisionBehaviorManager
    {
        private class RaycastOutputData
        {
            public BoundBase CollidedBound { set; get; }
            public float ShortestDistance { set; get; }
            public Vector3 IntersectionPosition { set; get; }
            public FRay ParentRay { set; get; }

            public RaycastOutputData(FRay ray, BoundBase boundBase, float shortestDistance, Vector3 intersectionPosition)
            {
                ParentRay = ray;
                CollidedBound = boundBase;
                this.ShortestDistance = shortestDistance;
                this.IntersectionPosition = intersectionPosition;
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

        #region physics_data

        private void UPDATE_PHYSICS_DATA(MovableEntity character)
        {
            BoundBase character_bound = character.GetCharacterCollisionBound();
            char_bound_origin = character_bound.GetOrigin();
            char_bound_max = character_bound.GetMax();
            char_bound_min = character_bound.GetMin();
            char_bound_tangentX = character_bound.GetTangentX();
            char_bound_tangentY = character_bound.GetTangentY();
            char_bound_tangentZ = character_bound.GetTangentZ();
            char_bound_extent = character_bound.GetExtent();
            char_velocity = character.Velocity;
        }

        // this is necessary data for physics calculations
        private Vector3
              char_bound_origin
            , char_bound_min
            , char_bound_max
            , char_velocity
            , char_bound_tangentX
            , char_bound_tangentY
            , char_bound_tangentZ
            , char_bound_extent;

        #endregion

        private bool IsCollisionInDistanceOccursInOneTick(RaycastOutputData raycastDown, float distanceBetweenCollidedPoints)
        {
            return (raycastDown.ShortestDistance <= distanceBetweenCollidedPoints);
        }

        private List<Vector3> GetPreviousBoundEdgePoints_WithCurrentVelocity(ref Vector3 currentVelocity, BoundBase.BoundType charBoundType)
        {
            Vector3 prev_boundMax = char_bound_max - currentVelocity;
            Vector3 prev_boundMin = char_bound_min - currentVelocity;
            Vector3 prev_boundOrigin = char_bound_origin - currentVelocity;

            FRay[] rays = new FRay[9] // all points of bound + center
           {
               new FRay(new Vector3(prev_boundMax.X, prev_boundMax.Y, prev_boundMax.Z), char_velocity),
               new FRay(new Vector3(prev_boundMax.X, prev_boundMax.Y, prev_boundMin.Z), char_velocity),
               new FRay(new Vector3(prev_boundMax.X, prev_boundMin.Y, prev_boundMax.Z), char_velocity),
               new FRay(new Vector3(prev_boundMax.X, prev_boundMin.Y, prev_boundMin.Z), char_velocity),
               new FRay(new Vector3(prev_boundMin.X, prev_boundMax.Y, prev_boundMax.Z), char_velocity),
               new FRay(new Vector3(prev_boundMin.X, prev_boundMax.Y, prev_boundMin.Z), char_velocity),
               new FRay(new Vector3(prev_boundMin.X, prev_boundMin.Y, prev_boundMax.Z), char_velocity),
               new FRay(new Vector3(prev_boundMin.X, prev_boundMin.Y, prev_boundMin.Z), char_velocity),
               new FRay(prev_boundOrigin, char_velocity)
           };

            return GetClosestPointsOnBoundByInsideRaycasting(rays, charBoundType, prev_boundMax, prev_boundMin, prev_boundOrigin);
        }

        private RaycastOutputData IsRaycastDownCollided(MovableEntity character, BoundBase characterBound, List<BoundBase> collidedBounds)
        {
            RaycastOutputData raycastDownResult = null;

            List<Vector3> currentPositionsForRaycast = GetCurrentBoundMiddleEdgePoints(characterBound.GetBoundType());
            List<FRay> listOfRays = new List<FRay>();

            for (Int32 i = 0; i < currentPositionsForRaycast.Count; i++)
                listOfRays.Add(new FRay(currentPositionsForRaycast[i], -GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector()));

            raycastDownResult = RaycastMultiple(listOfRays, collidedBounds, character);

            return raycastDownResult;
        }

        private bool RAYCAST_COLLIDED(RaycastOutputData raycastResult)
        {
            return (raycastResult.ShortestDistance >= 0f);
        }

        private bool RAYCAST_COLLIDED(float distanceToIntersection)
        {
            return (distanceToIntersection > -1f);
        }

        private bool RAYCAST_INTERSECTION_FAR(float characterSpeed, float intersectionDistance)
        {
            return (intersectionDistance > characterSpeed);
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

        private RaycastOutputData Raycast(FRay ray, List<BoundBase> collidedBounds)
        {
            float resultShortestDistance = -1.0f;
            BoundBase resultBound = null;

            // DO RAY CAST IN ALL COLLIDED BOUNDING BOXES
            for (Int32 i = 0; i < collidedBounds.Count; i++)
            {
                float localIntersectionDistance = 0.0f;
                BoundBase.BoundType boundType = collidedBounds[i].GetBoundType();
                if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                    localIntersectionDistance = GeometryMath.Intersection_RayAABB(ray, collidedBounds[i] as AABB);
                else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                    localIntersectionDistance = GeometryMath.Intersection_RayOBB(ray, collidedBounds[i] as OBB);

                if (resultShortestDistance <= -1.0f || (localIntersectionDistance > 0.0f && localIntersectionDistance < resultShortestDistance))
                {
                    resultShortestDistance = localIntersectionDistance;
                    resultBound = collidedBounds[i];
                }
            }
           
            Vector3 intersectionPosition = ray.GetPositionInTime(resultShortestDistance);

            return new RaycastOutputData(ray, resultBound, resultShortestDistance, intersectionPosition);
        }

        private RaycastOutputData RaycastMultiple(List<FRay> Rays, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            RaycastOutputData resultOutput = null;
            float resultShortestDistance = -1.0f;

            foreach (FRay ray in Rays)
            {
                var localRaycastResult = Raycast(ray, collidedBounds);

                if (!RAYCAST_COLLIDED(resultShortestDistance) || (RAYCAST_COLLIDED(localRaycastResult) && resultShortestDistance >= localRaycastResult.ShortestDistance))
                {
                    resultShortestDistance = localRaycastResult.ShortestDistance;
                    resultOutput = localRaycastResult;
                }
            }

            return resultOutput;
        }

        private RaycastOutputData RaycastMultipleWithFilter(List<FRay> Rays, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            // Get closest ray cast result with existing priority - (high > middle > bottom)

            RaycastOutputData result = null;

            var multipleRaycast = GetRaycastResultsFromMultipleRaycast(Rays, collidedBounds, characterEntity);
            var highPoint = GetClosestAndHighestRaycastResult(multipleRaycast, characterEntity.GetCharacterCollisionBound());
            var middlePoint = multipleRaycast.Last();

            if (highPoint != null && RAYCAST_COLLIDED(highPoint) && highPoint.ShortestDistance <= middlePoint.ShortestDistance)
            {
                result = highPoint;
            }
            else if (RAYCAST_COLLIDED(middlePoint))
            {
                result = middlePoint;
            }
            else
                result = GetClosestRaycastResultFromMultiple(multipleRaycast);

            return result;
        }

        private List<RaycastOutputData> GetRaycastResultsFromMultipleRaycast(List<FRay> Rays, List<BoundBase> collidedBounds, MovableEntity characterEntity)
        {
            List<RaycastOutputData> resultOutput = new List<RaycastOutputData>();

            foreach (FRay ray in Rays)
            {
                resultOutput.Add(Raycast(ray, collidedBounds));
            }

            return resultOutput;
        }

        private RaycastOutputData GetClosestRaycastResultFromMultiple(List<RaycastOutputData> raycastOutputs)
        {
            RaycastOutputData resultOutput = null;
            float resultShortestDistance = -1.0f;

            foreach (var data in raycastOutputs)
            {
                if (!RAYCAST_COLLIDED(resultShortestDistance) || (RAYCAST_COLLIDED(data) && resultShortestDistance >= data.ShortestDistance))
                {
                    resultShortestDistance = data.ShortestDistance;
                    resultOutput = data;
                }
            }

            return resultOutput;
        }

        private RaycastOutputData GetClosestAndHighestRaycastResult(List<RaycastOutputData> rayCastResults, BoundBase characterBound)
        {
            float boundMaxY = char_bound_max.Y;
            RaycastOutputData rayCastOutputData = null;

            List<RaycastOutputData> mostHighRaycastPositionResults = new List<RaycastOutputData>();

            // find rays with the most high start position
            foreach (var result in rayCastResults)
            {
                if (RAYCAST_COLLIDED(result) && GeometryMath.CMP(result.ParentRay.StartPosition.Y, boundMaxY) > 0)
                {
                    mostHighRaycastPositionResults.Add(result);
                }
            }

            // If multiple results
            if (mostHighRaycastPositionResults.Count > 1)
            {
                // find ray with the most close intersection position
                float intersectionDistance = mostHighRaycastPositionResults.First().ShortestDistance;
                rayCastOutputData = mostHighRaycastPositionResults.First();

                for (Int32 i = 1; i < mostHighRaycastPositionResults.Count; i++)
                {
                    if (mostHighRaycastPositionResults[i].ShortestDistance <= intersectionDistance)
                    {
                        rayCastOutputData = mostHighRaycastPositionResults[i];
                        intersectionDistance = rayCastOutputData.ShortestDistance;
                    }
                }
            }

            return rayCastOutputData;
        }

        private List<Vector3> GetClosestPointsOnBoundByInsideRaycasting(FRay[] insideCastRays, BoundBase.BoundType charBoundType,
            Vector3 boundMax, Vector3 boundMin, Vector3 boundOrigin)
        {
            List<Vector3> rayCastPositions = new List<Vector3>();
            bool bIntersectionOccured = false;

            for (Int32 i = 0; i < insideCastRays.Length; i++)
            {
                FRay ray = insideCastRays[i];
                float localIntersectionDistance = -1.0f;

                if ((charBoundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                {
                    localIntersectionDistance = GeometryMath.Intersection_RayAABBExt(ray, boundMax, boundMin);
                }
                else if ((charBoundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
                {
                    localIntersectionDistance = GeometryMath.Intersection_RayOBBExt(ray, char_bound_tangentX, char_bound_tangentY, char_bound_tangentZ, boundOrigin, char_bound_extent);
                }

                if (RAYCAST_COLLIDED(localIntersectionDistance))
                    bIntersectionOccured = true;

                Vector3 intersectionPosition = ray.GetPositionInTime(localIntersectionDistance);
                rayCastPositions.Add(new Vector3(intersectionPosition));
            }

            Contract.Assert(bIntersectionOccured, "Ray cast bound from inside finished with logic errors, there were no collision.");

            return rayCastPositions;
        }

        private List<Vector3> GetCurrentBoundMiddleEdgePoints(BoundBase.BoundType boundType)
        {
            float bound_origin_height = char_bound_origin.Y;

            FRay[] rays = new FRay[5]
            {
                new FRay(char_bound_origin, char_bound_tangentY),
                new FRay(new Vector3(char_bound_max.X, bound_origin_height, char_bound_max.Z), char_bound_tangentY),
                new FRay(new Vector3(char_bound_max.X, bound_origin_height, char_bound_min.Z), char_bound_tangentY),
                new FRay(new Vector3(char_bound_min.X, bound_origin_height, char_bound_min.Z), char_bound_tangentY),
                new FRay(new Vector3(char_bound_min.X, bound_origin_height, char_bound_max.Z), char_bound_tangentY),
            };

            List<Vector3> closestPoints = GetClosestPointsOnBoundByInsideRaycasting(rays, boundType, char_bound_max, char_bound_min, char_bound_origin);
            // Set origin height
            closestPoints.ForEach(point => point.Y = bound_origin_height);

            return closestPoints;
        }

        private void InterProcessDownRaycastCollision(MovableEntity character, BoundBase characterBound, List<BoundBase> collidedBounds)
        {
            List<Vector3> currentPositionsForRaycast = GetCurrentBoundMiddleEdgePoints(characterBound.GetBoundType());
            List<FRay> listOfRays = new List<FRay>();
            for (Int32 i = 0; i < currentPositionsForRaycast.Count; i++)
                listOfRays.Add(new FRay(currentPositionsForRaycast[i], -GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector()));

            RaycastOutputData closestRaycastDown = RaycastMultiple(listOfRays, collidedBounds, character);

            if (RAYCAST_COLLIDED(closestRaycastDown))
            {
                // Character could be elevated on collided mesh

                if (closestRaycastDown.ShortestDistance <= (char_bound_extent.Y * 2))
                {
                    float distanceToStep = (char_bound_extent.Y - closestRaycastDown.ShortestDistance);
                    Vector3 elevationPosition = char_bound_origin;
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
            if (char_velocity.LengthSquared > 0)
            {
                // Ray cast from bottom height point
                Vector3 rayCastStartPosition = char_bound_origin;
                rayCastStartPosition.Y = char_bound_min.Y;

                FRay ray = new FRay(rayCastStartPosition, char_velocity);
                float intersectionDistance = LandscapeRayIntersection.Intersection_TerrainRay(GameWorld.GetWorldInstance().GetLevel().Terrain.GetData(), ray);

                // Character is still in free fall, just update position
                if (!RAYCAST_COLLIDED(intersectionDistance) || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(char_velocity), intersectionDistance))
                    character.SetPosition(BodyMechanics.UpdateFreeFallPosition(character.ComponentTranslation, char_velocity));

                // Character could be elevated on terrain 
                else
                {
                    Vector3 CharacterNewPosition = ray.GetPositionInTime(intersectionDistance);
                    CharacterNewPosition.Y += char_bound_extent.Y;
                    character.SetPosition(CharacterNewPosition);
                    character.ActorState = BehaviorState.IDLE;
                }
            }

            character.pushPosition();
        }

        private void ProcessNoCollisionAtState_Move(MovableEntity character)
        {
            Vector3 boundMin = char_bound_min;
            Vector3 origin = char_bound_origin;

            // Ray cast from middle height position to avoid miss ray casting
            Vector3 rayCastStartPosition = origin;

            FRay rayDown = new FRay(rayCastStartPosition, -GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector());
            float intersectionDistance = LandscapeRayIntersection.Intersection_TerrainRay(GameWorld.GetWorldInstance().GetLevel().Terrain.GetData(), rayDown);

            // Subtract length of bound extent from middle height position
            float boundExtent = origin.Y - boundMin.Y;
            float actualIntersectionDistance = intersectionDistance - boundExtent;

            // Character is in free fall, next position will be calculated in next tick
            if (intersectionDistance < 0.0f || RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(char_velocity), actualIntersectionDistance))
                character.ActorState = BehaviorState.FREE_FALLING;

            // Check if character can reach that height
            else
            {  // Character could be elevated on terrain 
                Vector3 CharacterNewPosition = rayDown.GetPositionInTime(intersectionDistance);
                CharacterNewPosition.Y += char_bound_extent.Y;
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
                // If character collided another character during move
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
                        Vector3 currentVelocity = (char_velocity * characterEntity.Speed);
                        List<Vector3> previousPositionsForRaycast = GetPreviousBoundEdgePoints_WithCurrentVelocity(ref currentVelocity, characterBound.GetBoundType());

                        List<FRay> listOfRays = new List<FRay>();
                        for (Int32 i = 0; i < previousPositionsForRaycast.Count; i++)
                            listOfRays.Add(new FRay(previousPositionsForRaycast[i], char_velocity));
                        RaycastOutputData outputData = RaycastMultipleWithFilter(listOfRays, collidedBounds, characterEntity);

                        // Ray intersected with one of collided bounds
                        if (RAYCAST_COLLIDED(outputData))
                        {
                            // Distance to collided bound is too large and character can't step so far
                            // In this case do ray cast down from current character position
                            if (RAYCAST_INTERSECTION_FAR((char_velocity * characterEntity.Speed).Length, outputData.ShortestDistance))
                            {
                                InterProcessDownRaycastCollision(characterEntity, characterBound, collidedBounds);
                            }
                            // Distance to collided bound is small enough to step there
                            // In this case acquire normal to that plane to find out can character step on that surface, if no - pop previous position and set to idle
                            else
                            {
                                var rayCastResults = GetRaycastResultsFromMultipleRaycast(listOfRays, collidedBounds, characterEntity);
                                var upperRayResult = GetClosestAndHighestRaycastResult(rayCastResults, characterBound);

                                bool bCanElevateOnMesh = true;

                                if (upperRayResult != null)
                                {
                                    // check normal of collided plane
                                    BoundBase bound = outputData.CollidedBound;
                                    Vector3 normalToCollidedPlane = bound.GetNormalToIntersectedPosition(outputData.IntersectionPosition);

                                    if (!REACHABLE_INCLINE(normalToCollidedPlane))
                                    {
                                        bCanElevateOnMesh = false;
                                        RaycastOutputData raycast_down = IsRaycastDownCollided(characterEntity, characterBound, collidedBounds);

                                        characterEntity.popPosition();
                                        characterEntity.Velocity = GetAvoidingVelocity(normalToCollidedPlane, char_velocity);
                                        char_velocity = characterEntity.Velocity;
                                        var newPosition = characterEntity.ComponentTranslation + char_velocity * characterEntity.Speed;
                                        characterEntity.SetPosition(newPosition);
                                        characterEntity.pushPosition();
                                        characterEntity.ActorState = BehaviorState.IDLE;

                                        if (!RAYCAST_COLLIDED(raycast_down) || !IsCollisionInDistanceOccursInOneTick(raycast_down, (char_bound_extent.Y * 2)))
                                        {
                                            characterEntity.ActorState = BehaviorState.FREE_FALLING;
                                        }
                                    }
                                }

                                if (bCanElevateOnMesh)
                                {
                                    Vector3 NewCharacterPosition = char_bound_origin;
                                    NewCharacterPosition.Y = outputData.IntersectionPosition.Y + char_bound_extent.Y;
                                    characterEntity.SetPosition(NewCharacterPosition);
                                    characterEntity.pushPosition();
                                    characterEntity.ActorState = BehaviorState.IDLE;
                                }
                            }
                        }
                        /*  There was no intersection with ray, this could be one of these reasons :
                         *   Probably case : 
                         *   1) Character is on terrain;
                         *   2) Character is in free falling.
                         */
                        else
                        {
                            InterProcessDownRaycastCollision(characterEntity, characterBound, collidedBounds);
                        }
                        break;
                    }
            }
        }

        private Vector3 GetAvoidingVelocity(Vector3 normalToCollidedPlane, Vector3 velocity)
        {
            Vector3 binormalToCollidedPlane = Vector3.Cross(normalToCollidedPlane, GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector());
            float velocityProjectBinormal = GeometryMath.ProjectVectorOnNormalizedVector(binormalToCollidedPlane, velocity);
            Vector3 avoidingVelocity = velocityProjectBinormal * binormalToCollidedPlane;
            return avoidingVelocity;
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
                        character.Velocity = -GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector();
                        char_velocity = character.Velocity;
                        break;
                    }

                // If character is in free falling but has encountered some collisions with bounds
                case EntityType.STATIC_ENTITY:
                    {
                        BoundBase characterBound = character.GetCharacterCollisionBound();
                        Vector3 currentVelocity = BodyMechanics.GetFreeFallVelocity(char_velocity);
                        List<Vector3> previousPositionsForRaycast = GetPreviousBoundEdgePoints_WithCurrentVelocity(ref currentVelocity, characterBound.GetBoundType());

                        List<FRay> listOfRays = new List<FRay>();
                        for (Int32 i = 0; i < previousPositionsForRaycast.Count; i++)
                            listOfRays.Add(new FRay(previousPositionsForRaycast[i], char_velocity));

                        FRay rayFromMiddleBottom = listOfRays.First();

                        // Necessary data for subsequent calculations
                        RaycastOutputData rayCastOutputData = null;
                        float terrainIntersectionDistance = LandscapeRayIntersection.Intersection_TerrainRay(GameWorld.GetWorldInstance().GetLevel().Terrain.GetData(), rayFromMiddleBottom);

                        bool bTerrainIntersection = !(terrainIntersectionDistance < 0.0f ||
                            RAYCAST_INTERSECTION_FAR(BodyMechanics.GetFreeFallDistanceInVelocity(char_velocity), terrainIntersectionDistance));

                        // No terrain intersection - check intersection with bounds
                        if (!bTerrainIntersection)
                        {
                            rayCastOutputData = RaycastMultiple(listOfRays, collidedBounds, character);

                            // Ray collided with one of the bounds, check angle of that plane, if can elevate on it - do it
                            if (RAYCAST_COLLIDED(rayCastOutputData))
                            {
                                Vector3 normalToCollidedPlane = rayCastOutputData.CollidedBound.GetNormalToIntersectedPosition(rayCastOutputData.IntersectionPosition);
                                // Character can step on this surface
                                if (REACHABLE_INCLINE(normalToCollidedPlane))
                                {
                                    Vector3 BoundOrigin = char_bound_origin;
                                    Vector3 NewCharacterPosition = new Vector3(BoundOrigin.X, rayCastOutputData.IntersectionPosition.Y + char_bound_extent.Y,
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
                                    character.Velocity = -GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector();
                                    char_velocity = character.Velocity;
                                }
                            }
                            // No ray collision, but bound collision exists, bound position is unknown - return to previous position and set velocity to down
                            else
                            {
                                character.popPosition();
                                character.Velocity = -GameWorld.GetWorldInstance().GetLevel().Camera.GetLocalSpaceUpVector();
                                char_velocity = character.Velocity;
                            }
                        }
                        // Character could be elevated on terrain 
                        else
                        {
                            Vector3 CharacterNewPosition = rayFromMiddleBottom.GetPositionInTime(terrainIntersectionDistance);
                            CharacterNewPosition.Y += char_bound_extent.Y;
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
            // THIS IS VERY IMPORTANT UPDATE
            UPDATE_PHYSICS_DATA(character);

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

            // THIS IS VERY IMPORTANT UPDATE
            UPDATE_PHYSICS_DATA(character);

            switch (character.ActorState)
            {
                case BehaviorState.FREE_FALLING: ProcessCollisionAtState_FreeFalling(character, collidedEntity, collidedBounds); break;
                case BehaviorState.MOVE: ProcessCollisionAtState_Move(character, collidedEntity, collidedBounds); break;
            }
        }
    }
}
