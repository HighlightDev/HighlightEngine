using MassiveGame.Core.ComponentCore;
using System;
using System.Collections.Generic;

namespace MassiveGame.CollisionEditor.Core.SerializeAPI
{
    [Serializable]
    public class CollisionComponentsWrapper
    {
        public List<Component> SerializedComponents { set; get; }
       
        public CollisionComponentsWrapper()
        {
            SerializedComponents = null;
        }

        public CollisionComponentsWrapper(Component component)
        {
            foreach (var child in component.ChildrenComponents)
            {
                child.ParentComponent = null;
            }
            SerializedComponents = component.ChildrenComponents;
        }
    }
}
