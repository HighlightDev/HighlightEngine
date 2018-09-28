using OpenTK;

namespace MassiveGame.Core.MathCore.MathTypes
{
    public class FSphere
    {
        public Vector3 Origin { set; get; }
        public float Radius { set; get; }

        public FSphere(Vector3 origin, float radius)
        {
            Origin = origin;
            Radius = radius;
        }

    }
}
