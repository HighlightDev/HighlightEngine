using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsBox.MathTypes
{
    [Serializable]
    public class OBB : BoundBase
    {
        public Matrix4 TransformationMatrix;

        public OBB(Vector3 Origin, Vector3 Extent, Matrix4 TransformationMatrix) : base(Origin, Extent)
        {
            this.TransformationMatrix = TransformationMatrix;
            type = BoundType.OBB;
        }
    }
}
