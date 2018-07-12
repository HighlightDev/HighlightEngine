using GpuGraphics;
using ShaderPattern;
using MassiveGame.API.Collector.ModelCollect;
using MassiveGame.API.Collector.ShaderCollect;
using MassiveGame.API.Collector.TextureCollect;
using System;
using System.Linq;
using System.Reflection;
using TextureLoader;
using MassiveGame.API.Collector.RenderTargetCollect;

namespace MassiveGame.API.Collector
{
    public static class ResourcePool
    {
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

        public static VAO GetModel(string key)
        {
            return modelCollector.GetModel(key);
        }

        public static void AddModelToRoot(VAO modelBuffer, string key)
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

        public static Shader GetShaderProgram(string vsKey, string fsKey, string gsKey, Type type)
        {
            var ctor = GetCtor(type, vsKey, fsKey, gsKey);
            var key = GetShaderCompositeKey(vsKey, fsKey, gsKey);
            return shaderCollector.GetShader(key, ctor);
        }

        private static ConstructorInfo GetCtor(Type ownerType, string vsKey, string fsKey, string gsKey)
        {
            Type[] argsType = new Type[gsKey == String.Empty ? 2 : 3];
            for (Int32 i = 0; i < argsType.Length; i++)
                argsType[i] = typeof(string);
            return ownerType.GetConstructor(argsType);
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
