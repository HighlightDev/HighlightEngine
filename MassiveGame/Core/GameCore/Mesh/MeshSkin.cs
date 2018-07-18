using MassiveGame.API.Collector;
using System;
using VBO;

namespace MassiveGame.Core.GameCore.Mesh
{
    public class MeshSkin : IDisposable
    {
        private string Key { set; get; }
        public VertexArrayObject Buffer { private set; get; }

        public MeshSkin(string key)
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
