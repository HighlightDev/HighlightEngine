using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.PostFX
{
    public class PostProcessSettings
    {
        public bool bSupported_Bloom { set; get; } = false;
        public bool bSupported_LightShafts { set; get; } = false;
        public bool bSupported_LensFlare { set; get; } = false;
    }
}
