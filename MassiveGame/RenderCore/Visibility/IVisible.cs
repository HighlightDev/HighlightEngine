﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.Optimization
{
    public interface IVisible
    {
        bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix);
    }
}
