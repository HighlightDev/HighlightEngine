﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Settings
{
    public class GlobalSettings
    {
        //Resolution
        public Point DomainFramebufferRezolution { set; get; } = new Point(800, 600);

        //Shadow
        public Point ShadowMapRezolution { set; get; } = new Point(256, 256);

        // PostProcess
        public bool bSupported_DepthOfField { get; set; } = false;
        public bool bSupported_Bloom { set; get; } = false;
        public bool bSupported_LightShafts { set; get; } = false;
        public bool bSupported_LensFlare { set; get; } = false;

        // Texture
        public bool bSupported_MipMap { set; get; } = false;
        public float AnisotropicFilterValue { set; get; } = 0.0f;
    }
}