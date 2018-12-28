﻿using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.SettingsCore;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TextureLoader;
using VBO;
using MassiveGame.API.ResourcePool.Pools;
using MassiveGame.Core.GameCore;

namespace MassiveGame.Core.DebugCore.UiPanel
{
    public class UiFrameMaster
    {
        private VertexArrayObject _buffer;
        private UiFrameShader _shader;
        private bool _postConstructor;

        public List<ITexture> frameTextures;
        public readonly Int32 MAX_FRAME_COUNT = 3;

#if DEBUG || ENGINE_EDITOR || COLLISION_EDITOR
        public Int32 DebugRenderTargetIndex = 0;
#endif

        public UiFrameMaster()
        {
            frameTextures = new List<ITexture>(MAX_FRAME_COUNT);
            _postConstructor = true;
        }
#if DEBUG || ENGINE_EDITOR || COLLISION_EDITOR
        public void PushDebugRenderTarget()
        {
            Int32 totalCount = PoolProxy.GetResourceCountInPool<GetRenderTargetPool>();
            DebugRenderTargetIndex = DebugRenderTargetIndex > (totalCount - 1) ? 0 : DebugRenderTargetIndex;

            PushFrame((new GetRenderTargetPool().GetPool() as RenderTargetPool).GetRenderTargetAt(DebugRenderTargetIndex));

            ++DebugRenderTargetIndex;
        }
#endif

        public void PushFrame(ITexture texture)
        {
            if (frameTextures.Count >= MAX_FRAME_COUNT)
                PopFrame();

            frameTextures.Add(texture);
        }

        public void PopFrame()
        {
            if (frameTextures.Count != 0)
            {
                frameTextures.Remove(frameTextures.First());
            }
        }

        public void RenderFrames()
        {
            for (Int32 i = 0; i< frameTextures.Count; i++)
            {
                ITexture frameTexture = frameTextures[i];
                Render(frameTexture, i);
            }
        }

        private Matrix4 GetScreenSpaceMatrix(Int32 layoutIndex)
        {
            Matrix4 resultMatrix = Matrix4.Identity;

            Vector2 Origin = new Vector2(-0.75f, -0.7f);
            Vector2 translation = new Vector2(Origin.X, (layoutIndex * 0.5f) + (0.15f * layoutIndex) + Origin.Y);

            resultMatrix *= Matrix4.CreateScale(0.2f, 0.25f, 1);
            resultMatrix *= Matrix4.CreateTranslation(new Vector3(translation.X, translation.Y, 0.0f));

            return resultMatrix;
        }

        private void Render(ITexture renderTexture, Int32 index)
        {
            PostConstructor();

            var texturePixelFormat = renderTexture.GetTextureParameters().TexPixelFormat;
            bool bDepthTexture = (texturePixelFormat == PixelFormat.DepthComponent || texturePixelFormat == PixelFormat.DepthComponent);


            var screenSpaceMatrix = GetScreenSpaceMatrix(index);
            _shader.startProgram();
            renderTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetIsDepthTexture(bDepthTexture);
            _shader.SetIsSeparatedScreen(false);
            _shader.SetUiTextureSampler(0);
            _shader.SetScreenSpaceMatrix(screenSpaceMatrix);
            _buffer.RenderVAO(PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public void RenderFullScreenInputTexture(ITexture renderTexture, Point screenRezolution)
        {
            PostConstructor();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, screenRezolution.X, screenRezolution.Y);
            _shader.startProgram();
            renderTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetUiTextureSampler(0);
            _shader.SetScreenSpaceMatrix(Matrix4.Identity);
            _shader.SetIsDepthTexture(false);
            _shader.SetIsSeparatedScreen(false);
            _buffer.RenderVAO(PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        public void RenderSeparatedScreen(ITexture separatedTexture, Point screenRezolution)
        {
            PostConstructor();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, screenRezolution.X, screenRezolution.Y);
            _shader.startProgram();
            separatedTexture.BindTexture(TextureUnit.Texture0);
            _shader.SetUiTextureSampler(0);
            _shader.SetIsDepthTexture(false);
            _shader.SetIsSeparatedScreen(true);
            _shader.SetScreenSpaceMatrix(Matrix4.Identity);
            _buffer.RenderVAO(PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        private void PostConstructor()
        {
            if (_postConstructor)
            {
                _buffer = ScreenQuad.GetScreenQuadBuffer();

                _shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<UiFrameShader>, string, UiFrameShader>(ProjectFolders.ShadersPath + "uiVS.glsl," + ProjectFolders.ShadersPath + "uiFS.glsl");
                _postConstructor = false;

            }
        }

        #region Cleaning

        public void CleanUp()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<UiFrameShader>, string, UiFrameShader>(_shader);
        }

        #endregion
    }
}