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

        private List<BoneTransform> m_cachedBoneTransforms;

        private double m_sequenceloopTime;
        private double m_previousFrameTime;

        private bool bSequenceDirty = true;

        public AnimationHolder(List<AnimationSequence> animationSequences)
        {
            m_animationSequences = animationSequences;
            m_currentSuquence = null;
            m_sequenceloopTime = 0.0;
            m_cachedBoneTransforms = new List<BoneTransform>();
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
                bSequenceDirty = true;
            }
        }

        public AnimationSequence GetCurrentSequence()
        {
            return m_currentSuquence;
        }

        public List<BoneTransform> GetAnimatedPoseTransformsList()
        {
            // BLEND BONE MATRICES WITHIN CURRENT ANIMATION FRAME AND RETURN THEM

            // Update animation pose only if sequence loop was updated
            if (bSequenceDirty)
            {
                m_cachedBoneTransforms.Clear();
                var nextAndPrevFrames = m_currentSuquence.GetNextAndPrevFrames(m_sequenceloopTime);
                Int32 bonesCount = nextAndPrevFrames.Item1.Count;

                for (Int32 i = 0; i < bonesCount; i++)
                {
                    BoneTransform nextTransform = nextAndPrevFrames.Item1[i].Item2;
                    BoneTransform prevTransform = nextAndPrevFrames.Item2[i].Item2;

                    double nextTransformTime = nextAndPrevFrames.Item1[i].Item1;
                    double prevTransformTime = nextAndPrevFrames.Item2[i].Item1;

                    m_previousFrameTime = prevTransformTime;

                    float blendFactor = (float)((m_sequenceloopTime - prevTransformTime) / (nextTransformTime - prevTransformTime));

                    BoneTransform lerpedTransform = BoneTransform.SLerp(prevTransform, nextTransform, blendFactor);
                    m_cachedBoneTransforms.Add(lerpedTransform);
                }

                bSequenceDirty = false;
            }

            return m_cachedBoneTransforms;
        }

        public List<Matrix4> GetAnimatedPoseMatricesList()
        {
            List<Matrix4> resultAnimatedPoseMatrices = new List<Matrix4>();

            var poseTransforms = GetAnimatedPoseTransformsList();

            foreach (var boneTransform in poseTransforms)
            {
                resultAnimatedPoseMatrices.Add(boneTransform.GetToBoneSpaceMatrix());
            }

            return resultAnimatedPoseMatrices;
        }

        public Matrix4[] GetAnimatedOffsetedMatrices(Bone rootBone)
        {
            List<Matrix4> relevantBoneTransformations = GetAnimatedPoseMatricesList();

            List<Matrix4> animatedMatrices = new List<Matrix4>();
            CollectAnimatedMatrices(rootBone, Matrix4.Identity, relevantBoneTransformations, ref animatedMatrices);

            List<Matrix4> offsetBones = rootBone.GetSortedWithIdOffsetMatrices();

            Matrix4[] skinningMatrices = new Matrix4[animatedMatrices.Count];

            for (Int32 i = 0; i < animatedMatrices.Count; i++)
            {
                Matrix4 animatedBoneMatrix = animatedMatrices[i];
                Matrix4 offsetBoneMatrix = offsetBones[i];

                skinningMatrices[i] = offsetBoneMatrix * animatedBoneMatrix;
            }

            return skinningMatrices;
        }

        // This is just for another one attempt to start animation working and ... now it works!!
        private void CollectAnimatedMatrices(Bone parentBone, Matrix4 parentMatrix, List<Matrix4> srcTransformation, ref List<Matrix4> dstMatrices)
        {
            Matrix4 currentBoneMatrix = srcTransformation[parentBone.GetId()] * parentMatrix;
            dstMatrices.Add(currentBoneMatrix);
            foreach (var bone in parentBone.GetChildren())
            {
                CollectAnimatedMatrices(bone, currentBoneMatrix, srcTransformation, ref dstMatrices);
            }
        }
    }
}
