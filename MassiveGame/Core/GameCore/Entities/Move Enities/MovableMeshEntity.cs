using OpenTK;

namespace MassiveGame.Core.GameCore.Entities.MoveEntities
{
    public sealed class MovableMeshEntity : MovableEntity
    {
        public MovableMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, Vector3 translation, Vector3 rotation, Vector3 scale)
            : base(modelPath, texturePath, normalMapPath, specularMapPath, Speed, translation, rotation, scale)
        { } 
    }
}
