﻿using Grid;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using TextureLoader;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Settings;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.GameCore.Terrain
{
    public class Landscape : IDrawable
    {
        #region Definitions 

        public TableGrid LandscapeMap { set; get; }
        public const float MAX_PIXEL_COLOUR = 256 * 3;
        public readonly float MapSize;
        public readonly float MaximumHeight;
        private VertexArrayObject _buffer;
        private bool _postConstructor;
        private LandscapeShader _shader;
        private Material _terrainMaterial;
        private ITexture _textureR;
        private ITexture _textureG;
        private ITexture _textureB;
        private ITexture _textureBlack;
        private ITexture _blendMap;
        private ITexture _normalMapR;
        private ITexture _normalMapG;
        private ITexture _normalMapB;
        private ITexture _normalMapBlack;
        private Int32 _normalsSmoothLvl;
        private MistComponent _mist;

        private WaterReflectionTerrainShader liteReflectionShader;
        private WaterRefractionTerrainShader liteRefractionShader;
        

        #endregion

        #region Seter

        public void SetMist(MistComponent mist)
        {
            this._mist = mist;
        }

        public void SetNormalMapR(string nm)
        {
            this._normalMapR = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(nm);
        }

        public void SetNormalMapG(string nm)
        {
            this._normalMapG = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(nm);
        }

        public void SetNormalMapB(string nm)
        {
            this._normalMapB = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(nm);
        }

        public void SetNormalMapBlack(string nm)
        {
            this._normalMapBlack = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(nm);
        }

        #endregion

        #region Getter

        public float getLandscapeHeight(float x, float z)
        {
            float yHigh = 0.0f;
            float tileSize = Convert.ToSingle(this.LandscapeMap.GridStep);
            float centerX = x;
            float centerZ = z;

            float xPoint = (Int32)(centerX / tileSize);
            float zPoint = (Int32)(centerZ / tileSize);

            if ((xPoint < 0.0 || xPoint >= this.LandscapeMap.TableSize - 1.0f)
                || (zPoint < 0.0 || zPoint >= this.LandscapeMap.TableSize - 1.0f))
            {
                return 0.0f;
            }

            float xTile = (Int32)(centerX) % tileSize;
            float zTile = (Int32)(centerZ) % tileSize;

            float xPoint1, xPoint2, xPoint3, yPoint1, yPoint2, yPoint3, zPoint1, zPoint2, zPoint3;

            xPoint2 = xPoint * tileSize;
            zPoint2 = (1 + zPoint) * tileSize;
            yPoint2 = this.LandscapeMap[(Int32)xPoint, (Int32)(1 + zPoint)];

            xPoint3 = (xPoint + 1) * tileSize;
            zPoint3 = zPoint * tileSize;
            yPoint3 = this.LandscapeMap[(Int32)xPoint + 1, (Int32)zPoint];

            if (xTile + zTile >= tileSize)
            {
                xPoint1 = xPoint * tileSize;
                zPoint1 = zPoint * tileSize;
                yPoint1 = this.LandscapeMap[(Int32)xPoint, (Int32)zPoint];
            }
            else
            {
                xPoint1 = (xPoint + 1) * tileSize;
                zPoint1 = (zPoint + 1) * tileSize;
                yPoint1 = this.LandscapeMap[(Int32)xPoint + 1, (Int32)zPoint + 1];
            }

            float a = -(zPoint3 * yPoint2 - zPoint1 * yPoint2 - zPoint3 * yPoint1 + yPoint1 * zPoint2 + yPoint3 * zPoint1 - zPoint2 * yPoint3);
            float b = (zPoint1 * xPoint3 + zPoint2 * xPoint1 + zPoint3 * xPoint2 - zPoint2 * xPoint3 - zPoint1 * xPoint2 - zPoint3 * xPoint1);
            float c = (yPoint2 * xPoint3 + yPoint1 * xPoint2 + yPoint3 * xPoint1 - yPoint1 * xPoint3 - yPoint2 * xPoint1 - xPoint2 * yPoint3);
            float d = (-a * xPoint1) - (b * yPoint1) - (c * zPoint1);

            yHigh = -((a * centerX) + (c * centerZ + d)) / b;
            return yHigh;
        }

        #endregion

        #region Renderer

        public void RenderWaterReflection(WaterPlane water, DirectionalLight Sun, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane)
        {
            if (_postConstructor)
                return;

            float translationPositionY = (2 * water.GetTranslation().Y);
            Matrix4 mirrorMatrix, modelMatrix = Matrix4.Identity;
            mirrorMatrix = Matrix4.CreateScale(1, -1, 1);
            mirrorMatrix *= Matrix4.CreateTranslation(0, translationPositionY, 0);

            GL.Enable(EnableCap.ClipDistance0);

            liteReflectionShader.startProgram();

            _textureBlack.BindTexture(TextureUnit.Texture0);
            _textureR.BindTexture(TextureUnit.Texture1);
            _textureG.BindTexture(TextureUnit.Texture2);
            _textureB.BindTexture(TextureUnit.Texture3);
            _blendMap.BindTexture(TextureUnit.Texture4);

            liteReflectionShader.SetTextureR(1);
            liteReflectionShader.SetTextureG(2);
            liteReflectionShader.SetTextureB(3);
            liteReflectionShader.SetTextureBlack(0);
            liteReflectionShader.SetBlendMap(4);
            liteReflectionShader.SetMaterial(_terrainMaterial);
            liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteReflectionShader.SetDirectionalLight(Sun);
            liteReflectionShader.SetClippingPlane(ref clipPlane);
            _buffer.RenderVAO(PrimitiveType.Triangles);
            liteReflectionShader.stopProgram();
            GL.Disable(EnableCap.ClipDistance0);
        }

        public void RenderWaterRefraction(DirectionalLight Sun, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane)
        {
            if (_postConstructor)
                return;

            Matrix4 modelMatrix = Matrix4.Identity;

            GL.Enable(EnableCap.ClipDistance0);

            liteRefractionShader.startProgram();

            _textureBlack.BindTexture(TextureUnit.Texture0);
            _textureR.BindTexture(TextureUnit.Texture1);
            _textureG.BindTexture(TextureUnit.Texture2);
            _textureB.BindTexture(TextureUnit.Texture3);
            _blendMap.BindTexture(TextureUnit.Texture4);

            liteRefractionShader.SetTextureR(1);
            liteRefractionShader.SetTextureG(2);
            liteRefractionShader.SetTextureB(3);
            liteRefractionShader.SetTextureBlack(0);
            liteRefractionShader.SetBlendMap(4);
            liteRefractionShader.SetMaterial(_terrainMaterial);
            liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteRefractionShader.SetDirectionalLight(Sun);
            liteRefractionShader.SetClippingPlane(ref clipPlane);
            _buffer.RenderVAO(PrimitiveType.Triangles);
            liteRefractionShader.stopProgram();
            GL.Disable(EnableCap.ClipDistance0);
        }

        public void renderTerrain(PrimitiveType mode, DirectionalLight Sun,
            List<PointLight> pointLights, BaseCamera camera, Matrix4 ProjectionMatrix, Vector4 clipPlane = new Vector4())     //Rendering ландшафта
        {
            postConstructor();
            Matrix4 ModelMatrix = Matrix4.Identity;

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            _shader.startProgram();

            _textureBlack.BindTexture(TextureUnit.Texture0);
            _textureR.BindTexture(TextureUnit.Texture1);
            _textureG.BindTexture(TextureUnit.Texture2);
            _textureB.BindTexture(TextureUnit.Texture3);
            _blendMap.BindTexture(TextureUnit.Texture4);

            /*For normal mapping local variables*/
            Int32 nmR = -1, nmG = -1, nmB = -1, nmBlack = -1;

            /*TO DO :
             * if texture exists - bind this texture, and assign temp variable samplers ID,
             * else - leave temp variable with value -1. */
            if (_normalMapR != null) { nmR = 5; _normalMapR.BindTexture(TextureUnit.Texture5); }
            if (_normalMapG != null) { nmG = 6; _normalMapG.BindTexture(TextureUnit.Texture6); }
            if (_normalMapB != null) { nmB = 7; _normalMapB.BindTexture(TextureUnit.Texture7); }
            if (_normalMapBlack != null) { nmBlack = 8; _normalMapBlack.BindTexture(TextureUnit.Texture8); }

            if (Sun != null)
            {
                // Get shadow handler
                ITexture shadowMap = Sun.GetShadow().GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture9); // shadowmap
                _shader.SetDirectionalLightShadowMatrix(Sun.GetShadow().GetShadowMatrix(ref ModelMatrix, ref ProjectionMatrix));
            }

            _shader.SetTextureR(1, nmR, nmR > 0);
            _shader.SetTextureG(2, nmG, nmG > 0);
            _shader.SetTextureB(3, nmB, nmB > 0);
            _shader.SetTextureBlack(0, nmBlack, nmBlack > 0);
            _shader.SetBlendMap(4);
            _shader.SetMaterial(_terrainMaterial);
            _shader.SetTransformationMatrices(ref ModelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            _shader.SetDirectionalLight(Sun);
            _shader.SetPointLights(pointLights);
            _shader.SetMist(_mist);
            _shader.SetClippingPlane(ref clipPlane);
            _shader.SetDirectionalLightShadowMap(9);

            _buffer.RenderVAO(mode);
            _shader.stopProgram();
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (this._postConstructor)
            {
                _shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<LandscapeShader>, string, LandscapeShader>(ProjectFolders.ShadersPath + "terrainVertexShader.glsl" + "," + ProjectFolders.ShadersPath + "terrainFragmentShader.glsl");
                liteReflectionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionTerrainShader>, string, WaterReflectionTerrainShader>(ProjectFolders.ShadersPath + "waterReflectionTerrainVS.glsl" + "," + ProjectFolders.ShadersPath + "waterReflectionTerrainFS.glsl");
                liteRefractionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionTerrainShader>, string, WaterRefractionTerrainShader>(ProjectFolders.ShadersPath + "waterRefractionTerrainVS.glsl" + "," + ProjectFolders.ShadersPath + "waterRefractionTerrainFS.glsl");

                _buffer = LandscapeBuilder.getTerrainAttributes(this.LandscapeMap, this._normalsSmoothLvl);
                this._postConstructor = !this._postConstructor;
            }
        }

        public Landscape(float MapSize, float MaximumHeight, Int32 normalSmoothLvl, string mapFile, string textureR, string textureG,
            string textureB, string textureBlack, string blendMap)
        {
            this._postConstructor = true;
            this._terrainMaterial = new Material(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
               new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 10.0f, 1.0f);
            this._textureR = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(textureR);
            this._textureG = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(textureG);
            this._textureB = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(textureB);
            this._textureBlack = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(textureBlack);
            this._blendMap = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(blendMap);
            this.MapSize = MapSize;
            this.MaximumHeight = MaximumHeight;
            this._normalsSmoothLvl = normalSmoothLvl;
            Bitmap map = null;
            try
            {
                map = new Bitmap(mapFile);
                LandscapeMap = new TableGrid(map.Height, MapSize / map.Height);
            }
            catch (ArgumentException ef)
            {
                Debug.Log.addToLog("Terrain height map file load error : " + ef.Message);
                System.Environment.Exit(0);
            }
            LandscapeBuilder.loadHeightMap(map, this);
            map.Dispose();
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<LandscapeShader>, string, LandscapeShader>(_shader);
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionTerrainShader>, string, WaterReflectionTerrainShader>(liteReflectionShader);
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionTerrainShader>, string, WaterRefractionTerrainShader>(liteRefractionShader);
            _buffer.CleanUp();
            if (_normalMapR != null) PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_normalMapR);
            if (_normalMapG != null) PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_normalMapG);
            if (_normalMapB != null) PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_normalMapB);
            if (_normalMapBlack != null) PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_normalMapBlack);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_textureBlack);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_textureR);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_textureG);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_textureB);
            PoolProxy.FreeResourceMemory<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_blendMap);
        }

        public ITexture GetDiffuseMap()
        {
            return _blendMap;
        }

        public ITexture GetNormalMap()
        {
            return _normalMapBlack;
        }

        public ITexture GetSpecularMap()
        {
            return null;
        }

        public Matrix4 GetWorldMatrix()
        {
            return Matrix4.Identity;
        }

        public VertexArrayObject GetMeshVao()
        {
            return _buffer;
        }

        #endregion
    }
}
