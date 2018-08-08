using MassiveGame.Core.AnimationCore;
using System.Collections.Generic;

namespace MassiveGame.Core
{
    public class AnimationHolder
    {
        private List<AnimationSequence> m_animationSequences;
        private AnimationSequence m_currentSuquence;
        private double m_loopSecondsTime;

        public AnimationHolder(List<AnimationSequence> animationSequences)
        {
            m_animationSequences = animationSequences;
            m_currentSuquence = null;
            m_loopSecondsTime = 0.0;
        }

        public void SetAnimationByNameNoBlend(string animationName)
        {
            m_currentSuquence = m_animationSequences.Find(new System.Predicate<AnimationSequence>(predicate => predicate.GetName() == animationName));
            m_loopSecondsTime = 0.0;
        }

        public void UpdateAnimationLoopTime(float deltaTime)
        {
            if (m_currentSuquence != null)
            {
                m_loopSecondsTime += deltaTime;
                m_loopSecondsTime %= m_currentSuquence.GetAnimationDurationInSec();
            }
        }

        public List<BoneTransformation> GetAnimationSkinningTransformations()
        {
            // BLEND BONE MATRICES WITHIN CURRENT ANIMATION FRAME AND RETURN THEM
            return null;
        }
    }
}
