using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Sun.DayCycle
{
    public abstract class Phase
    {
        public Vector3 AmbientLight { set; get; }
        public Vector3 DiffuseLight { set; get; }
        public Vector3 SpecularLight { set; get; }

        public Phase(Vector3 ambientLight, Vector3 diffuseLight, Vector3 specularLight)
        {
            this.AmbientLight = ambientLight;
            this.DiffuseLight = diffuseLight;
            this.SpecularLight = specularLight;
        }
    }
}
