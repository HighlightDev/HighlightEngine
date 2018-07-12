using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.RenderCore.Visibility;

namespace MassiveGame.API.ObjectFactory
{
    public sealed class StaticEntityCreator : ICreator
    {
        public IVisible CreateInstance(Arguments a)
        {
            StaticEntityArguments arg = a as StaticEntityArguments;
            return new Building(arg.ModelPath, arg.TexturePath, arg.NormalMapPath, arg.SpecularMapPath,
                arg.Translation, arg.Rotation, arg.Scale);
        }
    }
}
