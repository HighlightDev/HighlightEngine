using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.ComponentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.CollisionEditor.Core.SerializeAPI
{

    // Try to get rid of SerializedComponentsContainer -> it is unnecessary

    [Serializable]
    public class CollisionComponentsWrapper
    {
        public List<Component> SerializedComponents { set; get; }

        //private void CopyFields(ref SceneComponent Dst, SceneComponent Src)
        //{
        //    Dst.Bound = Src.Bound;
        //    if (Dst.Bound != null)
        //        Dst.Bound.ParentComponent = Dst;
        //    Dst.ComponentTranslation = Src.ComponentTranslation;
        //    Dst.ComponentRotation = Src.ComponentRotation;
        //    Dst.ComponentScale = Src.ComponentScale;
        //    Dst.Type = Src.Type;
        //    foreach (Component destComponent in Src.ChildrenComponents)
        //    {
        //        Component child = new Component();
        //        child.ParentComponent = Dst;
        //        CopyFields(ref child, destComponent);
        //        Dst.ChildrenComponents.Add(child);
        //    }
        //}

        public CollisionComponentsWrapper()
        {
            SerializedComponents = null;
        }

        public CollisionComponentsWrapper(Component component)
        {
            //var SerializableComponent = new SceneComponent();
            //SerializableComponent.ParentComponent = null;
            ////CopyFields(ref SerializableComponent, component);
            foreach (var child in component.ChildrenComponents)
            {
                child.ParentComponent = null;
            }
            SerializedComponents = component.ChildrenComponents;
        }
    }
}
