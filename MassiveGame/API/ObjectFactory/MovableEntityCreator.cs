using MassiveGame.API.ObjectFactory.ObjectArguments;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.RenderCore.Visibility;

namespace MassiveGame.API.ObjectFactory
{
    public sealed class MovableEntityCreator : ICreator
    {
        public IVisible CreateInstance(Arguments a)
        {
            MovableEntityArguments arg = a as MovableEntityArguments;
            return new MovableMeshEntity(arg.ModelPath, arg.TexturePath, arg.NormalMapPath, arg.SpecularMapPath
                , arg.Speed, arg.Translation, arg.Rotation, arg.Scale);
        }
    }
}
