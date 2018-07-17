using OpenTK;
using TextureLoader;
using VBO;

namespace MassiveGame.Core.RenderCore
{
    public interface IDrawable
    {
        VertexArrayObject GetMeshVao();
        ITexture GetDiffuseMap();
        ITexture GetNormalMap();
        ITexture GetSpecularMap();
        Matrix4 GetWorldMatrix();
    }
}
