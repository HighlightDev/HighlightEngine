using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MassiveGame.API.Factory;
using MassiveGame.Optimization;
using MassiveGame.API.Factory.ObjectArguments;

namespace MassiveGame.API
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
                        creator = new Factory.MovableEntityCreator();
                        break;
                    }
                case EntityType.STATIC_ENTITY:
                    {
                        creator = new Factory.StaticEntityCreator();
                        break;
                    }
            }

            return creator.CreateInstance(a);
        }
    }
}
