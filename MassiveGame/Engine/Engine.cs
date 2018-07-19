using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Engine;
using System.Collections.Generic;
using MassiveGame.Settings;
using MassiveGame.API.ObjectFactory.ObjectArguments;
using PhysicsBox.ComponentCore;
using MassiveGame.Debug.UiPanel;
using System.IO;
using TextureLoader;
using MassiveGame.API.Collector;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.GameCore;
using MassiveGame.API.ObjectFactory;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.RenderCore.Visibility;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.Sun.DayCycle;
using static MassiveGame.Core.GameCore.Sun.DayCycle.DayPhases;
using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.GameCore.Skybox;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.GameCore.Sun;
using MassiveGame.API.MouseObjectDetector;
using MassiveGame.Core.RenderCore.Light_visualization;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.GameCore.Terrain;
using MassiveGame.API.Collector.Policies;

namespace MassiveGame.UI
{
    public partial class Engine : Form
    {
        private Stopwatch renderTickTime;
        private RenderThread renderThread;
        private GameThread gameThread;
        private Point actualScreenRezolution;
       
        private CollisionHeadUnit collisionHeadUnit;

        private bool bPostConstructor = true;

        #region Constructors
        public Engine()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            preConstructor();
        }

        public Engine(Int32 width, Int32 height) 
            : this()
        {
            this.Width = width;
            this.Height = height;
        }

        private void LoadIniSettings()
        {
            SettingsLoader settingsLoader = new SettingsLoader();
            settingsLoader.SetGlobalSettings();
        }

        private void preConstructor() //Start initialize values
        {
            EngineStatics.Camera = new ThirdPersonCamera(new Vector3(0.5f, -0.8f, 0), 45);
            EngineStatics.PrevCursorPosition = new System.Drawing.Point(-1, -1);
            EngineStatics.ElapsedTime = DateTime.Now;
            EngineStatics.playerController = new PlayerController(EngineStatics.Camera as ThirdPersonCamera);

            LoadIniSettings();

            renderTickTime = new Stopwatch();
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                collisionHeadUnit = new CollisionHeadUnit();
                EngineStatics.ProjectionMatrix = Matrix4.Identity;
                EngineStatics.City = new List<Building>();
                // need to delete NewMesh.msh if it exists
                if (File.Exists(@"NewModel.msh"))
                    File.Delete(@"NewModel.msh");
                defaultMatrixSettings();


                setTestValues();
                KeyboardBindingsLoader bindingsLoader = new KeyboardBindingsLoader();
                bindingsLoader.SetKeyboardBindings();

                // add objects to optimization list
                EngineStatics.RenderableMeshCollection = new List<IVisible> { EngineStatics.SunReplica, EngineStatics.Water,
                    EngineStatics.Player, EngineStatics.Enemy };
                EngineStatics.RenderableMeshCollection.AddRange(EngineStatics.City);

                EngineStatics.LitByLightSourcesMeshCollection = new List<ILightHit> { EngineStatics.Player, EngineStatics.Enemy };
                EngineStatics.LitByLightSourcesMeshCollection.AddRange(EngineStatics.City);

                // Start game and render thread execution
                renderThread = new RenderThread();
                gameThread = new GameThread(100, 1);
            }
        }
        #endregion
 
        private void setTestValues()
        {
            var rtParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.DepthComponent16, EngineStatics.globalSettings.ShadowMapRezolution.X, EngineStatics.globalSettings.ShadowMapRezolution.Y, PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat);
            EngineStatics.Sun = new DirectionalLight(rtParams, new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));
            EngineStatics.Sun.GetShadow().CreateShadowMapCache();

            var dayPhases = new DayPhases(new Morning(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(.7f)),
                    new DayPhases.Day(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.9f, 0.79f, 0.79f), new Vector3(1.0f)),
                new Evening(new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.30f, 0.30f), new Vector3(0.9f)),
                new Night(new Vector3(0.09f, 0.09f, 0.09f), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.0f)));

            EngineStatics.DayCycle = new DayLightCycle(EngineStatics.Sun,
                EngineStatics.MAP_SIZE, dayPhases);
            EngineStatics.DayCycle.SetTime(25);
            EngineStatics.DayCycle.TimeFlow = 0.001f;

            EngineStatics.PointLight = new List<PointLight>();
          
            /*Create mist component*/
            EngineStatics.Mist = new MistComponent(0.003f, 1f, new Vector3(0.7f, 0.75f, 0.8f));

            // temporary

            //EngineStatics.terrain = new Landscape(EngineStatics.MAP_SIZE, EngineStatics.MAP_HEIGHT, 3,
            //       ProjectFolders.HeightMapsTexturesPath + "heightmap2.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01.jpg",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/tundra02.jpg",
            //       ProjectFolders.MultitexturesPath + "b.png",
            //       ProjectFolders.MultitexturesPath + "NewLandscape/snow01.jpg",
            //       ProjectFolders.MultitexturesPath + "blendMap.png");

            //EngineStatics.terrain.SetNormalMapR(ProjectFolders.MultitexturesPath + "NewLandscape/volcanictundrarocks01_n.png");
            //EngineStatics.terrain.SetNormalMapG(ProjectFolders.MultitexturesPath + "NewLandscape/tundra02_n.png");
            ////EngineStatics.Map.SetNormalMapBlack(ProjectFolders.MultitexturesPath + "NewLandscape/snow01_n.png");
            //EngineStatics.terrain.SetNormalMapB(ProjectFolders.NormalMapsPath + "brick_nm_high.png");
            //EngineStatics.terrain.SetMist(EngineStatics.Mist);

            string modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            string texturePath = ProjectFolders.MultitexturesPath + "b.png";
            string normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            string specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //    new Vector3(230, 0 + EngineStatics.MAP_HEIGHT, 310), new Vector3(20, 0, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 2.5f + EngineStatics.MAP_HEIGHT, 248), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 5f + EngineStatics.MAP_HEIGHT, 180), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //  new Vector3(230, 7.5f + EngineStatics.MAP_HEIGHT, 115), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(230, 10 + EngineStatics.MAP_HEIGHT, 48), new Vector3(0, 180, 0), new Vector3(30, 30, 30))));
            EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
              new Vector3(170, 13f + EngineStatics.MAP_HEIGHT, 170), new Vector3(0, 180, 0), new Vector3(10, 10, 10))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(280, 10, 350), new Vector3(0, 180, 0), new Vector3(10))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //    new Vector3(230, 10, 410), new Vector3(0, 180, 0), new Vector3(10))));
            //EngineStatics.City.Add((Building)EngineObjectCreator.CreateInstance(new StaticEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
            //   new Vector3(260, 10, 400), new Vector3(0, 180, 0), new Vector3(10))));

            // TEST components
            ComponentSerializer serializer = new ComponentSerializer();
            SerializedComponentsContainer container;
            Component parent = new Component();
            Component component;
            foreach (var item in EngineStatics.City)
            {
                // TEST
                container = serializer.DeserializeComponents("12345.cl");
                parent.ChildrenComponents = container.SerializedComponents;
                component = convertToSceneComponent(parent);
                item.SetComponents(component.ChildrenComponents);
                item.SetCollisionHeadUnit(collisionHeadUnit);

                item.SetMistComponent(EngineStatics.Mist);
            }

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "path.png";
            normalMapPath = ProjectFolders.NormalMapsPath + "brick_nm_high.png";
            specularMapPath = ProjectFolders.SpecularMapsPath + "brick_sm.png";

            MovableEntityArguments arg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                0.6f, new Vector3(170, 1200, 170), new Vector3(0), new Vector3(5));

            EngineStatics.Player = (MovableMeshEntity)EngineObjectCreator.CreateInstance(arg);
            EngineStatics.Player.SetMistComponent(EngineStatics.Mist);

            // TEST components
            container = serializer.DeserializeComponents("123.cl");
            parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            component = convertToSceneComponent(parent);
            // TEST
            EngineStatics.Player.SetComponents(component.ChildrenComponents);

            EngineStatics.Player.SetCollisionHeadUnit(collisionHeadUnit);

            modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            texturePath = ProjectFolders.MultitexturesPath + "b.png";

            arg = new MovableEntityArguments(modelPath, texturePath, normalMapPath, specularMapPath,
                0.3f, new Vector3(180, 200, 220), new Vector3(0, 0, 0), new Vector3(10));

            EngineStatics.Enemy = (MovableMeshEntity)EngineObjectCreator.CreateInstance(arg);
            EngineStatics.Enemy.SetMistComponent(EngineStatics.Mist);

            container = serializer.DeserializeComponents("123.cl");
            parent = new Component();
            parent.ChildrenComponents = container.SerializedComponents;
            component = convertToSceneComponent(parent);
            EngineStatics.Enemy.SetComponents(component.ChildrenComponents);
            EngineStatics.Enemy.SetCollisionHeadUnit(collisionHeadUnit);
            arg = null;

            var pool = TestClass.GetPool();
            pool.ToString();

            //EngineStatics.Grass = new PlantReadyMaster(
            //    4000, EngineStatics.MAP_SIZE, PlantModels.getBillboardModel1(), new Vector3(1),
            //    new string[] { ProjectFolders.GrassTexturesPath + "grass1.png",
            //        ProjectFolders.GrassTexturesPath + "grass2.png",
            //        ProjectFolders.GrassTexturesPath + "grass3.png"}, new WindComponent(2.35f, 1.1f, 0.6f, new Vector3(0.6f, 0, 0.3f)), EngineStatics.Mist);    //Добавление травы

            //EngineSingleton.Grass = new PlantBuilderMaster(100, PlantModels.getBillboardModel1(), TextureSet.PlantTextureSet, new WindComponent(2.35f, 1.1f, 0.6f, new Vector3(0.6f, 0, 0.3f)));    //Добавление травы

            //EngineStatics.Plant1 = new PlantReadyMaster(13, EngineStatics.MAP_SIZE, PlantModels.getPlantModel2(), new Vector3(1),
            //    new string[] { ProjectFolders.GrassTexturesPath + "fern.png" },
            //  new WindComponent(0.95f, 0.35f, 0.5f, new Vector3(0.5f, 0, 0.5f)), EngineStatics.Mist);

            EngineStatics.Skybox = new Skybox(
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
            EngineStatics.Skybox.setMistComponent(EngineStatics.Mist);

            EngineStatics.Water = new WaterPlane(ProjectFolders.WaterTexturePath + "DUDV.png", ProjectFolders.WaterTexturePath + "normal.png",
                new Vector3(160, 29, 200), new Vector3(0, 0, 0), new Vector3(200, 1, 200), new WaterQuality(true, true, true), 10);
            //EngineStatics.Water.setMist(EngineStatics.Mist);

            EngineStatics.SunReplica = new SunRenderer(EngineStatics.Sun, ProjectFolders.SunTexturePath + "sunC.png",
                    ProjectFolders.SunTexturePath + "sunB.png");

            EngineStatics.Picker = new MousePicker(EngineStatics.ProjectionMatrix, EngineStatics.Camera);

            //EngineSingleton.EnvObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(true), TextureSet.PlayerTextureSet2,
            //    TextureSet.SkyboxDayCubemapTexture, new Vector3(180, 0, 220), new Vector3(0, 0, 0), new Vector3(10));

            EngineStatics.pointLightDebugRenderer = new PointLightsDebugRenderer(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png"
                , EngineStatics.PointLight);

            //gras = new Grass(new Vector3(1, 0, 1), new Vector3(1), new Vector3(0), new Vector3(0.2f, 0.8f, 0.3f));
            //envObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(false), TextureSet.PlayerTextureSet2, TextureSet.SkyboxDayCubemapTexture,
            //    new Vector3(40, 70, 40), new Vector3(0, 0, 0), new Vector3(0.5f));

            if (EngineStatics.Camera as ThirdPersonCamera != null)
            {
                (EngineStatics.Camera as ThirdPersonCamera).SetThirdPersonTarget(EngineStatics.Player);
            }
            EngineStatics.Camera.SetCollisionHeadUnit(collisionHeadUnit);
            //EngineStatics.Camera.SetFirstPerson();

            EngineStatics.shadowList = new List<IDrawable>();
            EngineStatics.City.ForEach(new Action<Building>((house) => { EngineStatics.shadowList.Add(house); }));
            EngineStatics.shadowList.Add(EngineStatics.Player);
            EngineStatics.shadowList.Add(EngineStatics.Enemy);
            EngineStatics.shadowList.Add(EngineStatics.terrain);


            EngineStatics.uiFrameCreator = new UiFrameMaster();
           
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
            renderTickTime.Start();
        }

        private void OnRender(object sender, PaintEventArgs e)
        {
            postConstructor();

            // Maybe somehow I can remove this trick
            AdjustMouseCursor();

            renderTickTime.Restart();
            renderThread.ThreadExecution(ref actualScreenRezolution, bPostConstructor);
            EngineStatics.RENDER_TIME = (float)renderTickTime.Elapsed.TotalSeconds;
            GLControl.SwapBuffers();
            GLControl.Invalidate();

            bPostConstructor = false;
        }

        #endregion


        private void defaultMatrixSettings()
        {
            // create projection matrix
            EngineStatics.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(EngineStatics.FoV), EngineStatics.SCREEN_ASPECT_RATIO,
                EngineStatics.NEAR_CLIPPING_PLANE, EngineStatics.FAR_CLIPPING_PLANE);
        }

        #region Form Move&Resize events

        private void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            actualScreenRezolution = new Point(this.Width, this.Height);
            GLControl.Invalidate();
        }

        private void OnMove(object sender, EventArgs e)
        {
            EngineStatics.SCREEN_POSITION_X = this.Left;
            EngineStatics.SCREEN_POSITION_Y = this.Top;
        }
        #endregion

        #region Mouse events
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (EngineStatics.Camera.SwitchCamera)
            {
                EngineStatics.Camera.Rotate(e.X, e.Y, new Point(Width, GLControl.Height));
                Cursor.Hide();

                if ((EngineStatics.PrevCursorPosition.X != -1) && (EngineStatics.PrevCursorPosition.Y != -1)) // need to calculate delta of mouse position
                {
                    Int32 xDelta = e.X - EngineStatics.PrevCursorPosition.X;
                    Int32 yDelta = e.Y - EngineStatics.PrevCursorPosition.Y;
                }

                EngineStatics.PrevCursorPosition = e.Location;
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
                        EngineStatics.Camera.SwitchCamera = !EngineStatics.Camera.SwitchCamera;
                        break;
                    }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (EngineStatics.DayCycle != null)
            {
                if (e.Delta > 0)
                {
                    EngineStatics.DayCycle.TimeFlow += 0.01f;
                }
                else if (e.Delta < 0 && EngineStatics.DayCycle.TimeFlow > 0)
                {
                    EngineStatics.DayCycle.TimeFlow -= 0.01f;
                }
                else if (EngineStatics.DayCycle.TimeFlow < 0)
                {
                    EngineStatics.DayCycle.TimeFlow = 0.0f;
                }
            }
        }
        #endregion

        #region Key events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            FirstPersonCamera firstPersonCamera = EngineStatics.Camera as FirstPersonCamera;
            if (firstPersonCamera != null)
            {
                switch (keyData)
                {
                    case Keys.Up: firstPersonCamera.moveCamera(CameraDirections.FORWARD); return true;
                    case Keys.Down: firstPersonCamera.moveCamera(CameraDirections.BACK); return true;
                    case Keys.Left: firstPersonCamera.moveCamera(CameraDirections.LEFT); return true;
                    case Keys.Right: firstPersonCamera.moveCamera(CameraDirections.RIGHT); return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        Int32 renderTargetIndex = 0;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                #region In-game settings
                case Keys.R:
                    {

                        break;
                    }
                case Keys.N: EngineStatics.NormalMapTrigger = !EngineStatics.NormalMapTrigger; break;
                case Keys.M:   
                    {
                        if (EngineStatics.Mode == PrimitiveType.Triangles)
                        {
                            EngineStatics.Mode = PrimitiveType.Lines;
                        }
                        else
                        {
                            EngineStatics.Mode = PrimitiveType.Triangles;
                        }
                        break;
                    }
                case Keys.Escape: this.Close(); break;//Exit
                case Keys.Add:
                    {
                        EngineStatics.Water.WaveSpeed += 0.1f;
                        EngineStatics.Water.WaveStrength += 0.1f;
                        break;
                    }
                case Keys.Subtract:
                    {
                        EngineStatics.Water.WaveSpeed -= 0.1f;
                        EngineStatics.Water.WaveStrength -= 0.1f;
                        break;
                    }
                case Keys.Insert:
                    {
                        EngineStatics.uiFrameCreator.PushFrame(API.Collector.ResourcePool.GetRenderTargetAt(renderTargetIndex));
                        Int32 count = API.Collector.ResourcePool.GetRenderTargetCount();
                        if (renderTargetIndex + 1 >= count)
                        {
                            renderTargetIndex = 0;
                        }
                        else
                        {
                            ++renderTargetIndex;
                        }
                        break;
                    }
                    #endregion
            }
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            Keys key = (Keys)char.ToUpper(args.KeyChar);
            EngineStatics.playerController.GetKeyboardHandler().KeyPress(key);
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            EngineStatics.playerController.GetKeyboardHandler().KeyRelease(args.KeyData);
        }

        #endregion

        #region Closing events
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            cleanEverythingUp();
            Debug.Log.addToLog(String.Format("\nTime elapsed : {0}", DateTime.Now - EngineStatics.ElapsedTime));
            Environment.Exit(0);
        }
        #endregion

        #region System functions

        private void AdjustMouseCursor()
        {
            EngineStatics.SCREEN_POSITION_X = this.Location.X + 8;
            EngineStatics.SCREEN_POSITION_Y = this.Location.Y + 8;
            //для корректной работы камеры с учетом рамки
            //+ 8 из - за того, что при открытии на полный экран, смещение стартовой позиции окна = -8
            EngineStatics.SCREEN_POSITION_X = ((EngineStatics.WINDOW_BORDER != WindowBorder.Hidden) && (EngineStatics.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                ? this.Location.X + 8 : this.Location.X;
            EngineStatics.SCREEN_POSITION_Y = ((EngineStatics.WINDOW_BORDER != WindowBorder.Hidden) && (EngineStatics.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                ? this.Location.Y + 8 : this.Location.Y;
        }

       
        #endregion

        #region Cleaning
      
        private void cleanEverythingUp()
        {
            if (EngineStatics.Water != null) EngineStatics.Water.cleanUp();
            if (EngineStatics.SunReplica != null) EngineStatics.SunReplica.cleanUp();
            if (EngineStatics.terrain != null) EngineStatics.terrain.cleanUp();
            if (EngineStatics.Player != null) EngineStatics.Player.cleanUp();
            if (EngineStatics.Enemy != null) EngineStatics.Enemy.cleanUp();
            if (EngineStatics.Grass != null) EngineStatics.Grass.cleanUp();
            if (EngineStatics.Plant1 != null) EngineStatics.Plant1.cleanUp();
            if (EngineStatics.City != null) foreach (Building house in EngineStatics.City) { house.cleanUp(); }
            if (EngineStatics.Skybox != null) EngineStatics.Skybox.cleanUp();
        }

        #endregion
    }
}
