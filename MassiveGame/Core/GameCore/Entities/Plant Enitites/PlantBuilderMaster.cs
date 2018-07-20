using GpuGraphics;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.Terrain;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using TextureLoader;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using VBO;
using CParser;

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{
    public sealed class PlantBuilderMaster
    {
        #region Definitions

        private List<PlantUnit> _plants;

        private readonly Int32 INIT_BUFFER_SIZE;
        private const Int32 MAX_ENTITIES_COUNT = 8000;
        private PlantShader _shader;
        private Material _grassMaterial;
        private Vector3 _meshCenter;
        private bool _postConstructor;
        private List<ITexture> _texture;
        private MistComponent _mist;
        private WindComponent _wind;

        private float timeElapsed = 0;
        private float windSpeed = 55.5f;

        private bool _bufferAssembled;
        private VertexArrayObject _buffer;

        #endregion

        #region Serialize

        public IEnumerable<PlantUnit> GetMemento()
        {
            return this._plants;
        }

        public void SetMemento(IEnumerable<PlantUnit> plants, Landscape terrain)
        {
            if (plants.Count() > MAX_ENTITIES_COUNT)
            {
                for (Int32 i = 0; i < MAX_ENTITIES_COUNT; i++)
                {
                    this._plants.Add(plants.ElementAt(i));
                }
            }
            else
            {
                this._plants = (List<PlantUnit>)plants;
            }
            AlignPlantsToTerrain(terrain);
            this._bufferAssembled = true;
        }

        #endregion

        #region Add

        public void add(PlantUnit plant, Landscape terrain = null)
        {
            //AlignPlantToTerrain(terrain, plant);
            // TODO : assemble buffer first or if it's already assembled just add new values to buffer
            if (_bufferAssembled)
            {
                //PlantUserAttributeBuilder.AddBuilderUserAttribute(plant, _buffer, _plants.Count);
            }
            else
            {
                this._bufferAssembled = true;
            }
            this._plants.Add(plant);
        }

        #endregion

        #region Delete



        #endregion

        #region Seter

        public void SetMist(MistComponent mist)
        {
            this._mist = mist;
        }

        #endregion

        #region Geter

        private Vector3 GetCenter()
        {
            Vector3 center = Vector3.Zero;
            float tempX = 0.0f;
            float tempY = 0.0f;
            float tempZ = 0.0f;

            //Int32 iterationCount = _attribs.Vertices.Length / 3;
            //for (Int32 i = 0; i < iterationCount; i++)
            //{
            //    tempX += _attribs.Vertices[i, 0];
            //    tempY += _attribs.Vertices[i, 1];
            //    tempZ += _attribs.Vertices[i, 2];
            //}
            //center = new Vector3(
            //    tempX / iterationCount,
            //    tempY / iterationCount,
            //    tempZ / iterationCount
            //    );
            return center;
        }

        #endregion

        #region Align_layout

        private void AlignPlantsToTerrain(Landscape terrain)
        {
            if (terrain != null)
            {
                foreach (PlantUnit grass in _plants)
                {
                    /*    get current landscape height    */
                    grass.Translation = new Vector3(grass.Translation.X, terrain.getLandscapeHeight(grass.Translation.X + _meshCenter.X,
                        grass.Translation.Z + _meshCenter.Z), grass.Translation.Z);
                }
            }
        }

        private void AlignPlantToTerrain(Landscape terrain, PlantUnit plant)
        {
            var translation = new Vector3(plant.Translation.X, terrain.getLandscapeHeight(plant.Translation.X + _meshCenter.X,
                        plant.Translation.Z + _meshCenter.Z), plant.Translation.Z);
            plant.Translation = translation;
        }

        #endregion

        #region Renderer

        public void renderEntities(DirectionalLight sun, BaseCamera camera, Matrix4 projectionMatrix, float time, Landscape terrain = null,
            Vector4 clipPlane = new Vector4())
        {
            if (this._bufferAssembled)
            {
                postConstructor(terrain);

                timeElapsed += time * windSpeed;
                timeElapsed = timeElapsed > 360 ? timeElapsed - 360 : timeElapsed;

                _shader.startProgram();
                _texture[0].BindTexture(TextureUnit.Texture0);      //Биндим текстуру
                if (_texture.Count > 1)
                {
                    _texture[1].BindTexture(TextureUnit.Texture1);      //Биндим текстуру
                    if (_texture.Count == 3)
                    {
                        _texture[2].BindTexture(TextureUnit.Texture2);      //Биндим текстуру
                    }
                }

                _shader.setTextureSampler(0);
                _shader.setMaterial(_grassMaterial);
                _shader.setViewMatrix(camera.GetViewMatrix());
                _shader.setProjectionMatrix(ref projectionMatrix);
                _shader.setSun(sun);
                _shader.setWind(_wind);
                _shader.setTime(timeElapsed);
                _shader.setClipPlane(ref clipPlane);
                _shader.setMist(_mist);

                //VAOManager.renderInstanced(_buffer, PrimitiveType.Triangles, _plants.Count);
                _shader.stopProgram();
            }
        }

        #endregion

        #region Constructor

        private void postConstructor(Landscape terrain = null)
        {
            if (this._postConstructor)
            {
                if (_shader == null)
                {
                    _shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<PlantShader>, string, PlantShader>(ProjectFolders.ShadersPath + "plantVertexShader.glsl" + "," + ProjectFolders.ShadersPath + "plantFragmentShader.glsl");
                }
                this._postConstructor = false;
            }
        }

        private VertexArrayObject AllocateVaoMemory(string path)
        {
            ModelLoader loader = new ModelLoader(path);

            VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(loader.Verts, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> normalsVBO = new VertexBufferObjectTwoDimension<float>(loader.N_Verts, BufferTarget.ArrayBuffer, 1, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
            VertexBufferObjectTwoDimension<float> texCoordsVBO = new VertexBufferObjectTwoDimension<float>(loader.T_Verts, BufferTarget.ArrayBuffer, 2, 2, VertexBufferObjectBase.DataCarryFlag.Invalidate);

            var transformationVboTuples = PlantUserAttributeBuilder.GetInstacedTransformationBuffer(_plants, INIT_BUFFER_SIZE);
            var windVBO = PlantUserAttributeBuilder.GetInstancedWindBuffer(_plants, INIT_BUFFER_SIZE);
            var samplerVBO = PlantUserAttributeBuilder.GetInstanceSamplerBuffer(_plants, INIT_BUFFER_SIZE);

            VertexArrayObject vao = new VertexArrayObject();
            vao.AddVBO(verticesVBO, normalsVBO, texCoordsVBO,
                transformationVboTuples.Item1.Item1, transformationVboTuples.Item1.Item2, transformationVboTuples.Item2.Item1, transformationVboTuples.Item2.Item2,
                windVBO, samplerVBO);

            vao.BindVbosToVao();

            return vao;
        }

        public PlantBuilderMaster(Int32 init_buffer_size, string pathToModel, string[] textureSets, WindComponent component)
        {
            this.INIT_BUFFER_SIZE = init_buffer_size;
            this._plants = new List<PlantUnit>();
            this._bufferAssembled = false;
            this._postConstructor = true;
            _texture = new List<ITexture>();
            foreach (var item in textureSets)
            {
                _texture.Add(PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(item));
            }
            this._buffer = AllocateVaoMemory(pathToModel);
            this._wind = component;
            this._grassMaterial = new Material(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
              new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 10.0f, 1.0f);
            this._meshCenter = GetCenter();
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            _plants.Clear();
            //VAOManager.cleanUp(_buffer);
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<PlantShader>, string, PlantShader>(_shader);
            foreach (var item in _texture)
            {
                PoolProxy.FreeResourceMemoryByValue<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(item);
            }
        }

        #endregion
    }
}
