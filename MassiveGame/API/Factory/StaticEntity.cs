using MassiveGame.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GpuGraphics;
using OpenTK;
using TextureLoader;
using MassiveGame.API.Factory.ObjectArguments;

namespace MassiveGame.API.Factory
{
    public sealed class StaticEntity : ICreator
    {
        public IVisible CreateInstance(Arguments a)
        {
            StaticEntityArguments arg = a as StaticEntityArguments;
            return new Building(arg.ModelPath, arg.TexturePath, arg.NormalMapPath, arg.SpecularMapPath,
                arg.Translation, arg.Rotation, arg.Scale);
        }
    }
}
