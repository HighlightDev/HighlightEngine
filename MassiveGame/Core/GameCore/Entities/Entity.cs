using System;
using System.Collections.Generic;
using OpenTK;
using TextureLoader;
using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.GameCore.EntityComponents;
using VBO;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.Mesh;
using MassiveGame.Core.MathCore.MathTypes;
using MassiveGame.Core.MathCore;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore.Entities
{
    [Serializable]
    public abstract class Entity: 
          Component
        , IVisible
        , ILightHit
        , IDrawable
        , IObservable
        , ISerializable
    { 
        #region Definitions 

        protected MistComponent m_mist;

        [NonSerialized]
        protected bool bVisibleByCamera, bPostConstructor;

        [NonSerialized]
        protected CollisionHeadUnit m_collisionHeadUnit;

        [NonSerialized]
        protected BoolMap m_lightVisibilityMap;

        [NonSerialized]
        protected Skin m_skin;

        [NonSerialized]
        protected ShaderBase m_shader;

        [NonSerialized]
        protected ITexture m_texture;

        [NonSerialized]
        protected ITexture m_normalMap;

        [NonSerialized]
        protected ITexture m_specularMap;

        [NonSerialized]
        protected bool bIsCollidable = false;

        #region Constructor

        public Entity() : base() { }

        public Entity(string modelPath, string texturePath, string normalMapPath, string specularMapPath,
            Vector3 translation, Vector3 rotation, Vector3 scale) : base()
        {
            ComponentTranslation = translation;
            ComponentRotation = rotation;
            ComponentScale = scale;
            bVisibleByCamera = true;
            m_mist = null;

            InitResources(modelPath, texturePath, normalMapPath, specularMapPath);
        }

        #endregion

        protected abstract void InitShader();

        protected abstract void FreeShader();

        private void InitResources(string modelPath, string texturePath, string normalMapPath, string specularMapPath)
        {
            m_lightVisibilityMap = new BoolMap();
            m_texture = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(texturePath);
            m_normalMap = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(normalMapPath);
            m_specularMap = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(specularMapPath);
            m_skin = PoolProxy.GetResource<ObtainModelPool, ModelAllocationPolicy, string, Skin>(modelPath);
            InitShader();
        }

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
            this.m_collisionHeadUnit = collisionHeadUnit;
            bIsCollidable = true;
        }

        #region LightOptimization

        protected virtual List<PointLight> GetRelevantPointLights(List<PointLight> PotentialyAffectedPointLights)
        {
            List<PointLight> AffectedPointLights = new List<PointLight>();
            for (Int32 i = 0; i < PotentialyAffectedPointLights.Count; i++)
            {
                if (m_lightVisibilityMap[i])
                {
                    AffectedPointLights.Add(PotentialyAffectedPointLights[i]);
                }
            }
            return AffectedPointLights;
        }

        #endregion

        public bool IsVisibleByCamera { protected set { bVisibleByCamera = value; } get { return bVisibleByCamera; } }

        #endregion

        #region Serialization

        protected Entity(SerializationInfo info, StreamingContext context)
        {
            // TODO -> deserialize all properties

            bIsCollidable = false;
            bPostConstructor = true;

            var texturePath = info.GetString("albedo");
            var normalTexPath = info.GetString("normalMap");
            var specularTexPath = info.GetString("specularMap");
            var skinPath = info.GetString("skin");
            InitResources(skinPath, texturePath, normalTexPath, specularTexPath);

            m_mist = info.GetValue("mistComponent", typeof(MistComponent)) as MistComponent;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // TODO -> serialize all properties

            var texturePath = PoolProxy.GetResourceKey<ObtainTexturePool, string, ITexture>(m_texture);
            var normalTexPath = PoolProxy.GetResourceKey<ObtainTexturePool, string, ITexture>(m_normalMap);
            var specularTexPath = PoolProxy.GetResourceKey<ObtainTexturePool, string, ITexture>(m_specularMap);
            var skinPath = PoolProxy.GetResourceKey<ObtainModelPool, string, Skin>(m_skin);

            info.AddValue("albedo", texturePath);
            info.AddValue("normalMap", normalTexPath);
            info.AddValue("specularMap", specularTexPath);
            info.AddValue("skin", skinPath);
            info.AddValue("mistComponent", m_mist, typeof(MistComponent));
        }

        #endregion

        #region Interface implementation

        public virtual void IsLitByLightSource(List<PointLight> LightList)
        {
            m_lightVisibilityMap.Init(LightList.Count, false);
            for (Int32 i = 0; i < LightList.Count; i++)
            {
                BoundBase bound = GetAABBFromAllChildComponents();
                FSphere boundSphere = (FSphere)bound;
                FSphere lightSphere = new FSphere(LightList[i].Position.Xyz, LightList[i].AttenuationRadius);
                m_lightVisibilityMap[i] = GeometryMath.IsSphereVsSphereIntersection(ref boundSphere, ref lightSphere);
            }
        }

        private void CheckComponentsAreVisible(ref Component parentComponent, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix, ref bool bVisible)
        {
            if (parentComponent.Bound != null)
            {
                bVisible = FrustumCulling.GetIsBoundInFrustum(parentComponent.Bound, ref viewMatrix, ref projectionMatrix);
                if (bVisible)
                    return;
            }
            foreach (Component childComponent in parentComponent.ChildrenComponents)
            {
                var child = childComponent;
                CheckComponentsAreVisible(ref child, ref viewMatrix, ref projectionMatrix, ref bVisible);
                if (bVisible)
                    return;
            }
        }

        public virtual bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (bPostConstructor)
            {
                IsVisibleByCamera = true;
            }
            else
            {
                Component current = this;
                bool bVisible = false;
                CheckComponentsAreVisible(ref current, ref viewMatrix, ref projectionMatrix, ref bVisible);
                IsVisibleByCamera = bVisible;
            }

            return IsVisibleByCamera;
        }

        public VertexArrayObject GetMeshVao()
        {
            return m_skin.Buffer;
        }

        public ITexture GetDiffuseMap()
        {
            return m_texture;
        }

        public ITexture GetNormalMap()
        {
            return m_normalMap;
        }

        public ITexture GetSpecularMap()
        {
            return m_specularMap;
        }

        public void NotifyAdded()
        {
            GameWorld.GetWorldInstance().GetLevel().ShadowCastCollection.Add(this);
            GameWorld.GetWorldInstance().GetLevel().LitCheckCollection.Add(this);
            GameWorld.GetWorldInstance().GetLevel().VisibilityCheckCollection.Add(this);
        }

        public void NotifyRemoved()
        {
            GameWorld.GetWorldInstance().GetLevel().ShadowCastCollection.Remove(this);
            GameWorld.GetWorldInstance().GetLevel().LitCheckCollection.Remove(this);
            GameWorld.GetWorldInstance().GetLevel().VisibilityCheckCollection.Remove(this);
        }

        #endregion

        #region Setter

        public void SetMistComponent(MistComponent mist)
        {
            this.m_mist = mist;
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

        #region Cleaning

        public virtual void CleanUp()
        {
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(m_texture);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(m_normalMap);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(m_specularMap);
            PoolProxy.FreeResourceMemory<ObtainModelPool, ModelAllocationPolicy, string, Skin>(m_skin);
            FreeShader();
        }

        #endregion
    }
}
