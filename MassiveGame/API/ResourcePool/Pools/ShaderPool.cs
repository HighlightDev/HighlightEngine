using ShaderPattern;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class ShaderPool : Pool
    {
        public ShaderPool() { }

        protected override bool IsValidResourceType(object arg)
        {
            return arg is Shader;
        }
    }
}
