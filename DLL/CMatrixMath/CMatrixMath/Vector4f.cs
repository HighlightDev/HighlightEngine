using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public class Vector4f
    {
        private float _x, _y, _z,_w;
        public float this[int index]
        {
            set
            {
                if (index == 0) { this._x = value; }
                else if (index == 1) { this._y = value; }
                else if (index == 2) { this._z = value; }
                else if (index == 3) { this._w = value; }
                else throw new Exception("Out of range.");
            }
            get
            {
                if (index == 0) { return this._x; }
                else if (index == 1) { return this._y; }
                else if (index == 2) { return this._z; }
                else if (index == 3) { return this._w; }
                else throw new Exception("Out of range.");
            }
        }
        public float x
        {
            set
            {
                this._x = value;
            }
            get { return this._x; }
        }
        public float y
        {
            set
            {
                this._y = value;
            }
            get { return this._y; }
        }
        public float z
        {
            set
            {
                this._z = value;
            }
            get { return this._z; }
        }
        public float w
        {
            set
            {
                this._w = value;
            }
            get { return this._w; }
        }

        public static Vector4f operator -(Vector4f vec1, Vector4f vec2)
        {
            return Vector4f.vectorBinaryOperations(vec1, vec2, BinaryOperations.SUBSTRACTION);
        }
        public static Vector4f operator -(Vector4f leftVec, float value)
        {
            return new Vector4f(leftVec._x - value, leftVec._y - value, leftVec._z - value,leftVec._w - value);
        }
        public static Vector4f operator +(Vector4f vec1, Vector4f vec2)
        {
            return Vector4f.vectorBinaryOperations(vec1, vec2, BinaryOperations.ADDITION);
        }
        public static Vector4f operator +(Vector4f leftVec, float value)
        {
            return new Vector4f(leftVec._x + value, leftVec._y + value, leftVec._z + value,leftVec._w + value);
        }
        public static Vector4f operator *(Vector4f vec1, Vector4f vec2)
        {
            return Vector4f.vectorBinaryOperations(vec1, vec2, BinaryOperations.MULTIPLICATION);
        }
        public static Vector4f operator *(Vector4f leftVec, float value)
        {
            return new Vector4f(leftVec._x * value, leftVec._y * value, leftVec._z * value,leftVec._w * value);
        }
        public static Vector4f operator *(float value, Vector4f rightVec)
        {
            return new Vector4f(rightVec._x * value, rightVec._y * value, rightVec._z * value,rightVec._w * value);
        }
        public static Vector4f operator /(Vector4f vec1, float value)
        {
            return new Vector4f(vec1._x / value, vec1._y / value, vec1._z / value, vec1._w / value);
        }
        public static Vector4f operator -(Vector4f vector)
        {
            Vector4f operatorVector = new Vector4f(vector);
            operatorVector.x = -vector.x;
            operatorVector.y = -vector.y;
            operatorVector.z = -vector.z;
            operatorVector.w = -vector.w;
            return operatorVector;
        }

        public float[] toArray()
        {
            return new float[4] { _x, _y, _z, _w };
        }

        /// <summary>
        /// Математические операции с двумя векторами
        /// </summary>
        /// <param name="vector1">Первый вектор</param>
        /// <param name="vector2">Второй вектор</param>
        /// <param name="operation">Тип операции</param>
        /// <returns></returns>
        public static Vector4f vectorBinaryOperations(Vector4f vector1, Vector4f vector2, BinaryOperations operation)
        {
            Vector4f resultVector = new Vector4f();
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
        public static Vector4f vectorSingleOperations(Vector4f vector, UnoOperations operation)
        {
            Vector4f resultVector = new Vector4f();
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
                        resultVector.x = Convert.ToSingle(Math.Pow(vector.x, 2));
                        resultVector.y = Convert.ToSingle(Math.Pow(vector.y, 2));
                        resultVector.z = Convert.ToSingle(Math.Pow(vector.z, 2));
                        break;
                    }
                case UnoOperations.SQRT:
                    {
                        resultVector.x = Convert.ToSingle(Math.Sqrt(vector.x));
                        resultVector.y = Convert.ToSingle(Math.Sqrt(vector.y));
                        resultVector.z = Convert.ToSingle(Math.Sqrt(vector.z));
                        break;
                    }
            }
            return resultVector;
        }
        public Vector4f()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
            w = 0.0f;
        }
        public Vector4f(float value)
        {
            this._x = this._y = this._z = this._w = value;
        }
        public Vector4f(float x, float y, float z,float w)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
        }
        public Vector4f(Vector4f clone)
        {
            this.x = clone.x;
            this.y = clone.y;
            this.z = clone.z;
            this.w = clone.w;
        }
        public Vector4f(Vector3f clone, float w)
        {
            this.x = clone.x;
            this.y = clone.y;
            this.z = clone.z;
            this.w = w;
        }
    }
}
