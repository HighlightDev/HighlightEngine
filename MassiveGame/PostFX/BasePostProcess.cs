using GpuGraphics;
using MassiveGame.RenderCore;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.PostFX
{
    public abstract class BasePostProcess
    {
        protected VAO quadBuffer;
        protected bool bPostConstructor;

        public BasePostProcess()
        {
            bPostConstructor = true;
            quadBuffer = ScreenQuad.GetScreenQuadBuffer();
        }

        public virtual ITexture GetPostProcessResult(ITexture frameTexture, Int32 actualScreenWidth, Int32 actualScreenHeight)
        {
            RenderScene(DOUEngine.Camera);
            return null;
        }

        protected virtual void RenderScene(LiteCamera camera)
        {
        }
    }
}
