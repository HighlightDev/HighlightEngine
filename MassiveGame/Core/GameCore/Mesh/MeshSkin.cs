using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool.PoolHandling;
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
            Buffer = PoolProxy.GetResource<ObtainModelPool, ModelAllocationPolicy, string, VertexArrayObject>(Key);
        }

        public void Dispose()
        {
            PoolProxy.FreeResourceMemoryByKey<ObtainModelPool, ModelAllocationPolicy, string, VertexArrayObject>(Key);
            Key = null;
        }
    }
}
