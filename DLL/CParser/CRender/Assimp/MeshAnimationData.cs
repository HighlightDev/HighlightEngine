using Assimp;
using System.Collections.Generic;

namespace CParser.Assimp
{
    public class MeshAnimationData
    {
        private Scene m_scene;
        private Animation[] m_animations;

        public List<LoaderAnimation> Animations { get; private set; }

        public MeshAnimationData(Scene scene)
        {
            m_scene = scene;
            m_animations = scene.Animations;
            GetAnimations();
        }

        private void GetAnimations()
        {
            if (m_scene.HasAnimations)
            {
                Animations = new List<LoaderAnimation>();
                foreach (var animation in m_animations)
                {
                    Animations.Add(new LoaderAnimation(animation));
                }
            }
        }

        public void CleanUp()
        {
            m_animations = null;
            m_animations = null;
        }
    }
}
