using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace OGLMatrixMath
{
    public static class MatrixMath
    {
        public static Vector3f normal(Vector3f Vertex1, Vector3f Vertex2, Vector3f Vertex3)
        {
            float wrki;
            Vector3f n = new Vector3f();
            Vector3f v1 = new Vector3f(), v2 = new Vector3f();

            v1 = Vertex1 - Vertex2;

            v2 = Vertex2 - Vertex3;

            wrki = Convert.ToSingle(Math.Sqrt(Sqr(v1.y * v2.z - v1.z * v2.y) + Sqr(v1.z * v2.x - v1.x * v2.z) + Sqr(v1.x * v2.y - v1.y * v2.x)));
            n.x = (v1.y * v2.z - v1.z * v2.y) / wrki;
            n.y = (v1.z * v2.x - v1.x * v2.z) / wrki;
            n.z = (v1.x * v2.y - v1.y * v2.x) / wrki;
            return n;
        }
        private static float Sqr(float x) { return x * x; }

        public static Matrix4f createModelMatrix(Vector3f translation, Vector3f rotate, Vector3f scale)
        {
            Matrix4f modelMatrix = Matrix4f.identity();
            modelMatrix = translate(modelMatrix, translation);
            modelMatrix = MatrixMath.rotate(modelMatrix, Trigonometric.toRadians(rotate.x), new Vector3f(1, 0, 0));
            modelMatrix = MatrixMath.rotate(modelMatrix, Trigonometric.toRadians(rotate.y), new Vector3f(0, 1, 0));
            modelMatrix = MatrixMath.rotate(modelMatrix, Trigonometric.toRadians(rotate.z), new Vector3f(0, 0, 1));
            modelMatrix = MatrixMath.scale(modelMatrix, scale);
            return modelMatrix;
        }

        public static Matrix4f createViewMatrix(Vector3f AtVec, Vector3f EyeVec, Vector3f UpVec)
        {
            Matrix4f viewMatrix = Matrix4f.identity();
            viewMatrix = lookAt(AtVec, EyeVec, UpVec);
            return viewMatrix;
        }

        public static Matrix4f createProjectionMatrix(float FovY, float aspectRatio, float zNear, float zFar)
        {
            Matrix4f projectionMatrix = new Matrix4f(1);
            projectionMatrix = perspective(Trigonometric.toRadians(FovY), aspectRatio, zNear, zFar);
            return projectionMatrix;
        }

        public static Matrix3f createNormalMatrix(Matrix4f viewMatrix, Matrix4f modelMatrix)
        {
            Matrix3f result;
            result = MatrixTransforms.Transpose(MatrixTransforms.Inverse(viewMatrix * modelMatrix)).toMatrix3();
            return result;
        }

        /// <summary>
        /// Creates a frustrum projection matrix.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <param name="nearVal">The near val.</param>
        /// <param name="farVal">The far val.</param>
        /// <returns></returns>
        public static Matrix4f frustum(float left, float right, float bottom, float top, float nearVal, float farVal)
        {
            var result = new Matrix4f(1);
            result[0, 0] = (2.0f * nearVal) / (right - left);
            result[1, 1] = (2.0f * nearVal) / (top - bottom);
            result[2, 0] = (right + left) / (right - left);
            result[2, 1] = (top + bottom) / (top - bottom);
            result[2, 2] = -(farVal + nearVal) / (farVal - nearVal);
            result[2, 3] = -1.0f;
            result[3, 2] = -(2.0f * farVal * nearVal) / (farVal - nearVal);
            return result;
        }

        /// <summary>
        /// Creates a matrix for a symmetric perspective-view frustum with far plane at infinite.
        /// </summary>
        /// <param name="fovy">The fovy.</param>
        /// <param name="aspect">The aspect.</param>
        /// <param name="zNear">The z near.</param>
        /// <returns></returns>
        public static Matrix4f infinitePerspective(float fovy, float aspect, float zNear)
        {

            float range = Trigonometric.Tan(fovy / (2f)) * zNear;

            float left = -range * aspect;
            float right = range * aspect;
            float bottom = -range;
            float top = range;

            var result = new Matrix4f(0);
            result[0, 0] = ((2f) * zNear) / (right - left);
            result[1, 1] = ((2f) * zNear) / (top - bottom);
            result[2, 2] = -(1f);
            result[2, 3] = -(1f);
            result[3, 2] = -(2f) * zNear;
            return result;
        }

        /// <summary>
        /// Build a look at view matrix.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <param name="center">The center.</param>
        /// <param name="up">Up.</param>
        /// <returns></returns>
        public static Matrix4f lookAt(Vector3f eye, Vector3f center, Vector3f up)
        {
            Vector3f f = new Vector3f(Geometric.Normalize(center - eye));
            Vector3f s = new Vector3f(Geometric.Normalize(Geometric.cross(f, up)));
            Vector3f u = new Vector3f(Geometric.cross(s, f));

            Matrix4f Result = new Matrix4f(1);
            Result[0, 0] = s.x;
            Result[1, 0] = s.y;
            Result[2, 0] = s.z;
            Result[0, 1] = u.x;
            Result[1, 1] = u.y;
            Result[2, 1] = u.z;
            Result[0, 2] = -f.x;
            Result[1, 2] = -f.y;
            Result[2, 2] = -f.z;
            Result[3, 0] = -Geometric.Dot(s, eye);
            Result[3, 1] = -Geometric.Dot(u, eye);
            Result[3, 2] = Geometric.Dot(f, eye);
            return Result;
        }

        /// <summary>
        /// Creates a matrix for an orthographic parallel viewing volume.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <param name="zNear">The z near.</param>
        /// <param name="zFar">The z far.</param>
        /// <returns></returns>
        public static Matrix4f ortho(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            var result = new Matrix4f(1);
            result[0, 0] = (2f) / (right - left);
            result[1, 1] = (2f) / (top - bottom);
            result[2, 2] = -(2f) / (zFar - zNear);
            result[3, 0] = -(right + left) / (right - left);
            result[3, 1] = -(top + bottom) / (top - bottom);
            result[3, 2] = -(zFar + zNear) / (zFar - zNear);
            return result;
        }

        /// <summary>
        /// Creates a matrix for projecting two-dimensional coordinates onto the screen.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <returns></returns>
        public static Matrix4f ortho(float left, float right, float bottom, float top)
        {
            var result = new Matrix4f(1);
            result[0, 0] = (2f) / (right - left);
            result[1, 1] = (2f) / (top - bottom);
            result[2, 2] = -(1f);
            result[3, 0] = -(right + left) / (right - left);
            result[3, 1] = -(top + bottom) / (top - bottom);
            return result;
        }

        /// <summary>
        /// Creates a perspective transformation matrix.
        /// </summary>
        /// <param name="fovy">The field of view angle, in radians.</param>
        /// <param name="aspect">The aspect ratio.</param>
        /// <param name="zNear">The near depth clipping plane.</param>
        /// <param name="zFar">The far depth clipping plane.</param>
        /// <returns>A <see cref="mat4"/> that contains the projection matrix for the perspective transformation.</returns>
        public static Matrix4f perspective(float fovy, float aspect, float zNear, float zFar)
        {
            var tanHalfFovy = (float)Math.Tan(fovy / 2.0f);

            Matrix4f result = new Matrix4f(1.0f);
            result[0, 0] = 1.0f / (aspect * tanHalfFovy);
            result[1, 1] = 1.0f / (tanHalfFovy);
            result[2, 2] = -(zFar + zNear) / (zFar - zNear);
            result[2, 3] = -1.0f;
            result[3, 2] = -(2.0f * zFar * zNear) / (zFar - zNear);
            return result;
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">The fov (in radians).</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="zNear">The z near.</param>
        /// <param name="zFar">The z far.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static Matrix4f perspectiveFov(float fov, float width, float height, float zNear, float zFar)
        {
            if (width <= 0 || height <= 0 || fov <= 0)
                throw new ArgumentOutOfRangeException();

            var rad = fov;

            var h = Trigonometric.Cos((0.5f) * rad) / Trigonometric.Sin((0.5f) * rad);
            var w = h * height / width;

            var result = new Matrix4f(0);
            result[0, 0] = w;
            result[1, 1] = h;
            result[2, 2] = -(zFar + zNear) / (zFar - zNear);
            result[2, 3] = -(1f);
            result[3, 2] = -((2f) * zFar * zNear) / (zFar - zNear);
            return result;
        }

        /// <summary>
        /// Define a picking region.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="delta">The delta.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static Matrix4f pickMatrix(Vector2f center, Vector2f delta, Vector4f viewport)
        {
            if (delta.x <= 0 || delta.y <= 0)
                throw new ArgumentOutOfRangeException();
            var Result = new Matrix4f(1.0f);

            if (!(delta.x > (0f) && delta.y > (0f)))
                return Result; // Error

            Vector3f Temp = new Vector3f(
                ((viewport[2]) - (2f) * (center.x - (viewport[0]))) / delta.x,
                ((viewport[3]) - (2f) * (center.y - (viewport[1]))) / delta.y,
                (0f));

            // Translate and scale the picked region to the entire window
            Result = translate(Result, Temp);
            return scale(Result, new Vector3f((viewport[2]) / delta.x, (viewport[3]) / delta.y, (1)));
        }

        /// <summary>
        /// Map the specified object coordinates (obj.x, obj.y, obj.z) into window coordinates.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="model">The model.</param>
        /// <param name="proj">The proj.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        public static Vector3f project(Vector3f obj, Matrix4f model, Matrix4f proj, Vector4f viewport)
        {
            Vector4f tmp = new Vector4f(obj, (1f));
            tmp = model * tmp;
            tmp = proj * tmp;

            tmp /= tmp.w;
            tmp = tmp * 0.5f + 0.5f;
            tmp[0] = tmp[0] * viewport[2] + viewport[0];
            tmp[1] = tmp[1] * viewport[3] + viewport[1];

            return new Vector3f(tmp.x, tmp.y, tmp.z);
        }

        /// <summary>
        /// Builds a rotation 4 * 4 matrix created from an axis vector and an angle.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static Matrix4f rotate(Matrix4f m, float angle, Vector3f v)
        {
            float c = Trigonometric.Cos(angle);
            float s = Trigonometric.Sin(angle);

            Vector3f axis = Geometric.Normalize(v);
            Vector3f temp = (1.0f - c) * axis;

            Matrix4f rotate = Matrix4f.identity();
            rotate[0, 0] = c + temp[0] * axis[0];
            rotate[0, 1] = 0 + temp[0] * axis[1] + s * axis[2];
            rotate[0, 2] = 0 + temp[0] * axis[2] - s * axis[1];

            rotate[1, 0] = 0 + temp[1] * axis[0] - s * axis[2];
            rotate[1, 1] = c + temp[1] * axis[1];
            rotate[1, 2] = 0 + temp[1] * axis[2] + s * axis[0];

            rotate[2, 0] = 0 + temp[2] * axis[0] + s * axis[1];
            rotate[2, 1] = 0 + temp[2] * axis[1] - s * axis[0];
            rotate[2, 2] = c + temp[2] * axis[2];

            Matrix4f result = Matrix4f.identity();
            result[0] = m[0] * rotate[0][0] + m[1] * rotate[0][1] + m[2] * rotate[0][2];
            result[1] = m[0] * rotate[1][0] + m[1] * rotate[1][1] + m[2] * rotate[1][2];
            result[2] = m[0] * rotate[2][0] + m[1] * rotate[2][1] + m[2] * rotate[2][2];
            result[3] = m[3];
            return result;
        }

        public static Matrix4f rotate(float angle, Vector3f v)
        {
            return rotate(Matrix4f.identity(), angle, v);
        }


        /// <summary>
        /// Applies a scale transformation to matrix <paramref name="m"/> by vector <paramref name="v"/>.
        /// </summary>
        /// <param name="m">The matrix to transform.</param>
        /// <param name="v">The vector to scale by.</param>
        /// <returns><paramref name="m"/> scaled by <paramref name="v"/>.</returns>
        public static Matrix4f scale(Matrix4f m, Vector3f v)
        {
            Matrix4f result = m;
            result[0] = m[0] * v[0];
            result[1] = m[1] * v[1];
            result[2] = m[2] * v[2];
            result[3] = m[3];
            return result;
        }

        /// <summary>
        /// Applies a translation transformation to matrix <paramref name="m"/> by vector <paramref name="v"/>.
        /// </summary>
        /// <param name="m">The matrix to transform.</param>
        /// <param name="v">The vector to translate by.</param>
        /// <returns><paramref name="m"/> translated by <paramref name="v"/>.</returns>
        public static Matrix4f translate(Matrix4f m, Vector3f v)
        {
            Matrix4f result = m;
            result[3] = (m[0] * v[0]) + (m[1] * v[1]) + (m[2] * v[2]) + m[3];
            result[3][3] = 1.0f;
            return result;
        }

        /// <summary>
        /// Creates a matrix for a symmetric perspective-view frustum with far plane 
        /// at infinite for graphics hardware that doesn't support depth clamping.
        /// </summary>
        /// <param name="fovy">The fovy.</param>
        /// <param name="aspect">The aspect.</param>
        /// <param name="zNear">The z near.</param>
        /// <returns></returns>
        public static Matrix4f tweakedInfinitePerspective(float fovy, float aspect, float zNear)
        {
            float range = Trigonometric.Tan(fovy / (2)) * zNear;
            float left = -range * aspect;
            float right = range * aspect;
            float bottom = -range;
            float top = range;

            Matrix4f Result = new Matrix4f((0f));
            Result[0, 0] = ((2) * zNear) / (right - left);
            Result[1, 1] = ((2) * zNear) / (top - bottom);
            Result[2, 2] = (0.0001f) - (1f);
            Result[2, 3] = (-1);
            Result[3, 2] = -((0.0001f) - (2)) * zNear;
            return Result;
        }

        /// <summary>
        /// Map the specified window coordinates (win.x, win.y, win.z) into object coordinates.
        /// </summary>
        /// <param name="win">The win.</param>
        /// <param name="model">The model.</param>
        /// <param name="proj">The proj.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        public static Vector3f unProject(Vector3f win, Matrix4f model, Matrix4f proj, Vector4f viewport)
        {
            Matrix4f Inverse = MatrixTransforms.Inverse(proj * model);

            Vector4f tmp = new Vector4f(win, (1f));
            tmp.x = (tmp.x - (viewport[0])) / (viewport[2]);
            tmp.y = (tmp.y - (viewport[1])) / (viewport[3]);
            tmp = tmp * (2f) - (1f);

            Vector4f obj = Inverse * tmp;
            obj /= obj.w;

            return new Vector3f(obj);
        }
    }
}
