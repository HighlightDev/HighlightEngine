using CParser;
using CParser.Assimp;
using MassiveGame.Core.AnimationCore;
using System;
using System.Collections.Generic;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class AnimationAllocationPolicy : AllocationPolicy<string, List<AnimationSequence>>
    {
        public override List<AnimationSequence> AllocateMemory(string arg)
        {
            List<AnimationSequence> resultAnimationCollection = null;

            using (AssimpModelLoader loader = new AssimpModelLoader(arg, CParser.Assimp.Strategy.SkeletonPerVertexBoneInfluenceType.ThreeBones))
            {
                if (loader.GetHasAnimationData())
                {
                    MeshAnimationData animationData = loader.GetAnimationData();
                    if (animationData != null)
                    {
                        resultAnimationCollection = AssimpConverter.Converter.ConvertAssimpAnimationToEngineAnimation(animationData.Animations);
                    }
                }
            }

            return resultAnimationCollection;
        }

        public override void FreeMemory(List<AnimationSequence> arg)
        {
            foreach (var sequence in arg)
            {
                sequence.ClearAnimationFrameCollection();
            }
        }
    }
}
