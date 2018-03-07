using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame
{
    public static class TextureSet
    {
        public static Texture2D PlayerTextureSet { private set; get; }
        public static Texture2D PlayerTextureSet2 { private set; get; }
        public static Texture2D PlantTextureSet { private set; get; }
        public static Texture2D TerrainTextureSet { private set; get; }
        public static Texture2D BuildingTextureSet_Wall_1 { private set; get; }
        public static Texture2D BuildingTextureSet_Wall_1_Door { private set; get; }
        public static Texture2D CityTextureSet { private set; get; }
        public static Texture2D LampTextureSet { private set; get; }
        public static Texture2D FernTextureSet { private set; get; }
        public static Texture2D WaterTextureSet { private set; get; }
        public static Texture2D SunTextureSet { private set; get; }
        public static Texture2D LensFlareTextureSet { private set; get; }
        public static Texture1D LensFlareTextureSet2 { private set; get; }
        public static CubemapTexture SkyboxDayCubemapTexture { private set; get; }
        public static CubemapTexture SkyboxNightCubemapTexture { private set; get; }
        public static SingleTexture2D LightLampTexture { private set; get; }
        public static Texture2D GlowingMap { private set; get; }


        public static void LOAD_TEXTURE_SETS()
        {
            GlowingMap = new Texture2D(new string[] { ProjectFolders.TextureAtlasPath + "glowingMap.png" });
            LampTextureSet = new Texture2D(new string[] { ProjectFolders.TextureAtlasPath + "lamp1.png" });
            CityTextureSet = new Texture2D(new string[] {ProjectFolders.TextureAtlasPath + "city_house_2_Col.jpg", 
            ProjectFolders.NormalMapsPath + "city_house_2_Nor.jpg", ProjectFolders.SpecularMapsPath + "city_house_2_Spec.jpg"});
            LensFlareTextureSet = new Texture2D(new string[] { ProjectFolders.LensFlareTexturePath + "lensdirt.png",
                    ProjectFolders.LensFlareTexturePath + "lensstar.png"}, OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge, false, -1f);
            LensFlareTextureSet2 = new Texture1D(new string[] { ProjectFolders.LensFlareTexturePath + "lenscolor.png" }, false, 0);
            SunTextureSet = new Texture2D(new string[] { ProjectFolders.SunTexturePath + "sunC.png",
                    ProjectFolders.SunTexturePath + "sunB.png" });
            WaterTextureSet = new Texture2D(new string[] { ProjectFolders.WaterTexturePath + "DUDV.png",
                    ProjectFolders.WaterTexturePath + "normal.png"});

            PlayerTextureSet = new Texture2D(new string[] { ProjectFolders.MultitexturesPath + "path.png",
                    ProjectFolders.NormalMapsPath + "brick_nm_high.png",
                    ProjectFolders.SpecularMapsPath + "brick_sm.png" });
            PlayerTextureSet2 = new Texture2D(new string[] { ProjectFolders.MultitexturesPath + "b.png",
                    ProjectFolders.NormalMapsPath + "brick_nm_high.png",
                    ProjectFolders.SpecularMapsPath + "brick_sm.png" });

            PlantTextureSet = new Texture2D(new string[] { ProjectFolders.GrassTexturesPath + "grass1.png",
                    ProjectFolders.GrassTexturesPath + "grass2.png",
                    ProjectFolders.GrassTexturesPath + "grass3.png"});

            FernTextureSet = new Texture2D(new string[] { ProjectFolders.GrassTexturesPath + "fern.png" });
            TerrainTextureSet = new Texture2D(new string[] { ProjectFolders.MultitexturesPath + "grass.png",
                    ProjectFolders.MultitexturesPath + "mud.png",
                    ProjectFolders.MultitexturesPath + "grassFlowers.png",
                    ProjectFolders.MultitexturesPath + "b.png",
                    ProjectFolders.MultitexturesPath + "blendMap.png"});

            //_buildingTextureSet_wall_1 = new Texture2D(new string[] { ProjectFolders.TextureAtlasPath + "TA1.png" });
            BuildingTextureSet_Wall_1 = new Texture2D(new string[] { ProjectFolders.TextureAtlasPath + "Wall/wall_ta.jpg",
                    ProjectFolders.NormalMapsPath + "NM.png",
                    ProjectFolders.SpecularMapsPath + "spec.bmp"});
            BuildingTextureSet_Wall_1_Door = new Texture2D(new string[] { ProjectFolders.TextureAtlasPath + "Wall/wall_9_door_ta.png",
                    ProjectFolders.NormalMapsPath + "NM.png",
                    ProjectFolders.SpecularMapsPath + "spec.bmp"});

            SkyboxDayCubemapTexture = new CubemapTexture(new string[] { ProjectFolders.SkyboxTexturesPath + "/Day/" + "right.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "left.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "top.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "bottom.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "back.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "front.bmp" });
            SkyboxNightCubemapTexture = new CubemapTexture(new string[] { ProjectFolders.SkyboxTexturesPath + "/Night/" + "right.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "left.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "top.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "bottom.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "back.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "front.png" });
            LightLampTexture = new SingleTexture2D(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png", false, false);


            if (SkyboxDayCubemapTexture.getInfo() == -1)  //Проблемы с загрузкой кубической карты
            {
                Thread.Sleep(5000); //Пауза для вывода сообщения об ошибке
                Environment.Exit(0);
            }
        }
        public static void CLEAN_TEXTURE_SETS()
        {
            CityTextureSet.cleanUp();
            PlantTextureSet.cleanUp();
            PlayerTextureSet.cleanUp();
            PlayerTextureSet2.cleanUp();
            TerrainTextureSet.cleanUp();
            BuildingTextureSet_Wall_1.cleanUp();
            BuildingTextureSet_Wall_1_Door.cleanUp();
            SkyboxDayCubemapTexture.CleanUp();
            SkyboxNightCubemapTexture.CleanUp();
            FernTextureSet.cleanUp();
            WaterTextureSet.cleanUp();
            SunTextureSet.cleanUp();
            LensFlareTextureSet.cleanUp();
            LensFlareTextureSet2.cleanUp();
            LightLampTexture.cleanUp();
        }
    }
}
