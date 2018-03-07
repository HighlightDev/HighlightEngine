using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public static class Geometric
    {
        public static Vector3f cross(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f(
                lhs.y * rhs.z - rhs.y * lhs.z,
                lhs.z * rhs.x - rhs.z * lhs.x,
                lhs.x * rhs.y - rhs.x * lhs.y);
        }

        public static float Dot(Vector2f x, Vector2f y)
        {
            Vector2f tmp = new Vector2f(x * y);
            return tmp.x + tmp.y;
        }

        public static float Dot(Vector3f x, Vector3f y)
        {
            Vector3f tmp = new Vector3f(x * y);
            return tmp.x + tmp.y + tmp.z;
        }

        public static float Dot(Vector4f x, Vector4f y)
        {
            Vector4f tmp = new Vector4f(x * y);
            return (tmp.x + tmp.y) + (tmp.z + tmp.w);
        }

        public static Vector2f Normalize(Vector2f v)
        {
            float sqr = v.x * v.x + v.y * v.y;
            return v * (1.0f / (float)Math.Sqrt(sqr));
        }

        public static Vector3f Normalize(Vector3f v)
        {
            float sqr = v.x * v.x + v.y * v.y + v.z * v.z;
            return v * (1.0f / (float)Math.Sqrt(sqr));
        }

        public static Vector4f Normalize(Vector4f v)
        {
            float sqr = v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w;
            return v * (1.0f / (float)Math.Sqrt(sqr));
        }
    }
}
