using GpuGraphics;
using MassiveGame.API.Collector;
using MassiveGame.Entities.Plant_Enitites;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using TextureLoader;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public sealed class PlantBuilderMaster
    {
        #region Definitions

        private List<PlantUnit> _plants;

        private readonly Int32 INIT_BUFFER_SIZE;
        private const Int32 MAX_ENTITIES_COUNT = 8000;
        private VBOArrayF _attribs;
        private VAO _buffer;
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

        #endregion

        #region Serialize

        public IEnumerable<PlantUnit> GetMemento()
        {
            return this._plants;
        }

        public void SetMemento(IEnumerable<PlantUnit> plants, Terrain terrain)
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

        public void add(PlantUnit plant, Terrain terrain = null)
        {
            //AlignPlantToTerrain(terrain, plant);
            // TODO : assemble buffer first or if it's already assembled just add new values to buffer
            if (_bufferAssembled)
            {
                PlantUserAttributeBuilder.AddBuilderUserAttribute(plant, _buffer, _plants.Count);
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
            Vector3 center;
            float tempX = 0.0f;
            float tempY = 0.0f;
            float tempZ = 0.0f;

            Int32 iterationCount = _attribs.Vertices.Length / 3;
            for (Int32 i = 0; i < iterationCount; i++)
            {
                tempX += _attribs.Vertices[i, 0];
                tempY += _attribs.Vertices[i, 1];
                tempZ += _attribs.Vertices[i, 2];
            }
            center = new Vector3(
                tempX / iterationCount,
                tempY / iterationCount,
                tempZ / iterationCount
                );
            return center;
        }

        #endregion

        #region Align_layout

        private VBOArrayF AlignMeshYAxis(VBOArrayF attribs)
        {
            //Выравнивает координаты травы до их обработки
            float tempBottom = attribs.Vertices[0, 1];
            Int32 iterationCount = attribs.Vertices.Length / 3;

            for (Int32 i = 0; i < iterationCount; i++)
            {
                if (tempBottom > attribs.Vertices[i, 1])   //Находим минимум по Y
                {
                    tempBottom = attribs.Vertices[i, 1];
                }
            }

            attribs.verticesShift(0, -tempBottom, 0);
            return attribs;
        }


        private void AlignPlantsToTerrain(Terrain terrain)
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

        private void AlignPlantToTerrain(Terrain terrain, PlantUnit plant)
        {
            var translation = new Vector3(plant.Translation.X, terrain.getLandscapeHeight(plant.Translation.X + _meshCenter.X,
                        plant.Translation.Z + _meshCenter.Z), plant.Translation.Z);
            plant.Translation = translation;
        }

        #endregion

        #region Renderer

        public void renderEntities(DirectionalLight sun, LiteCamera camera, Matrix4 projectionMatrix, float time, Terrain terrain = null,
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
                _shader.setViewMatrix(camera.getViewMatrix());
                _shader.setProjectionMatrix(ref projectionMatrix);
                _shader.setSun(sun);
                _shader.setWind(_wind);
                _shader.setTime(timeElapsed);
                _shader.setClipPlane(ref clipPlane);
                _shader.setMist(_mist);

                VAOManager.renderInstanced(_buffer, PrimitiveType.Triangles, _plants.Count);
                _shader.stopProgram();
            }
        }

        #endregion

        #region Constructor

        private void postConstructor(Terrain terrain = null)
        {
            if (this._postConstructor)
            {
                this._attribs = PlantUserAttributeBuilder.BuildBuilderUserAttributeBuffer(_plants, _attribs, INIT_BUFFER_SIZE);
                _buffer = new VAO(_attribs);

                VAOManager.genVAO(_buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);

                if (Object.Equals(_shader, null))
                {
                    _shader = (PlantShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "plantVertexShader.glsl",
                 ProjectFolders.ShadersPath + "plantFragmentShader.glsl", "", typeof(PlantShader));
                }
                this._postConstructor = false;
            }
        }

        public PlantBuilderMaster(Int32 init_buffer_size, VBOArrayF modelAttribs, string[] textureSets, WindComponent component)
        {
            this.INIT_BUFFER_SIZE = init_buffer_size;
            this._plants = new List<PlantUnit>();
            this._bufferAssembled = false;
            this._postConstructor = true;
            _texture = new List<ITexture>();
            foreach (var item in textureSets)
            {
                _texture.Add(ResourcePool.GetTexture(item));
            }
            this._attribs = AlignMeshYAxis(modelAttribs);
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
            VAOManager.cleanUp(_buffer);
            ResourcePool.ReleaseShaderProgram(_shader);
            foreach (var item in _texture)
            {
                ResourcePool.ReleaseTexture(item);
            }
        }

        #endregion
    }
}
