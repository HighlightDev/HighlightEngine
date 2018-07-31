using System.Collections.Generic;

namespace CParser.Assimp
{
    public class LoaderBoneFrameCollection
    {
        public string BoneName { set; get; }
        public List<LoaderAnimationFrame> Frames { set; get; }

        public LoaderBoneFrameCollection(string name)
        {
            BoneName = name;
            Frames = new List<LoaderAnimationFrame>();
        }

        public void AddFrame(LoaderAnimationFrame frame)
        {
            Frames.Add(frame);
        }
    }
}
