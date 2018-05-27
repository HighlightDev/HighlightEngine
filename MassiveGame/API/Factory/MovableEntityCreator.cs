using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GpuGraphics;

using MassiveGame.Entities;
using TextureLoader;
using MassiveGame.Optimization;
using MassiveGame.API.Factory.ObjectArguments;

namespace MassiveGame.API.Factory
{
    public sealed class MovableEntityCreator : ICreator
    {
        public IVisible CreateInstance(Arguments a)
        {
            MovableEntityArguments arg = a as MovableEntityArguments;
            return new Player(arg.ModelPath, arg.TexturePath, arg.NormalMapPath, arg.SpecularMapPath
                , arg.Speed, arg.ID, arg.Translation, arg.Rotation, arg.Scale);
        }
    }
}
