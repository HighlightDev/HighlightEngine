using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationFrame
    {
        private Dictionary<string, BoneTransformation> boneTransformationsMap;

        private float animationTime;

        public AnimationFrame(Dictionary<string, BoneTransformation> boneTransformations, float animTime)
        {
            boneTransformationsMap = boneTransformations;
            animationTime = animTime;
        }
    }
}
