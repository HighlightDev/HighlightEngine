using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public static class MatrixTransforms
    {
        public static Matrix2f Inverse(Matrix2f m)
        {

            float OneOverDeterminant = (1f) / (
                +m[0][0] * m[1][1]
                - m[1][0] * m[0][1]);

            Matrix2f inverse = new Matrix2f(
                +m[1][1] * OneOverDeterminant,
                -m[0][1] * OneOverDeterminant,
                -m[1][0] * OneOverDeterminant,
                +m[0][0] * OneOverDeterminant);

            return inverse;
        }
        public static Matrix3f Inverse(Matrix3f m)
        {
            float OneOverDeterminant = (1f) / (
                +m[0][0] * (m[1][1] * m[2][2] - m[2][1] * m[1][2])
                - m[1][0] * (m[0][1] * m[2][2] - m[2][1] * m[0][2])
                + m[2][0] * (m[0][1] * m[1][2] - m[1][1] * m[0][2]));

            Matrix3f inverse = new Matrix3f(0);
            inverse[0, 0] = +(m[1][1] * m[2][2] - m[2][1] * m[1][2]) * OneOverDeterminant;
            inverse[1, 0] = -(m[1][0] * m[2][2] - m[2][0] * m[1][2]) * OneOverDeterminant;
            inverse[2, 0] = +(m[1][0] * m[2][1] - m[2][0] * m[1][1]) * OneOverDeterminant;
            inverse[0, 1] = -(m[0][1] * m[2][2] - m[2][1] * m[0][2]) * OneOverDeterminant;
            inverse[1, 1] = +(m[0][0] * m[2][2] - m[2][0] * m[0][2]) * OneOverDeterminant;
            inverse[2, 1] = -(m[0][0] * m[2][1] - m[2][0] * m[0][1]) * OneOverDeterminant;
            inverse[0, 2] = +(m[0][1] * m[1][2] - m[1][1] * m[0][2]) * OneOverDeterminant;
            inverse[1, 2] = -(m[0][0] * m[1][2] - m[1][0] * m[0][2]) * OneOverDeterminant;
            inverse[2, 2] = +(m[0][0] * m[1][1] - m[1][0] * m[0][1]) * OneOverDeterminant;

            return inverse;

        }
        public static Matrix4f Inverse(Matrix4f m)
        {
            float Coef00 = m[2][2] * m[3][3] - m[3][2] * m[2][3];
            float Coef02 = m[1][2] * m[3][3] - m[3][2] * m[1][3];
            float Coef03 = m[1][2] * m[2][3] - m[2][2] * m[1][3];

            float Coef04 = m[2][1] * m[3][3] - m[3][1] * m[2][3];
            float Coef06 = m[1][1] * m[3][3] - m[3][1] * m[1][3];
            float Coef07 = m[1][1] * m[2][3] - m[2][1] * m[1][3];

            float Coef08 = m[2][1] * m[3][2] - m[3][1] * m[2][2];
            float Coef10 = m[1][1] * m[3][2] - m[3][1] * m[1][2];
            float Coef11 = m[1][1] * m[2][2] - m[2][1] * m[1][2];

            float Coef12 = m[2][0] * m[3][3] - m[3][0] * m[2][3];
            float Coef14 = m[1][0] * m[3][3] - m[3][0] * m[1][3];
            float Coef15 = m[1][0] * m[2][3] - m[2][0] * m[1][3];

            float Coef16 = m[2][0] * m[3][2] - m[3][0] * m[2][2];
            float Coef18 = m[1][0] * m[3][2] - m[3][0] * m[1][2];
            float Coef19 = m[1][0] * m[2][2] - m[2][0] * m[1][2];

            float Coef20 = m[2][0] * m[3][1] - m[3][0] * m[2][1];
            float Coef22 = m[1][0] * m[3][1] - m[3][0] * m[1][1];
            float Coef23 = m[1][0] * m[2][1] - m[2][0] * m[1][1];

            Vector4f Fac0 = new Vector4f(Coef00, Coef00, Coef02, Coef03);
            Vector4f Fac1 = new Vector4f(Coef04, Coef04, Coef06, Coef07);
            Vector4f Fac2 = new Vector4f(Coef08, Coef08, Coef10, Coef11);
            Vector4f Fac3 = new Vector4f(Coef12, Coef12, Coef14, Coef15);
            Vector4f Fac4 = new Vector4f(Coef16, Coef16, Coef18, Coef19);
            Vector4f Fac5 = new Vector4f(Coef20, Coef20, Coef22, Coef23);

            Vector4f Vec0 = new Vector4f(m[1][0], m[0][0], m[0][0], m[0][0]);
            Vector4f Vec1 = new Vector4f(m[1][1], m[0][1], m[0][1], m[0][1]);
            Vector4f Vec2 = new Vector4f(m[1][2], m[0][2], m[0][2], m[0][2]);
            Vector4f Vec3 = new Vector4f(m[1][3], m[0][3], m[0][3], m[0][3]);

            Vector4f Inv0 = new Vector4f(Vec1 * Fac0 - Vec2 * Fac1 + Vec3 * Fac2);
            Vector4f Inv1 = new Vector4f(Vec0 * Fac0 - Vec2 * Fac3 + Vec3 * Fac4);
            Vector4f Inv2 = new Vector4f(Vec0 * Fac1 - Vec1 * Fac3 + Vec3 * Fac5);
            Vector4f Inv3 = new Vector4f(Vec0 * Fac2 - Vec1 * Fac4 + Vec2 * Fac5);

            Vector4f SignA = new Vector4f(+1, -1, +1, -1);
            Vector4f SignB = new Vector4f(-1, +1, -1, +1);
            Matrix4f inverse = new Matrix4f(Inv0 * SignA, Inv1 * SignB, Inv2 * SignA, Inv3 * SignB);

            Vector4f Row0 = new Vector4f(inverse[0][0], inverse[1][0], inverse[2][0], inverse[3][0]);

            Vector4f Dot0 = new Vector4f(m[0] * Row0);
            float Dot1 = (Dot0.x + Dot0.y) + (Dot0.z + Dot0.w);

            float OneOverDeterminant = (1f) / Dot1;

            return inverse * OneOverDeterminant;
        }

        public static Matrix4f Transpose(Matrix4f m)
        {
            Vector4f[] vec = new Vector4f[4];
            vec[0] = new Vector4f(m[0]);
            vec[1] = new Vector4f(m[1]);
            vec[2] = new Vector4f(m[2]);
            vec[3] = new Vector4f(m[3]);
            for (int i = 0; i < vec.Length; i++)
            {
                for (int j = 0; j < vec.Length; j++)
                {
                    m[i][j] = vec[j][i];
                }
            }
            return m;
        }
        public static Matrix3f Transpose(Matrix3f m)
        {
            Vector3f[] vec = new Vector3f[3];
            vec[0] = new Vector3f(m[0]);
            vec[1] = new Vector3f(m[1]);
            vec[2] = new Vector3f(m[2]);
            for (int i = 0; i < vec.Length; i++)
            {
                for (int j = 0; j < vec.Length; j++)
                {
                    m[i][j] = vec[j][i];
                }
            }
            return m;


            //var vec0 = new Vector3f(m[0]);
            //var vec1 = new Vector3f(m[1]);
            //var vec2 = new Vector3f(m[2]);
            //m[0][1] = vec1[0];
            //m[0][2] = vec2[2];
            //m[1][0] = vec0[1];
            //m[1][2] = vec2[1];
            //m[2][0] = vec0[2];
            //m[2][1] = vec1[2];
            //return m;
        }
        public static Matrix2f Transpose(Matrix2f m)
        {
            Vector2f[] vec = new Vector2f[2];
            vec[0] = new Vector2f(m[0]);
            vec[1] = new Vector2f(m[1]);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < vec.Length; j++)
                {
                    m[i][j] = vec[j][i];
                }
            }
            return m;


            //var vec0 = new Vector2f(m[0]);
            //var vec1 = new Vector2f(m[1]);
            //m[0][0] = vec0[0];
            //m[0][1] = vec1[0];
            //m[1][0] = vec0[1];
            //m[1][1] = vec1[1];
            //return m;
        }
    }
}
