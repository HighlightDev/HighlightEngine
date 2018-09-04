using MassiveGame.Core.AnimationCore;
using VBO;

namespace MassiveGame.API.Mesh
{
    public class AnimatedSkin : Skin
    {
        private ParentBone m_rootBone;

        public AnimatedSkin(VertexArrayObject vao, ParentBone rootBone) 
            : base(vao)
        {
            m_rootBone = rootBone;
        }

        public ParentBone GetRootBone()
        {
            return m_rootBone;
        }

        public override void CleanUp()
        {
            base.CleanUp();
            m_rootBone.ClearChildrenBones();
        }
    }
}
