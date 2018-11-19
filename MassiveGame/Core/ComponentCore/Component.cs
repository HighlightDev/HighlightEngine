using MassiveGame.Core.ioCore;
using MassiveGame.Core.MathCore.MathTypes;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MassiveGame.Core.ComponentCore
{
    [Serializable]
    public class Component :  
          ISerializable
        , IPostDeserializable
    {
        public enum ComponentType
        {
            SceneComponent
        }

        public Component()
        {
            ChildrenComponents = new List<Component>();
            ComponentTranslation = new Vector3(0);
            ComponentRotation = new Vector3(0);
            ComponentScale = new Vector3(1);
            bTransformationDirty = true;
            ParentComponent = null;
        }

        protected bool bTransformationDirty;

        protected Vector3 componentTranslation;
        protected Vector3 componentRotation;
        protected Vector3 componentScale;

        public ComponentType Type { set; get; }

        public Component ParentComponent { set; get; }

        public List<Component> ChildrenComponents { set; get; }

        // For serialization collision bounds, maybe there is a better way
        public BoundBase Bound { set; get; }

        private object lockObject = new object();

        public Vector3 ComponentTranslation
        {
            set
            {
                componentTranslation = value;
                bTransformationDirty = true;
            }
            get
            {
                return componentTranslation;
            }
        }

        public Vector3 ComponentRotation
        {
            set
            {
                componentRotation = value;
                bTransformationDirty = true;
            }
            get
            {
                return componentRotation;
            }
        }

        public Vector3 ComponentScale
        {
            set
            {
                componentScale = value;
                bTransformationDirty = true;
            }
            get
            {
                return componentScale;
            }
        }

        #region Serialization

        // Called directly after deserialization
        public virtual void PostDeserializeInit() {  }

        protected Component(SerializationInfo info, StreamingContext context)
        {
            bTransformationDirty = true; 
            componentTranslation = (Vector3)info.GetValue("translation", typeof(Vector3));
            componentRotation = (Vector3)info.GetValue("rotation", typeof(Vector3));
            componentScale = (Vector3)info.GetValue("scale", typeof(Vector3));
            Type = (ComponentType)info.GetValue("componentType", typeof(ComponentType));
            ParentComponent = (Component)info.GetValue("parentComponent", typeof(Component));
            ChildrenComponents = (List<Component>)info.GetValue("childrenComponents", typeof(List<Component>));
            Bound = (BoundBase)info.GetValue("bound", typeof(BoundBase));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("translation", componentTranslation, typeof(Vector3));
            info.AddValue("rotation", componentRotation, typeof(Vector3));
            info.AddValue("scale", componentScale, typeof(Vector3));
            info.AddValue("componentType", Type, typeof(ComponentType));
            info.AddValue("parentComponent", ParentComponent, typeof(Component));
            info.AddValue("childrenComponents", ChildrenComponents, typeof(List<Component>));
            info.AddValue("bound", Bound, typeof(BoundBase));
        }

        #endregion

        // Return the base component (parent of all hierarchy)
        public Component GetRootComponent()
        {
            Component component = this;
            while (component.ParentComponent != null)
            {
                component = component.ParentComponent;
            }
            return component;
        }

        public Vector3 GetTotalScale()
        {
            Vector3 totalScale = componentScale;

            Vector3 parentScale = new Vector3(1);
            if (ParentComponent != null)
            {
                parentScale *= ParentComponent.GetTotalScale();
            }
            totalScale *= parentScale;
            return totalScale;
        }

        private void GetHierarchyBoundingBoxes(Component collectableComponent, ref List<BoundBase> collectedBoundingBoxes)
        {
            foreach (Component childComponent in collectableComponent.ChildrenComponents)
            {
                collectedBoundingBoxes.Add(childComponent.Bound);
                GetHierarchyBoundingBoxes(childComponent, ref collectedBoundingBoxes);
            }
        }

        public AABB GetAABBFromAllChildComponents()
        {
            List<BoundBase> childBoundBoxes = new List<BoundBase>();
            GetHierarchyBoundingBoxes(this, ref childBoundBoxes);

            var max = childBoundBoxes[0].GetMax();
            var min = childBoundBoxes[0].GetMin();
            float minX = min.X, minY = min.Y, minZ = min.Z, maxX = max.X, maxY = max.Y, maxZ = max.Z;
            for (Int32 i = 1; i < childBoundBoxes.Count; i++)
            {
                var localMax = childBoundBoxes[i].GetMax();
                var localMin = childBoundBoxes[i].GetMin();
                maxX = Math.Max(maxX, localMax.X);
                maxY = Math.Max(maxY, localMax.Y);
                maxZ = Math.Max(maxZ, localMax.Z);
                minX = Math.Min(minX, localMin.X);
                minY = Math.Min(minY, localMin.Y);
                minZ = Math.Min(minZ, localMin.Z);
            }
            return AABB.CreateFromMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ), this);
        }

        public virtual Matrix4 GetWorldMatrix()
        {
            Matrix4 parentWorldMatrix = Matrix4.Identity;
            if (ParentComponent != null)
            {
                parentWorldMatrix *= ParentComponent.GetWorldMatrix();
            }

            var WorldMatrix = Matrix4.Identity;
            WorldMatrix *= Matrix4.CreateScale(ComponentScale);
            WorldMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(ComponentRotation.X));
            WorldMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ComponentRotation.Y));
            WorldMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(ComponentRotation.Z));
            WorldMatrix *= Matrix4.CreateTranslation(ComponentTranslation);

            WorldMatrix *= parentWorldMatrix;
            return WorldMatrix;
        }

        public ComponentType GetComponentType()
        {
            return Type;
        }

        public virtual void UpdateTransformation()
        {
            UpdateBoundTransformationMatrices(this);
        }

        private void UpdateBoundTransformationMatrices(Component childComponent)
        {
            foreach (var component in childComponent.ChildrenComponents)
            {
                UpdateBoundTransformationMatrices(component);

                // Update bounds transformation
                Matrix4 parentTransformationMatrix = component.GetWorldMatrix();
                Quaternion rotation = parentTransformationMatrix.ExtractRotation(true);
                if (component.Bound.GetBoundType() == BoundBase.BoundType.AABB)
                {
                    if (rotation.Xyz.LengthSquared > 0.01f)
                        component.Bound = new OBB(component.Bound.GetLocalSpaceOrigin(), component.Bound.GetLocalSpaceExtent(), parentTransformationMatrix, component);
                    else
                        (component.Bound as AABB).ScalePlusTranslation = parentTransformationMatrix;
                }
                else
                {
                    if (rotation.Xyz.LengthSquared > 0.01f)
                        (component.Bound as OBB).TransformationMatrix = parentTransformationMatrix;
                    else
                    {
                        component.Bound = new AABB(component.Bound.GetLocalSpaceOrigin(), component.Bound.GetLocalSpaceExtent(), component);
                        (component.Bound as AABB).ScalePlusTranslation = parentTransformationMatrix;
                    }
                }
            }
        }

        public virtual void Tick(float deltaTime)
        {
            lock (lockObject)
            {
                if (bTransformationDirty)
                {
                    UpdateTransformation();
                    bTransformationDirty = false;
                }
                foreach (var component in ChildrenComponents)
                {
                    component.Tick(deltaTime);
                }
            }
        }

        public virtual void RenderBound(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix, Color4 color)
        {
            foreach (var component in ChildrenComponents)
            {
                component.RenderBound(ref projectionMatrix, ref viewMatrix, color);
            }
        }

        public virtual void AttachComponent(Component component)
        {
            lock (lockObject)
            {
                ChildrenComponents.Add(component);
                component.ParentComponent = this;
            }
        }

        public virtual void DetachComponent(Component component)
        {
            lock (lockObject)
            {
                ChildrenComponents.Remove(component);
                component.ParentComponent = null;
            }
        }

    }
}
