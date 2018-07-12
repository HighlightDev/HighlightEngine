using GpuGraphics;
using OpenTK;
using TextureLoader;

namespace MassiveGame.Core.RenderCore
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
