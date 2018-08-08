using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationSequence
    {
        private string m_name;
        private List<BoneFrames> m_frames;
        private double m_sequenceDurationInSec;

        public AnimationSequence(string animationName, List<BoneFrames> frames, double sequenceDurationInSec)
        {
            m_frames = frames;
            m_name = animationName;
            m_sequenceDurationInSec = sequenceDurationInSec;
        }

        public string GetName()
        {
            return m_name;
        }

        public List<BoneFrames> GetAnimationFrames()
        {
            return m_frames;
        }

        public double GetAnimationDurationInSec()
        {
            return m_sequenceDurationInSec;
        }

        public void SetAnimationDurationInSec(double sequenceDurationInSec)
        {
            m_sequenceDurationInSec = sequenceDurationInSec;
        }

        public void SetAnimationFrames(List<BoneFrames> frames)
        {
            m_frames = frames;
        }

        public void AddAnimationFrame(BoneFrames frame)
        {
            m_frames.Add(frame);
        }

        public void AddAnimationFrames(params BoneFrames[] frames)
        {
            m_frames.AddRange(frames);
        }

        public void ClearAnimationFrameCollection()
        {
            m_frames.ForEach(frame => { frame.CleanUp(); frame = null; });
            m_frames.Clear();
        }
    }
}
