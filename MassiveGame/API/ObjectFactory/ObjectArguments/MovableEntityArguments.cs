using OpenTK;

namespace MassiveGame.API.ObjectFactory.ObjectArguments
{
    public sealed class MovableEntityArguments : Arguments
    {
        public float Speed { private set; get; }
        public Vector3 Translation { private set; get; }
        public Vector3 Rotation { private set; get; }
        public Vector3 Scale { private set; get; }

        public MovableEntityArguments(string modelPath, string texturePath, string normalMapPath, string specularMapPath,
            float speed, Vector3 translation, Vector3 rotation, Vector3 scale)
            : base(EntityType.MOVABLE_ENTITY,  modelPath,  texturePath,  normalMapPath,  specularMapPath)
        {
            Speed = speed;
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
