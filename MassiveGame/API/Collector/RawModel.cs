using System;
using VBO;

namespace MassiveGame.API.Collector
{
    public class RawModel : IDisposable
    {
        private string Key { set; get; }
        public VertexArrayObject Buffer { private set; get; }

        public RawModel(string key)
        {
            Key = key;
            LoadBuffer();
        }

        private void LoadBuffer()
        {
            Buffer = ResourcePool.GetModel(Key);
        }

        public void Dispose()
        {
            ResourcePool.ReleaseModel(Key);
            Key = null;
        }
    }
}
