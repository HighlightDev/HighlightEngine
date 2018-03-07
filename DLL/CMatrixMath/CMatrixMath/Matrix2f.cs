using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public class Matrix2f
    {
        /// <summary>
        /// The columms of the matrix.
        /// </summary>
        private Vector2f[] _cols;

        /// <summary>
        /// Initializes a new instance of the <see cref="mat2"/> struct.
        /// This matrix is the identity matrix scaled by <paramref name="scale"/>.
        /// </summary>
        /// <param name="scale">The scale.</param>

        public Matrix2f(float scale)
        {
            _cols = new[]
            {
                new Vector2f(scale, 0.0f),
                new Vector2f(0.0f, scale)
            };
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="mat2"/> struct.
        /// The matrix is initialised with the <paramref name="cols"/>.
        /// </summary>
        /// <param name="cols">The colums of the matrix.</param>
        public Matrix2f(Vector2f[] cols)
        {
            this._cols = new[]
            {
                cols[0],
                cols[1]
            };
        }

        public Matrix2f(Vector2f a, Vector2f b)
        {
            this._cols = new[]
            {
                a, b
            };
        }

        public Matrix2f(float a, float b, float c, float d)
        {
            this._cols = new[]
            {
                new Vector2f(a,b), new Vector2f(c,d)
            };
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <returns>A new identity matrix.</returns>
        public void toIdentity()
        {
            this._cols = new Vector2f[]
                {
                    new Vector2f(1,0),
                    new Vector2f(0,1)
                };
        }

       

        /// <summary>
        /// Gets or sets the <see cref="vec2"/> column at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="vec2"/> column.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <returns>The column at index <paramref name="column"/>.</returns>
        public Vector2f this[int column]
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
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> vector.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS vector.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static Vector2f operator *(Matrix2f lhs, Vector2f rhs)
        {
            Vector2f tempV = new Vector2f();
            tempV.x = ((lhs[0][0] * rhs.x) + (lhs[0][1] * rhs.y));
            tempV.y = ((lhs[1][0] * rhs.x) + (lhs[1][1] * rhs.y));
            return tempV;
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> matrix.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS matrix.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static Matrix2f operator *(Matrix2f lhs, Matrix2f rhs)
        {
            Matrix2f tempM = new Matrix2f(0);
            tempM[0][0] = (lhs[0][0] * rhs[0][0]) + (lhs[0][1] * rhs[1][0]);
            tempM[1][0] = (lhs[1][0] * rhs[0][0]) + (lhs[1][1] * rhs[1][0]);
            tempM[0][1] = (lhs[0][0] * rhs[0][1]) + (lhs[0][1] * rhs[1][1]);
            tempM[1][1] = (lhs[1][0] * rhs[0][1]) + (lhs[1][1] * rhs[1][1]);
            return tempM;
        }

        public static Matrix2f operator *(Matrix2f lhs, float s)
        {
            return new Matrix2f(new[]
            {
                lhs[0]*s,
                lhs[1]*s
            });
        }

        
    }
}
