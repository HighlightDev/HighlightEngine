using MassiveGame.RenderCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.PostFX
{
    public class PostProcessStage
    {
        private bool bPostProcessEnabled = true;
        private LightShaftPostProcess lightShaftsPP;

        public PostProcessStage()
        {
            // if light shafts enabled

            lightShaftsPP = new LightShaftPostProcess();
        }

        public void ExecutePostProcessPass(ITexture frameTexture, Point actualScreenRezolution)
        {
            ITexture subsequentPostProcessResult = null;

            // Light shafts
            if (bPostProcessEnabled && DOUEngine.SunReplica.IsInCameraView && lightShaftsPP != null)
            {
                subsequentPostProcessResult = lightShaftsPP.GetPostProcessResult(frameTexture, actualScreenRezolution.X, actualScreenRezolution.Y);
            }

            if (subsequentPostProcessResult != null)
            {
                TextureResolver.ResolvePostProcessResultToDefaultFramebuffer(frameTexture, subsequentPostProcessResult, actualScreenRezolution);
            }
            else
            {
                DOUEngine.uiFrameCreator.RenderInputTexture(frameTexture, actualScreenRezolution);
            }
        }
    }
}
