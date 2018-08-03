using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool
{
    public class PoolCollector
    {
        public ModelPool s_ModelPool { private set; get; }
        public ShaderPool s_ShaderPool { private set; get; }
        public RenderTargetPool s_RenderTargetPool { private set; get; }
        public TexturePool s_TexturePool { private set; get; }
        public AnimationPool s_AnimationPool { private set; get; }

        private static PoolCollector m_collector;

        private PoolCollector()
        {
            s_ModelPool = new ModelPool();
            s_ShaderPool = new ShaderPool();
            s_RenderTargetPool = new RenderTargetPool();
            s_TexturePool = new TexturePool();
            s_AnimationPool = new AnimationPool();
        }

        public static PoolCollector GetInstance()
        {
            if (m_collector == null)
                m_collector = new PoolCollector();
            return m_collector;
        }
    }
}
