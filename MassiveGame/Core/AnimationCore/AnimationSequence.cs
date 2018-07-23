using System.Collections.Generic;

namespace MassiveGame.Core.AnimationCore
{
    public class AnimationSequence
    {
        private List<AnimationFrame> animationFrames;
        private float animationTimeTotal;

        public AnimationSequence()
        {
            animationFrames = new List<AnimationFrame>();
            animationTimeTotal = 0.0f;
        }

        public AnimationSequence(List<AnimationFrame> frames)
        {
            animationFrames = frames;
        }

        public void SetAnimationFrames(List<AnimationFrame> frames)
        {
            animationFrames = frames;
        }

        public void AddAnimationFrame(AnimationFrame frame)
        {
            animationFrames.Add(frame);
        }

        public void AddAnimationFrames(params AnimationFrame[] frames)
        {
            animationFrames.AddRange(frames);
        }

        public void ClearAnimationFrameCollection()
        {
            animationFrames.Clear();
        }
    }
}
