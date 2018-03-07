using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public class Matrix3f
    {
        /// <summary>
        /// The columms of the matrix.
        /// </summary>
        private Vector3f[] _cols;

        /// <summary>
        /// Initializes a new instance of the <see cref="mat3"/> struct.
        /// This matrix is the identity matrix scaled by <paramref name="scale"/>.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public Matrix3f(float scale)
        {
            _cols = new[]
            {
                new Vector3f(scale, 0.0f, 0.0f),
                new Vector3f(0.0f, scale, 0.0f),
                new Vector3f(0.0f, 0.0f, scale)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="mat3"/> struct.
        /// The matrix is initialised with the <paramref name="cols"/>.
        /// </summary>
        /// <param name="cols">The colums of the matrix.</param>
        public Matrix3f(Vector3f[] cols)
        {
            this._cols = new[]
            {
                cols[0],
                cols[1],
                cols[2]
            };
        }

        public Matrix3f(Vector3f a, Vector3f b, Vector3f c)
        {
            this._cols = new[]
            {
                a, b, c
            };
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <returns>A new identity matrix.</returns>
        public void toIdentity()
        {
            this._cols = new[] 
                {
                    new Vector3f(1,0,0),
                    new Vector3f(0,1,0),
                    new Vector3f(0,0,1)
                };
        }
        /// <summary>
        /// Gets or sets the <see cref="vec3"/> column at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="vec3"/> column.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <returns>The column at index <paramref name="column"/>.</returns>
        public Vector3f this[int column]
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
        public Matrix2f toMatrix2()
        {
            return new Matrix2f(new[] {
			new Vector2f(_cols[0][0], _cols[0][1]),
			new Vector2f(_cols[1][0], _cols[1][1])});
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> vector.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS vector.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static Vector3f operator *(Matrix3f lhs, Vector3f rhs)
        {
            Vector3f tempV = new Vector3f();
            tempV.x = ((lhs[0][0] * rhs.x) + (lhs[0][1] * rhs.y) + (lhs[0][2] * rhs.z));
            tempV.y = ((lhs[1][0] * rhs.x) + (lhs[1][1] * rhs.y) + (lhs[1][2] * rhs.z));
            tempV.z = ((lhs[2][0] * rhs.x) + (lhs[2][1] * rhs.y) + (lhs[2][2] * rhs.z));
            return tempV;
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> matrix.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS matrix.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static Matrix3f operator *(Matrix3f lhs, Matrix3f rhs)
        {
            Matrix3f tempM = new Matrix3f(0);
            tempM[0][0] = (lhs[0][0] * rhs[0][0]) + (lhs[0][1] * rhs[1][0]) + (lhs[0][2] * rhs[2][0]);
            tempM[1][0] = (lhs[1][0] * rhs[0][0]) + (lhs[1][1] * rhs[1][0]) + (lhs[1][2] * rhs[2][0]);
            tempM[2][0] = (lhs[2][0] * rhs[0][0]) + (lhs[2][1] * rhs[1][0]) + (lhs[2][2] * rhs[2][0]);

            tempM[0][1] = (lhs[0][0] * rhs[0][1]) + (lhs[0][1] * rhs[1][1]) + (lhs[0][2] * rhs[2][1]);
            tempM[1][1] = (lhs[1][0] * rhs[0][1]) + (lhs[1][1] * rhs[1][1]) + (lhs[1][2] * rhs[2][1]);
            tempM[2][1] = (lhs[2][0] * rhs[0][1]) + (lhs[2][1] * rhs[1][1]) + (lhs[2][2] * rhs[2][1]);

            tempM[0][2] = (lhs[0][0] * rhs[0][2]) + (lhs[0][1] * rhs[1][2]) + (lhs[0][2] * rhs[2][2]);
            tempM[1][2] = (lhs[1][0] * rhs[0][2]) + (lhs[1][1] * rhs[1][2]) + (lhs[1][2] * rhs[2][2]);
            tempM[2][2] = (lhs[2][0] * rhs[0][2]) + (lhs[2][1] * rhs[1][2]) + (lhs[2][2] * rhs[2][2]);
            return tempM;
        }

        public static Matrix3f operator *(Matrix3f lhs, float s)
        {
            return new Matrix3f(new[]
            {
                lhs[0]*s,
                lhs[1]*s,
                lhs[2]*s
            });
        }

      
    }
}
