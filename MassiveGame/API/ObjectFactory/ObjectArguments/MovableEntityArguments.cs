using OpenTK;

namespace MassiveGame.API.ObjectFactory.ObjectArguments
{
    public sealed class MovableEntityArguments : Arguments
    {
        public Vector3 Translation { private set; get; }
        public Vector3 Rotation { private set; get; }
        public Vector3 Scale { private set; get; }

        public MovableEntityArguments(string modelPath, string texturePath, string normalMapPath, string specularMapPath, Vector3 translation, Vector3 rotation, Vector3 scale)
            : base(EntityType.MOVABLE_ENTITY,  modelPath,  texturePath,  normalMapPath,  specularMapPath)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
