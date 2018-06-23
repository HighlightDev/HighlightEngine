using OpenTK;

namespace ShaderPattern
{
    public static class ConverterHelper
    {
        public enum KnownTypes
        {
            Undefined,
            Int,
            Float,
            Bool,
            Vec2,
            Vec3,
            Vec4,
            Mat2,
            Mat3,
            Mat4
        }

        public static KnownTypes GetValueType<T>()
        {
            KnownTypes result = KnownTypes.Undefined;

            var type = typeof(T);

            if (type == typeof(int))
                result = KnownTypes.Int;
            else if (type == typeof(float))
                result = KnownTypes.Float;
            else if (type == typeof(bool))
                result = KnownTypes.Bool;
            else if (type == typeof(Vector2))
                result = KnownTypes.Vec2;
            else if (type == typeof(Vector3))
                result = KnownTypes.Vec3;
            else if (type == typeof(Vector4))
                result = KnownTypes.Vec4;
            else if (type == typeof(Matrix2))
                result = KnownTypes.Mat2;
            else if (type == typeof(Matrix3))
                result = KnownTypes.Mat3;
            else if (type == typeof(Matrix4))
                result = KnownTypes.Mat4;

            return result;
        }
    }

    public static class ShaderMacrosConverter<T> where T : struct
    {
        public static string ConvertToString(T macroDefine)
        {
            string result = macroDefine.ToString();

            switch (ConverterHelper.GetValueType<T>())
            {
                case ConverterHelper.KnownTypes.Int: break;
                case ConverterHelper.KnownTypes.Float: result = result.Replace(',', '.'); break;
                case ConverterHelper.KnownTypes.Bool: break;
                case ConverterHelper.KnownTypes.Vec2:
                    {
                        result = result.Replace(',', '.');
                        result = "vec2" + result.Replace(';', ',');
                        break;
                    }
                case ConverterHelper.KnownTypes.Vec3:
                    {
                        result = result.Replace(',', '.');
                        result = "vec3" + result.Replace(';', ',');
                        break;
                    }
                case ConverterHelper.KnownTypes.Vec4:
                    {
                        result = result.Replace(',', '.');
                        result = "vec4" + result.Replace(';', ',');
                        break;
                    }
                case ConverterHelper.KnownTypes.Mat2:
                    {
                        result = result.Replace(',', '.');
                        result = "mat2" + result.Replace(';', ',');
                        break;
                    }
                case ConverterHelper.KnownTypes.Mat3:
                    {
                        result = result.Replace(',', '.');
                        result = "mat3" + result.Replace(';', ',');
                        break;
                    }
                case ConverterHelper.KnownTypes.Mat4:
                    {
                        result = result.Replace(',', '.');
                        result = "mat4" + result.Replace(';', ',');
                        break;
                    }
            }

            return result;
        }
    }
}
