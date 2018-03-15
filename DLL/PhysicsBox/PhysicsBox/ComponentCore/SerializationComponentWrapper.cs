using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.ComponentCore
{
    public class SerializationComponentWrapper
    {
        public Component SerializableComponent { set; get; }

        private void CopyFields(Component Src, Component Dest)
        {
            Src.Bound = Dest.Bound;
            Src.Translation = Dest.Translation;
            Src.Rotation = Dest.Rotation;
            Src.Scale = Dest.Scale;
            Src.Type = Dest.Type;
            foreach (Component destComponent in Dest.ChildrenComponents)
            {
                Component children = new Component();
                children.ParentComponent = Src;
                CopyFields(children, destComponent);
                Src.ChildrenComponents.Add(children);
            }
        }

        public SerializationComponentWrapper(Component component)
        {
            SerializableComponent = new Component();
            SerializableComponent.ParentComponent = null;
            CopyFields(SerializableComponent, component );
        }
    }

    [Serializable]
    public class SerializedComponentsContainer
    {
        public List<Component> SerializedComponents { set; get; }

        public SerializedComponentsContainer(SerializationComponentWrapper wrapper)
        {
            SerializedComponents = wrapper.SerializableComponent.ChildrenComponents;
        }

        public SerializedComponentsContainer()
        {
            SerializedComponents = null;
        }
    }
}
