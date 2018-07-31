using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationSequence
    {
        private string m_name;
        private List<AnimationFrame> m_frames;
        private float m_sequenceDurationInSec;

        public AnimationSequence(string animationName)
        {
            m_name = animationName;
            m_frames = new List<AnimationFrame>();
            m_sequenceDurationInSec = 0.0f;
        }

        public AnimationSequence(List<AnimationFrame> frames)
        {
            m_frames = frames;
        }

        public List<AnimationFrame> GetAnimationFrames()
        {
            return m_frames;
        }

        public float GetAnimationDurationInSec()
        {
            return m_sequenceDurationInSec;
        }

        public void SetAnimationFrames(List<AnimationFrame> frames)
        {
            m_frames = frames;
        }

        public void AddAnimationFrame(AnimationFrame frame)
        {
            m_frames.Add(frame);
        }

        public void AddAnimationFrames(params AnimationFrame[] frames)
        {
            m_frames.AddRange(frames);
        }

        public void ClearAnimationFrameCollection()
        {
            m_frames.Clear();
        }
    }
}
