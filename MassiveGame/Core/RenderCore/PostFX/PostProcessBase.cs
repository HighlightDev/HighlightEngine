using GpuGraphics;
using MassiveGame.Core.GameCore;
using System;
using System.Drawing;
using TextureLoader;

namespace MassiveGame.Core.RenderCore.PostFX
{
    public abstract class PostProcessBase
    {
        public const int BLUR_MAX_PASS_COUNT = 20;
        public const int BLUR_MIN_PASS_COUNT = 1;

        public const Int32 MAX_BLUR_WIDTH = 10;
        public const Int32 MIN_BLUR_WIDTH = 2;

        protected bool blurWidthChanged;
        protected float[] blurWeights;

        private Int32 blurWidth;
        public Int32 BlurWidth
        {
            set
            {
                blurWidth = value < MIN_BLUR_WIDTH ? MIN_BLUR_WIDTH :
                    value > MAX_BLUR_WIDTH ? MAX_BLUR_WIDTH : value;
                this.blurWidthChanged = true;
            }
            get { return blurWidth; }
        }

        private int blurPassCount;
        public int BlurPassCount
        {
            set { blurPassCount = value < BLUR_MIN_PASS_COUNT ? BLUR_MIN_PASS_COUNT : value > BLUR_MAX_PASS_COUNT ? BLUR_MAX_PASS_COUNT : value; }
            get { return blurPassCount; }
        }

        protected VAO quadBuffer;
        protected bool bPostConstructor;

        public PostProcessBase()
        {
            blurWidthChanged = true;
            bPostConstructor = true;
            quadBuffer = ScreenQuad.GetScreenQuadBuffer();
        }

        #region Blur_functions

        protected float gaussFunction(float x, float sigma)
        {
            float pi = Convert.ToSingle(Math.PI);
            float e = Convert.ToSingle(Math.E);
            float power = -((x * x) / (2 * (sigma * sigma)));
            float oneDimentionGauss = (1.0f / (Convert.ToSingle(Math.Sqrt(2 * pi * (sigma * sigma))))) * Convert.ToSingle(Math.Pow(e, power));
            return oneDimentionGauss;
        }

        protected float[] normalizedWeights(Int32 BlurWidth) 
        {
            float[] weights = new float[BlurWidth];
            float sum, sigma2 = 4.0f;
            // Compute and sum the weights
            weights[0] = gaussFunction(0, sigma2); // The 1-D Gaussian function
            sum = weights[0];
            for (int i = 1; i < weights.Length; i++)
            {
                weights[i] = gaussFunction(i, sigma2);
                sum += 2 * weights[i];
            }
            // Normalize the weights and set the uniform
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = weights[i] / sum;
            }
            return weights;
        }

        protected int[] getPixOffset(Int32 BlurWidth)
        {
            Int32[] pixOffset = new Int32[BlurWidth];
            for (int i = 0; i < pixOffset.Length; i++)
            {
                pixOffset[i] = i;
            }
            return pixOffset;
        }

        #endregion

        public virtual ITexture GetPostProcessResult(ITexture frameColorTexture, ITexture frameDepthTexture, Point actualScreenRezolution, ITexture previousPostProcessResult = null)
        {
            RenderScene(EngineStatics.Camera);
            return null;
        }

        protected virtual void RenderScene(BaseCamera camera)
        {
        }

        public abstract void CleanUp();
    }
}
