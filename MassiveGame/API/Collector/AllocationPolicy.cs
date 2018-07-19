using CParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TextureLoader;
using VBO;
using OpenTK.Graphics.OpenGL;
using ShaderPattern;
using MassiveGame.Core.RenderCore;

namespace MassiveGame.API.Collector.Policies
{
    #region POLICY

    public abstract class Policy { }

    public abstract class AllocationPolicy<ArgType, ReturnType> : Policy
    {
        // this class is base
        // it's children implement strategy of data allocation and freeing

        public abstract ReturnType AllocateMemory(ArgType arg);

        public abstract void FreeMemory(ReturnType arg);
    }

    public class ModelAllocationPolicy : AllocationPolicy<string, VertexArrayObject>
    {
        public override VertexArrayObject AllocateMemory(string arg)
        {
            VertexArrayObject resultBufferArray = null;
            resultBufferArray = SendDataToGpu(arg);
            return resultBufferArray;
        }

        private VertexArrayObject SendDataToGpu(string modelPath)
        {
            // Get data from mesh
            ModelLoader model = new ModelLoader(modelPath);
            var vertices = model.Verts;
            var normals = model.N_Verts;
            var texCoords = model.T_Verts;
            var tangents = VectorMath.AdditionalVertexInfoCreator.CreateTangentVertices(vertices, texCoords);
            var bitangents = VectorMath.AdditionalVertexInfoCreator.CreateBitangentVertices(vertices, texCoords);

            VertexArrayObject vao = new VertexArrayObject();

            VertexBufferObject<float> vertexVBO = new VertexBufferObject<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> normalsVBO = new VertexBufferObject<float>(normals, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> texCoordsVBO = new VertexBufferObject<float>(texCoords, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> tangentsVBO = new VertexBufferObject<float>(tangents, BufferTarget.ArrayBuffer, 4, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObject<float> bitangentsVBO = new VertexBufferObject<float>(bitangents, BufferTarget.ArrayBuffer, 5, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            vao.AddVBO(vertexVBO, normalsVBO, texCoordsVBO, tangentsVBO, bitangentsVBO);
            vao.BindVbosToVao();

            return vao;
        }

        public override void FreeMemory(VertexArrayObject arg)
        {
            arg.CleanUp();
        }
    }

    public class RenderTargetAllocationPolicy : AllocationPolicy<TextureParameters, ITexture>
    {
        public override ITexture AllocateMemory(TextureParameters arg)
        {
            ITexture result = null;
            result = new Texture2D(arg);
            return result;
        }

        public override void FreeMemory(ITexture arg)
        {
            arg.CleanUp();
        }
    }

    public class ShaderAllocationPolicy<ShaderType> : AllocationPolicy<string, ShaderType>
        where ShaderType : Shader, new()
    {
        public override ShaderType AllocateMemory(string arg)
        {
            return LoadShaderFromFile(arg);
        }

        public override void FreeMemory(ShaderType arg)
        {
            arg.cleanUp();
        }

        private ShaderType LoadShaderFromFile(string compositeKey)
        {
            string[] shaderFiles = compositeKey.Split(',');
            return (ShaderType)Activator.CreateInstance(typeof(ShaderType), shaderFiles);
        }
    }

    public class TextureAllocationPolicy : AllocationPolicy<string, ITexture>
    {
        public override ITexture AllocateMemory(string arg)
        {
            var pathToTexture = arg.Split(',');
            ITexture resultTexture = null;
            switch (pathToTexture.Length)
            {
                case 1:
                    resultTexture = LoadTexture2dFromFile(pathToTexture.First());
                    break;
                case 6:
                    resultTexture = LoadTextureCubeFromFile(pathToTexture);
                    break;
                default: throw new ArgumentException("Undefined count of files.");
            }
            return resultTexture;
        }

        public override void FreeMemory(ITexture arg)
        {
            arg.CleanUp();
        }

        private ITexture LoadTexture2dFromFile(string pathToFile)
        {
            return new Texture2D(pathToFile, EngineStatics.globalSettings.bSupported_MipMap, EngineStatics.globalSettings.AnisotropicFilterValue);
        }

        private ITexture LoadTextureCubeFromFile(string[] pathToFiles)
        {
            return new CubemapTexture(pathToFiles);
        }

        public void ReleaseTexture(ITexture texture)
        {

        }
    }

    #endregion

    #region POOL

    public abstract class BasePool
    {
        protected Dictionary<object, object> resourceMap;
        protected Dictionary<object, Int32> referenceMap;

        public BasePool()
        {
            resourceMap = new Dictionary<object, object>();
            referenceMap = new Dictionary<object, int>();
        }

        private bool AlreadyIsInPool<ArgType, ReturnType>(ArgType key, ref ReturnType resource)
        {
            object result;
            bool exist = resourceMap.TryGetValue(key, out result);
            resource = (ReturnType)result;
            return exist;
        }

        private void IncreaseRefCounter<ArgType>(ArgType key, bool exist)
        {
            if (exist)
            {
                referenceMap[key]++;
            }
            else
            {
                referenceMap.Add(key, 1);
            }
        }

        private void FreeResource<AllocationPolicy, ArgType, ReturnType>(ArgType key)
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            referenceMap[key]--;
            if (referenceMap[key] == 0)
            {
                new AllocationPolicy().FreeMemory((ReturnType)resourceMap[key]);
                resourceMap.Remove(key);
                referenceMap.Remove(key);
            }
        }

        private bool TryToFreeMemory<AllocationPolicy, ArgType, ReturnType>(ArgType key) 
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            bool bMemoryFreed = false;
            object resource;
            bool bExist = resourceMap.TryGetValue(key, out resource);

            if (bExist)
            {
                bMemoryFreed = true;
                FreeResource<AllocationPolicy, ArgType, ReturnType>((ArgType)key);
            }

            return bMemoryFreed;
        }

        private bool TryToFreeMemory<AllocationPolicy, ArgType, ReturnType>(ReturnType value)
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            object key = null;
            bool bMemoryFreed = false;
            bool bExist = resourceMap.Any(item =>
            {
                if (item.Value == (object)value)
                {
                    key = item.Key;
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (bExist)
            {
                bMemoryFreed = true;
                FreeResource<AllocationPolicy, ArgType, ReturnType>((ArgType)key);
            }

            return bMemoryFreed;
        }

        public ReturnType GetResourceFromPool<AllocationPolicy, ArgType, ReturnType>(ArgType arg)
          where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            ReturnType resource = default(ReturnType);
            bool bHasInPool = AlreadyIsInPool(arg, ref resource);
            if (!bHasInPool)
            {
                resource = new AllocationPolicy().AllocateMemory(arg);
                resourceMap.Add(arg, resource);
            }
            IncreaseRefCounter(arg, bHasInPool);
            return resource;
        }

        public void CleanUpByKey<AllocationPolicy, ArgType, ReturnType>(ArgType arg) 
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            TryToFreeMemory<AllocationPolicy, ArgType, ReturnType>(arg);
        }

        public void CleanUpByValue<AllocationPolicy, ArgType, ReturnType>(ReturnType arg)
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            TryToFreeMemory<AllocationPolicy, ArgType, ReturnType>(arg);
        }
    }

    public class ModelPool : BasePool
    {
        public ModelPool() { }

        public void AddModelToRoot(VertexArrayObject meshVAO, string key)
        {
            bool bModelIsInRoot = resourceMap.Any((keyValue) => { return (string)keyValue.Key == key; });
            if (bModelIsInRoot)
            {
                referenceMap[key]++;
            }
            else
            {
                resourceMap.Add(key, meshVAO);
                referenceMap.Add(key, 1);
            }
        }

        public Int32 GetModelReferenceCount(string key)
        {
            object value;
            if (resourceMap.TryGetValue(key, out value))
                return referenceMap[key];

            return 0;
        }
    }

    public class RenderTargetPool : BasePool
    {
        public RenderTargetPool() { }
    }

    public class ShaderPool : BasePool
    {
        public ShaderPool() { }
    }

    public class TexturePool : BasePool
    {
        public TexturePool() { }
    }

    public static class PoolProxy
    {
        public static ReturnType GetResource<Pool, AllocationPolicy, ArgType, ReturnType>(ArgType arg)
              where Pool : IPool<BasePool>, new()
              where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()

        {
            Pool pool = new Pool();
            return pool.GetPool().GetResourceFromPool<AllocationPolicy, ArgType, ReturnType>(arg);
        }

        public static void FreeResourceMemoryByKey<Pool, AllocationPolicy, ArgType, ReturnType>(ArgType arg)
             where Pool : IPool<BasePool>, new()
              where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            Pool pool = new Pool();
            pool.GetPool().CleanUpByKey<AllocationPolicy, ArgType, ReturnType>(arg);
        }

        public static void FreeResourceMemoryByValue<Pool, AllocationPolicy, ArgType, ReturnType>(ReturnType arg)
             where Pool : IPool<BasePool>, new()
              where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            Pool pool = new Pool();
            pool.GetPool().CleanUpByValue<AllocationPolicy, ArgType, ReturnType>(arg);
        }
    }

    public class PoolCollector
    {
        public ModelPool ModelPool { private set; get; }
        public ShaderPool ShaderPool { private set; get; }
        public RenderTargetPool RenderTargetPool { private set; get; }
        public TexturePool TexturePool { private set; get; }

        private static PoolCollector m_collector;

        private PoolCollector()
        {
            ModelPool = new ModelPool();
            ShaderPool = new ShaderPool();
            RenderTargetPool = new RenderTargetPool();
            TexturePool = new TexturePool();
        }

        public static PoolCollector GetInstance()
        {
            if (m_collector == null)
                m_collector = new PoolCollector();
            return m_collector;
        }
    }

    public interface IPool<PoolType>
    {
        PoolType GetPool();
    }

    public class GetModelPool : IPool<BasePool>
    {
        public BasePool GetPool() { return PoolCollector.GetInstance().ModelPool; }
    }

    public class GetRenderTargetPool : IPool<BasePool>
    {
        public BasePool GetPool() { return PoolCollector.GetInstance().RenderTargetPool; }
    }

    public class GetShaderPool : IPool<BasePool>
    {
        public BasePool GetPool() { return PoolCollector.GetInstance().ShaderPool; }
    }

    public class GetTexturePool : IPool<BasePool>
    {
        public BasePool GetPool() { return PoolCollector.GetInstance().TexturePool; }
    }

    #endregion

    public static class TestClass
    {
        static void TryGetModel()
        {
            string key = Settings.ProjectFolders.ModelsPath + "playerCube.obj";

            var model = PoolProxy.GetResource<GetModelPool, ModelAllocationPolicy, string, VertexArrayObject>(key);
            PoolProxy.FreeResourceMemoryByKey<GetModelPool, ModelAllocationPolicy, string, VertexArrayObject>(key);

            var shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<CopyTextureShader>, string, CopyTextureShader>
                (Settings.ProjectFolders.ShadersPath + "copyTextureVS.glsl," + Settings.ProjectFolders.ShadersPath + "copyTextureFS.glsl");
            PoolProxy.FreeResourceMemoryByValue<GetShaderPool, ShaderAllocationPolicy<CopyTextureShader>, string, CopyTextureShader>(shader);
            //var renderTarget = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters());
            //var playerShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<MovableEntityShader>, string, MovableEntityShader>("playerVS.glsl, playerFS.glsl");
        }

        public static void GetPool()

        {
            TryGetModel();
        }

    }
}
