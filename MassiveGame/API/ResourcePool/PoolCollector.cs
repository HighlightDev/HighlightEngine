using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool
{
    public class PoolCollector
    {
        public ModelPool ModelPool { private set; get; }
        public ShaderPool ShaderPool { private set; get; }
        public RenderTargetPool RenderTargetPool { private set; get; }
        public TexturePool TexturePool { private set; get; }
        public AnimationPool AnimationPool { private set; get; }

        private static PoolCollector m_collector;

        private PoolCollector()
        {
            ModelPool = new ModelPool();
            ShaderPool = new ShaderPool();
            RenderTargetPool = new RenderTargetPool();
            TexturePool = new TexturePool();
            AnimationPool = new AnimationPool();
        }

        public static PoolCollector GetInstance()
        {
            if (m_collector == null)
                m_collector = new PoolCollector();
            return m_collector;
        }
    }
}
