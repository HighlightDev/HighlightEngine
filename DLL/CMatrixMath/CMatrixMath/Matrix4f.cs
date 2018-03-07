using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public class Matrix4f
    {
        /// <summary>
        /// The columms of the matrix.
        /// </summary>
        private Vector4f[] _cols;

        /// <summary>
        /// Initializes a new instance of the <see cref="mat4"/> struct.
        /// This matrix is the identity matrix scaled by <paramref name="scale"/>.
        /// </summary>
        /// <param name="scale">The scale.</param>
		public Matrix4f(float scale)
        {
            _cols = new []
            {
                new Vector4f(scale, 0.0f, 0.0f, 0.0f),
                new Vector4f(0.0f, scale, 0.0f, 0.0f),
                new Vector4f(0.0f, 0.0f, scale, 0.0f),
                new Vector4f(0.0f, 0.0f, 0.0f, scale),
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="mat4"/> struct.
        /// The matrix is initialised with the <paramref name="cols"/>.
        /// </summary>
        /// <param name="cols">The colums of the matrix.</param>
        public Matrix4f(Vector4f[] cols)
        {
            this._cols = new []
            {
                cols[0],
                cols[1],
                cols[2],
                cols[3]
            };
        }
        public Matrix4f(Matrix4f matrix)
        {
            this._cols[0] = matrix._cols[0];
            this._cols[1] = matrix._cols[1];
            this._cols[2] = matrix._cols[2];
            this._cols[3] = matrix._cols[3];
        }
        public Matrix4f(Vector4f a, Vector4f b, Vector4f c, Vector4f d)
        {
            this._cols = new[]
            {
                a, b, c, d
            };
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <returns>A new identity matrix.</returns>
        public void toIdentity()
        {
            _cols = new Vector4f[] {
                    new Vector4f(1,0,0,0),
                    new Vector4f(0,1,0,0),
                    new Vector4f(0,0,1,0),
                    new Vector4f(0,0,0,1)  };
        }
        public static Matrix4f identity()
        {
            return new Matrix4f(new Vector4f[] {
                    new Vector4f(1,0,0,0),
                    new Vector4f(0,1,0,0),
                    new Vector4f(0,0,1,0),
                    new Vector4f(0,0,0,1)  });
        }
        public Matrix4f(float[] matrix)
        {
            this._cols[0] = new Vector4f(matrix[0], matrix[1], matrix[2], matrix[3]);
            this._cols[1] = new Vector4f(matrix[4], matrix[5], matrix[6], matrix[7]);
            this._cols[2] = new Vector4f(matrix[8], matrix[9], matrix[10], matrix[11]);
            this._cols[3] = new Vector4f(matrix[12], matrix[13], matrix[14], matrix[15]);
        }

        /// <summary>
        /// Gets or sets the <see cref="vec4"/> column at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="vec4"/> column.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <returns>The column at index <paramref name="column"/>.</returns>
        public Vector4f this[int column]
		{
            get { return _cols[column]; }
            set { _cols[column] = value; }
		}

        /// <summary>
        /// Gets or sets the element at <paramref name="column"/> and <paramref name="row"/>.
        /// </summary>
        /// <value>
        /// The element at <paramref name="column"/> and <paramref name="row"/>.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <param name="row">The row index.</param>
        /// <returns>
        /// The element at <paramref name="column"/> and <paramref name="row"/>.
        /// </returns>
        public float this[int column, int row]
        {
            get { return _cols[column][row]; }
            set { _cols[column][row] = value; }
        }

        /// <summary>
        /// Returns the matrix as a flat array of elements, column major.
        /// </summary>
        /// <returns></returns>
        public float[] toArray()
        {
            return _cols.SelectMany(v => v.toArray()).ToArray();
        }

        /// <summary>
        /// Returns the <see cref="mat3"/> portion of this matrix.
        /// </summary>
        /// <returns>The <see cref="mat3"/> portion of this matrix.</returns>
        public Matrix3f toMatrix3()
        {
            return new Matrix3f(new Vector3f[]
            {
			    new Vector3f(_cols[0][0], _cols[0][1], _cols[0][2]),
			    new Vector3f(_cols[1][0], _cols[1][1], _cols[1][2]),
			    new Vector3f(_cols[2][0], _cols[2][1], _cols[2][2])
            });
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> vector.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS vector.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static Vector4f operator *(Matrix4f lhs, Vector4f rhs)
        {
            Vector4f tempV = new Vector4f();
            tempV.x = ((lhs[0][0] * rhs.x) + (lhs[0][1] * rhs.y) + (lhs[0][2] * rhs.z) + (lhs[0][3] * rhs.w));
            tempV.y = ((lhs[1][0] * rhs.x) + (lhs[1][1] * rhs.y) + (lhs[1][2] * rhs.z) + (lhs[1][3] * rhs.w));
            tempV.z = ((lhs[2][0] * rhs.x) + (lhs[2][1] * rhs.y) + (lhs[2][2] * rhs.z) + (lhs[2][3] * rhs.w));
            tempV.w = ((lhs[3][0] * rhs.x) + (lhs[3][1] * rhs.y) + (lhs[3][2] * rhs.z) + (lhs[3][3] * rhs.w));
            return tempV;
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> matrix.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS matrix.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static Matrix4f operator *(Matrix4f lhs, Matrix4f rhs)
        {
            Matrix4f tempM = new Matrix4f(0);
            tempM[0][0] = (lhs[0][0] * rhs[0][0]) + (lhs[0][1] * rhs[1][0]) + (lhs[0][2] * rhs[2][0]) + (lhs[0][3] * rhs[3][0]);
            tempM[1][0] = (lhs[1][0] * rhs[0][0]) + (lhs[1][1] * rhs[1][0]) + (lhs[1][2] * rhs[2][0]) + (lhs[1][3] * rhs[3][0]);
            tempM[2][0] = (lhs[2][0] * rhs[0][0]) + (lhs[2][1] * rhs[1][0]) + (lhs[2][2] * rhs[2][0]) + (lhs[2][3] * rhs[3][0]);
            tempM[3][0] = (lhs[3][0] * rhs[0][0]) + (lhs[3][1] * rhs[1][0]) + (lhs[3][2] * rhs[2][0]) + (lhs[3][3] * rhs[3][0]);

            tempM[0][1] = (lhs[0][0] * rhs[0][1]) + (lhs[0][1] * rhs[1][1]) + (lhs[0][2] * rhs[2][1]) + (lhs[0][3] * rhs[3][1]);
            tempM[1][1] = (lhs[1][0] * rhs[0][1]) + (lhs[1][1] * rhs[1][1]) + (lhs[1][2] * rhs[2][1]) + (lhs[1][3] * rhs[3][1]);
            tempM[2][1] = (lhs[2][0] * rhs[0][1]) + (lhs[2][1] * rhs[1][1]) + (lhs[2][2] * rhs[2][1]) + (lhs[2][3] * rhs[3][1]);
            tempM[3][1] = (lhs[3][0] * rhs[0][1]) + (lhs[3][1] * rhs[1][1]) + (lhs[3][2] * rhs[2][1]) + (lhs[3][3] * rhs[3][1]);

            tempM[0][2] = (lhs[0][0] * rhs[0][2]) + (lhs[0][1] * rhs[1][2]) + (lhs[0][2] * rhs[2][2]) + (lhs[0][3] * rhs[3][2]);
            tempM[1][2] = (lhs[1][0] * rhs[0][2]) + (lhs[1][1] * rhs[1][2]) + (lhs[1][2] * rhs[2][2]) + (lhs[1][3] * rhs[3][2]);
            tempM[2][2] = (lhs[2][0] * rhs[0][2]) + (lhs[2][1] * rhs[1][2]) + (lhs[2][2] * rhs[2][2]) + (lhs[2][3] * rhs[3][2]);
            tempM[3][2] = (lhs[3][0] * rhs[0][2]) + (lhs[3][1] * rhs[1][2]) + (lhs[3][2] * rhs[2][2]) + (lhs[3][3] * rhs[3][2]);

            tempM[0][3] = (lhs[0][0] * rhs[0][3]) + (lhs[0][1] * rhs[1][3]) + (lhs[0][2] * rhs[2][3]) + (lhs[0][3] * rhs[3][3]);
            tempM[1][3] = (lhs[1][0] * rhs[0][3]) + (lhs[1][1] * rhs[1][3]) + (lhs[1][2] * rhs[2][3]) + (lhs[1][3] * rhs[3][3]);
            tempM[2][3] = (lhs[2][0] * rhs[0][3]) + (lhs[2][1] * rhs[1][3]) + (lhs[2][2] * rhs[2][3]) + (lhs[2][3] * rhs[3][3]);
            tempM[3][3] = (lhs[3][0] * rhs[0][3]) + (lhs[3][1] * rhs[1][3]) + (lhs[3][2] * rhs[2][3]) + (lhs[3][3] * rhs[3][3]);
            return tempM;
        }

        public static Matrix4f operator *(Matrix4f lhs, float s)
        {
            return new Matrix4f(new[]
            {
                lhs[0]*s,
                lhs[1]*s,
                lhs[2]*s,
                lhs[3]*s
            });
        }
    }
}
