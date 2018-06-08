﻿using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using MassiveGame.UI;

namespace MassiveGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            UI.Engine form = new UI.Engine();
            form.Left = DOUEngine.SCREEN_POSITION_X;
            form.Top = DOUEngine.SCREEN_POSITION_Y;
            form.ShowDialog();
        }
    }
}