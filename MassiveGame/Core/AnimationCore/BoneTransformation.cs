using OpenTK;
using VectorMath;

namespace MassiveGame.Core.AnimationCore
{
    public class BoneTransformation
    {
        // Transformation is relative to parent bone

        // skin matrix in local space
        private Matrix4 m_localSpaceMatrix;
        // inverse skin matrix in local space
        private Matrix4 m_localSpaceInverseMatrix;

        private Quaternion m_localSpaceRotation;
        private Vector3 m_localSpaceTranslation;
        private Vector3 m_localSpaceScale;

        public BoneTransformation(Matrix4 transformationMatrix)
        {
            m_localSpaceMatrix = transformationMatrix;
            m_localSpaceInverseMatrix = Matrix4.Identity;
            ExtractRotationTranslationScale();
        }

        public BoneTransformation(Quaternion rotation, Vector3 translation, Vector3 scale)
        {
            m_localSpaceRotation = rotation;
            m_localSpaceTranslation = translation;
            m_localSpaceScale = scale;
            Matrix4 transformMat = Matrix4.CreateFromQuaternion(rotation);
            transformMat *= Matrix4.CreateTranslation(translation);
            m_localSpaceMatrix = transformMat;
        }

        public BoneTransformation()
        {
            m_localSpaceMatrix = Matrix4.Identity;
            m_localSpaceInverseMatrix = Matrix4.Identity;
            m_localSpaceRotation = Quaternion.Identity;
            m_localSpaceTranslation = Vector3.Zero;
        }

        public static BoneTransformation Lerp(BoneTransformation lhv, BoneTransformation rhv, float blend)
        {
            Vector3 lerpPosition = VectorMathOperations.LerpVector(blend, 0, 1, lhv.m_localSpaceTranslation, rhv.m_localSpaceTranslation);
            Vector3 lerpScale = VectorMathOperations.LerpVector(blend, 0, 1, lhv.m_localSpaceScale, rhv.m_localSpaceScale);
            Quaternion lerpRotation = Quaternion.Slerp(lhv.m_localSpaceRotation, rhv.m_localSpaceRotation, blend);
            return new BoneTransformation(lerpRotation, lerpPosition, lerpScale);
        }

        public void SetLocalOffsetMatrix(Matrix4 offsetMatrix)
        {
            m_localSpaceMatrix = offsetMatrix;
            ExtractRotationTranslationScale();
        }

        public void SetLocalInverseOffsetMatrix(Matrix4 inverseSkinningMatrix)
        {
            m_localSpaceInverseMatrix = inverseSkinningMatrix;
        }

        public Matrix4 GetLocalOffsetMatrix()
        {
            return m_localSpaceMatrix;
        }

        public Matrix4 GetLocalInverseOffsetMatrix()
        {
            return m_localSpaceInverseMatrix;
        }

        private void ExtractRotationTranslationScale()
        {
            m_localSpaceRotation = m_localSpaceMatrix.ExtractRotation();
            m_localSpaceTranslation = m_localSpaceMatrix.ExtractTranslation();
            m_localSpaceScale = m_localSpaceMatrix.ExtractScale();
        }
    }
}
