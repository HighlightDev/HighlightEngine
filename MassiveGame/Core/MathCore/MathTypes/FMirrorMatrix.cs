using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace MassiveGame.Core.MathCore.MathTypes
{
    public class FMirrorMatrix
    {
        private Matrix4 InMatrix;

        public static implicit operator Matrix4(FMirrorMatrix m)
        {
            return m.InMatrix;
        }

        public FMirrorMatrix(FPlane plane)
        {
            InMatrix = new Matrix4(
                new Vector4(-2f * plane.X * plane.X + 1f, -2f * plane.Y * plane.X, -2f * plane.Z * plane.X, 0.0f).Normalized(),
                new Vector4(-2f * plane.X * plane.Y, -2f * plane.Y * plane.Y + 1f, -2f * plane.Z * plane.Y, 0.0f).Normalized(),
                new Vector4(-2f * plane.X * plane.Z, -2f * plane.Y * plane.Z, -2f * plane.Z * plane.Z + 1f, 0.0f).Normalized(),
                new Vector4(2f * plane.X * plane.D, 2f * plane.Y * plane.D, 2f * plane.Z * plane.D, 1.0f)
                );
        }
    }
}
