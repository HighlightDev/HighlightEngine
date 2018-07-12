using OpenTK;

namespace MassiveGame.Core.RenderCore.Visibility
{
    public interface IVisible
    {
        bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix);
    }
}
