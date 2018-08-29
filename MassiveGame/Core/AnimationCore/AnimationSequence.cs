using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationSequence
    {
        private string m_name;
        private List<AnimationFrame> m_frames;
        private double m_sequenceDurationInSec;

        public AnimationSequence(string animationName, List<AnimationFrame> frames, double sequenceDurationInSec)
        {
            m_frames = frames;
            m_name = animationName;
            m_sequenceDurationInSec = sequenceDurationInSec;
        }

        public string GetName()
        {
            return m_name;
        }

        public Int32 GetBonesCount()
        {
            return m_frames.Count;
        }

        public Tuple<List<Tuple<double, BoneTransformation>>, List<Tuple<double, BoneTransformation>>> GetNextAndPrevFrames(double animationLoopTime)
        {
            Tuple<List<Tuple<double, BoneTransformation>>, List<Tuple<double, BoneTransformation>>> result = null;
            var nextFrames = new List<Tuple<double, BoneTransformation>>();
            var prevFrames = new List<Tuple<double, BoneTransformation>>();
            foreach (var animationFrame in m_frames)
            {
                Tuple<double, double> interval_max_min = animationFrame.GetIntervalsOnTimeBoundaries(animationLoopTime);
                var nextFrame = animationFrame.GetFrameByTime(interval_max_min.Item1);
                var prevFrame = animationFrame.GetFrameByTime(interval_max_min.Item2);
                nextFrames.Add(nextFrame);
                prevFrames.Add(prevFrame);
            }

            result = new Tuple<List<Tuple<double, BoneTransformation>>, List<Tuple<double, BoneTransformation>>>(nextFrames, prevFrames);
            return result;
        }
        
        public List<AnimationFrame> GetAnimationFrames()
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
            m_frames.ForEach(frame => { frame.CleanUp(); frame = null; });
            m_frames.Clear();
        }
    }
}
