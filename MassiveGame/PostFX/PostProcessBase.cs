using GpuGraphics;
using MassiveGame.RenderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.PostFX
{
    public class PostProcessBase
    {
        protected VAO quadBuffer;
        private ITexture[] Result;

        protected ITexture GetResult()
        {
            return Result[0];
        }

        public ITexture[] GetWrappedPostProcessResult()
        {
            return Result;
        }

        public PostProcessBase()
        {
            Result = new ITexture[1];
           
            quadBuffer = ScreenQuad.GetScreenQuadBuffer();
        }
    }
}
