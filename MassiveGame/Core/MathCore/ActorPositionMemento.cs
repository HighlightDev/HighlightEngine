using System;

using OpenTK;

namespace MassiveGame.Core.MathCore
{
    [Serializable]
    public struct ActorPositionMemento
    {
        private Vector3 savedOffset;

        public void SetSavedOffset(Vector3 offset)
        {
            savedOffset = offset;
        }

        public Vector3 GetSavedOffset()
        {
            return savedOffset;
        }
    }
}
