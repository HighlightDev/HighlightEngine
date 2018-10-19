﻿using OpenTK;
using System;

namespace MassiveGame.Core.GameCore.Entities.MoveEntities
{
    [Serializable]
    public sealed class MovableMeshEntity : MovableEntity
    {
        public MovableMeshEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, Vector3 translation, Vector3 rotation, Vector3 scale)
            : base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        { } 
    }
}
