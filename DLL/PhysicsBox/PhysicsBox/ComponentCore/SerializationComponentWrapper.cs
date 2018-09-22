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

        private void CopyFields(Component Dst, Component Src)
        {
            Dst.Bound = Src.Bound;
            if (Dst.Bound != null)
                Dst.Bound.ParentComponent = Dst;
            Dst.ComponentTranslation = Src.ComponentTranslation;
            Dst.ComponentRotation = Src.ComponentRotation;
            Dst.ComponentScale = Src.ComponentScale;
            Dst.Type = Src.Type;
            foreach (Component destComponent in Src.ChildrenComponents)
            {
                Component child = new Component();
                child.ParentComponent = Dst;
                CopyFields(child, destComponent);
                Dst.ChildrenComponents.Add(child);
            }
        }

        public SerializationComponentWrapper(Component component)
        {
            SerializableComponent = new Component();
            SerializableComponent.ParentComponent = null;
            CopyFields(SerializableComponent, component);
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
