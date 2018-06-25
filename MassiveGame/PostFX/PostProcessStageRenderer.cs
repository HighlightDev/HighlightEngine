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
using MassiveGame.PostFX.DepthOfField;

namespace MassiveGame.PostFX
{
    public class PostProcessStageRenderer
    {
        private bool bPostProcessEnabled = true;
        private PostProcessBase depthOfFieldPP = null;
        private PostProcessBase bloomPP = null;
        private PostProcessBase lightShaftsPP = null;
        private PostProcessBase lensFlaresPP = null;

        public PostProcessStageRenderer()
        {

            if (DOUEngine.globalSettings.bSupported_DepthOfField)
            {
                //if (DOUEngine.globalSettings.bSupported_LensFlare)
                //{
                //    depthOfFieldPP = new DepthOfFieldPostProcess<ApplySubsequentPostProcessResult>();
                //}
                //else
                {
                    depthOfFieldPP = new DepthOfFieldPostProcess<DiscardSubsequentPostProcessResult>();
                }
            }

            if (DOUEngine.globalSettings.bSupported_Bloom)
            {
                if (DOUEngine.globalSettings.bSupported_DepthOfField)
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
                if (DOUEngine.globalSettings.bSupported_Bloom || DOUEngine.globalSettings.bSupported_DepthOfField)
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
                if (DOUEngine.globalSettings.bSupported_LightShafts || DOUEngine.globalSettings.bSupported_Bloom || DOUEngine.globalSettings.bSupported_DepthOfField)
                {
                    lensFlaresPP = new LensFlarePostProcess<ApplySubsequentPostProcessResult>();
                }
                else
                {
                    lensFlaresPP = new LensFlarePostProcess<DiscardSubsequentPostProcessResult>();
                }
            }
          
        }

        public void ExecutePostProcessPass(ITexture frameColorTexture, ITexture frameDepthTexture, ref Point actualScreenRezolution)
        {
            ITexture subsequentPostProcessResult = null;

            // DoF
            if (bPostProcessEnabled && depthOfFieldPP != null)
            {
                subsequentPostProcessResult = depthOfFieldPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, null);
            }

            // Bloom
            if (bPostProcessEnabled && bloomPP != null)
            {
                subsequentPostProcessResult = bloomPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, subsequentPostProcessResult);
            }

            // Light shafts
            if (bPostProcessEnabled && DOUEngine.SunReplica.IsInCameraView && lightShaftsPP != null)
            {
                subsequentPostProcessResult = lightShaftsPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, subsequentPostProcessResult);
            }

            // Lens flares
            if (bPostProcessEnabled && DOUEngine.SunReplica.IsInCameraView && lensFlaresPP != null)
            {
                subsequentPostProcessResult = lensFlaresPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, subsequentPostProcessResult);
            }

            // Resolve post process result texture or default color texture to default framebuffer
            if (subsequentPostProcessResult != null)
            {
                if (depthOfFieldPP == null)
                    TextureResolver.ResolvePostProcessResultToDefaultFramebuffer(frameColorTexture, subsequentPostProcessResult, actualScreenRezolution);
                else
                    DOUEngine.uiFrameCreator.RenderFullScreenInputTexture(subsequentPostProcessResult, actualScreenRezolution);
            }
            else
            {
                DOUEngine.uiFrameCreator.RenderFullScreenInputTexture(frameColorTexture, actualScreenRezolution);
            }
        }
    }
}
