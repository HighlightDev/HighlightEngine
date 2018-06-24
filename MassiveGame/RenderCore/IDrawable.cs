using GpuGraphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.RenderCore
{
    public interface IDrawable
    {
        VAO GetMesh();
        ITexture GetDiffuseMap();
        ITexture GetNormalMap();
        ITexture GetSpecularMap();
        Matrix4 GetWorldMatrix();
    }
}
