using OpenTK;
using System;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore.Entities.MoveEntities
{
    [Serializable]
    public class MovableMeshEntity : MovableEntity
    {
        protected MovableMeshEntity(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public MovableMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, Vector3 translation, Vector3 rotation, Vector3 scale)
            : base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        { }
    }
}
