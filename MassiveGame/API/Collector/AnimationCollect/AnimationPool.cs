using System;
using VBO;

namespace MassiveGame.API.Collector.AnimationCollect
{
    public class AnimationPool
    {
        private AnimationCollection modelCollection;

        public AnimationPool()
        {
            modelCollection = new AnimationCollection();
        }
        
        public object GetAnimation(string key)
        {
            return modelCollection.RetrieveAnimation(key);
        }

        public void ReleaseAnimation(string key)
        {
            this.modelCollection.ReleaseAnimation(key);
        }

        public Int32 GetAnimationReferenceCount(string key)
        {
            return modelCollection.GetAnimationReferenceCount(key);
        }
    }
}
