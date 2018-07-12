using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace PhysicsBox
{
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
