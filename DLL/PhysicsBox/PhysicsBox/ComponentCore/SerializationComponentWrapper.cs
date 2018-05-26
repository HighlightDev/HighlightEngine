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
            if (Src.Bound != null)
                Src.Bound.ParentComponent = Src;
            Src.ComponentTranslation = Dest.ComponentTranslation;
            Src.ComponentRotation = Dest.ComponentRotation;
            Src.ComponentScale = Dest.ComponentScale;
            Src.Type = Dest.Type;
            foreach (Component destComponent in Dest.ChildrenComponents)
            {
                Component child = new Component();
                child.ParentComponent = Src;
                CopyFields(child, destComponent);
                Src.ChildrenComponents.Add(child);
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
