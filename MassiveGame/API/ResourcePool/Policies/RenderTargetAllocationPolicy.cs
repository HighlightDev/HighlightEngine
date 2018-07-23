using System;
using TextureLoader;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class RenderTargetAllocationPolicy : AllocationPolicy<TextureParameters, ITexture>
    {
        public override ITexture AllocateMemory(TextureParameters arg)
        {
            return RenderTargetAllocator(arg);
        }

        private ITexture RenderTargetAllocator(TextureParameters arg)
        {
            ITexture result = null;
            if (arg.TexTarget == OpenTK.Graphics.OpenGL.TextureTarget.Texture2D)
            {
                result = new Texture2D(arg);
            }
            else if (arg.TexTarget == OpenTK.Graphics.OpenGL.TextureTarget.Texture3D)
            {
                throw new NotImplementedException("Texture 3d not implemented.");
                //result = new Texture3D(arg);
            }
            return result;
        }

        public override void FreeMemory(ITexture arg)
        {
            arg.CleanUp();
        }
    }
}
