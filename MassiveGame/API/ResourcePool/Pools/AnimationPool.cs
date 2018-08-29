using MassiveGame.Core.AnimationCore;
using System.Collections.Generic;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class AnimationPool : Pool
    {
        public AnimationPool() { }

        protected override bool IsValidResourceType(object arg)
        {
            return arg is List<AnimationSequence>;
        }
    }
}
