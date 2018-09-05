using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationFrame
    {
        private Dictionary<double, BoneTransform> m_frames;
        private string m_boneName;

        public AnimationFrame(string boneName, Int32 frameCapacity = default(Int32))
        {
            m_boneName = boneName;
            if (frameCapacity != 0)
                m_frames = new Dictionary<double, BoneTransform>(frameCapacity);
            else
                m_frames = new Dictionary<double, BoneTransform>();
        }

        public void AddFrame(BoneTransform boneFrameTransform, double frameTime)
        {
            m_frames.Add(frameTime, boneFrameTransform);
        }

        public string GetBoneName()
        {
            return m_boneName;
        }

        public Dictionary<double, BoneTransform> GetFrames()
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
            double[] allTimeIntervals = GetFrameTimeIntervals();
            Tuple<double, double> resultInterval = null;

            // In case there is only one state in animation
            if (allTimeIntervals.Length == 1)
            {
                resultInterval = new Tuple<double, double>(allTimeIntervals[0], allTimeIntervals[0]);
            }
            // -FIXME
            else if (time <= 0.000005)
            {
                resultInterval = new Tuple<double, double>(allTimeIntervals[0], allTimeIntervals[1]);
            }
            else
            {
                double next = 0.0, prev = allTimeIntervals[0];
                for (Int32 i = 0; i < allTimeIntervals.Length; i++)
                {
                    double reverseTimeInterval = allTimeIntervals[allTimeIntervals.Length - i - 1];
                    double forwardTimeInterval = allTimeIntervals[i];

                    if (reverseTimeInterval > time)
                        next = reverseTimeInterval;

                    if (forwardTimeInterval < time)
                        prev = forwardTimeInterval;
                }
                resultInterval = new Tuple<double, double>(next, prev);
            }

            return resultInterval;
        }

        public Tuple<double, BoneTransform> GetFrameByTime(double key)
        {
            return new Tuple<double, BoneTransform>(key, m_frames[key]);
        }

        public void CleanUp()
        {
            m_frames = null;
        }
    }
}
