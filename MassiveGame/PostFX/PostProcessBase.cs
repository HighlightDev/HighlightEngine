﻿using GpuGraphics;
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
    public abstract class PostProcessBase
    {
        protected VAO quadBuffer;
        protected bool bPostConstructor;

        public PostProcessBase()
        {
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
