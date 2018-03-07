using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTKWinForm.Core
{
    public abstract class Component
    {
        public class ComponentCreator
        {
            public static Dictionary<UInt32, Component> ComponentMap;
            private static UInt32 IdCounter;

            static ComponentCreator()
            {
                IdCounter = 0;
                ComponentMap = new Dictionary<uint, Component>();
            }

            public static Component GetComponentById(UInt32 id)
            {
                return ComponentMap[id];
            }

            public static UInt32 GetIdByComponent(Component component)
            {
                UInt32 Id = ComponentMap.ElementAt(ComponentMap.Values.ToList<Component>().IndexOf(component)).Key;
                return Id;
            }

            public static void AddComponentToRoot(Component createdComponent)
            {
                ComponentMap.Add(IdCounter, createdComponent);
                ++IdCounter;
            }

            public static void RemoveComponentFromRoot(Component component)
            {
                ComponentMap.Remove(ComponentMap.ElementAt(ComponentMap.Values.ToList<Component>().IndexOf(component)).Key);
            }
        }

        protected List<Component> childrenComponents;

        public Component parentComponent;
        public Actor parentActor;

        public Vector3 Translation { set; get; }
        public Vector3 Rotation { set; get; }
        public Vector3 Scale { set; get; }

        public List<Component> GetComponents()
        {
            return childrenComponents;
        }

        public Matrix4 GetWorldMatrix()
        {
            Matrix4 resultWorldMatrix = Matrix4.Identity;
            if (parentComponent != null)
            {
                 resultWorldMatrix *= parentComponent.GetWorldMatrix();
            }
            else if (parentActor != null)
            {
                resultWorldMatrix *= parentActor.WorldMatrix;
            }
            var WorldMatrix = Matrix4.Identity;
            WorldMatrix *= Matrix4.CreateScale(Scale);
            WorldMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
            WorldMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
            WorldMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
            WorldMatrix *= Matrix4.CreateTranslation(Translation);

            WorldMatrix *= resultWorldMatrix;
            return WorldMatrix;
        }

        public virtual void Tick(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            foreach (var component in childrenComponents)
            {
                component.Tick(viewMatrix, projectionMatrix);
            }
        }

        public Component()
        {
            ComponentCreator.AddComponentToRoot(this);
            childrenComponents = new List<Component>();
            Translation = new Vector3(0);
            Rotation = new Vector3(0);
            Scale = new Vector3(1);
        }

        public virtual void AttachComponent(Component component)
        {
            childrenComponents.Add(component);
            component.parentComponent = this;
        }

        public virtual void DetachComponent(Component component)
        {
            childrenComponents.Remove(component);
            component.parentComponent = null;
        }

        public override string ToString()
        {
            return "Component_" + ComponentCreator.GetIdByComponent(this).ToString();
        }
    }
}
