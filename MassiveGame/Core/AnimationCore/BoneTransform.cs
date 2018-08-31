using OpenTK;
using VectorMath;

namespace MassiveGame.Core.AnimationCore
{
    public class BoneTransform
    {
        // Transformation is relative to parent bone

        // matrix to bone local space
        private Matrix4 m_toBoneSpace;

        private Quaternion m_toBoneRotation;
        private Vector3 m_toBoneTranslation;
        private Vector3 m_toBoneScale;

        public BoneTransform(Matrix4 toBoneMatrix)
        {
            m_toBoneSpace = toBoneMatrix;
            ExtractRotationTranslationScale();
        }

        public BoneTransform(Quaternion rotation, Vector3 translation, Vector3 scale)
        {
            m_toBoneRotation = rotation;
            m_toBoneTranslation = translation;
            m_toBoneScale = scale;
            m_toBoneSpace = Matrix4.CreateScale(scale);
            m_toBoneSpace *= Matrix4.CreateFromQuaternion(rotation);
            m_toBoneSpace *= Matrix4.CreateTranslation(translation);
        }

        public BoneTransform()
        {
            m_toBoneSpace = Matrix4.Identity;
            m_toBoneRotation = Quaternion.Identity;
            m_toBoneTranslation = Vector3.Zero;
        }

        public static BoneTransform SLerp(BoneTransform lhv, BoneTransform rhv, float blend)
        {
            Vector3 lerpPosition = VectorMathOperations.LerpVector(blend, 0, 1, lhv.m_toBoneTranslation, rhv.m_toBoneTranslation);
            Vector3 lerpScale = VectorMathOperations.LerpVector(blend, 0, 1, lhv.m_toBoneScale, rhv.m_toBoneScale);
            Quaternion lerpRotation = Quaternion.Slerp(lhv.m_toBoneRotation, rhv.m_toBoneRotation, blend);
            return new BoneTransform(lerpRotation, lerpPosition, lerpScale);
        }

        public void SetToBoneSpaceMatrix(Matrix4 toBoneSpaceMatrix)
        {
            m_toBoneSpace = toBoneSpaceMatrix;
            ExtractRotationTranslationScale();
        }

        public Matrix4 GetToBoneSpaceMatrix()
        {
            return m_toBoneSpace;
        }

        private void ExtractRotationTranslationScale()
        {
            m_toBoneRotation = m_toBoneSpace.ExtractRotation();
            m_toBoneTranslation = m_toBoneSpace.ExtractTranslation();
            m_toBoneScale = m_toBoneSpace.ExtractScale();
        }
    }
}
