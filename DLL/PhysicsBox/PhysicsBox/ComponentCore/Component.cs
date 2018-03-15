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

        public Vector3 Translation { set; get; }

        public Vector3 Rotation { set; get; }

        public Vector3 Scale { set; get; }

        public ComponentType Type { set; get; }

        public Component ParentComponent { set; get; }

        public List<Component> ChildrenComponents { set; get; }

        // For serialization collision bounds, maybe there is a better way
        public BoundBase Bound { set; get; }

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

        public virtual void Tick(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
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
