using GpuGraphics;
using System;

namespace MassiveGame.API.Collector
{
    public class RawModel : IDisposable
    {
        private string Key { set; get; }
        public VAO Buffer { private set; get; }

        public RawModel(string key)
        {
            this.Key = key;
            LoadBuffer();
        }

        private void LoadBuffer()
        {
            this.Buffer = ResourcePool.GetModel(Key);
        }

        public void Dispose()
        {
            ResourcePool.ReleaseModel(Key);
            this.Key = null;
        }
    }
}
