using CParser;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using TextureLoader;
using VBO;
using OpenTK.Graphics.OpenGL;
using ShaderPattern;

namespace MassiveGame.API.Collector.Policies
{
    #region POLICY
    public abstract class Policy { }

    public abstract class AllocationPolicy<ArgType, ReturnType> : Policy
    {
        // this class is base
        // it's children implement strategy of data allocation and freeing

        public abstract ReturnType AllocateMemory(ArgType arg);

        public abstract void CleanUp(ReturnType arg);
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

        public override void CleanUp(VertexArrayObject arg)
        {
            arg.CleanUp();
        }
    }

    public class RenderTargetAllocationPolicy : AllocationPolicy<TextureParameters, ITexture>
    {
        public override ITexture AllocateMemory(TextureParameters arg)
        {
            throw new NotImplementedException();
        }

        public override void CleanUp(ITexture arg)
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

        public override void CleanUp(ShaderType arg)
        {
            arg.cleanUp();
        }

        public ShaderType LoadShaderFromFile(string compositeKey)
        {
            string[] shaderFiles = GetKeys(compositeKey);
            return (ShaderType)Activator.CreateInstance(typeof(ShaderType), shaderFiles);
        }

        private string[] GetKeys(string compositeKey)
        {
            var result = compositeKey.Split(',');
            return result;
        }
    }


    #endregion

    #region POOL

    public abstract class Pool
    {
        protected Dictionary<object, object> resourceMap;
        protected Dictionary<object, Int32> referenceMap;

        public Pool()
        {
            resourceMap = new Dictionary<object, object>();
            referenceMap = new Dictionary<object, int>();
        }

        private bool AlreadyIsInPool<ArgType>(ArgType key)
        {
            object result = null;
            bool exist = resourceMap.TryGetValue(key, out result);
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

        private bool TryToFreeMemory<AllocationPolicy, ArgType, ReturnType>(ArgType key) 
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            bool bMemoryFreed = false;
            object resource;
            bool bExist = resourceMap.TryGetValue(key, out resource);
            if (bExist)
            {
                referenceMap[key]--;
                if (referenceMap[key] == 0)
                {
                    new AllocationPolicy().CleanUp((ReturnType)resourceMap[key]);
                    bMemoryFreed = true;
                    resourceMap.Remove(key);
                    referenceMap.Remove(key);
                }
            }

            return bMemoryFreed;
        }

        public ReturnType GetResourceFromPool<AllocationPolicy, ArgType, ReturnType>(ArgType arg)
          where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            bool bHasInPool = AlreadyIsInPool(arg);
            ReturnType resource = default(ReturnType);
            if (!bHasInPool)
            {
                resource = new AllocationPolicy().AllocateMemory(arg);
                resourceMap.Add(arg, resource);
            }
            IncreaseRefCounter(arg, bHasInPool);
            return resource;
        }

        public void CleanUp<AllocationPolicy, ArgType, ReturnType>(ArgType arg) 
            where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            TryToFreeMemory<AllocationPolicy, ArgType, ReturnType>(arg);
        }
    }

    public class ModelPool : Pool
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

    public class RenderTargetPool : Pool
    {
        public RenderTargetPool() { }
    }

    public class ShaderPool : Pool
    {
        public ShaderPool() { }
    }

    public class ResourcePool
    {
        public ReturnType GetResource<Pool, AllocationPolicy, ArgType, ReturnType>(ArgType arg)
              where Pool : IPool, new()
              where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()

        {
            Pool pool = new Pool();
            return pool.GetPool().GetResourceFromPool<AllocationPolicy, ArgType, ReturnType>(arg);
        }

        public void FreeResourceMemory<Pool, AllocationPolicy, ArgType, ReturnType>(ArgType arg)
             where Pool : IPool, new()
              where AllocationPolicy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            Pool pool = new Pool();
            pool.GetPool().CleanUp<AllocationPolicy, ArgType, ReturnType>(arg);
        }
    }

    public static class StaticPools
    {
        public static ModelPool modelPool = new ModelPool();
        public static RenderTargetPool renderTargetPool = new RenderTargetPool();
        public static ShaderPool shaderPool = new ShaderPool();
    }

    public interface IPool
    {
        Pool GetPool();
    }

    public class GetModelPool : IPool
    {
        public Pool GetPool() { return StaticPools.modelPool; }
    }

    public class GetRenderTargetPool : IPool
    {
        public Pool GetPool() { return StaticPools.renderTargetPool; }
    }

    public class GetShaderPool : IPool
    {
        public Pool GetPool() { return StaticPools.shaderPool; }
    }

    #endregion

    public static class TestClass
    {
        static ResourcePool pool;

        static void TryGetModel()
        {
            string key = MassiveGame.Settings.ProjectFolders.ModelsPath + "playerCube.obj";

            var model = pool.GetResource<GetModelPool, ModelAllocationPolicy, string, VertexArrayObject>(key);
            model = pool.GetResource<GetModelPool, ModelAllocationPolicy, string, VertexArrayObject>(key);
            pool.FreeResourceMemory<GetModelPool, ModelAllocationPolicy, string, VertexArrayObject>(key);
            pool.FreeResourceMemory<GetModelPool, ModelAllocationPolicy, string, VertexArrayObject>(key);
            //var renderTarget = pool.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters());
            //var playerShader = pool.GetResource<GetShaderPool, ShaderAllocationPolicy<MovableEntityShader>, string, MovableEntityShader>("playerVS.glsl, playerFS.glsl");
        }

        public static ResourcePool GetPool()

        {
            TryGetModel();
            return pool;
        }

        static TestClass()
        {
            pool = new ResourcePool();
        }
    }
}
