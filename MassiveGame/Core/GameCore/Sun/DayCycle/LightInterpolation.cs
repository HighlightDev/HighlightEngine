using OpenTK;

namespace MassiveGame.Core.GameCore.Sun.DayCycle
{
    public static class LightInterpolation
    {
        #region Interpolation

        public static Vector3 InterpolateColor(Vector3 y1, Vector3 y2, float x1, float x2, float x)
        {
            var y = (y1 + (y2 - y1) / (x2 - x1) * (x - x1));
            return y;
        }

        #endregion
    }
}
