using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace OGLMatrixMath
{
    public class Matrix4
    {
        public dynamic this[int index]
        {
            set
            {
                if (index == 0) { x1 = value; }
                else if (index == 1) { y1 = value; }
                else if (index == 2) { z1 = value; }
                else if (index == 3) { w1 = value; }
                else if (index == 4) { x2 = value; }
                else if (index == 5) { y2 = value; }
                else if (index == 6) { z2 = value; }
                else if (index == 7) { w2 = value; }
                else if (index == 8) { x3 = value; }
                else if (index == 9) { y3 = value; }
                else if (index == 10) { z3 = value; }
                else if (index == 11) { w3 = value; }
                else if (index == 12) { x4 = value; }
                else if (index == 13) { y4 = value; }
                else if (index == 14) { z4 = value; }
                else if (index == 15) { w4 = value; }
            }
            get { return _matrix[index]; }
        }

        public dynamic[] Matrix { get { return _matrix; } }
        private dynamic[] _matrix { set; get; }
        private dynamic _x1, _x2, _x3, _x4, _y1, _y2, _y3, _y4, _z1, _z2, _z3, _z4, _w1, _w2, _w3, _w4;

        public dynamic x1 { set { _x1 = value; _matrix[0] = _x1; } get { return _x1; } }
        public dynamic y1 { set { _y1 = value; _matrix[1] = _y1; } get { return _y1; } }
        public dynamic z1 { set { _z1 = value; _matrix[2] = _z1; } get { return _z1; } }
        public dynamic w1 { set { _w1 = value; _matrix[3] = _w1; } get { return _w1; } }
        public dynamic x2 { set { _x2 = value; _matrix[4] = _x2; } get { return _x2; } }
        public dynamic y2 { set { _y2 = value; _matrix[5] = _y2; } get { return _y2; } }
        public dynamic z2 { set { _z2 = value; _matrix[6] = _z2; } get { return _z2; } }
        public dynamic w2 { set { _w2 = value; _matrix[7] = _w2; } get { return _w2; } }
        public dynamic x3 { set { _x3 = value; _matrix[8] = _x3; } get { return _x3; } }
        public dynamic y3 { set { _y3 = value; _matrix[9] = _y3; } get { return _y3; } }
        public dynamic z3 { set { _z3 = value; _matrix[10] = _z3; } get { return _z3; } }
        public dynamic w3 { set { _w3 = value; _matrix[11] = _w3; } get { return _w3; } }
        public dynamic x4 { set { _x4 = value; _matrix[12] = _x4; } get { return _x4; } }
        public dynamic y4 { set { _y4 = value; _matrix[13] = _y4; } get { return _y4; } }
        public dynamic z4 { set { _z4 = value; _matrix[14] = _z4; } get { return _z4; } }
        public dynamic w4 { set { _w4 = value; _matrix[15] = _w4; } get { return _w4; } }

        public static Matrix4 operator *(Matrix4 matrix1, Matrix4 matrix2)
        {
            Matrix4 result = new Matrix4();
            result.x1 = matrix1.x1 * matrix2.x1 + matrix1.y1 * matrix2.x2 + matrix1.z1 * matrix2.x3 + matrix1.w1 * matrix2.x4;
            result.y1 = matrix1.x1 * matrix2.y1 + matrix1.y1 * matrix2.y2 + matrix1.z1 * matrix2.y3 + matrix1.w1 * matrix2.y4;
            result.z1 = matrix1.x1 * matrix2.z1 + matrix1.y1 * matrix2.z2 + matrix1.z1 * matrix2.z3 + matrix1.w1 * matrix2.z4;
            result.w1 = matrix1.x1 * matrix2.w1 + matrix1.y1 * matrix2.w2 + matrix1.z1 * matrix2.w3 + matrix1.w1 * matrix2.w4;

            result.x2 = matrix1.x2 * matrix2.x1 + matrix1.y2 * matrix2.x2 + matrix1.z2 * matrix2.x3 + matrix1.w2 * matrix2.x4;
            result.y2 = matrix1.x2 * matrix2.y1 + matrix1.y2 * matrix2.y2 + matrix1.z2 * matrix2.y3 + matrix1.w2 * matrix2.y4;
            result.z2 = matrix1.x2 * matrix2.z1 + matrix1.y2 * matrix2.z2 + matrix1.z2 * matrix2.z3 + matrix1.w2 * matrix2.z4;
            result.w2 = matrix1.x2 * matrix2.w1 + matrix1.y2 * matrix2.w2 + matrix1.z2 * matrix2.w3 + matrix1.w2 * matrix2.w4;

            result.x3 = matrix1.x3 * matrix2.x1 + matrix1.y3 * matrix2.x2 + matrix1.z3 * matrix2.x3 + matrix1.w3 * matrix2.x4;
            result.y3 = matrix1.x3 * matrix2.y1 + matrix1.y3 * matrix2.y2 + matrix1.z3 * matrix2.y3 + matrix1.w3 * matrix2.y4;
            result.z3 = matrix1.x3 * matrix2.z1 + matrix1.y3 * matrix2.z2 + matrix1.z3 * matrix2.z3 + matrix1.w3 * matrix2.z4;
            result.w3 = matrix1.x3 * matrix2.w1 + matrix1.y3 * matrix2.w2 + matrix1.z3 * matrix2.w3 + matrix1.w3 * matrix2.w4;

            result.x4 = matrix1.x4 * matrix2.x1 + matrix1.y4 * matrix2.x2 + matrix1.z4 * matrix2.x3 + matrix1.w4 * matrix2.x4;
            result.y4 = matrix1.x4 * matrix2.y1 + matrix1.y4 * matrix2.y2 + matrix1.z4 * matrix2.y3 + matrix1.w4 * matrix2.y4;
            result.z4 = matrix1.x4 * matrix2.z1 + matrix1.y4 * matrix2.z2 + matrix1.z4 * matrix2.z3 + matrix1.w4 * matrix2.z4;
            result.w4 = matrix1.x4 * matrix2.w1 + matrix1.y4 * matrix2.w2 + matrix1.z4 * matrix2.w3 + matrix1.w4 * matrix2.w4;

            return result;
        }

        public Matrix4()
        {
            _matrix = new dynamic[16];
            for (int i = 0; i < 16; i++)
            {
                this[i] = 0.0f;
            }
        }
        public Matrix4(Matrix4 clone)
        {
            _matrix = new dynamic[16];
            x1 = clone.x1;
            x2 = clone.x2;
            x3 = clone.x3;
            x4 = clone.x4;
            y1 = clone.y1;
            y2 = clone.y2;
            y3 = clone.y3;
            y4 = clone.y4;
            z1 = clone.z1;
            z2 = clone.z2;
            z3 = clone.z3;
            z4 = clone.z4;
            w1 = clone.w1;
            w2 = clone.w2;
            w3 = clone.w3;
            w4 = clone.w4;
        }
        public Matrix4(dynamic matrix)
        {
            _matrix = new dynamic[16];
            for (int i = 0; i < 16; i++)
            {
                this[i] = matrix[i];
            }
        }
    }
}
