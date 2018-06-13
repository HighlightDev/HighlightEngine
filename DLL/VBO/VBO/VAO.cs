using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GpuGraphics
{
    public class VAO
    {
        private uint[] _vboID;
        private uint[] _iboID;
        private uint[] _vaoID;
        private BufferTarget[] _buffersType;
        private BufferUsageHint[] _buffersUsage;
        private VBOArrayF _vaoData;
        private bool _maped;
        private bool _dataInserted;

        public void mapBuffer()
        {
            _maped = true;
        }
        public void dataInserted()
        {
            _dataInserted = true;
        }
        public bool isMapped()
        {
            return _maped;
        }
        public bool isDataInserted()
        {
            return _dataInserted;
        }

        public bool this[Int32 index]
        {
            get
            {
                switch (index)
                {
                    case 0: { return _vaoData.Vertices != null; }
                    case 1: { return _vaoData.Normals != null; }
                    case 2: { return _vaoData.TextureCoordinates != null; }
                    case 3: { return _vaoData.Color != null; }
                    case 4: { return _vaoData.Tangent != null; }
                    case 5: { return _vaoData.Bitangent != null; }
                    case 6: { return _vaoData.SingleAttribute1 != null; }
                    case 7: { return _vaoData.SingleAttribute2 != null; }
                    case 8: { return _vaoData.VecAttribute1 != null; }
                    case 9: { return _vaoData.VecAttribute2 != null; }
                    case 10: { return _vaoData.VecAttribute3 != null; }
                    case 11: { return _vaoData.VecAttribute4 != null; }
                    default: return false;
                }
            }
        }

        public uint[] Vbo { get { return this._vboID; } }
        public uint[] Vao { get { return this._vaoID; } }
        public uint[] Ibo { get { return this._iboID; } }
        public BufferTarget getBufferTypeAt(uint bufferIndex)
        {
            return _buffersType[bufferIndex];
        }
        public BufferUsageHint getBufferUsageAt(uint bufferIndex)
        {
            return _buffersUsage[bufferIndex];
        }
        public Int32 getBuffersCount()
        {
            return _vboID.Length;
        }
        public VBOArrayF getBufferData()
        {
            return this._vaoData;
        }

        public void setBufferTypeAt(uint bufferIndex, BufferTarget bufferType)
        {
            if (_buffersType.Length > bufferIndex)
            {
                _buffersType[bufferIndex] = bufferType;
            }
        }
        public void setBufferUsageAt(uint bufferIndex, BufferUsageHint bufferUsage)
        {
            if (_buffersUsage.Length > bufferIndex)
            {
                _buffersUsage[bufferIndex] = bufferUsage;
            }
        }

        public void changeBufferData(VBOArrayF bufferData)
        {
            _vaoData = bufferData;
        }

        public VAO(VBOArrayF bufferData)
        {
            _maped = false;
            _dataInserted = false;
            this._vaoData = bufferData;
            this._vboID = new uint[bufferData.getActiveAttribsCount()];
            this._vaoID = new uint[1];
            this._iboID = new uint[1];
        }
    }
    
}
