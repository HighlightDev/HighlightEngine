using OpenTK;
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

        // If transformation was changed
        protected bool bTransformationDirty = true;

        private Vector3 translation;
        private Vector3 rotation;
        private Vector3 scale;

        public Vector3 Translation
        {
            set
            {
                translation = value;
                bTransformationDirty = true;
            }
            get
            {
                return translation;
            }
        }

        public Vector3 Rotation
        {
            set
            {
                rotation = value;
                bTransformationDirty = true;
            }
            get
            {
                return rotation;
            }
        }

        public Vector3 Scale
        {
            set
            {
                scale = value;
                bTransformationDirty = true;
            }
            get
            {
                return scale;
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

        public Matrix4 GetWorldMatrix()
        {
            Matrix4 parentWorldMatrix = Matrix4.Identity;
            if (ParentComponent != null)
            {
                parentWorldMatrix *= ParentComponent.GetWorldMatrix();
            }

            var WorldMatrix = Matrix4.Identity;
            WorldMatrix *= Matrix4.CreateScale(Scale);
            WorldMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
            WorldMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
            WorldMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
            WorldMatrix *= Matrix4.CreateTranslation(Translation);

            WorldMatrix *= parentWorldMatrix;
            return WorldMatrix;
        }

        public ComponentType GetComponentType()
        {
            return Type;
        }

        public void UpdateTransformation()
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
                        component.Bound = new OBB(component.Bound.Origin, component.Bound.Extent, parentTransformationMatrix);
                    else
                        (component.Bound as AABB).ScalePlusTranslation = parentTransformationMatrix;
                }
                else
                {
                    if (rotation.Xyz.LengthSquared > 0.01)
                    {
                        component.Bound = new AABB(component.Bound.Origin, component.Bound.Extent);
                        (component.Bound as AABB).ScalePlusTranslation = parentTransformationMatrix;
                    }
                    else
                        (component.Bound as OBB).TransformationMatrix = parentTransformationMatrix;
                }
            }
        }

        public virtual void Tick(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (bTransformationDirty)
            {
                UpdateTransformation();
                bTransformationDirty = false;
            }
            lock (this)
            {
                foreach (var component in ChildrenComponents)
                {
                    component.Tick(viewMatrix, projectionMatrix);
                }
            }
        }

        public Component()
        {
            ChildrenComponents = new List<Component>();
            Translation = new Vector3(0);
            Rotation = new Vector3(0);
            Scale = new Vector3(1);
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
