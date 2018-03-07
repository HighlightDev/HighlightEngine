using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collector.ModelCollect
{
    public class ModelPool
    {
        private ModelCollection modelCollection;

        public ModelPool()
        {
            modelCollection = new ModelCollection();
        }
        
        public VAO GetModel(string key)
        {
            return modelCollection.RetrieveModel(key);
        }

        public void ReleaseModel(string key)
        {
            this.modelCollection.ReleaseModel(key);
        }
    }
}
