using ShaderPattern;
using MassiveGame.API.Collector.ModelCollect;
using MassiveGame.API.Collector.ShaderCollect;
using MassiveGame.API.Collector.TextureCollect;
using System;
using System.Linq;
using TextureLoader;
using MassiveGame.API.Collector.RenderTargetCollect;
using VBO;
using MassiveGame.API.Collector.AnimationCollect;

namespace MassiveGame.API.Collector
{
    [Obsolete("Use memory allocation policies instead")]
    public static class ResourcePool
    {
        private static AnimationPool animationCollector;
        private static ModelPool modelCollector;
        private static ShaderPool shaderCollector;
        private static TexturePool textureCollector;
        private static RenderTargetPool renderTargetCollector;
        //sound collector
   
        const Int32 MAX_RENDER_TARGETS = 8;

        static ResourcePool()
        {
            modelCollector = new ModelPool();
            shaderCollector = new ShaderPool();
            textureCollector = new TexturePool();
            renderTargetCollector = new RenderTargetPool(MAX_RENDER_TARGETS);
        }

        #region FreeMemory

        public static void ReleaseModel(string key)
        {
            modelCollector.ReleaseModel(key);
        }

        public static void ReleaseTexture(ITexture texture)
        {
            textureCollector.ReleaseTexture(texture);
        }

        public static void ReleaseTexture(string key)
        {
            textureCollector.ReleaseTexture(key);
        }

        public static void ReleaseShaderProgram(string key)
        {
            shaderCollector.ReleaseShader(key);
        }

        public static void ReleaseShaderProgram(Shader shader)
        {
            shaderCollector.ReleaseShader(shader);
        }

        public static void ReleaseRenderTarget(TextureParameters RenderTargetParam)
        {
            renderTargetCollector.ReleaseRenderTarget(RenderTargetParam);
        }

        public static void ReleaseRenderTarget(ITexture RenderTarget)
        {
            renderTargetCollector.ReleaseRenderTarget(RenderTarget);
        }

        #endregion

        #region Get

        public static ITexture GetRenderTargetAt(Int32 index)
        {
            return renderTargetCollector.GetRenderTargetAt(index);
        }

        public static Int32 GetRenderTargetCount()
        {
            return renderTargetCollector.GetRenderTargetCount();
        }

        public static ITexture GetRenderTarget(TextureParameters RenderTargetParam)
        {
            return renderTargetCollector.AllocateTextureBuffer(RenderTargetParam);
        }

        public static Int32 GetModelReferenceCount(string key)
        {
            return modelCollector.GetModelReferenceCount(key);
        }

        public static VertexArrayObject GetModel(string key)
        {
            return modelCollector.GetModel(key);
        }

        public static void AddModelToRoot(VertexArrayObject modelBuffer, string key)
        {
            modelCollector.AddModelToRoot(modelBuffer, key);
        }

        public static ITexture GetTexture(params string[] keys)
        {
            var key = GetTextureCompositeKey(keys);
            ITexture result = null;
            if (!object.Equals(key, null))
                result = textureCollector.GetTexture(key);
            return result;
        }

        public static T GetShaderProgram<T>(string vsKey, string fsKey, string gsKey) where T : new()
        {
            var key = GetShaderCompositeKey(vsKey, fsKey, gsKey);
            return shaderCollector.GetShader<T>(key);
        }

        private static string GetShaderCompositeKey(string vsKey, string fsKey, string gsKey)
        {
            return String.Format("{0}{3}{1}{3}{2}", vsKey, fsKey, gsKey, ",").TrimEnd(',');
        }

        private static string GetTextureCompositeKey(string[] keys)
        {
            Int32 keysCount = 0;
            foreach (var item in keys)
                if (!item.Equals(String.Empty))
                    keysCount++;

            if (keysCount == 0) return null;
            if (keysCount == 1) return keys.First();

            string result = String.Empty;
            foreach (var item in keys)
            {
                result += item + ",";
            }
            result = result.TrimEnd(',');
            return result;
        }

        #endregion
    }
}
