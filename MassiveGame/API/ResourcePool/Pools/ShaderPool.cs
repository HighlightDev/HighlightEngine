using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class ShaderPool : Pool
    {
        public ShaderPool() { }

        public bool RecompileAllShaders()
        {
            bool bSuccess = false;
           
            for (Int32 i = 0; i < GetResourceCount(); i++)
            {
                var item = resourceMap.ElementAt(i);
                Shader shaderItem = item.Value as Shader;
                bSuccess = shaderItem.RecompileShader();
                if (!bSuccess)
                    break;
            }
            return bSuccess;
        }

        protected override bool IsValidResourceType(object arg)
        {
            return arg is Shader;
        }
    }
}
