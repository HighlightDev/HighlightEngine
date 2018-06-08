﻿using System;
using System.Collections.Generic;

using MassiveGame.Optimization;
using GpuGraphics;
using OpenTK;
using PhysicsBox;
using TextureLoader;
using VMath;
using MassiveGame.API.Collector;
using MassiveGame.Core;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.RenderCore.Lights;
using MassiveGame.RenderCore;
using PhysicsBox.ComponentCore;
using MassiveGame.Physics;

namespace MassiveGame
{
    public abstract class Entity: Component, IVisible, ILightAffection, IDrawable
    {
        #region Definitions 

        protected bool _isInCameraView, _postConstructor;

        protected CollisionHeadUnit collisionHeadUnit;

        protected MistComponent _mist;

        protected RawModel _model;

        protected ITexture _texture;

        protected ITexture _normalMap;

        protected ITexture _specularMap;

        protected RenderMap LightVisibilityMap;

        public void SetComponents(List<Component> components)
        {
            ChildrenComponents = components;
            Component current = this;
            SetRelatedParentToComponent(ref current);
        }

        private void SetRelatedParentToComponent(ref Component parent)
        {
            for (Int32 i = 0; i < parent.ChildrenComponents.Count; i++)
            {
                Component child = parent.ChildrenComponents[i];

                child.ParentComponent = parent;
                child.Bound.ParentComponent = child;

                SetRelatedParentToComponent(ref child);
            }
        }

        public virtual void SetCollisionHeadUnit(CollisionHeadUnit collisionHeadUnit)
        {
            this.collisionHeadUnit = collisionHeadUnit;
        }

        public override void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            base.Tick(ref projectionMatrix, ref viewMatrix);
        }

        #region LightOptimization

        protected virtual List<PointLight> GetRelevantPointLights(List<PointLight> PotentialyAffectedPointLights)
        {
            List<PointLight> AffectedPointLights = new List<PointLight>();
            for (int i = 0; i < PotentialyAffectedPointLights.Count; i++)
            {
                if (LightVisibilityMap[i])
                {
                    AffectedPointLights.Add(PotentialyAffectedPointLights[i]);
                }
            }
            return AffectedPointLights;
        }

        public virtual void IsLightAffecting(List<PointLight> LightList)
        {
            LightVisibilityMap.Init(LightList.Count, false);
            for (int i = 0; i < LightList.Count; i++)
            {
                var componentAABB = GetAABBFromAllChildComponents();
                LightVisibilityMap[i] = GeometricMath.IsSphereVsSphereIntersection(componentAABB.GetOrigin(), componentAABB.GetExtent().Length,
                    LightList[i].Position.Xyz, LightList[i].AttenuationRadius);
            }
        }

        #endregion

        public bool IsInCameraView { protected set { _isInCameraView = value; } get { return _isInCameraView; } }

        #endregion

        #region Interface implementation

        public virtual bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (_postConstructor)
            {
                IsInCameraView = true;
            }
            else
            {
                var componentAABB = GetAABBFromAllChildComponents();
                throw new NotImplementedException("Update to new bounding box mechanics");
                IsInCameraView = FrustumCulling.isBoxIntersection(null, viewMatrix, ref projectionMatrix);
                //IsInCameraView = true;
            }
                    
            return IsInCameraView;
        }

        public VAO GetModel()
        {
            return _model.Buffer;
        }

        public ITexture GetDiffuseMap()
        {
            return _texture;
        }

        public ITexture GetNormalMap()
        {
            return _normalMap;
        }

        public ITexture GetSpecularMap()
        {
            return _specularMap;
        }

        #endregion

        #region Setter

        public void SetMistComponent(MistComponent mist)
        {
            this._mist = mist;
        }

        #endregion

        #region Transformation

        protected Matrix4 BuildTransformationMatrix()
        {
            Matrix4 ModelMatrix = Matrix4.Identity;

            if (ComponentRotation[0] != 0 || ComponentRotation[1] != 0 || ComponentRotation[2] != 0)
            {
                ModelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(ComponentRotation.X));
                ModelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ComponentRotation.Y));
                ModelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(ComponentRotation.Z));
            }

            if (ComponentScale[0] != 0 && ComponentScale[1] != 0 && ComponentScale[2] != 0)
            {
                if (ComponentScale[0] != 1 || ComponentScale[1] != 1 || ComponentScale[2] != 1)
                {
                    ModelMatrix *= Matrix4.CreateScale(ComponentScale);
                }
            }
            else
            {
                ModelMatrix *= Matrix4.CreateScale(1);
            }

            if (ComponentTranslation[0] != 0.0f || ComponentTranslation[1] != 0.0f || ComponentTranslation[2] != 0.0f)
            {
                ModelMatrix *= Matrix4.CreateTranslation(ComponentTranslation);
            }
            return ModelMatrix;
        } 

        #endregion

        #region Constructor

        public Entity() : base()
        {
        }
        public Entity(string modelPath, string texturePath, string normalMapPath, string specularMapPath,
            Vector3 translation, Vector3 rotation, Vector3 scale) : base()
        {
            ComponentTranslation = translation; 
            ComponentRotation = rotation;
            ComponentScale = scale;
            _texture = ResourcePool.GetTexture(texturePath);
            _normalMap = ResourcePool.GetTexture(normalMapPath);
            _specularMap = ResourcePool.GetTexture(specularMapPath);
            _model = new RawModel(modelPath);

            this._isInCameraView = true;
            this._mist = null;
            this._postConstructor = true;
            LightVisibilityMap = new RenderMap();
        }

        private float[,] GetTransformedVertices(ref Matrix4 modelMatrix, float[,] vertices)
        {
            for (int i = 0; i < vertices.Length / 3; i++)
            {
                Vector4 vertex = new Vector4(vertices[i, 0], vertices[i, 1], vertices[i, 2], 1.0f);
                vertex = VectorMath.multMatrix(modelMatrix, vertex);
                vertices[i, 0] = vertex.X;
                vertices[i, 1] = vertex.Y;
                vertices[i, 2] = vertex.Z;
            }
            return vertices;
        }

        private Vector3[] GetVerticesVectors(float[,] vertices)
        {
            Vector3[] vectors = new Vector3[vertices.Length / 3];
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = new Vector3(vertices[i, 0], vertices[i, 1], vertices[i, 2]);
            }
            return vectors;
        }

        #endregion

        #region Cleaning

        public virtual void cleanUp()
        {
            _model.Dispose();
            if (_texture != null)
                _texture.CleanUp();
            if (_normalMap != null)
                _normalMap.CleanUp();
            if (_specularMap != null)
                _specularMap.CleanUp();
        }

        #endregion
    }
}
