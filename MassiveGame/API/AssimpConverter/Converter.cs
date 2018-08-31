using System.Collections.Generic;
using MassiveGame.Core.AnimationCore;
using OpenTK;

using EngineBone = MassiveGame.Core.AnimationCore.Bone;
using System;

namespace MassiveGame.API.AssimpConverter
{
    public static class Converter
    {
        public static EngineBone ConvertAssimpBoneToEngineBone(CParser.Assimp.LoaderSkeletonParentBone rootBone)
        {
            EngineBone resultBone = null;

            foreach (var bone in rootBone.GetChildren())
            {
                EngineBone root = new EngineBone(bone.GetBoneId(), bone.GetBoneInfo().Name, null);
                root.SetOffsetMatrix(ConvertAssimpMatrix4x4ToOpenTKMatrix4(bone.GetBoneInfo().OffsetMatrix));
                IterateBoneTree(root, bone);


                throw new NotImplementedException();
                // This is temporary until engine bone doesn't support multiple parent bones
                resultBone = root;
                break;
            }

            return resultBone;
        }

        private static Matrix4 ConvertAssimpMatrix4x4ToOpenTKMatrix4(Assimp.Matrix4x4 srcMatrix)
        {
            Matrix4 dstMatrix = new Matrix4(
                srcMatrix.A1, srcMatrix.B1, srcMatrix.C1, srcMatrix.D1,
                srcMatrix.A2, srcMatrix.B2, srcMatrix.C2, srcMatrix.D2,
                srcMatrix.A3, srcMatrix.B3, srcMatrix.C3, srcMatrix.D3,
                srcMatrix.A4, srcMatrix.B4, srcMatrix.C4, srcMatrix.D4);

            return dstMatrix;
        }

        private static void IterateBoneTree(EngineBone dstParentBone, CParser.Assimp.LoaderSkeletonBone srcParentNode)
        {
            foreach (var srcNode in srcParentNode.GetChildren())
            {
                EngineBone dstChildBone = new EngineBone(srcNode.GetBoneId(), srcNode.GetBoneInfo().Name, dstParentBone);
                dstChildBone.SetOffsetMatrix(ConvertAssimpMatrix4x4ToOpenTKMatrix4(srcNode.GetBoneInfo().OffsetMatrix));
                dstParentBone.AddChildBone(dstChildBone);
                IterateBoneTree(dstChildBone, srcNode);
            }
        }

        public static List<AnimationSequence> ConvertAssimpAnimationToEngineAnimation(List<CParser.Assimp.LoaderAnimation> srcAnimations)
        {
            List<AnimationSequence> dstAnimations = new List<AnimationSequence>(srcAnimations.Capacity);

            foreach (var srcAnimation in srcAnimations)
            {
                List<AnimationFrame> dstFrames = new List<AnimationFrame>();
                foreach (var srcFrameCollection in srcAnimation.FramesBoneCollection)
                {
                    var dstFrame = new AnimationFrame(srcFrameCollection.BoneName, srcFrameCollection.Frames.Count);
                    foreach (var srcFrame in srcFrameCollection.Frames)
                    {
                        var dstRotation = new Quaternion(srcFrame.Rotation.Value.X, srcFrame.Rotation.Value.Y, srcFrame.Rotation.Value.Z, srcFrame.Rotation.Value.W);
                        var dstTranslation = new Vector3(srcFrame.Translation.Value.X, srcFrame.Translation.Value.Y, srcFrame.Translation.Value.Z);
                        var dstScaling = new Vector3(srcFrame.Scale.Value.X, srcFrame.Scale.Value.Y, srcFrame.Scale.Value.Z);

                        dstFrame.AddFrame(new BoneTransform(dstRotation, dstTranslation, dstScaling), srcFrame.TimeStart);
                    }
                    dstFrames.Add(dstFrame);
                }
                AnimationSequence dstSequence = new AnimationSequence(srcAnimation.Name, dstFrames, srcAnimation.AnimationDuration);
                dstAnimations.Add(dstSequence);
            }

            return dstAnimations;
        }
    }
}
