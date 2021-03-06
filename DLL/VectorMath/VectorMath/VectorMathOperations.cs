﻿using System;
using OpenTK;

namespace VectorMath
{
    public static class VectorMathOperations
    {
        #region Vector Operations

        public static Vector3 TransformVec3(ref Vector3 vec, ref Matrix3 mat)
        {
            return new Vector3(
                Vector3.Dot(vec, mat.Column0),
                Vector3.Dot(vec, mat.Column1),
                Vector3.Dot(vec, mat.Column2)
                );
        }

        /// <summary>
        /// Возвращает нормаль к плоскости
        /// </summary>
        /// <param name="vTriangle">Массив из трех вершин треугольников в виде вектора</param>
        /// <returns>Вектор нормали</returns>
        public static Vector3 GetNormalToPlane(Vector3[] vTriangle)
        {
            Vector3 vVector1 = vTriangle[1] - vTriangle[2];
            Vector3 vVector2 = vTriangle[1] - vTriangle[0];
            Vector3 vNormal = Vector3.Cross(vVector1, vVector2);
            vNormal = Vector3.Normalize(vNormal);
            return vNormal;
        }

        public static float Magnitude(Vector3 vector)
        {
            return Convert.ToSingle(Math.Sqrt((vector.X * vector.X) +
                   (vector.Y * vector.Y) +
                   (vector.Z * vector.Z)));
        }
        
        public static Vector3 LerpVector(float t, float t1, float t2, Vector3 position1, Vector3 position2)
        {
            Vector3 resultPosition = Vector3.Zero;

            float x_delta = t2 - t1;
            float x_zero_offset = t - t1;

            resultPosition.X = ((position2.X - position1.X) / x_delta) * x_zero_offset + position1.X;
            resultPosition.Y = ((position2.Y - position1.Y) / x_delta) * x_zero_offset + position1.Y;
            resultPosition.Z = ((position2.Z - position1.Z) / x_delta) * x_zero_offset + position1.Z;

            return resultPosition;
        }

        #endregion 
        #region Multiplications

        public static Vector4 multMatrix(Matrix4 lhs, Vector4 rhs)
        {
            Vector4 tempV = new Vector4();
            tempV.X = ((lhs[0,0] * rhs.X) + (lhs[1,0] * rhs.Y) + (lhs[2,0] * rhs.Z) + (lhs[3,0] * rhs.W));
            tempV.Y = ((lhs[0,1] * rhs.X) + (lhs[1,1] * rhs.Y) + (lhs[2,1] * rhs.Z) + (lhs[3,1] * rhs.W));
            tempV.Z = ((lhs[0,2] * rhs.X) + (lhs[1,2] * rhs.Y) + (lhs[2,2] * rhs.Z) + (lhs[3,2] * rhs.W));
            tempV.W = ((lhs[0,3] * rhs.X) + (lhs[1,3] * rhs.Y) + (lhs[2,3] * rhs.Z) + (lhs[3,3] * rhs.W));
            return tempV;
        }

        public static Vector3 multMatrix(Matrix3 lhs, Vector3 rhs)
        {
            Vector3 tempV = new Vector3();
            tempV.X = Vector3.Dot(lhs.Row0, rhs);
            tempV.Y = Vector3.Dot(lhs.Row1, rhs);
            tempV.Z = Vector3.Dot(lhs.Row2, rhs);
            return tempV;
        }

        #endregion
        #region Conversions

        public static Matrix3 matrix4ToMatrix3(Matrix4 matrix)
        {
            return new Matrix3(new Vector3(matrix.Row0), new Vector3(matrix.Row1), new Vector3(matrix.Row2));
        }

        public static Vector3 ExtractMaxVector(Vector3 left, Vector3 right)
        {
            Vector3 max = new Vector3(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y), Math.Max(left.Z, right.Z));
            return max;
        }

        public static Vector3 ExtractMinVector(Vector3 left, Vector3 right)
        {
            Vector3 min = new Vector3(Math.Min(left.X, right.X), Math.Max(left.Y, right.Y), Math.Max(left.Z, right.Z));
            return min;
        }

        #endregion
    }
}
