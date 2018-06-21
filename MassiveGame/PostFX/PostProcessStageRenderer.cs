using MassiveGame.RenderCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

using MassiveGame.PostFX.Bloom;
using MassiveGame.PostFX.LensFlare;
using MassiveGame.PostFX.LightShafts;

namespace MassiveGame.PostFX
{
    public class PostProcessStageRenderer
    {
        private bool bPostProcessEnabled = true;
        private PostProcessBase bloomPP = null;
        private PostProcessBase lightShaftsPP = null;
        private PostProcessBase lensFlaresPP = null;

        public PostProcessStageRenderer()
        {
            if (DOUEngine.globalSettings.bSupported_Bloom)
            {
                if (false)
                {
                    bloomPP = new BloomPostProcess<ApplySubsequentPostProcessResult>();
                }
                else
                {
                    bloomPP = new BloomPostProcess<DiscardSubsequentPostProcessResult>();
                }
            }

            if (DOUEngine.globalSettings.bSupported_LightShafts)
            {
                if (DOUEngine.globalSettings.bSupported_Bloom)
                {
                    lightShaftsPP = new LightShaftPostProcess<ApplySubsequentPostProcessResult>();
                }
                else
                {
                    lightShaftsPP = new LightShaftPostProcess<DiscardSubsequentPostProcessResult>();
                }
            }

            if (DOUEngine.globalSettings.bSupported_LensFlare)
            {
                if (DOUEngine.globalSettings.bSupported_LightShafts)
                {
                    lensFlaresPP = new LensFlarePostProcess<ApplySubsequentPostProcessResult>();
                }
                else
                {
                    lensFlaresPP = new LensFlarePostProcess<DiscardSubsequentPostProcessResult>();
                }
            }
        }

        public void ExecutePostProcessPass(ITexture frameTexture, ref Point actualScreenRezolution)
        {
            ITexture subsequentPostProcessResult = null;

            // Bloom
            if (bPostProcessEnabled && bloomPP != null)
            {
                subsequentPostProcessResult = bloomPP.GetPostProcessResult(frameTexture, actualScreenRezolution, subsequentPostProcessResult);
            }

            // Light shafts
            if (bPostProcessEnabled && DOUEngine.SunReplica.IsInCameraView && lightShaftsPP != null)
            {
                subsequentPostProcessResult = lightShaftsPP.GetPostProcessResult(frameTexture, actualScreenRezolution, subsequentPostProcessResult);
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
                DOUEngine.uiFrameCreator.RenderFullScreenInputTexture(frameTexture, actualScreenRezolution);
            }
        }
    }
}
