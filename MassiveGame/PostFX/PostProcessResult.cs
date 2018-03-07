
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.PostFX
{
    public class PostProcessResult 
    {
        private ITexture[] Texture;
        public ITexture GetResult()
        {
            return Texture[0];
        }

        public PostProcessResult(PostProcessBase postProcessBase)
        {
            Texture = postProcessBase.GetWrappedPostProcessResult(); 
        }
    }
}
