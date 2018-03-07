using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace OGLMatrixMath
{
    public class Vector4
    {
        public dynamic this[int index]
        {
            set
            {
                if (index == 0) { this.x = value; }
                else if (index == 1) { this.y = value; }
                else if (index == 2) { this.z = value; }
                else if (index == 3) { this.w = value; }
            }
            get { return _vector[index]; }
        }

        private dynamic[] _vector = new dynamic[4];
        private dynamic _x, _y, _z,_w;

        public dynamic x
        {
            set
            {
                this._x = value;
                _vector[0] = value;
            }
            get { return this._x; }
        }
        public dynamic y
        {
            set
            {
                this._y = value;
                _vector[1] = value;
            }
            get { return this._y; }
        }
        public dynamic z
        {
            set
            {
                this._z = value;
                _vector[2] = value;
            }
            get { return this._z; }
        }
        public dynamic w
        {
            set
            {
                this._w = value;
                _vector[3] = value;
            }
            get { return this._w; }
        }

        public static Vector4 operator +(Vector4 vec1, Vector4 vec2)
        {
            return Vector4.vectorBinaryOperations(vec1, vec2, BinaryOperations.ADDITION);
        }
        public static Vector4 operator -(Vector4 vec1, Vector4 vec2)
        {
            return Vector4.vectorBinaryOperations(vec1, vec2, BinaryOperations.SUBSTRACTION);
        }
        public static Vector4 operator *(Vector4 vec1, Vector4 vec2)
        {
            return Vector4.vectorBinaryOperations(vec1, vec2, BinaryOperations.MULTIPLICATION);
        }
        public static Vector4 operator /(Vector4 vec1, Vector4 vec2)
        {
            return Vector4.vectorBinaryOperations(vec1, vec2, BinaryOperations.DIVISION);
        }

        /// <summary>
        /// Математические операции с двумя векторами
        /// </summary>
        /// <param name="vector1">Первый вектор</param>
        /// <param name="vector2">Второй вектор</param>
        /// <param name="operation">Тип операции</param>
        /// <returns></returns>
        public static Vector4 vectorBinaryOperations(Vector4 vector1, Vector4 vector2, BinaryOperations operation)
        {
            Vector4 resultVector = new Vector4();
            switch (operation)
            {
                case BinaryOperations.ADDITION:
                    {
                        resultVector.x = vector1.x + vector2.x;
                        resultVector.y = vector1.y + vector2.y;
                        resultVector.z = vector1.z + vector2.z;
                        resultVector.w = (vector1.w + vector2.w) > 1 ? 1 : 0; 
                        break;
                    }
                case BinaryOperations.SUBSTRACTION:
                    {
                        resultVector.x = vector1.x - vector2.x;
                        resultVector.y = vector1.y - vector2.y;
                        resultVector.z = vector1.z - vector2.z;
                        resultVector.w = (vector1.w - vector2.w) <= 0 ? 0 : 1; 
                        break;
                    }
                case BinaryOperations.MULTIPLICATION:
                    {
                        resultVector.x = vector1.x * vector2.x;
                        resultVector.y = vector1.y * vector2.y;
                        resultVector.z = vector1.z * vector2.z;
                        resultVector.w = vector1.w * vector2.w;
                        break;
                    }
                case BinaryOperations.DIVISION:
                    {
                        resultVector.x = vector1.x / vector2.x;
                        resultVector.y = vector1.y / vector2.y;
                        resultVector.z = vector1.z / vector2.z;
                        resultVector.w = vector2.w == 0 ? vector1.w : vector1.w / vector2.w;
                        break;
                    }
            }
            return resultVector;
        }
        /// <summary>
        /// Математические операции с одним вектором
        /// </summary>
        /// <param name="vector">Вектор</param>
        /// <param name="operation">Тип операции</param>
        /// <returns></returns>
        public static Vector4 vectorSingleOperations(Vector4 vector, UnoOperations operation)
        {
            Vector4 resultVector = new Vector4();
            switch (operation)
            {
                case UnoOperations.ABS:
                    {
                        resultVector.x = Math.Abs(vector.x);
                        resultVector.y = Math.Abs(vector.y);
                        resultVector.z = Math.Abs(vector.z);
                        break;
                    }
                case UnoOperations.SQR:
                    {
                        resultVector.x = Math.Pow(vector.x, 2);
                        resultVector.y = Math.Pow(vector.y, 2);
                        resultVector.z = Math.Pow(vector.z, 2);
                        break;
                    }
                case UnoOperations.SQRT:
                    {
                        resultVector.x = Math.Sqrt(vector.x);
                        resultVector.y = Math.Sqrt(vector.y);
                        resultVector.z = Math.Sqrt(vector.z);
                        break;
                    }
            }
            return resultVector;
        }
        public Vector4()
        {
            x = 0.0;
            y = 0.0;
            z = 0.0;
            w = 0.0;
        }
        public Vector4(dynamic X, dynamic Y, dynamic Z,dynamic W)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }
        public Vector4(Vector4 clone)
        {
            this.x = clone.x;
            this.y = clone.y;
            this.z = clone.z;
            this.w = clone.w;
        }
        public Vector4(Vector3 clone)
        {
            this.x = clone.x;
            this.y = clone.y;
            this.z = clone.z;
            this.w = 1.0;
        }
    }
}
