﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace FramebufferAPI
{
    public abstract class Framebuffer
    {
        #region Defines

        protected List<Int32> framebufferID;
        protected List<Int32> renderbufferID;

        protected abstract void setTextures();
        protected abstract void setFramebuffers();
        protected abstract void setRenderbuffers();

        #endregion

        #region Generating

        protected void genFramebuffers(Int32 countFrameBuffers)
        {
            Int32[] id = new Int32[countFrameBuffers];
            GL.GenFramebuffers(countFrameBuffers, id);
            framebufferID = id.ToList();
        }

        protected void genRenderbuffers(Int32 countRenderBuffers)
        {
            Int32[] id = new Int32[countRenderBuffers];
            GL.GenRenderbuffers(countRenderBuffers, id);
            renderbufferID = id.ToList();
        }

        #endregion

        #region Binding

        protected void bindFramebuffer(Int32 framebufferNumber)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferID[framebufferNumber - 1]);
        }

        public void unbindFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        protected void bindRenderbuffer(Int32 renderbufferNumber)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbufferID[renderbufferNumber - 1]);
        }

        #endregion

        #region Attachment

        protected void renderbufferStorage(RenderbufferStorage renderbufferType, Int32 rezolutionWidth, Int32 rezolutionHeigth)
        {
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, renderbufferType, rezolutionWidth, rezolutionHeigth);
        }

        protected void renderbufferStorage(RenderbufferStorage renderbufferType, Point screenRezolution)
        {
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, renderbufferType, screenRezolution.X, screenRezolution.Y);
        }

        protected void attachTextureToFramebuffer(FramebufferAttachment bufferAttach, uint attachedTexture)
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, bufferAttach, TextureTarget.Texture2D, attachedTexture, 0);
        }

        protected void attachTextureToFramebuffer(FramebufferAttachment bufferAttach, Int32 attachedTexture)
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, bufferAttach, TextureTarget.Texture2D, attachedTexture, 0);
        }

        /// <summary>
        /// Attaches renderbuffer to framebuffer
        /// </summary>
        /// <param name="bufferAttach">type of buffer attachment</param>
        /// <param name="renderbufferNumber">number of renderbuffer in the list of renderbuffers</param>
        protected void attachRenderbufferToFramebuffer(FramebufferAttachment bufferAttach, Int32 renderbufferNumber)
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

        public Int32 getFramebufferErrorValue()
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

        private void bindFramebufferImpl(Int32 framebufferNumber, Int32 viewPortWidth, Int32 viewPortHeight)
        {
            bindFramebuffer(framebufferNumber);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, viewPortWidth, viewPortHeight);
        }

        public void renderToFBO(Int32 framebufferNumber, Int32 viewPortWidth, Int32 viewPortHeight)
        {
            bindFramebufferImpl(framebufferNumber, viewPortWidth, viewPortHeight);
        }

        public void renderToFBO(Int32 framebufferNumber, Point viewPortRezolution)
        {
            bindFramebufferImpl(framebufferNumber, viewPortRezolution.X, viewPortRezolution.Y);
        }

        #endregion

        #region Cleaning

        public virtual void cleanUp()
        {
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
            this.framebufferID = new List<Int32>();
            this.renderbufferID = new List<Int32>();
            this.setTextures();
            this.setFramebuffers();
            this.setRenderbuffers();
        }

        #endregion
    }
}
