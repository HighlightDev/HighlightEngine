using PhysicsBox.ComponentCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollisionEditor.Core
{
    public static class ComponentCreator
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

        public static void ClearRoot()
        {
            ComponentMap.Clear();
            IdCounter = 0;
        }
    }
}
