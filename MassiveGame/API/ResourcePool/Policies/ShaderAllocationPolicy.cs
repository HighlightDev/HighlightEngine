using ShaderPattern;
using System;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class ShaderAllocationPolicy<ShaderType> : AllocationPolicy<string, ShaderType>
            where ShaderType : Shader, new()
    {
        public override ShaderType AllocateMemory(string arg)
        {
            return LoadShaderFromFile(arg);
        }

        public override void FreeMemory(ShaderType arg)
        {
            arg.cleanUp();
        }

        private ShaderType LoadShaderFromFile(string compositeKey)
        {
            string[] shaderFiles = compositeKey.Split(',');
            return (ShaderType)Activator.CreateInstance(typeof(ShaderType), shaderFiles);
        }
    }
}