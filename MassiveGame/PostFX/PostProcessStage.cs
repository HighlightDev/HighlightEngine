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
        private PostProcessBase lightShaftsPP = null;
        private PostProcessBase lensFlaresPP = null;

        public PostProcessStage()
        {
            // if light shafts enabled

            //lensFlaresPP = new LensFlarePostProcess();
            if (false)
            {
                lightShaftsPP = new LightShaftPostProcess<ApplySubsequentPostProcessResult>();
            }
            else
            {
                lightShaftsPP = new LightShaftPostProcess<DiscardSubsequentPostProcessResult>();
            }

            if (DOUEngine.postProcessSettings.bEnable_LightShafts)
            {
                lensFlaresPP = new LensFlarePostProcess<ApplySubsequentPostProcessResult>();
            }
            else
            {
                lensFlaresPP = new LensFlarePostProcess<DiscardSubsequentPostProcessResult>();
            }
        }

        public void ExecutePostProcessPass(ITexture frameTexture, Point actualScreenRezolution)
        {
            ITexture subsequentPostProcessResult = null;

            // Light shafts
            if (bPostProcessEnabled && DOUEngine.SunReplica.IsInCameraView && lightShaftsPP != null)
            {
                subsequentPostProcessResult = lightShaftsPP.GetPostProcessResult(frameTexture, actualScreenRezolution);
            }

            // Lens flares
            if (bPostProcessEnabled && DOUEngine.SunReplica.IsInCameraView && lensFlaresPP != null)
            {
                subsequentPostProcessResult = lensFlaresPP.GetPostProcessResult(frameTexture, actualScreenRezolution, subsequentPostProcessResult);
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
