using System;
using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class BoneFrames
    {
        private List<Tuple<double, BoneTransformation>> m_frames;
        private string m_boneName;

        public BoneFrames(string boneName, Int32 frameCapacity = default(Int32))
        {
            m_boneName = boneName;
            if (frameCapacity != 0)
                m_frames = new List<Tuple<double, BoneTransformation>>(frameCapacity);
            else
                m_frames = new List<Tuple<double, BoneTransformation>>();
        }

        public void AddFrame(BoneTransformation boneFrameTransform, double frameTime)
        {
            m_frames.Add(new Tuple<double, BoneTransformation>(frameTime, boneFrameTransform));
        }

        public string GetBoneName()
        {
            return m_boneName;
        }

        public List<Tuple<double, BoneTransformation>> GetFrames()
        {
            return m_frames;
        }

        public void CleanUp()
        {
            m_frames = null;
        }
    }
}
