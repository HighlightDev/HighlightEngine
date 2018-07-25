using OpenTK;
using VectorMath;

namespace MassiveGame.Core.AnimationCore
{
    public class BoneTransformation
    {
        // Transformation is relative to parent bone

        // skin matrix in local space
        private Matrix4 localSkinningMatrix;

        // inverse skin matrix in local space
        private Matrix4 localInverseSkinningMatrix;

        private Quaternion localSkinningRotation;
        private Vector3 localSkinningTranslation;

        public BoneTransformation(Matrix4 skinMatrix)
        {
            localSkinningMatrix = skinMatrix;
            localInverseSkinningMatrix = Matrix4.Identity;
            UpdateTransformationInfo();
        }

        public BoneTransformation(Quaternion quaternion, Vector3 translation)
        {
            localSkinningRotation = quaternion;
            localSkinningTranslation = translation;
            Matrix4 transformMat = Matrix4.CreateFromQuaternion(quaternion);
            transformMat *= Matrix4.CreateTranslation(translation);
            localSkinningMatrix = transformMat;
        }

        public BoneTransformation()
        {
            localSkinningMatrix = Matrix4.Identity;
            localInverseSkinningMatrix = Matrix4.Identity;
            localSkinningRotation = Quaternion.Identity;
            localSkinningTranslation = Vector3.Zero;
        }

        public static BoneTransformation Lerp(BoneTransformation lhv, BoneTransformation rhv, float blend)
        {
            Vector3 lerpPosition = VectorMathOperations.LerpVector(blend, 0, 1, lhv.localSkinningTranslation, rhv.localSkinningTranslation);
            Quaternion lerpRotation = Quaternion.Slerp(lhv.localSkinningRotation, rhv.localSkinningRotation, blend);
            return new BoneTransformation(lerpRotation, lerpPosition);
        }

        public void SetLocalSkinningMatrix(Matrix4 skinningMatrix)
        {
            localSkinningMatrix = skinningMatrix;
            UpdateTransformationInfo();
        }

        public void SetLocalInverseSkinningMatrix(Matrix4 inverseSkinningMatrix)
        {
            localInverseSkinningMatrix = inverseSkinningMatrix;
        }

        public Matrix4 GetLocalSkinningMatrix()
        {
            return localSkinningMatrix;
        }

        public Matrix4 GetLocalInverseSkinningMatrix()
        {
            return localInverseSkinningMatrix;
        }

        private void UpdateTransformationInfo()
        {
            localSkinningRotation = localSkinningMatrix.ExtractRotation();
            localSkinningTranslation = localSkinningMatrix.ExtractTranslation();
        }
    }
}
