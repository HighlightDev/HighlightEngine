using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationFrame
    {
        private Dictionary<double, BoneTransformation> m_frames;
        private string m_boneName;

        public AnimationFrame(string boneName, Int32 frameCapacity = default(Int32))
        {
            m_boneName = boneName;
            if (frameCapacity != 0)
                m_frames = new Dictionary<double, BoneTransformation>(frameCapacity);
            else
                m_frames = new Dictionary<double, BoneTransformation>();
        }

        public void AddFrame(BoneTransformation boneFrameTransform, double frameTime)
        {
            m_frames.Add(frameTime, boneFrameTransform);
        }

        public string GetBoneName()
        {
            return m_boneName;
        }

        public Dictionary<double, BoneTransformation> GetFrames()
        {
            return m_frames;
        }

        public double[] GetFrameTimeIntervals()
        {
            double[] keys = new double[m_frames.Count];
            m_frames.Keys.CopyTo(keys, 0);
            return keys;
        }

        public Tuple<double, double> GetIntervalsOnTimeBoundaries(double time)
        {
            double[] timeIntervals = GetFrameTimeIntervals();

            if (time <= 0.000005)
                return new Tuple<double, double>(timeIntervals[1], timeIntervals[0]);

            double next = 0, prev = timeIntervals[0];
            for (Int32 i = 0; i < timeIntervals.Length; i++)
            {
                double reverseTimeInterval = timeIntervals[timeIntervals.Length - i - 1];
                double forwardTimeInterval = timeIntervals[i];

                if (reverseTimeInterval > time)
                    next = reverseTimeInterval;

                if (forwardTimeInterval < time)
                    prev = forwardTimeInterval;
            }
            return new Tuple<double, double>(next, prev);
        }

        public Tuple<double, BoneTransformation> GetFrameByTime(double key)
        {
            return new Tuple<double, BoneTransformation>(key, m_frames[key]);
        }

        public void CleanUp()
        {
            m_frames = null;
        }
    }
}
