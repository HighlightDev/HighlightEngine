using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public class Vector2f
    {
        private float _x, _y;

        public float x { set { _x = value; } get { return _x; } }
        public float y { set { _y = value; } get { return _y; } }
        public float this[int index]
        {
            set
            {
                if (index == 0) { this._x = value; }
                else if (index == 1) { this._y = value; }
                else throw new Exception("Out of range.");
            }
            get
            {
                if (index == 0) { return this._x; }
                else if (index == 1) { return this._y; }
                else throw new Exception("Out of range.");
            }
        }


        public static Vector2f operator +(Vector2f leftVec, Vector2f rightVec)
        {
            return new Vector2f(leftVec._x + rightVec.x,leftVec._y + rightVec._y);
        }
        public static Vector2f operator +(Vector2f leftVec, float value)
        {
            return new Vector2f(leftVec._x + value, leftVec._y + value);
        }
        public static Vector2f operator -(Vector2f leftVec, Vector2f rightVec)
        {
            return new Vector2f(leftVec._x - rightVec.x, leftVec._y - rightVec._y);
        }
        public static Vector2f operator -(Vector2f leftVec, float value)
        {
            return new Vector2f(leftVec._x - value, leftVec._y - value);
        }
        public static Vector2f operator *(Vector2f leftVec, float value)
        {
            return new Vector2f(leftVec._x * value, leftVec._y * value);
        }
        public static Vector2f operator *(float value, Vector2f rightVec)
        {
            return new Vector2f(rightVec._x * value, rightVec._y * value);
        }
        public static Vector2f operator *(Vector2f leftVec, Vector2f rightVec)
        {
            return new Vector2f(leftVec._x * rightVec._x, leftVec._y * rightVec._y);
        }
        public static Vector2f operator /(Vector2f leftVec, float value)
        {
            return new Vector2f(leftVec._x / value, leftVec._y / value);
        }
        public static Vector2f operator -(Vector2f vector)
        {
            Vector2f operatorVector = new Vector2f(vector);
            operatorVector.x = -vector.x;
            operatorVector.y = -vector.y;
            return operatorVector;
        }

        public float[] toArray()
        {
            return new float[2] { _x, _y };
        }

        public Vector2f(float x, float y)
        {
            this._x = x;
            this._y = y;
        }
        public Vector2f()
        {
            _x = 0.0f;
            _y = 0.0f;
        }
        public Vector2f(float value)
        {
            this._x = this._y = 0;
        }
        public Vector2f(float[] array)
        {
            this._x = array[0];
            this._y = array[1];
        }
        public Vector2f(Vector2f clone)
        {
            this._x = clone.x;
            this._y = clone.y;
        }
        public Vector2f(Vector3f vec)
        {
            this._x = vec.x;
            this._y = vec.y;
        }
        public Vector2f(Vector4f vec)
        {
            this._x = vec.x;
            this._y = vec.y;
        }

    }
}
