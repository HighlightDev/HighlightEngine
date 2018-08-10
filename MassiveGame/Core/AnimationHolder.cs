using System;
using MassiveGame.Core.AnimationCore;
using System.Collections.Generic;

namespace MassiveGame.Core
{
    public class AnimationHolder
    {
        private List<AnimationSequence> m_animationSequences;
        private AnimationSequence m_currentSuquence;

        private double m_sequenceloopTime;
        private double m_previousFrameTime;

        public AnimationHolder(List<AnimationSequence> animationSequences)
        {
            m_animationSequences = animationSequences;
            m_currentSuquence = null;
            m_sequenceloopTime = 0.0;
        }

        public void SetAnimationByNameNoBlend(string animationName)
        {
            m_currentSuquence = m_animationSequences.Find(new System.Predicate<AnimationSequence>(predicate => predicate.GetName() == animationName));
            m_sequenceloopTime = 0.0;
        }

        public void UpdateAnimationLoopTime(float deltaTime)
        {
            if (m_currentSuquence != null)
            {
                m_sequenceloopTime += deltaTime;
                m_sequenceloopTime %= m_currentSuquence.GetAnimationDurationInSec();
            }
        }

        public List<BoneTransformation> GetAnimationSkinningTransformations()
        {
            // BLEND BONE MATRICES WITHIN CURRENT ANIMATION FRAME AND RETURN THEM
            List<BoneTransformation> resultBoneTransformations = new List<BoneTransformation>();

            var nextAndPrevFrames = m_currentSuquence.GetNextAndPrevFrames(m_sequenceloopTime);
            Int32 bonesCount = nextAndPrevFrames.Item1.Count;

            for (Int32 i = 0; i < bonesCount; i++)
            {
                BoneTransformation nextTransform = nextAndPrevFrames.Item1[i].Item2;
                BoneTransformation prevTransform = nextAndPrevFrames.Item2[i].Item2;

                double nextTransformTime = nextAndPrevFrames.Item1[i].Item1;
                double prevTransformTime = nextAndPrevFrames.Item2[i].Item1;

                m_previousFrameTime = prevTransformTime;

                float blendFactor = (float)((m_sequenceloopTime - prevTransformTime) / (nextTransformTime - prevTransformTime));

                BoneTransformation lerpedTransform = BoneTransformation.SLerp(prevTransform, nextTransform, blendFactor);
                resultBoneTransformations.Add(lerpedTransform);
            }

            return resultBoneTransformations;
        }
    }
}
