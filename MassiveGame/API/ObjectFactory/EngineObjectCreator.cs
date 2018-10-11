using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.Core.RenderCore.Visibility;

namespace MassiveGame.API.ObjectFactory
{
    public static class EngineObjectCreator
    {
        public static T CreateInstance<T>(Arguments a) where T : IVisible
        {
            ICreator creator = null;
            switch (a.ObjectType)
            {
                case EntityType.MOVABLE_ENTITY:
                    {
                        creator = new MovableEntityCreator();
                        break;
                    }
                case EntityType.STATIC_ENTITY:
                    {
                        creator = new StaticEntityCreator();
                        break;
                    }
            }

            return (T)creator.CreateInstance(a);
        }
    }
}
