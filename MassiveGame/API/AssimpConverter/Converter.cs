using Assimp;
using CParser.Assimp;
using OpenTK;

using EngineBone = MassiveGame.Core.AnimationCore.Bone;

namespace MassiveGame.API.AssimpConverter
{
    public static class Converter
    {
        public static EngineBone ConvertAssimpBoneToEngineBone(LoaderSkeletonBone rootBone)
        {
            EngineBone root = new EngineBone(rootBone.GetBoneId(), rootBone.GetBoneInfo().Name, null);
            root.SetOffsetMatrix(ConvertAssimpMatrix4x4ToOpenTKMatrix4(rootBone.GetBoneInfo().OffsetMatrix));
            GoThroughBoneTree(root, rootBone);
            root.CalcInverseOffsetMatrices();
            return root;
        }

        private static Matrix4 ConvertAssimpMatrix4x4ToOpenTKMatrix4(Matrix4x4 srcMatrix)
        {
            Matrix4 dstMatrix = new Matrix4(
                srcMatrix.A1, srcMatrix.A2, srcMatrix.A3, srcMatrix.A4,
                srcMatrix.B1, srcMatrix.B2, srcMatrix.B3, srcMatrix.B4,
                srcMatrix.C1, srcMatrix.C2, srcMatrix.C3, srcMatrix.C4,
                srcMatrix.D1, srcMatrix.D2, srcMatrix.D3, srcMatrix.D4);

            return dstMatrix;
        }

        private static void GoThroughBoneTree(EngineBone dstParentBone, LoaderSkeletonBone srcParentNode)
        {
            foreach (var srcNode in srcParentNode.GetChildren())
            {
                EngineBone dstChildBone = new EngineBone(srcNode.GetBoneId(), srcNode.GetBoneInfo().Name, dstParentBone);
                dstChildBone.SetOffsetMatrix(ConvertAssimpMatrix4x4ToOpenTKMatrix4(srcNode.GetBoneInfo().OffsetMatrix));
                dstParentBone.AddChildBone(dstChildBone);
                GoThroughBoneTree(dstChildBone, srcNode);
            }
        }

    }
}
