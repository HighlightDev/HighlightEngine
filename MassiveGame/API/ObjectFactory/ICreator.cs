using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.Core.RenderCore.Visibility;

namespace MassiveGame.API.ObjectFactory
{
    public interface ICreator
    {
        IVisible CreateInstance(Arguments a);
    }
}
