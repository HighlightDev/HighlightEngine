using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collector.ShaderCollect
{
    public class ShaderPool
    {
        private ShaderCollection shaderCollection;

        public ShaderPool()
        {
            shaderCollection = new ShaderCollection();
        }

        public Shader GetShader(string key, ConstructorInfo ctor)
        {
            return shaderCollection.RetrieveShader(key, ctor);
        }

        public void ReleaseShader(string key)
        {
            this.shaderCollection.ReleaseShader(key);
        }

        public void ReleaseShader(Shader shader)
        {
            this.shaderCollection.ReleaseShader(shader);
        }
    }
}
