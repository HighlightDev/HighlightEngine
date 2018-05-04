using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PhysicsBox;
using MassiveGame.API;
using MassiveGame.RenderCore.Lights;

using System.Linq;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using IVisible = MassiveGame.Optimization.IVisible;
using MassiveGame.API.Factory.ObjectArguments;
using MassiveGame.API.IDgenerator;
using MassiveGame.API.Collision_Handler;
using MassiveGame.Sun.DayCycle;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.Optimization;
using MassiveGame.RenderCore.Shadows;
using FramebufferAPI;
using MassiveGame.RenderCore;
using MassiveGame.Debug.UiPanel;
using TextureLoader;
using MassiveGame.API.Collector;
using System.Threading;
using PhysicsBox.ComponentCore;
using MassiveGame.ComponentCore;
using MassiveGame.Settings;
using System.Drawing;
using ShaderPattern;

namespace MassiveGame.UI
{
    #region PostFxSettings

    [Flags]
    public enum PostProcessFlag
    {
        PostFxDisable = 0x0001,
        GrEffectsDisable = 0x0002,
        PostFx_and_GrEffects_Disable = PostFxDisable | GrEffectsDisable,
        PostFxEnable = 0x0010,
        GrEffectsEnable = 0x0020,
        PostFx_and_GrEffects_Enable = PostFxEnable | GrEffectsEnable
    }

    #endregion

    public partial class MainUI : Form
    {
        List<IDrawable> shadowList;
        DefaultFrameBuffer defaultFB;
        UiFrame DefaultFrame;

        private void setTestValues()
        {
            DefaultFrame = new UiFrame(0, 0, 1, 1);

            var rtParams = new RenderTargetParams(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, DOUEngine.ShadowMapRezolution.X, DOUEngine.ShadowMapRezolution.Y, PixelFormat.DepthComponent, PixelType.Float);
            DOUEngine.Sun = new DirectionalLight(rtParams, new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));
            DOUEngine.Sun.GetShadowHandler().CreateShadowMapCache();

            var dayPhases = new MassiveGame.Sun.DayCycle.DayPhases(new Sun.DayCycle.DayPhases.Morning(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(.7f)),
                    new Sun.DayCycle.DayPhases.Day(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.9f, 0.79f, 0.79f), new Vector3(1.0f)),
                new Sun.DayCycle.DayPhases.Evening(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.30f, 0.30f), new Vector3(0.9f)),
                new Sun.DayCycle.DayPhases.Night(new Vector3(0.09f, 0.09f, 0.09f), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.0f)));

            DOUEngine.DayCycle = new DayLightCycle(DOUEngine.Sun,
                DOUEngine.MAP_SIZE, dayPhases);
            DOUEngine.DayCycle.SetTime(25);
            DOUEngine.DayCycle.TimeFlow = 0.1f;

            DOUEngine.PointLight = new List<PointLight>();

            #region Audio initialization and loading
            //AudioMaster.Init(ALDistanceModel.LinearDistanceClamped);
            //AudioMaster.SetListenerData(
            //    camera.getPosition().X,
            //    camera.getPosition().Y,
            //    camera.getPosition().Z
            //);
            //SB_step = AudioMaster.LoadSound(new string[] {
            //    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass1.wav",
            //    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass2.wav",
            //    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass3.wav",
            //    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass4.wav"
            //});
            //SB_collide = AudioMaster.LoadSound(new string[] {
            //    ProjectFolders.AudioFolders.AudioActorCollidePath + "wav/clothes1.wav",
            //    ProjectFolders.AudioFolders.AudioActorCollidePath + "wav/clothes2.wav",
            //    ProjectFolders.AudioFolders.AudioActorCollidePath + "wav/clothes3.wav"
            //});
            //SB_ambient = AudioMaster.LoadSound(ProjectFolders.AudioFolders.AudioAmbientPath + "wav/howling_wind_u1.wav");
            #endregion

            /*Create mist component*/
            DOUEngine.Mist = new MistComponent(0.009f, 1f, new Vector3(0.7f, 0.75f, 0.8f));

            // temporary

            DOUEngine.Map = new Terrain(DOUEngine.MAP_SIZE, DOUEngine.MAP_HEIGHT, 3,  // Генерация ландшафта
                   ProjectFolders.HeightMapsTexturesPath + "heightmap2.png",
                   ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01.jpg",
                   ProjectFolders.MultitexturesPath + "NewLandscape/tundra02.jpg",
                   ProjectFolders.MultitexturesPath + "b.png",
                   ProjectFolders.MultitexturesPath + "NewLandscape/snow01.jpg",
                   ProjectFolders.MultitexturesPath + "blendMap.png");

            DOUEngine.Map.SetNormalMapR(ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01_n.png");
            DOUEngine.Map.SetNormalMapG(ProjectFolders.MultitexturesPath + "NewLandscape/tundra02_n.png");
            //DOUEngine.Map.SetNormalMapBlack(ProjectFolders.MultitexturesPath + "NewLandscape/snow01_n.png");
            DOUEngine.Map.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_high.png");

            //DOUEngine.Map = new Terrain(DOUEngine.MAP_SIZE, DOUEngine.MAP_HEIGHT, 3,  // Генерация ландшафта
            //        ProjectFolders.HeightMapsTexturesPath + "heightmap2.png",
            //        ProjectFolders.MultitexturesPath + "mud.png",
            //        ProjectFolders.MultitexturesPath + "grassFlowers.png",
            //        ProjectFolders.MultitexturesPath + "b.png",
            //        ProjectFolders.MultitexturesPath + "grass.png",
            //        ProjectFolders.MultitexturesPath + "blendMap.png");
            //DOUEngine.Map.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_high.png");

            DOUEngine.Map.SetMist(DOUEngine.Mist);

            string modelPath = ProjectFolders.ModelsPath + "City_House_2_BI.obj";
            string texturePath = ProjectFolders.TextureAtlasPath + "city_house_2_Col.jpg";
            string normalMapPath = ProjectFolders.NormalMapsPath + "city_house_2_Nor.jpg";
            string specularMapPath = ProjectFolders.SpecularMapsPath + "city_house_2_Spec.jpg";


            DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                new Vector3(220, 35, 350), new Vector3(0, 180, 0), new Vector3(10))));
            DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
               new Vector3(280, 35, 350), new Vector3(0, 180, 0), new Vector3(10))));
            DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                new Vector3(230, 35, 410), new Vector3(0, 180, 0), new Vector3(10))));
            DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
               new Vector3(260, 35, 400), new Vector3(0, 180, 0), new Vector3(10))));
            foreach (var item in DOUEngine.City)
            {
                item.SetMistComponent(DOUEngine.Mist);
                item.SetCollisionDetector(DOUEngine.Collision);
            }

           

            modelPath = ProjectFolders.ModelsPath + "Dino.3ds";
            texturePath = ProjectFolders.MultitexturesPath + "path.png";
            normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            MotionEntityArguments arg = new MotionEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                IdGenerator.GeneratePlayerId(), 0.3f, new Vector3(200, 0, 230), new Vector3(270, 90, 0), new Vector3(0.2f, 0.2f, 0.2f));

            DOUEngine.Player = (Player)EngineObjectCreator.CreateInstance(arg);
            DOUEngine.Player.setSoundAttachment(DOUEngine.SB_step, DOUEngine.SB_collide);
            DOUEngine.Player.SetMistComponent(DOUEngine.Mist);
            DOUEngine.Player.SetCollisionDetector(DOUEngine.Collision);

            // TEST components
            ComponentSerializer serializer = new ComponentSerializer();
            SerializedComponentsContainer container = serializer.DeserializeComponents("Collision.cl");
            Component parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            Component component = convertToSceneComponent(parent);
            DOUEngine.Player.SetComponents(component.ChildrenComponents);

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "b.png";

            arg = new MotionEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                IdGenerator.GeneratePlayerId(), 0.3f, new Vector3(180, 0, 220), new Vector3(0, 0, 0), new Vector3(10));

            DOUEngine.Enemy = (Player)EngineObjectCreator.CreateInstance(arg);
            DOUEngine.Enemy.setSoundAttachment(DOUEngine.SB_step, DOUEngine.SB_collide);
            DOUEngine.Enemy.SetMistComponent(DOUEngine.Mist);
            DOUEngine.Enemy.SetCollisionDetector(DOUEngine.Collision);
            arg = null;

            DOUEngine.Grass = new PlantReadyMaster(
                4000, DOUEngine.MAP_SIZE, PlantModels.getBillboardModel1(), new Vector3(1),
                new string[] { ProjectFolders.GrassTexturesPath + "grass1.png",
                    ProjectFolders.GrassTexturesPath + "grass2.png",
                    ProjectFolders.GrassTexturesPath + "grass3.png"}, new WindComponent(2.35f, 1.1f, 0.6f, new Vector3(0.6f, 0, 0.3f)), DOUEngine.Mist);    //Добавление травы

            //EngineSingleton.Grass = new PlantBuilderMaster(100, PlantModels.getBillboardModel1(), TextureSet.PlantTextureSet, new WindComponent(2.35f, 1.1f, 0.6f, new Vector3(0.6f, 0, 0.3f)));    //Добавление травы

            DOUEngine.Plant1 = new PlantReadyMaster(13, DOUEngine.MAP_SIZE, PlantModels.getPlantModel2(), new Vector3(1),
                new string[] { ProjectFolders.GrassTexturesPath + "fern.png" },
              new WindComponent(0.95f, 0.35f, 0.5f, new Vector3(0.5f, 0, 0.5f)), DOUEngine.Mist);

            DOUEngine.Skybox = new Skybox(
                    new string[] { ProjectFolders.SkyboxTexturesPath + "/Day/" + "right.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "left.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "top.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "bottom.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "back.bmp",
                    ProjectFolders.SkyboxTexturesPath + "/Day/" + "front.bmp" },
                    new string[] { ProjectFolders.SkyboxTexturesPath + "/Night/" + "right.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "left.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "top.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "bottom.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "back.png",
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "front.png" });       //Скайбокс
            DOUEngine.Skybox.setMistComponent(DOUEngine.Mist);

            //EngineSingleton.SourceAmbient = new Source(EngineSingleton.SB_ambient, 0.05f, 1, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            //EngineSingleton.SourceAmbient.SetMaxDistance(0);
            //EngineSingleton.SourceAmbient.SetLooping(true);
            //EngineSingleton.SourceAmbient.Play();

            DOUEngine.Water = new WaterEntity(ProjectFolders.WaterTexturePath + "DUDV.png", ProjectFolders.WaterTexturePath + "normal.png",
                new Vector3(160, 29, 254), new Vector3(0, 0, 0), new Vector3(70, 1, 100), new WaterQuality(true, true, true), 10);
            DOUEngine.Water.setMist(DOUEngine.Mist);

            DOUEngine.SunReplica = new SunRenderer(DOUEngine.Sun, ProjectFolders.SunTexturePath + "sunC.png",
                    ProjectFolders.SunTexturePath + "sunB.png");

            DOUEngine.Picker = new MousePicker(DOUEngine.ProjectionMatrix, DOUEngine.Camera);

            //EngineSingleton.EnvObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(true), TextureSet.PlayerTextureSet2,
            //    TextureSet.SkyboxDayCubemapTexture, new Vector3(180, 0, 220), new Vector3(0, 0, 0), new Vector3(10));
            //mirror = new Mirror[2];
            //mirror[0] = new Mirror(null, new Vector3(180, 25, 150), new Vector3(0, 45, 0), new Vector3(0.25f));
            //mirror[1] = new Mirror(null, new Vector3(180, 25, 180), new Vector3(0, 90, 0), new Vector3(0.25f));

            DOUEngine.Lights = new Light_visualization.VisualizeLight(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png"
                , DOUEngine.PointLight);

            DOUEngine.Lens = new LensFlareRenderer();
            DOUEngine.Ray = new GodRaysRenderer();
            //DOUEngine.PostProc = new PostprocessRenderer(PostprocessType.BLOOM);
            //DOUEngine.PostProc.BloomPass = 1;
            //DOUEngine.PostProc.BlurWidth = 18;

            setGraphicsSettings();

            //gras = new Grass(new Vector3(1, 0, 1), new Vector3(1), new Vector3(0), new Vector3(0.2f, 0.8f, 0.3f));
            //envObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(false), TextureSet.PlayerTextureSet2, TextureSet.SkyboxDayCubemapTexture,
            //    new Vector3(40, 70, 40), new Vector3(0, 0, 0), new Vector3(0.5f));

            DOUEngine.Camera.SetThirdPerson(DOUEngine.Player);
            //EngineSingleton.Camera.SetFirstPerson();

            shadowList = new List<IDrawable>();
            DOUEngine.City.ForEach(new Action<Building>((house) => { shadowList.Add(house); }));
            shadowList.Add(DOUEngine.Player);
            shadowList.Add(DOUEngine.Enemy);
            shadowList.Add(DOUEngine.Map);

            frame = new UiFrame(0, 0, 0.5f, 0.5f);
        }

        #region TEST

        private void convertToSceneComponentRecursive(Component existing, Component duplicate)
        {
            for (Int32 i = 0; i < existing.ChildrenComponents.Count; i++)
            {
                Component existingChild = existing.ChildrenComponents[i];
                duplicate.ChildrenComponents.Add(new SceneComponent(existingChild));
                Component duplicateChild = duplicate.ChildrenComponents[i];
                duplicate.ChildrenComponents[i].ParentComponent = duplicate;
                convertToSceneComponentRecursive(existingChild, duplicateChild);
            }
        }

        private Component convertToSceneComponent(Component existing)
        {
            Component duplicate = new Component();
            convertToSceneComponentRecursive(existing, duplicate);
            return duplicate;
        }

        #endregion

        #region Constructors
        public MainUI()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            preConstructor();
        }

        public MainUI(int width, int height)
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            this.Width = width;
            this.Height = height;
            preConstructor();
        }

        private void preConstructor() //Start initialize values
        {
            SettingsLoader settingsLoader = new SettingsLoader();
            DOUEngine.ScreenRezolution = settingsLoader.GetScreenRezolution();
            DOUEngine.ShadowMapRezolution = settingsLoader.GetDirectionalShadowMapRezolution();

            DOUEngine.PostConstructor = true;
            DOUEngine.Camera = new Camera(250.0f, 70, 260.0f, 140.0f, 70.0f, 160.0f + 50f, 0.0f, 1.0f, 0.0f);
            DOUEngine.PrevCursorPosition = new System.Drawing.Point(-1, -1);
            DOUEngine.ElapsedTime = DateTime.Now;
            DOUEngine.keyboardMaskArray = new bool[4];
        }

        private void EngineTickTimerCallback(object target)
        {
            DOUEngine.DayCycle.TickTimeFlow();
            // Do smth better (PlayerController)
            if (DOUEngine.keyboardMaskArray.Any<bool>((key) => key == true))
            {
                var previousPosition = DOUEngine.Player.ObjectPosition;
                DOUEngine.Player.move(DOUEngine.Map, DOUEngine.Camera);
                DOUEngine.Camera.UpdateHeight(previousPosition);
            }

            if (DOUEngine.SunReplica != null)
            {
                DOUEngine.SunReplica.UpdateFrustumView();
            }

            if (DOUEngine.Skybox != null)
            {
                DOUEngine.Skybox.AnimationTick(Convert.ToSingle(DOUEngine.RenderTime));
            }
        }

        private void StopEngineTickTimer()
        {
            DOUEngine.EngineTickTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void StartEngineTickTimer()
        {
            DOUEngine.EngineTickTimer.Change(0, 20);
        }

        private void postConstructor()
        {
            if (DOUEngine.PostConstructor)
            {
                DOUEngine.ProjectionMatrix = Matrix4.Identity;

                DOUEngine.Collision = new CollisionDetector();
                DOUEngine.Collision.BoxCollision += new EventHandler(new CollisionBoxReaction().Reaction);
                DOUEngine.Collision.NoCollision += new EventHandler(new CollisionMissReaction().Reaction);
                DOUEngine.Collision.WallCollision += new EventHandler(new CollisionWallReaction().Reaction);

                DOUEngine.Buildings = new List<Building>();
                DOUEngine.City = new List<Building>();
                // need to delete NewMesh.msh if it exists
                if (File.Exists(@"NewModel.msh"))
                    File.Delete(@"NewModel.msh");
                defaultMatrixSettings();

                setTestValues();

                // add objects to optimization list
                DOUEngine.RenderedPrimitives = new List<IVisible> { DOUEngine.SunReplica, DOUEngine.Water,
                    DOUEngine.Player, DOUEngine.Enemy, DOUEngine.EnvObj };
                DOUEngine.RenderedPrimitives.AddRange(DOUEngine.City);

                DOUEngine.AffectedByLightPrimitives = new List<ILightAffection> { DOUEngine.Player, DOUEngine.Enemy, DOUEngine.EnvObj };
                DOUEngine.AffectedByLightPrimitives.AddRange(DOUEngine.Buildings);
                DOUEngine.AffectedByLightPrimitives.AddRange(DOUEngine.City);

                // Main engine timer
                if (DOUEngine.EngineTickTimer == null)
                {
                    DOUEngine.EngineTickTimer = new System.Threading.Timer(new System.Threading.TimerCallback(EngineTickTimerCallback));
                    DOUEngine.EngineTickTimer.Change(100, 20);
                }
            }
        }
        #endregion

        #region Logics
        private void defaultMatrixSettings()
        {
            // create projection matrix
            DOUEngine.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(DOUEngine.FoV), DOUEngine.SCREEN_ASPECT_RATIO,
                DOUEngine.NEAR_CLIPPING_PLANE, DOUEngine.FAR_CLIPPING_PLANE);
        }

        private void gameLogics(bool redraw = false)
        {
            if (DOUEngine.PostConstructor)
            {
                if (DOUEngine.Player != null)
                    DOUEngine.Player.move(DOUEngine.Map, DOUEngine.Camera);
                //if (redraw)
                //{
                //    foreach (Building building in buildings)
                //    {
                //        collision.addPolygonArray(building.getCollisionTriangles());
                //    }
                //}
                //DOUEngine.PostConstructor = !DOUEngine.PostConstructor;
            }
            // EngineSingleton.Picker.update();
            DOUEngine.Mist.update();
            DOUEngine.Camera.Update(DOUEngine.Map);

            // Find which primitives are visible for current frame
            VisibilityApi.IsInView(DOUEngine.RenderedPrimitives,
               ref DOUEngine.ProjectionMatrix, DOUEngine.Camera.getViewMatrix());

            // Find which light sources affects on primitives
            LightAffectionApi.IsLightAffects(DOUEngine.AffectedByLightPrimitives, DOUEngine.PointLight);
        }
        #endregion

        UiFrame frame;

        #region Render queue
        private void RenderLoop(bool redraw)
        { 
            //Render scene to objects with framebuffers

            DepthPass();

            RenderToWaterRendertargets();

            /*  TO DO :
             *  PP_AND_LENS_DISABLED - render scene to default framebuffer;                   *
             *  PP_AND_LENS_ENABLED - render scene to postprocess framebuffer                 *
             *  and apply postprocess filters, then render scene with color masking,          *
             *  and sun without masking to lens framebuffer and apply lens flare. Add lens    *
             *  contribution to pp result image;                                              *
             *  PP_DISABLED_LENS_ENABLED - render scene with color masking,                   *
             *  and sun without masking to lens framebuffer and apply lens flare. Add lens    *
             *  contribution to default image;                                                *
             *  PP_ENABLED_LENS_DISABLED - render scene to postprocess framebuffer            *
             *  and apply postprocess filters.                                                */
       
            switch (DOUEngine.Settings)
            {
                case PostProcessFlag.PostFx_and_GrEffects_Disable:
                    {
                        defaultFB.Bind();
                        //SwitchToScreenBuffer();
                        RenderAll(redraw);
                        frame.Render(DOUEngine.Sun.GetShadowHandler().GetTextureHandler(), new Point(this.Width, this.Height));
                    //    frame.Render(new Texture2Dlite(
                    //(int)DOUEngine.Water._fbo.Texture.TextureID[1],
                    //new RectParams(DOUEngine.Water._fbo.Texture.Rezolution[1].widthRezolution, DOUEngine.Water._fbo.Texture.Rezolution[1].heightRezolution)
                    //), new Point(this.Width, this.Height));
                        defaultFB.Unbind();
                        break;
                    }
                case PostProcessFlag.PostFx_and_GrEffects_Enable:
                    {
                        // if sun isn't in camera's view - don't apply god rays and lens flare
                        if (DOUEngine.SunReplica.IsInCameraView)
                        {
                            /*Render to postprocess framebuffer*/
                            DOUEngine.PostProc.beginPostProcessing();
                            RenderAll(redraw);

                            DOUEngine.PostProc.sendPostProcessingToGraphicsFilter(this.Width, this.Height, blurEnable);
                            blurEnable = false; //disable motion blur

                            if (DOUEngine.Lens != null && DOUEngine.Ray != null)
                            {
                                /*Render to GodRays framebuffer*/
                                DOUEngine.Ray.beginGodRaysSpecial();
                                renderToGodRaysScene(DOUEngine.Camera, redraw);
                                /* Pass result to lens flare */
                                DOUEngine.Ray.sendGodRaysWithPostprocessToNextStage(this.Width, this.Height, DOUEngine.Sun.Position,
                                    DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix, DOUEngine.PostProc.PostprocessFilterResult);

                                /*Render to lens flare framebuffer*/
                                DOUEngine.Lens.beginLensFlareSpecialScene();
                                renderToLensFlareScene(DOUEngine.Camera, redraw);
                                /*Pass result on the screen*/
                                DOUEngine.Lens.endLensFlareWithPostprocess(DOUEngine.Camera, this.Width, this.Height, DOUEngine.Ray.FilterResult);
                            }
                            else if (DOUEngine.Lens != null && DOUEngine.Ray == null)
                            {
                                /*Render to lens flare framebuffer*/
                                DOUEngine.Lens.beginLensFlareSpecialScene();
                                renderToLensFlareScene(DOUEngine.Camera, redraw);
                                /*Pass result on the screen*/
                                DOUEngine.Lens.endLensFlareWithPostprocess(DOUEngine.Camera, this.Width, this.Height, DOUEngine.PostProc.PostprocessFilterResult);
                            }
                            else if (DOUEngine.Lens == null && DOUEngine.Ray != null)
                            {
                                /*Render to GodRays framebuffer*/
                                DOUEngine.Ray.beginGodRaysSpecial();
                                renderToGodRaysScene(DOUEngine.Camera, redraw);
                                /*Pass result on the screen*/
                                DOUEngine.Ray.endGodRaysWithPostprocess(this.Width, this.Height, DOUEngine.Sun.Position,
                                    DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix, DOUEngine.PostProc.PostprocessFilterResult);
                            }
                        }
                        else
                        {
                            DOUEngine.PostProc.beginPostProcessing();
                            RenderAll(redraw);
                            /*Pass result on the screen*/
                            DOUEngine.PostProc.endPostProcessing(this.Width, this.Height, blurEnable);
                            blurEnable = false; //disable motion blur
                        }
                        break;
                    }
                case  PostProcessFlag.PostFxDisable | PostProcessFlag.GrEffectsEnable:
                    {
                        // if sun isn't in camera's view - don't apply god rays and lens flare
                        if (DOUEngine.SunReplica.IsInCameraView)
                        {
                            if (DOUEngine.Lens != null && DOUEngine.Ray != null)
                            {
                                /*Render to GodRays framebuffer*/
                                DOUEngine.Ray.beginGodRaysSimple();
                                RenderAll(redraw);
                                DOUEngine.Ray.beginGodRaysSpecial();
                                renderToGodRaysScene(DOUEngine.Camera, redraw);
                                /*Pass result on the screen*/
                                DOUEngine.Ray.sendGodRaysWithoutPostprocessToNextStage(this.Width, this.Height,
                                    DOUEngine.Sun.Position, DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix);

                                /*Render to lens flare framebuffer*/
                                DOUEngine.Lens.beginLensFlareSpecialScene();
                                renderToLensFlareScene(DOUEngine.Camera, redraw);
                                /*Pass result on the screen*/
                                DOUEngine.Lens.endLensFlareWithPostprocess(DOUEngine.Camera, this.Width, this.Height, DOUEngine.Ray.FilterResult);
                            }
                            else if (DOUEngine.Lens != null && DOUEngine.Ray == null)
                            {
                                DOUEngine.Lens.beginLensFlareDefaultScene();
                                RenderAll(redraw);
                                //Render scene with color masking
                                DOUEngine.Lens.beginLensFlareSpecialScene();
                                renderToLensFlareScene(DOUEngine.Camera, redraw);
                                /*Pass result to screen*/
                                DOUEngine.Lens.endLensFlareWithoutPostprocess(DOUEngine.Camera, this.Width, this.Height);
                            }
                            else if (DOUEngine.Lens == null && DOUEngine.Ray != null)
                            {
                                /*Render to GodRays framebuffer*/
                                DOUEngine.Ray.beginGodRaysSimple();
                                RenderAll(redraw);
                                DOUEngine.Ray.beginGodRaysSpecial();
                                renderToGodRaysScene(DOUEngine.Camera, redraw);
                                /*Pass result on the screen*/
                                DOUEngine.Ray.endGodRaysWithoutPostprocess(this.Width, this.Height,
                                    DOUEngine.Sun.Position, DOUEngine.Camera.getViewMatrix(), DOUEngine.ProjectionMatrix);
                            }
                        }
                        else
                        {
                            defaultFB.Bind();
                            //SwitchToScreenBuffer();
                            RenderAll(redraw);
                            defaultFB.Unbind();
                            break;
                        }
                        break;
                    }
                case PostProcessFlag.PostFxEnable | PostProcessFlag.GrEffectsDisable:
                    {
                        DOUEngine.PostProc.beginPostProcessing();

                        RenderAll(redraw);
                        /*Pass result on the screen*/
                        DOUEngine.PostProc.endPostProcessing(this.Width, this.Height, blurEnable);
                        blurEnable = false; //disable motion blur
                        break;
                    }
            }

            DefaultFrame.Render(defaultFB.GetTextureHandler(), new Point(this.Width, this.Height));
        }
        #endregion

        #region System functions

        private void SwitchToScreenBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        private void setGraphicsSettings()
        {
            /*TO DO - postprocess settings assigning*/
            if (DOUEngine.PostProc == null && (DOUEngine.Lens == null && DOUEngine.Ray == null)) DOUEngine.Settings = PostProcessFlag.PostFx_and_GrEffects_Disable;
            if (DOUEngine.PostProc != null && (DOUEngine.Lens == null && DOUEngine.Ray == null)) DOUEngine.Settings = PostProcessFlag.PostFxEnable | PostProcessFlag.GrEffectsDisable;
            if (DOUEngine.PostProc == null && (DOUEngine.Lens != null || DOUEngine.Ray != null)) DOUEngine.Settings = PostProcessFlag.PostFxDisable | PostProcessFlag.GrEffectsEnable;
            if (DOUEngine.PostProc != null && (DOUEngine.Lens != null || DOUEngine.Ray != null)) DOUEngine.Settings = PostProcessFlag.PostFx_and_GrEffects_Enable;
            /*TO DO - postprocess settings assigning*/
        }

        private void AdjustMouseCursor()
        {
            DOUEngine.SCREEN_POSITION_X = this.Location.X + 8;
            DOUEngine.SCREEN_POSITION_Y = this.Location.Y + 8;
            // для корректной работы камеры с учетом рамки
            // + 8 из-за того, что при открытии на полный экран, смещение стартовой позиции окна = -8
            //EngineSingleton.SCREEN_POSITION_X = ((EngineSingleton.WINDOW_BORDER != WindowBorder.Hidden) && (EngineSingleton.WINDOW_STATE != WindowState.Fullscreen))
            //    ? this.Location.X + 8 : this.Location.X;
            //EngineSingleton.SCREEN_POSITION_Y = ((EngineSingleton.WINDOW_BORDER != WindowBorder.Hidden) && (EngineSingleton.WINDOW_STATE != WindowState.Fullscreen))
            //    ? this.Location.Y + 8 : this.Location.Y;
        }

        private void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);//Очистка буфера цвета
            GL.Clear(ClearBufferMask.DepthBufferBit);//Очистка буфера глубины
            GL.ClearColor(Color4.Black);//Очистка содержимого на экране + задание цвета бэка
        }
        #endregion

        #region Cleaning

        private void unsubscribeCollisions()
        {
            // unsubscribe from events
            DOUEngine.Collision.BoxCollision -= (new CollisionBoxReaction().Reaction);
            DOUEngine.Collision.NoCollision -= (new CollisionMissReaction().Reaction);
            DOUEngine.Collision.WallCollision -= (new CollisionWallReaction().Reaction);
        }

        private void cleanEverythingUp()
        {
            if (DOUEngine.Water != null) DOUEngine.Water.cleanUp();
            if (DOUEngine.SunReplica != null) DOUEngine.SunReplica.cleanUp();
            if (DOUEngine.Map != null) DOUEngine.Map.cleanUp();

            if (DOUEngine.Player != null) DOUEngine.Player.cleanUp();
            if (DOUEngine.Enemy != null) DOUEngine.Enemy.cleanUp();
            if (DOUEngine.Grass != null) DOUEngine.Grass.cleanUp();
            if (DOUEngine.Plant1 != null) DOUEngine.Plant1.cleanUp();

            if (DOUEngine.Buildings != null) foreach (Building building in DOUEngine.Buildings) building.cleanUp();

            if (DOUEngine.City != null) foreach (Building house in DOUEngine.City) { house.cleanUp(); }

            if (DOUEngine.Skybox != null) DOUEngine.Skybox.cleanUp();

            if (DOUEngine.PostProc != null) DOUEngine.PostProc.cleanUp();
            if (DOUEngine.Lens != null) DOUEngine.Lens.cleanUp();
            if (DOUEngine.Ray != null) DOUEngine.Ray.cleanUp();

            //this.sourceAmbient.Delete();
            //AudioMaster.CleanUp();
        }

        #endregion
    }
}

