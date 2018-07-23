using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationFrame
    {
        private List<BoneTransformation> frameBoneTransformations;

        private float animationTime;

        public AnimationFrame(List<BoneTransformation> boneTransformations, float animTime)
        {
            frameBoneTransformations = boneTransformations;
            animationTime = animTime;
        }
    }
}
