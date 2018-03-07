using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.API.Factory.ObjectArguments
{
    public class Arguments : EventArgs
    {
        public string ModelPath;
        public string TexturePath;
        public string NormalMapPath;
        public string SpecularMapPath;
        public EngineObjectType ObjectType { private set; get; }

        public Arguments(EngineObjectType type,
            string modelPath, string texturePath, string normalMapPath, string specularMapPath)
        {
            ObjectType = type;
            ModelPath = modelPath;
            TexturePath = texturePath;
            NormalMapPath = normalMapPath;
            SpecularMapPath = specularMapPath;
        }
    }
}
