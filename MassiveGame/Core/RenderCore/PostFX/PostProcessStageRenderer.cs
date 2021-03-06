﻿using MassiveGame.Core.GameCore;
using MassiveGame.Core.RenderCore.PostFX.Bloom;
using MassiveGame.Core.RenderCore.PostFX.DepthOfField;
using MassiveGame.Core.RenderCore.PostFX.LensFlare;
using MassiveGame.Core.RenderCore.PostFX.LightShafts;
using System.Drawing;
using TextureLoader;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.Core.RenderCore.PostFX
{
    public class PostProcessStageRenderer
    {
        private readonly bool bPostProcessEnabled = true;
        private PostProcessBase depthOfFieldPP = null;
        private PostProcessBase bloomPP = null;
        private PostProcessBase lightShaftsPP = null;
        private PostProcessBase lensFlaresPP = null;

        public PostProcessStageRenderer()
        {

            if (EngineStatics.globalSettings.bSupported_DepthOfField)
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

            if (EngineStatics.globalSettings.bSupported_Bloom)
            {
                if (EngineStatics.globalSettings.bSupported_DepthOfField)
                {
                    bloomPP = new BloomPostProcess<ApplySubsequentPostProcessResult>();
                }
                else
                {
                    bloomPP = new BloomPostProcess<DiscardSubsequentPostProcessResult>();
                }
            }

            if (EngineStatics.globalSettings.bSupported_LightShafts)
            {
                if (EngineStatics.globalSettings.bSupported_Bloom || EngineStatics.globalSettings.bSupported_DepthOfField)
                {
                    lightShaftsPP = new LightShaftPostProcess<ApplySubsequentPostProcessResult>();
                }
                else
                {
                    lightShaftsPP = new LightShaftPostProcess<DiscardSubsequentPostProcessResult>();
                }
            }

            if (EngineStatics.globalSettings.bSupported_LensFlare)
            {
                if (EngineStatics.globalSettings.bSupported_LightShafts || EngineStatics.globalSettings.bSupported_Bloom || EngineStatics.globalSettings.bSupported_DepthOfField)
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

            if (bPostProcessEnabled)
            {
                // DoF
                if (depthOfFieldPP != null)
                {
                    subsequentPostProcessResult = depthOfFieldPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, null);
                }

                // Bloom
                if (bloomPP != null)
                {
                    subsequentPostProcessResult = bloomPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, subsequentPostProcessResult);
                }

                // Light shafts
                if (lightShaftsPP != null && GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().IsInCameraView)
                {
                    subsequentPostProcessResult = lightShaftsPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, subsequentPostProcessResult);
                }

                // Lens flares
                if (lensFlaresPP != null && GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().IsInCameraView)
                {
                    subsequentPostProcessResult = lensFlaresPP.GetPostProcessResult(frameColorTexture, frameDepthTexture, actualScreenRezolution, subsequentPostProcessResult);
                }
            }

            // Resolve post process result texture or default color texture to default framebuffer
            if (subsequentPostProcessResult != null)
            {
                if (depthOfFieldPP == null)
                    TextureResolver.ResolvePostProcessResultToDefaultFramebuffer(frameColorTexture, subsequentPostProcessResult, actualScreenRezolution);
                else
                    GameWorld.GetWorldInstance().GetUiFrameCreator().RenderFullScreenInputTexture(subsequentPostProcessResult, actualScreenRezolution);
            }
            else
            {
                GameWorld.GetWorldInstance().GetUiFrameCreator().RenderFullScreenInputTexture(frameColorTexture, actualScreenRezolution);
            }
        }
    }
}
