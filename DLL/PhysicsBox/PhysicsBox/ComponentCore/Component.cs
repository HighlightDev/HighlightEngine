using OpenTK;
using OpenTK.Graphics;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.ComponentCore
{
    [Serializable]
    public class Component
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

        // If transformation was changed
        protected bool bTransformationDirty;

        protected Vector3 componentTranslation;
        protected Vector3 componentRotation;
        protected Vector3 componentScale;

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

        public ComponentType Type { set; get; }

        public Component ParentComponent { set; get; }

        public List<Component> ChildrenComponents { set; get; }

        // For serialization collision bounds, maybe there is a better way
        public BoundBase Bound { set; get; }

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
                    if (rotation.Xyz.LengthSquared > 0.01)
                        component.Bound = new OBB(component.Bound.GetLocalSpaceOrigin(), component.Bound.GetLocalSpaceExtent(), parentTransformationMatrix, component);
                    else
                        (component.Bound as AABB).ScalePlusTranslation = parentTransformationMatrix;
                }
                else
                {
                    if (rotation.Xyz.LengthSquared > 0.01)
                        (component.Bound as OBB).TransformationMatrix = parentTransformationMatrix;
                    else
                    {
                        component.Bound = new AABB(component.Bound.GetLocalSpaceOrigin(), component.Bound.GetLocalSpaceExtent(), component);
                        (component.Bound as AABB).ScalePlusTranslation = parentTransformationMatrix;
                    }
                }
            }
        }

        public virtual void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            lock (this)
            {
                if (bTransformationDirty)
                {
                    UpdateTransformation();
                    bTransformationDirty = false;
                }
                foreach (var component in ChildrenComponents)
                {
                    component.Tick(ref projectionMatrix, ref viewMatrix);
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
            lock (this)
            {
                ChildrenComponents.Add(component);
                component.ParentComponent = this;
            }
        }

        public virtual void DetachComponent(Component component)
        {
            lock (this)
            {
                ChildrenComponents.Remove(component);
                component.ParentComponent = null;
            }
        }
    }
}
