using System;
using Assimp;
using CParser.Assimp;
using CParser.Assimp.Strategy;

namespace CParser
{
    public class AssimpModelLoader : IDisposable
    {
        public SkeletonPerVertexBoneInfluenceType m_skeletonType { private set; get; }
        
        private Scene m_scene;
        private MeshVertexData m_meshData;
        private MeshAnimationData m_animationData;

        public AssimpModelLoader(string modelFilePath, SkeletonPerVertexBoneInfluenceType skeletonType = SkeletonPerVertexBoneInfluenceType.ThreeBones)
        {
            m_skeletonType = skeletonType;
            var importer = new AssimpImporter();
            m_scene = importer.ImportFile(modelFilePath, PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
        }

        public MeshVertexData GetMeshData()
        {
            if (m_meshData == null)
            {
                var strategy = GetBoneInfluenceStrategy(m_scene);
                m_meshData = new MeshVertexData(m_scene, strategy);
            }
            return m_meshData;
        }

        public MeshAnimationData GetAnimationData()
        {
            if (m_animationData == null)
                m_animationData = new MeshAnimationData(m_scene);

            return m_animationData;
        }

        private SkeletonPerVertexBoneInfluenceStrategy GetBoneInfluenceStrategy(Scene scene)
        {
            SkeletonPerVertexBoneInfluenceStrategy strategy = null;

            if (scene.HasAnimations)
            {
                switch (m_skeletonType)
                {
                    case SkeletonPerVertexBoneInfluenceType.ThreeBones: { strategy = new SkeletonThreeBonesInfluenceStrategy(); break; }
                    case SkeletonPerVertexBoneInfluenceType.FourBones: { strategy = new SkeletonFourBonesInfluenceStrategy(); break; }
                    case SkeletonPerVertexBoneInfluenceType.FiveBones: { strategy = new SkeletonFiveBonesInfluenceStrategy(); break; }
                    case SkeletonPerVertexBoneInfluenceType.SixBones: { strategy = new SkeletonSixBonesInfluenceStrategy(); break; }
                }
            }

            return strategy;
        }

        public void Dispose()
        {
            m_meshData.CleanUp();
        }
    }
}
