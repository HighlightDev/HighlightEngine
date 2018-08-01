using MassiveGame.Core.AnimationCore;
using VBO;

namespace MassiveGame.API.Mesh
{
    public class AnimatedSkin : Skin
    {
        private Bone m_rootBone;

        public AnimatedSkin(VertexArrayObject vao, Bone rootBone) 
            : base(vao)
        {
            m_rootBone = rootBone;
        }

        public override void CleanUp()
        {
            base.CleanUp();
            m_rootBone.ClearChilderBones();
        }
    }
}
