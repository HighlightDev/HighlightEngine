using System;
using VBO;

namespace MassiveGame.API.Collector.ModelCollect
{
    public class ModelPool
    {
        private ModelCollection modelCollection;

        public ModelPool()
        {
            modelCollection = new ModelCollection();
        }
        
        public VertexArrayObject GetModel(string key)
        {
            return modelCollection.RetrieveModel(key);
        }

        public void ReleaseModel(string key)
        {
            this.modelCollection.ReleaseModel(key);
        }

        public void AddModelToRoot(VertexArrayObject modelBuffer, string key)
        {
            modelCollection.AddModelToRoot(modelBuffer, key);
        }

        public Int32 GetModelReferenceCount(string key)
        {
            return modelCollection.GetModelReferenceCount(key);
        }
    }
}
