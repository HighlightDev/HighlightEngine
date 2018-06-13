using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore;
using MassiveGame.Engine;
using System.Collections.Generic;
using MassiveGame.Physics;
using MassiveGame.Settings;
using MassiveGame.Optimization;
using MassiveGame.RenderCore.Visibility;
using FramebufferAPI;
using MassiveGame.RenderCore.Lights;
using MassiveGame.Sun.DayCycle;
using MassiveGame.API;
using MassiveGame.API.Factory.ObjectArguments;
using PhysicsBox.ComponentCore;
using MassiveGame.Debug.UiPanel;
using MassiveGame.ComponentCore;
using System.IO;
using TextureLoader;

namespace MassiveGame.UI
{
    public partial class Engine : Form
    {
        private Stopwatch renderTime;
        private RenderThread renderThread;
        private GameThread gameThread;
       
        private CollisionHeadUnit collisionHeadUnit;

        private bool bPostConstructor = true;

        #region Constructors
        public Engine()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            preConstructor();
        }

        public Engine(Int32 width, Int32 height) : this()
        {
            this.Width = width;
            this.Height = height;
        }

        private void preConstructor() //Start initialize values
        {
            SettingsLoader settingsLoader = new SettingsLoader();
            DOUEngine.ScreenRezolution = settingsLoader.GetScreenRezolution();
            DOUEngine.ShadowMapRezolution = settingsLoader.GetDirectionalShadowMapRezolution();

            DOUEngine.Camera = new Camera();
            DOUEngine.PrevCursorPosition = new System.Drawing.Point(-1, -1);
            DOUEngine.ElapsedTime = DateTime.Now;
            DOUEngine.keyboardMask = new API.EventHandlers.KeyboardHandler();

            collisionHeadUnit = new CollisionHeadUnit();
            renderTime = new Stopwatch();
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                DOUEngine.ProjectionMatrix = Matrix4.Identity;
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
                DOUEngine.AffectedByLightPrimitives.AddRange(DOUEngine.City);

                // Start game and render thread execution
                renderThread = new RenderThread();
                gameThread = new GameThread(100, 1);

                renderThread.DefaultFB = new DefaultFrameBuffer(this.Width, this.Height);
            }
        }
        #endregion
 
        private void setTestValues()
        {
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
            DOUEngine.DayCycle.TimeFlow = 0.001f;

            DOUEngine.PointLight = new List<PointLight>();
          
            /*Create mist component*/
            DOUEngine.Mist = new MistComponent(0.003f, 1f, new Vector3(0.7f, 0.75f, 0.8f));

            // temporary

            //DOUEngine.terrain = new Terrain(DOUEngine.MAP_SIZE, DOUEngine.MAP_HEIGHT, 3, 
            //       ProjectFolders.HeightMapsTexturesPath + "heightmap2.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01.jpg",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/tundra02.jpg",
            //       ProjectFolders.MultitexturesPath + "b.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/snow01.jpg",
            //       ProjectFolders.MultitexturesPath + "blendMap.png");

            //DOUEngine.terrain.SetNormalMapR(ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01_n.png");
            //DOUEngine.terrain.SetNormalMapG(ProjectFolders.MultitexturesPath + "NewLandscape/tundra02_n.png");
            ////DOUEngine.Map.SetNormalMapBlack(ProjectFolders.MultitexturesPath + "NewLandscape/snow01_n.png");
            //DOUEngine.terrain.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_high.png");

            //DOUEngine.terrain.SetMist(DOUEngine.Mist);

            string modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            string texturePath = ProjectFolders.MultitexturesPath + "b.png";
            string normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            string specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //    new Vector3(230, 0 + DOUEngine.MAP_HEIGHT, 310), new Vector3(20, 0, 0), new Vector3(30, 30, 30))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 2.5f + DOUEngine.MAP_HEIGHT, 248), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 5f + DOUEngine.MAP_HEIGHT, 180), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //  new Vector3(230, 7.5f + DOUEngine.MAP_HEIGHT, 115), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 10 + DOUEngine.MAP_HEIGHT, 48), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
              new Vector3(170, 13f + DOUEngine.MAP_HEIGHT, 170), new Vector3(0, 180, 0), new Vector3(10, 10, 10))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(280, 10, 350), new Vector3(0, 180, 0), new Vector3(10))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //    new Vector3(230, 10, 410), new Vector3(0, 180, 0), new Vector3(10))));
            //DOUEngine.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(260, 10, 400), new Vector3(0, 180, 0), new Vector3(10))));

            // TEST components
            ComponentSerializer serializer = new ComponentSerializer();
            SerializedComponentsContainer container;
            Component parent = new Component();
            Component component;
            foreach (var item in DOUEngine.City)
            {
                // TEST
                container = serializer.DeserializeComponents("12345.cl");
                parent.ChildrenComponents = container.SerializedComponents;
                component = convertToSceneComponent(parent);
                item.SetComponents(component.ChildrenComponents);
                item.SetCollisionHeadUnit(collisionHeadUnit);

                item.SetMistComponent(DOUEngine.Mist);
            }

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "path.png";
            normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            MovableEntityArguments arg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                0.9f, new Vector3(170, 1000, 170), new Vector3(0), new Vector3(5));

            DOUEngine.Player = (Player)EngineObjectCreator.CreateInstance(arg);
            DOUEngine.Player.SetMistComponent(DOUEngine.Mist);

            // TEST components
            container = serializer.DeserializeComponents("123.cl");
            parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            component = convertToSceneComponent(parent);
            // TEST
            DOUEngine.Player.SetComponents(component.ChildrenComponents);

            DOUEngine.Player.SetCollisionHeadUnit(collisionHeadUnit);

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "b.png";

            arg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                0.3f, new Vector3(180, 20, 220), new Vector3(0, 0, 0), new Vector3(10));

            DOUEngine.Enemy = (Player)EngineObjectCreator.CreateInstance(arg);
            DOUEngine.Enemy.SetMistComponent(DOUEngine.Mist);

            container = serializer.DeserializeComponents("123.cl");
            parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            component = convertToSceneComponent(parent);
            DOUEngine.Enemy.SetComponents(component.ChildrenComponents);
            DOUEngine.Enemy.SetCollisionHeadUnit(collisionHeadUnit);
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
                    ProjectFolders.SkyboxTexturesPath + "/Night/" + "front.png" });
            DOUEngine.Skybox.setMistComponent(DOUEngine.Mist);

            //EngineSingleton.SourceAmbient = new Source(EngineSingleton.SB_ambient, 0.05f, 1, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            //EngineSingleton.SourceAmbient.SetMaxDistance(0);
            //EngineSingleton.SourceAmbient.SetLooping(true);
            //EngineSingleton.SourceAmbient.Play();

            DOUEngine.Water = new WaterPlane(ProjectFolders.WaterTexturePath + "DUDV.png", ProjectFolders.WaterTexturePath + "normal.png",
                new Vector3(160, 29, 200), new Vector3(0, 0, 0), new Vector3(200, 1, 200), new WaterQuality(true, true, true), 10);
            //DOUEngine.Water.setMist(DOUEngine.Mist);

            DOUEngine.SunReplica = new SunRenderer(DOUEngine.Sun, ProjectFolders.SunTexturePath + "sunC.png",
                    ProjectFolders.SunTexturePath + "sunB.png");

            DOUEngine.Picker = new MousePicker(DOUEngine.ProjectionMatrix, DOUEngine.Camera);

            //EngineSingleton.EnvObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(true), TextureSet.PlayerTextureSet2,
            //    TextureSet.SkyboxDayCubemapTexture, new Vector3(180, 0, 220), new Vector3(0, 0, 0), new Vector3(10));

            DOUEngine.Lights = new Light_visualization.VisualizeLight(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png"
                , DOUEngine.PointLight);

            //DOUEngine.Lens = new LensFlareRenderer();
            DOUEngine.Ray = new GodRaysRenderer();
            //DOUEngine.PostProc = new PostprocessRenderer(PostprocessType.BLOOM);
            //DOUEngine.PostProc.BloomPass = 1;
            //DOUEngine.PostProc.BlurWidth = 18;

            setGraphicsSettings();

            //gras = new Grass(new Vector3(1, 0, 1), new Vector3(1), new Vector3(0), new Vector3(0.2f, 0.8f, 0.3f));
            //envObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(false), TextureSet.PlayerTextureSet2, TextureSet.SkyboxDayCubemapTexture,
            //    new Vector3(40, 70, 40), new Vector3(0, 0, 0), new Vector3(0.5f));

            DOUEngine.Camera.SetThirdPerson(DOUEngine.Player);
            //DOUEngine.Player.SetActionMovedDelegateListener((o, e) => DOUEngine.Camera.SetThirdPerson(o as MovableEntity));
            //DOUEngine.Camera.SetFirstPerson();

            DOUEngine.shadowList = new List<IDrawable>();
            DOUEngine.City.ForEach(new Action<Building>((house) => { DOUEngine.shadowList.Add(house); }));
            DOUEngine.shadowList.Add(DOUEngine.Player);
            DOUEngine.shadowList.Add(DOUEngine.Enemy);
            DOUEngine.shadowList.Add(DOUEngine.terrain);


            DOUEngine.uiFrameCreator = new UiFrameMaster();
            DOUEngine.uiFrameCreator.PushFrame(DOUEngine.Sun.GetShadowHandler().GetTextureHandler());
           
            //ch = new ComputeShader();
            //ch.Init();
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

        #region FormLoad & GLControlPaint events

        private void OnLoad(object sender, EventArgs e)
        {
            // Every frame capture time of draw call execution
            renderTime.Start();
        }

        private void OnRender(object sender, PaintEventArgs e)
        {
            postConstructor();
            // Maybe somehow I can remove this trick
            AdjustMouseCursor();
            renderTime.Restart();
            renderThread.ThreadExecution(this.Width, this.Height, bPostConstructor);
            bPostConstructor = false;
            DOUEngine.RenderTime = (float)renderTime.Elapsed.TotalSeconds;
            GLControl.SwapBuffers();
            GLControl.Invalidate();
        }

        #endregion


        private void defaultMatrixSettings()
        {
            // create projection matrix
            DOUEngine.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(DOUEngine.FoV), DOUEngine.SCREEN_ASPECT_RATIO,
                DOUEngine.NEAR_CLIPPING_PLANE, DOUEngine.FAR_CLIPPING_PLANE);
        }

        #region Form Move&Resize events

        private void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            defaultMatrixSettings();
            GLControl.Invalidate();

            if (renderThread != null)
            {
                renderThread.DefaultFB.CleanUp();
                renderThread.DefaultFB = new DefaultFrameBuffer(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y);
            }
        }

        private void OnMove(object sender, EventArgs e)
        {
            DOUEngine.SCREEN_POSITION_X = this.Left;
            DOUEngine.SCREEN_POSITION_Y = this.Top;
        }
        #endregion

        #region Mouse events
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DOUEngine.Camera.SwitchCamera)
            {
                DOUEngine.Camera.RotateByMouse(e.X, e.Y, GLControl.Width, GLControl.Height);
                Cursor.Hide();

                if ((DOUEngine.PrevCursorPosition.X != -1) && (DOUEngine.PrevCursorPosition.Y != -1)) // need to calculate delta of mouse position
                {
                    Int32 xDelta = e.X - DOUEngine.PrevCursorPosition.X;
                    Int32 yDelta = e.Y - DOUEngine.PrevCursorPosition.Y;
                }

                DOUEngine.PrevCursorPosition = e.Location;
            }
            else
            {
                Cursor.Show();
                Cursor.Draw(this.CreateGraphics(),
                    new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height));
            }

            GLControl.Update(); // need to update frame after invalidation to redraw changes
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        //mist.fade(this.RenderTime, 10000, FadeType.LINEARLY, 0.0f);
                        //PlantUnit plant = new PlantUnit(TerrainIntersaction.getIntersactionPoint(EngineSingleton.Map, EngineSingleton.Picker.currentRay, EngineSingleton.Camera.getPosition()), new Vector3(), new Vector3(10), 0, null);
                        //EngineSingleton.Grass.add(plant, EngineSingleton.Map);

                        break;
                    }
                case MouseButtons.Right:
                    {
                        //mist.aEngineSingleton.PostProcear(this.RenderTime, 10000, FadeType.LINEARLY, 0.016f);
                        DOUEngine.Camera.SwitchCamera = !DOUEngine.Camera.SwitchCamera;
                        break;
                    }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            //if (DOUEngine.PostProc != null)
            //{
            //    if (e.Delta > 0)
            //    {
            //        DOUEngine.PostProc.BlurWidth--;
            //        DOUEngine.PostProc.BloomThreshold -= 0.1f;
            //    }
            //    if (e.Delta < 0)
            //    {
            //        DOUEngine.PostProc.BlurWidth++;
            //        DOUEngine.PostProc.BloomThreshold += 0.1f;
            //    }
            //}
            //else
            //{
            //    if (e.Delta > 0)
            //    {
            //        DOUEngine.Camera.setThirdPersonZoom(-1);
            //    }
            //    if (e.Delta < 0)
            //    {
            //        DOUEngine.Camera.setThirdPersonZoom(1);
            //    }
            //}

            if (DOUEngine.DayCycle != null)
            {
                if (e.Delta > 0)
                {
                    DOUEngine.DayCycle.TimeFlow += 0.05f;
                }
                if (e.Delta < 0)
                {
                    DOUEngine.DayCycle.TimeFlow -= 0.05f;
                }
            }
        }
        #endregion

        #region Key events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            switch (keyData)
            {
                case Keys.Up: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.FORWARD); return true;
                case Keys.Down: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.BACK); return true;
                case Keys.Left: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.LEFT); return true;
                case Keys.Right: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.RIGHT); return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                #region In-game settings
                case Keys.Back: DOUEngine.CollisionBoxRender = !DOUEngine.CollisionBoxRender; break;
                case Keys.R:
                    {

                        break;
                    }
                case Keys.N: DOUEngine.NormalMapTrigger = !DOUEngine.NormalMapTrigger; break;
                case Keys.M:   //Меняем типы полигонов
                    {
                        DOUEngine.ShowLightSource = !DOUEngine.ShowLightSource;
                        if (DOUEngine.Mode == PrimitiveType.Triangles)
                        {
                            DOUEngine.Mode = PrimitiveType.Lines;
                        }
                        else
                        {
                            DOUEngine.Mode = PrimitiveType.Triangles;
                        }
                        break;
                    }
                case Keys.Escape: this.Close(); break;//Exit
                case Keys.Add:
                    {
                        DOUEngine.Water.WaveSpeed += 0.1f;
                        DOUEngine.Water.WaveStrength += 0.1f;
                        break;
                    }
                case Keys.Subtract:
                    {
                        DOUEngine.Water.WaveSpeed -= 0.1f;
                        DOUEngine.Water.WaveStrength -= 0.1f;
                        break;
                    }
                    #endregion
            }
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            if (args.KeyChar == 'W' || args.KeyChar == 'w')
            { DOUEngine.keyboardMask[0] = true; }
            else if (args.KeyChar == 'A' || args.KeyChar == 'a')
            { DOUEngine.keyboardMask[1] = true; }
            else if (args.KeyChar == 'S' || args.KeyChar == 's')
            { DOUEngine.keyboardMask[2] = true; }
            else if (args.KeyChar == 'D' || args.KeyChar == 'd')
            { DOUEngine.keyboardMask[3] = true; }
            else if (args.KeyChar == ' ')
            {
                DOUEngine.keyboardMask[4] = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            switch (args.KeyData)
            {
                case Keys.W: { DOUEngine.keyboardMask[0] = false; break; }
                case Keys.A: { DOUEngine.keyboardMask[1] = false; break; }
                case Keys.S: { DOUEngine.keyboardMask[2] = false; break; }
                case Keys.D: { DOUEngine.keyboardMask[3] = false; break; }
                case Keys.Space: { DOUEngine.keyboardMask[4] = false; break; }
            }
        }

        #endregion

        #region Closing events
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            cleanEverythingUp();
            Debug.Log.addToLog(String.Format("\nTime elapsed : {0}", DateTime.Now - DOUEngine.ElapsedTime));
            Environment.Exit(0);
        }
        #endregion

        #region System functions

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
            //для корректной работы камеры с учетом рамки
            //+ 8 из - за того, что при открытии на полный экран, смещение стартовой позиции окна = -8
            DOUEngine.SCREEN_POSITION_X = ((DOUEngine.WINDOW_BORDER != WindowBorder.Hidden) && (DOUEngine.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                ? this.Location.X + 8 : this.Location.X;
            DOUEngine.SCREEN_POSITION_Y = ((DOUEngine.WINDOW_BORDER != WindowBorder.Hidden) && (DOUEngine.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                ? this.Location.Y + 8 : this.Location.Y;
        }

       
        #endregion

        #region Cleaning
      
        private void cleanEverythingUp()
        {
            if (DOUEngine.Water != null) DOUEngine.Water.cleanUp();
            if (DOUEngine.SunReplica != null) DOUEngine.SunReplica.cleanUp();
            if (DOUEngine.terrain != null) DOUEngine.terrain.cleanUp();

            if (DOUEngine.Player != null) DOUEngine.Player.cleanUp();
            if (DOUEngine.Enemy != null) DOUEngine.Enemy.cleanUp();
            if (DOUEngine.Grass != null) DOUEngine.Grass.cleanUp();
            if (DOUEngine.Plant1 != null) DOUEngine.Plant1.cleanUp();


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
