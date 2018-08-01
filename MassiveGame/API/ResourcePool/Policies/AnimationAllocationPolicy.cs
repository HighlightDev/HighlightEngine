using MassiveGame.Core.AnimationCore;
using System;
using System.Collections.Generic;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class AnimationAllocationPolicy : AllocationPolicy<string, List<AnimationSequence>>
    {
        public override List<AnimationSequence> AllocateMemory(string arg)
        {
            throw new NotImplementedException();
        }

        public override void FreeMemory(List<AnimationSequence> arg)
        {
            throw new NotImplementedException();
        }
    }
}
