using System;

namespace MassiveGame.API.ObjectFactory.ObjectArguments
{
    public class Arguments : EventArgs
    {
        public string ModelPath;
        public string TexturePath;
        public string NormalMapPath;
        public string SpecularMapPath;
        public EntityType ObjectType { private set; get; }

        public Arguments(EntityType type,
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
