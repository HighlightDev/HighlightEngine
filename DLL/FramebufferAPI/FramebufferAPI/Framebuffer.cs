using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TextureLoader;

namespace FramebufferAPI
{
    public abstract class Framebuffer
    {
        #region Defines

        protected List<int> framebufferID;
        protected List<int> renderbufferID;
        protected Texture2D textures;
        protected SingleTexture2D singleTexture;

        protected abstract void setTextures();
        protected abstract void setFramebuffers();
        protected abstract void setRenderbuffers();

        #endregion

        #region Generating

        protected void genFramebuffers(byte countFrameBuffers)
        {
            int[] id = new int[countFrameBuffers];
            GL.GenFramebuffers(countFrameBuffers, id);
            framebufferID = id.ToList();
        }

        protected void genRenderbuffers(byte countRenderBuffers)
        {
            int[] id = new int[countRenderBuffers];
            GL.GenRenderbuffers(countRenderBuffers, id);
            renderbufferID = id.ToList();
        }

        #endregion

        #region Binding

        protected void bindFramebuffer(byte framebufferNumber)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferID[framebufferNumber - 1]);
        }

        public void unbindFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        protected void bindRenderbuffer(byte renderbufferNumber)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbufferID[renderbufferNumber - 1]);
        }

        #endregion

        #region Attachment

        protected void renderbufferStorage(RenderbufferStorage renderbufferType,int rezolutionWidth, int rezolutionHeigth)
        {
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, renderbufferType, rezolutionWidth, rezolutionHeigth);
        }

        protected void attachTextureToFramebuffer(FramebufferAttachment bufferAttach, uint attachedTexture)
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, bufferAttach, TextureTarget.Texture2D, attachedTexture, 0);
        }

        protected void attachTextureToFramebuffer(FramebufferAttachment bufferAttach, int attachedTexture)
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, bufferAttach, TextureTarget.Texture2D, attachedTexture, 0);
        }

        /// <summary>
        /// Attaches renderbuffer to framebuffer
        /// </summary>
        /// <param name="bufferAttach">type of buffer attachment</param>
        /// <param name="renderbufferNumber">number of renderbuffer in the list of renderbuffers</param>
        protected void attachRenderbufferToFramebuffer(FramebufferAttachment bufferAttach, byte renderbufferNumber)
        {
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, bufferAttach, RenderbufferTarget.Renderbuffer,
                renderbufferID[renderbufferNumber - 1]);
        }

        /// <summary>
        /// Attaches first renderbuffer(if exist) to framebuffer
        /// </summary>
        /// <param name="bufferAttach">type of buffer attachment</param>
        protected void attachRenderbufferToFramebuffer(FramebufferAttachment bufferAttach)
        {
            if (this.renderbufferID.Count > 0)
            {
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, bufferAttach, RenderbufferTarget.Renderbuffer, renderbufferID[0]);
            }  
        }

        #endregion

        #region ErrorLogs

        private FramebufferErrorCode framebufferSupport()
        {
            return GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        }

        public string getFramebufferLog()
        {
            var log = framebufferSupport();
            switch (log)
            {
                case FramebufferErrorCode.FramebufferComplete: { return "Framebuffer : complete."; }
                case FramebufferErrorCode.FramebufferIncompleteAttachment: { return "Framebuffer : Not all framebuffer attachment points are framebuffer attachment complete."; }
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt: { return "Framebuffer : Not all attached images have the same width and height."; }
                case FramebufferErrorCode.FramebufferIncompleteDrawBuffer: { return "Framebuffer : Each draw buffer must specify color attachment points that have images attached or must be GL_NONE."; }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt: { return "Framebuffer : Incomplete formats"; }
                case FramebufferErrorCode.FramebufferIncompleteLayerCount: { return "Framebuffer : Incomplete layer count"; }
                case FramebufferErrorCode.FramebufferIncompleteLayerTargets: { return "Framebuffer : All attachments must be layered attachments."; }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachment: { return "Framebuffer : No images are attached to the framebuffer."; }
                case FramebufferErrorCode.FramebufferIncompleteMultisample: { return "Framebuffer : All images must have the same number of multisample samples."; }
                case FramebufferErrorCode.FramebufferIncompleteReadBuffer: { return "Framebuffer : ReadBuffer must specify an attachment point that has an image attached. "; }
                case FramebufferErrorCode.FramebufferUndefined: { return "Framebuffer : FBO object number 0 is bound"; }
                case FramebufferErrorCode.FramebufferUnsupported: { return "Framebuffer : The combination of internal formats of the attached images violates an implementation-dependent set of restrictions."; }
                default: return "Framebuffer : Undefined error.";
            }
        }

        public int getFramebufferErrorValue()
        {
            var log = framebufferSupport();
            if (log == FramebufferErrorCode.FramebufferComplete)
            {
                return 0;
            }
            else return -1;
        }

        public FramebufferErrorCode getFramebufferErrorCode()
        {
            return framebufferSupport();
        }

        #endregion

        #region Renderer

        public void renderToFBO(byte framebufferNumber, int viewPortWidth, int viewPortHeight)
        {
            bindFramebuffer(framebufferNumber);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, viewPortWidth, viewPortHeight);
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            this.textures.cleanUp();
            GL.DeleteRenderbuffers(this.renderbufferID.Count, this.renderbufferID.ToArray());
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffers(this.framebufferID.Count, this.framebufferID.ToArray());
            this.renderbufferID.Clear();
            this.framebufferID.Clear();
        }

        #endregion

        #region Constructor

        public Framebuffer()
        {
            this.framebufferID = new List<int>();
            this.renderbufferID = new List<int>();
            this.setTextures();
            this.setFramebuffers();
            this.setRenderbuffers();
        }

        public Framebuffer(SingleTexture2D texture)
        {
            this.framebufferID = new List<int>();
            this.renderbufferID = new List<int>();
            this.singleTexture = texture;
        }

        #endregion
    }
}
