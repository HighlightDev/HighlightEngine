using System;
using Assimp;
using CParser.Assimp;
using CParser.Assimp.Strategy;

namespace CParser
{
    public class AssimpModelLoader : IDisposable
    {
        public SkeletonPerVertexBoneInfluenceType skeletonType { private set; get; }
        public MeshVertexData MeshData { private set; get; }

        private AssimpImporter importer;

        public AssimpModelLoader(string modelFilePath, SkeletonPerVertexBoneInfluenceType skeletonType = SkeletonPerVertexBoneInfluenceType.ThreeBones)
        {
            this.skeletonType = skeletonType;
            importer = new AssimpImporter();
            var scene = importer.ImportFile(modelFilePath, PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            MeshData = new MeshVertexData(scene, GetBoneInfluenceStrategy(scene));
            importer = null;
        }

        private SkeletonPerVertexBoneInfluenceStrategy GetBoneInfluenceStrategy(Scene scene)
        {
            SkeletonPerVertexBoneInfluenceStrategy strategy = null;

            if (scene.HasAnimations)
            {
                switch (skeletonType)
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
            MeshData.CleanUp();
        }
    }
}
