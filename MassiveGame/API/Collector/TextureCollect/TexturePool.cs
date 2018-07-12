using TextureLoader;

namespace MassiveGame.API.Collector.TextureCollect
{
    public class TexturePool
    {
        private TextureCollection textureCollection;

        public TexturePool()
        {
            textureCollection = new TextureCollection();
        }

        public ITexture GetTexture(string key)
        {
            return textureCollection.RetrieveTexture(key);
        }

        public void ReleaseTexture(string key)
        {
            this.textureCollection.ReleaseTexture(key);
        }

        public void ReleaseTexture(ITexture texture)
        {
            this.textureCollection.ReleaseTexture(texture);
        }
    }
}
