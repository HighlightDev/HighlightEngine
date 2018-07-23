using OpenTK;

namespace MassiveGame.Core.AnimationCore
{
    public class BoneTransformation
    {
        // skin matrix in local space
        private Matrix4 localSkinningMatrix;

        // inverse skin matrix in local space
        private Matrix4 localInverseSkinningMatrix;

        private Quaternion localSkinningRotation;
        private Vector3 localSkinningTranslation;

        public BoneTransformation(Matrix4 skinMatrix, Matrix4 inverseSkinMatrix)
        {
            localSkinningMatrix = skinMatrix;
            localInverseSkinningMatrix = inverseSkinMatrix;
            UpdateTransformationInfo();
        }

        public BoneTransformation()
        {
            localSkinningMatrix = Matrix4.Identity;
            localInverseSkinningMatrix = Matrix4.Identity;
            localSkinningRotation = Quaternion.Identity;
            localSkinningTranslation = Vector3.Zero;
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
