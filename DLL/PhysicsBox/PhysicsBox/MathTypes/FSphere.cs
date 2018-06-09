using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.MathTypes
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
