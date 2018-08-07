using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace ShaderPattern
{
    public class Uniform
    {
        private Int32 uniformLocation;

        internal Uniform(int program, string uniformName)
        {
            uniformLocation = GL.GetUniformLocation(program, uniformName);
            if (uniformLocation < 0)
                throw new ArgumentNullException(String.Format("Could not bind uniform {0}.", uniformName));
        }

        public void LoadUniform(bool arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform1(uniformLocation, arg ? 1.0f : 0.0f);
        }
        public void LoadUniform(float arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform1(uniformLocation, arg);
        }
        public void LoadUniform(Int32 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform1(uniformLocation, arg);
        }
        public void LoadUniform(ref Vector2 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform2(uniformLocation, arg);
        }
        public void LoadUniform(Vector2 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform2(uniformLocation, arg);
        }
        public void LoadUniform(ref Vector3 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform3(uniformLocation, arg);
        }
        public void LoadUniform(Vector3 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform3(uniformLocation, arg);
        }
        public void LoadUniform(ref Vector4 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform4(uniformLocation, arg);
        }
        public void LoadUniform(Vector4 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.Uniform4(uniformLocation, arg);
        }
        public void LoadUniform(ref Matrix2 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.UniformMatrix2(uniformLocation, false, ref arg);
        }
        public void LoadUniform(ref Matrix3 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.UniformMatrix3(uniformLocation, false, ref arg);
        }
        public void LoadUniform(ref Matrix4 arg)
        {
            if (uniformLocation == -1)
                return;
            GL.UniformMatrix4(uniformLocation, false, ref arg);
        }
    }
}
