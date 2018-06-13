using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using VMath;
using OpenTK.Graphics;

using OpenTK.Graphics.OpenGL;

namespace GpuGraphics
{
    public class VBOArrayF
    {
        #region Iterator

        public float[,] this[uint iterat]
        {
            get
            {
                switch (iterat)
                {
                    case 0:
                        {
                            try
                            {
                                if (Vertices == null)
                                {
                                    throw new NullReferenceException("null reference at 0 index!");
                                }
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine(e.Message);
                                return null;
                            }
                            return Vertices;
                        }
                    case 1:
                        {
                            try
                            {
                                if (Normals == null)
                                {
                                    throw new NullReferenceException("null reference at 0 index!");
                                }
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine(e.Message);
                                return null;
                            }
                            return Normals;
                        }
                    case 2:
                        {
                            try
                            {
                                if (TextureCoordinates == null)
                                {
                                    throw new NullReferenceException("null reference at 0 index!");
                                }
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine(e.Message);
                                return null;
                            }
                            return TextureCoordinates;
                        }
                    case 3:
                        {
                            try
                            {
                                if (Color == null)
                                {
                                    throw new NullReferenceException("null reference at 0 index!");
                                }
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine(e.Message);
                                return null;
                            }
                            return Color;
                        }
                    default: try { throw new IndexOutOfRangeException("wrong index"); }
                        catch (IndexOutOfRangeException e)
                        {
                            Console.WriteLine(e.Message);
                            return null;
                        }
                }
            }
        }

        #endregion

        #region Defines

        private float[,] _vertices;
        private float[,] _normals;
        private float[,] _textureCoordinates;
        private float[,] _color;
        private Int32[] _indices;
        private float[,] _tangent;
        private float[,] _bitangent;

        private float[] _singeleAttribute1;
        private float[] _singeleAttribute2;
        private float[,] _vAttribute1;
        private float[,] _vAttribute2;
        private float[,] _vAttribute3;
        private float[,] _vAttribute4;

        #endregion

        #region Properties

        public float[,] Vertices { set { this._vertices = value; } get { return this._vertices; } }
        public float[,] Normals { get { return this._normals; } }
        public float[,] TextureCoordinates { get { return this._textureCoordinates; } }
        public float[,] Color { get { return this._color; } }
        public Int32[] Indices { get { return this._indices; } }
        public float[,] Tangent { get { return this._tangent; } }
        public float[,] Bitangent { get { return this._bitangent; } }

        public float[] SingleAttribute1 { get { return this._singeleAttribute1; } }
        public float[] SingleAttribute2 { get { return this._singeleAttribute2; } }
        public float[,] VecAttribute1 { get { return this._vAttribute1; } }
        public float[,] VecAttribute2 { get { return this._vAttribute2; } }
        public float[,] VecAttribute3 { get { return this._vAttribute3; } }
        public float[,] VecAttribute4 { get { return this._vAttribute4; } }

        #endregion

        #region Attribs transformations

        public void MultMatrix(ref Matrix4 modelMatrix)
        {
            for (Int32 i = 0; i < Vertices.Length / 3; i++)
            {
                Vector4 vertex = new Vector4(Vertices[i, 0], Vertices[i, 1], Vertices[i, 2], 1.0f);
                vertex = VectorMath.multMatrix(modelMatrix, vertex);
                this.Vertices[i, 0] = vertex.X;
                this.Vertices[i, 1] = vertex.Y;
                this.Vertices[i, 2] = vertex.Z;
                if (_normals != null)
                {
                    Vector4 normal = new Vector4(Normals[i, 0], Normals[i, 1], Normals[i, 2], 0.0f);
                    normal = VectorMath.multMatrix(modelMatrix, normal);
                    this.Normals[i, 0] = normal.X;
                    this.Normals[i, 1] = normal.Y;
                    this.Normals[i, 2] = normal.Z;
                }
            }
        }

        public void verticesShift(float shiftX, float shiftY, float shiftZ)
        {
            for (Int32 i = 0;i < this._vertices.Length / 3;i++)
            {
                if (shiftX != 0) { this._vertices[i, 0] += shiftX; }
                if (shiftY != 0) { this._vertices[i, 1] += shiftY; }
                if (shiftZ != 0) { this._vertices[i, 2] += shiftZ; }
            }
        }

        #endregion

        #region Create Tangent and Bitangent

        private void calculateTangentAndBitangent()
        {
            for (Int32 i = 0; i < _vertices.Length / 3; i += 3)
            {
                Vector3 v0 = new Vector3(_vertices[i, 0], _vertices[i, 1], _vertices[i, 2]);
                Vector3 v1 = new Vector3(_vertices[i + 1, 0], _vertices[i + 1, 1], _vertices[i + 1, 2]);
                Vector3 v2 = new Vector3(_vertices[i + 2, 0], _vertices[i + 2, 1], _vertices[i + 2, 2]);
                Vector2 uv0 = new Vector2(_textureCoordinates[i, 0], _textureCoordinates[i, 1]);
                Vector2 uv1 = new Vector2(_textureCoordinates[i + 1, 0], _textureCoordinates[i + 1, 1]);
                Vector2 uv2 = new Vector2(_textureCoordinates[i + 2, 0], _textureCoordinates[i + 2, 1]);
                // стороны треугольника
                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;
                // дельта UV
                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                Vector3 lTangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                _tangent[i, 0] = lTangent.X;
                _tangent[i, 1] = lTangent.Y;
                _tangent[i, 2] = lTangent.Z;
                _tangent[i + 1, 0] = lTangent.X;
                _tangent[i + 1, 1] = lTangent.Y;
                _tangent[i + 1, 2] = lTangent.Z;
                _tangent[i + 2, 0] = lTangent.X;
                _tangent[i + 2, 1] = lTangent.Y;
                _tangent[i + 2, 2] = lTangent.Z;

                Vector3 bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;
                _bitangent[i, 0] = bitangent.X;
                _bitangent[i, 1] = bitangent.Y;
                _bitangent[i, 2] = bitangent.Z;
                _bitangent[i + 1, 0] = bitangent.X;
                _bitangent[i + 1, 1] = bitangent.Y;
                _bitangent[i + 1, 2] = bitangent.Z;
                _bitangent[i + 2, 0] = bitangent.X;
                _bitangent[i + 2, 1] = bitangent.Y;
                _bitangent[i + 2, 2] = bitangent.Z;
            }
        }

        #endregion

        #region Getters

        public Vector3[] getVertices()
        {
            Vector3[] vertices = new Vector3[Vertices.Length / 3];
            for (Int32 i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(Vertices[i, 0], Vertices[i, 1], Vertices[i, 2]);
            }
            return vertices;
        }
        public Vector3[] getNormals()
        {
            Vector3[] normals = new Vector3[Normals.Length / 3];
            for (Int32 i = 0; i < normals.Length; i++)
            {
                normals[i] = new Vector3(Normals[i, 0], Normals[i, 1], Normals[i, 2]);
            }
            return normals;
        }
        public Vector2[] getTexCoords()
        {
            Vector2[] texCoords = new Vector2[TextureCoordinates.Length / 2];
            for (Int32 i = 0; i < texCoords.Length; i++)
            {
                texCoords[i] = new Vector2(TextureCoordinates[i, 0], TextureCoordinates[i, 1]);
            }
            return texCoords;
        }
        public Vector3[] getColor()
        {
            Vector3[] color = new Vector3[Color.Length / 3];
            for (Int32 i = 0; i < color.Length; i++)
            {
                color[i] = new Vector3(Color[i, 0], Color[i, 1], Color[i, 2]);
            }
            return color;
        }
        public Vector3[] getTangent()
        {
            Vector3[] tangent = new Vector3[Tangent.Length / 3];
            for (Int32 i = 0; i < tangent.Length; i++)
            {
                tangent[i] = new Vector3(Tangent[i, 0], Tangent[i, 1], Tangent[i, 2]);
            }
            return tangent;
        }
        public Vector3[] getBitangent()
        {
            Vector3[] bitangent = new Vector3[Bitangent.Length / 3];
            for (Int32 i = 0; i < bitangent.Length; i++)
            {
                bitangent[i] = new Vector3(Bitangent[i, 0], Bitangent[i, 1], Bitangent[i, 2]);
            }
            return bitangent;
        }
        public IntPtr getAtrributeByteSize(uint bufferIndex)
        {
            switch (bufferIndex)
            {
                case 0:
                    {
                        if (_vertices != null)
                        {
                            return getVerticesByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }
                case 1:
                    {
                        if (_normals != null)
                        {
                            return getNormalsByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }
                case 2:
                    {
                        if (_textureCoordinates != null)
                        {
                            return getTexCoordByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }
                case 3:
                    {
                        if (_color != null)
                        {
                            return getColorByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }
                case 4:
                    {
                        if (_indices != null)
                        {
                            return getIndicesByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }
                case 5:
                    {
                        if (_tangent != null)
                        {
                            return getTangentByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }
                case 6:
                    {
                        if (_bitangent != null)
                        {
                            return getBitangentByteSize();
                        }
                        else { return new IntPtr(-1); }
                    }

                default: try { new IndexOutOfRangeException("out of range"); }
                    catch (IndexOutOfRangeException e) { Console.WriteLine(e.Message); }
                    return new IntPtr(-1);
            }
        }
        public IntPtr getVerticesByteSize()
        {
            return new IntPtr(sizeof(float) * Vertices.Length);
        }
        public IntPtr getNormalsByteSize()
        {
            return new IntPtr(sizeof(float) * Normals.Length);
        }
        public IntPtr getTexCoordByteSize()
        {
            return new IntPtr(sizeof(float) * TextureCoordinates.Length);
        }
        public IntPtr getColorByteSize()
        {
            return new IntPtr(sizeof(float) * Color.Length);
        }
        public IntPtr getIndicesByteSize()
        {
            return new IntPtr(sizeof(Int32) * Indices.Length);
        }
        public IntPtr getTangentByteSize()
        {
            return new IntPtr(sizeof(float) * Tangent.Length);
        }
        public IntPtr getBitangentByteSize()
        {
            return new IntPtr(sizeof(float) * Bitangent.Length);
        }
        public IntPtr getSingleAttribute1Size()
        {
            return new IntPtr(sizeof(float) * _singeleAttribute1.Length);
        }
        public IntPtr getSingleAttribute2Size()
        {
            return new IntPtr(sizeof(float) * _singeleAttribute1.Length);
        }
        public IntPtr getVectorAttribute1Size()
        {
            return new IntPtr(sizeof(float) * _vAttribute1.Length);
        }
        public IntPtr getVectorAttribute2Size()
        {
            return new IntPtr(sizeof(float) * _vAttribute2.Length);
        }
        public IntPtr getVectorAttribute3Size()
        {
            return new IntPtr(sizeof(float) * _vAttribute3.Length);
        }
        public IntPtr getVectorAttribute4Size()
        {
            return new IntPtr(sizeof(float) * _vAttribute4.Length);
        }
       
        public Int32 getCountVertices()
        {
            return this.Vertices.Length / 3;
        }
        public byte getActiveAttribsCount()
        {
            byte counter = 0;
            if (Vertices != null) { counter++; }
            if (Normals != null) { counter++; }
            if (TextureCoordinates != null) { counter++; }
            if (Color != null) { counter++; }
            if (Tangent != null) { counter++; }
            if (Bitangent != null) { counter++; }

            // for user attributes
            if (_singeleAttribute1 != null) { counter++; }
            if (_singeleAttribute2 != null) { counter++; }
            if (_vAttribute1 != null) { counter++; }
            if (_vAttribute2 != null) { counter++; }
            if (_vAttribute3 != null) { counter++; }
            if (_vAttribute4 != null) { counter++; }

            return counter;
        }

        #endregion

        #region Constructors

        public VBOArrayF(Vector3[] vertices)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            _vertices = new float[vertices.Length, 3];
            for (Int32 i = 0; i < vertices.Length; i++)
            {
                Vertices[i, 0] = vertices[i].X;
                Vertices[i, 1] = vertices[i].Y;
                Vertices[i, 2] = vertices[i].Z;
            }
        }
        public VBOArrayF(float[,] vertices)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            this._vertices = vertices;
        }
        public VBOArrayF(float[,] vertices, Int32[] indices)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            this._vertices = vertices;
            this._indices = indices;
        }
        public VBOArrayF(float[,] vertices, float[,] textureCoordinates, Int32[] indices = null)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            this._vertices = vertices;
            this._textureCoordinates = textureCoordinates;
            this._indices = indices;
        }
        public VBOArrayF(float[,] vertices, float[,] normals, float[,] textureCoordinates, bool genNormalMapAttribs = false)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); Environment.Exit(0); }
            _vertices = vertices;
            this._normals = normals == null ? this._normals : normals;
            this._textureCoordinates = textureCoordinates == null ? this._textureCoordinates : textureCoordinates;
            if (genNormalMapAttribs) 
            {
                _tangent = new float[_vertices.Length / 3, 3];
                _bitangent = new float[_vertices.Length / 3, 3];
                calculateTangentAndBitangent();
            }
        }
        public VBOArrayF(float[,] vertices, float[,] normals, float[,] textureCoordinates, float[] attribute1, float[] attribute2,
            float[,] vAttribute1, float[,] vAttribute2, float[,] vAttribute3, float[,] vAttribute4)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); Environment.Exit(0); }
            _vertices = vertices;
            this._normals = normals == null ? this._normals : normals;
            this._textureCoordinates = textureCoordinates == null ? this._textureCoordinates : textureCoordinates;
            this._singeleAttribute1 = attribute1;
            this._singeleAttribute2 = attribute2;
            this._vAttribute1 = vAttribute1;
            this._vAttribute2 = vAttribute2;
            this._vAttribute3 = vAttribute3;
            this._vAttribute4 = vAttribute4;
        }
        public VBOArrayF(float[,] vertices, float[,] normals, float[,] textureCoordinates, Int32[] indices = null)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            _vertices = vertices;
            this._normals = normals == null ? this._normals : normals;
            this._textureCoordinates = textureCoordinates == null ? this._textureCoordinates : textureCoordinates;
            this._indices = indices;
        }
        public VBOArrayF(float[,] vertices, float[,] normals, float[,] textureCoordinates, Int32[] indices = null, bool genNormalMapAttribs = false)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            _vertices = vertices;
            this._normals = normals == null ? this._normals : normals;
            this._textureCoordinates = textureCoordinates == null ? this._textureCoordinates : textureCoordinates;
            this._indices = indices;
            if (genNormalMapAttribs)
            {
                _tangent = new float[_vertices.Length / 3, 3];
                _bitangent = new float[_vertices.Length / 3, 3];
                calculateTangentAndBitangent();
            }
        }
        public VBOArrayF(float[,] vertices, float[,] normals, float[,] textureCoordinates, float[,] color, Int32[] indices = null)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            this._vertices = vertices;
            this._normals = normals;
            this._textureCoordinates = textureCoordinates;
            this._color = color;
            this._indices = indices;
        }
        public VBOArrayF(float[,] vertices, float[,] normals, float[,] textureCoordinates, float[,] color, bool genNormalMapAttribs,
             Int32[] indices = null)
        {
            try
            {
                if (vertices == null)
                {
                    throw new ArgumentException("argument value cannot be null");
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
            this._vertices = vertices;
            this._normals = normals;
            this._textureCoordinates = textureCoordinates;
            this._color = color;
            if (genNormalMapAttribs)
            {
                _tangent = new float[_vertices.Length / 3, 3];
                _bitangent = new float[_vertices.Length / 3, 3];
                calculateTangentAndBitangent();
            }
            this._indices = indices;
        }

        #endregion
    }
}
