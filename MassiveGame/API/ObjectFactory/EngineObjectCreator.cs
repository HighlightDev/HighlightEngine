using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.Core.RenderCore.Visibility;

namespace MassiveGame.API.ObjectFactory
{
    public static class EngineObjectCreator
    {
        public static IVisible CreateInstance(Arguments a)
        {
            ICreator creator = null;
            switch (a.ObjectType)
            {
                case EntityType.MOVABLE_ENTITY:
                    {
                        creator = new ObjectFactory.MovableEntityCreator();
                        break;
                    }
                case EntityType.STATIC_ENTITY:
                    {
                        creator = new ObjectFactory.StaticEntityCreator();
                        break;
                    }
            }

            return creator.CreateInstance(a);
        }
    }
}
