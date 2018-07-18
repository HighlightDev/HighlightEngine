﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using PhysicsBox.ComponentCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore;

namespace MassiveGame.Core.RenderCore.Shadows
{
    public class ShadowDirectionalLight : ShadowBase
    {
        private DirectionalLight LightSource;
        private ShadowMapCache cache;
        private ShadowOrthoBuilder Builder;
        private BaseCamera ViewerCamera;

        public ShadowDirectionalLight(BaseCamera viewerCamera, TextureParameters ShadowMapSettings, DirectionalLight LightSource) : base(ShadowMapSettings)
        {
            ViewerCamera = viewerCamera;
            Builder = new ShadowOrthoBuilder();
            cache = null;
            this.LightSource = LightSource;
        }

        public override ShadowTypes GetShadowType()
        {
            return ShadowTypes.DirectionalLight;
        }

        private Vector3 GetTargetOriginPosition()
        {
            ThirdPersonCamera thirdPersonCamera = ViewerCamera as ThirdPersonCamera;

            if (thirdPersonCamera != null)
                return (thirdPersonCamera.GetThirdPersonTarget() as Component).ComponentTranslation;
            else
                return ViewerCamera.GetEyeVector();

        }

        public override Matrix4 GetShadowViewMatrix()
        {
            Vector3 normLightDir = LightSource.Direction.Normalized();

            Vector3 targetPositon = GetTargetOriginPosition();
            Vector3 shadowCastPosition = new Vector3(targetPositon.X, LightSource.Position.Y, targetPositon.Z);
            Vector3 lightTranslatedPosition = normLightDir * 300;
            var lightEye = new Vector3(shadowCastPosition.X - lightTranslatedPosition.X, shadowCastPosition.Y + lightTranslatedPosition.Y, shadowCastPosition.Z - lightTranslatedPosition.Z);
            var lightTarget = shadowCastPosition + (LightSource.Direction * 100);
            return Matrix4.LookAt(lightEye, lightTarget, new Vector3(0, 1, 0));
        }

        public override Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            return Matrix4.CreateOrthographic(400, 400, 1, 500);
        }

        protected override void PrepareRenderTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, ShadowMapTexture.GetTextureDescriptor(), 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void CreateShadowMapCache()
        {
            cache = new ShadowMapCache();
        }

    }
}