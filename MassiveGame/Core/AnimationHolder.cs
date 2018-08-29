using System;
using MassiveGame.Core.AnimationCore;
using System.Collections.Generic;
using OpenTK;

namespace MassiveGame.Core
{
    public class AnimationHolder
    {
        private List<AnimationSequence> m_animationSequences;
        private AnimationSequence m_currentSuquence;

        private List<BoneTransformation> m_cachedBoneTransformation;

        private double m_sequenceloopTime;
        private double m_previousFrameTime;

        private bool bSequenceDirty = true;

        public AnimationHolder(List<AnimationSequence> animationSequences)
        {
            m_animationSequences = animationSequences;
            m_currentSuquence = null;
            m_sequenceloopTime = 0.0;
            m_cachedBoneTransformation = new List<BoneTransformation>();
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

        public AnimationSequence GetCurrentSequence()
        {
            return m_currentSuquence;
        }

        public List<BoneTransformation> GetAnimatedPoseTransformsList()
        {
            // BLEND BONE MATRICES WITHIN CURRENT ANIMATION FRAME AND RETURN THEM

            // Update animation pose only if sequence loop was updated
            if (bSequenceDirty)
            {
                m_cachedBoneTransformation.Clear();
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
                    m_cachedBoneTransformation.Add(lerpedTransform);
                }
            }

            return m_cachedBoneTransformation;
        }

        public List<Matrix4> GetAnimatedPoseMatricesList()
        {
            List<Matrix4> resultAnimatedPoseMatrices = new List<Matrix4>();

            foreach (var boneTransformation in m_cachedBoneTransformation)
            {
                resultAnimatedPoseMatrices.Add(boneTransformation.GetLocalOffsetMatrix());
            }

            return resultAnimatedPoseMatrices;
        }
    }
}
