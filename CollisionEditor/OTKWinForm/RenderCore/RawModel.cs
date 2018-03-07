using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTKWinForm.RenderCore
{
    public class RawModel
    {
        public VAO Buffer { private set; get; }
        private VBOArrayF shaderAttributes;
        
        public RawModel(VBOArrayF attributes)
        {
            this.shaderAttributes = attributes;
            Buffer = new VAO(this.shaderAttributes);
            VAOManager.genVAO(Buffer);
            VAOManager.setBufferData(OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, Buffer);
        }
    }
}
