using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrameCreator
    {
        public List<UiFrame> frameList;
        public List<ITexture> frameTextures;
        public readonly Int32 MAX_FRAME_COUNT = 10;

        public UiFrameCreator()
        {
            frameList = new List<UiFrame>(MAX_FRAME_COUNT);
            frameTextures = new List<ITexture>(MAX_FRAME_COUNT);
        }

        public void PushFrame(Vector2 origin, Vector2 extent, ITexture texture)
        {
            if (frameList.Count < MAX_FRAME_COUNT)
            {
                frameList.Add(new UiFrame(origin, extent));
                frameTextures.Add(texture);
            }
        }

        public void PopFrame()
        {
            if (frameList.Count != 0 && frameTextures.Count != 0)
            {
                frameList.Remove(frameList.Last());
                frameTextures.Remove(frameTextures.Last());
            }
        }

        public void Render()
        {
            for (Int32 i = 0; i< frameList.Count; i++)
            {
                ITexture frameTexture = frameTextures[i];
                frameList[i].Render(frameTexture);
            }
        }
    }
}
