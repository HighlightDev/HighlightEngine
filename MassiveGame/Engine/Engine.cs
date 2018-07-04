﻿using System;
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
using MassiveGame.API.Collector;
using MassiveGame.Core;

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
            DOUEngine.Camera = new ThirdPersonCamera(new Vector3(1, 0, 0), 45);
            DOUEngine.PrevCursorPosition = new System.Drawing.Point(-1, -1);
            DOUEngine.ElapsedTime = DateTime.Now;
            DOUEngine.keyboardMask = new API.EventHandlers.KeyboardHandler();

            LoadIniSettings();

            renderTickTime = new Stopwatch();
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                collisionHeadUnit = new CollisionHeadUnit();
                DOUEngine.ProjectionMatrix = Matrix4.Identity;
                DOUEngine.City = new List<Building>();
                // need to delete NewMesh.msh if it exists
                if (File.Exists(@"NewModel.msh"))
                    File.Delete(@"NewModel.msh");
                defaultMatrixSettings();


                setTestValues();

                // add objects to optimization list
                DOUEngine.RenderableMeshCollection = new List<IVisible> { DOUEngine.SunReplica, DOUEngine.Water,
                    DOUEngine.Player, DOUEngine.Enemy };
                DOUEngine.RenderableMeshCollection.AddRange(DOUEngine.City);

                DOUEngine.LitByLightSourcesMeshCollection = new List<ILightHit> { DOUEngine.Player, DOUEngine.Enemy };
                DOUEngine.LitByLightSourcesMeshCollection.AddRange(DOUEngine.City);

                // Start game and render thread execution
                renderThread = new RenderThread();
                gameThread = new GameThread(100, 1);
            }
        }
        #endregion
 
        private void setTestValues()
        {
            var rtParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.DepthComponent16, DOUEngine.globalSettings.ShadowMapRezolution.X, DOUEngine.globalSettings.ShadowMapRezolution.Y, PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat);
            DOUEngine.Sun = new DirectionalLight(rtParams, new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));
            DOUEngine.Sun.GetShadow().CreateShadowMapCache();

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

            DOUEngine.pointLightDebugRenderer = new Light_visualization.PointLightsDebugRenderer(ProjectFolders.TexturesPath + "/LightTextures/" + "light-bulb-icon (1).png"
                , DOUEngine.PointLight);

            //gras = new Grass(new Vector3(1, 0, 1), new Vector3(1), new Vector3(0), new Vector3(0.2f, 0.8f, 0.3f));
            //envObj = new EnvironmentEntities(PlayerModels.getPlayerModel1(false), TextureSet.PlayerTextureSet2, TextureSet.SkyboxDayCubemapTexture,
            //    new Vector3(40, 70, 40), new Vector3(0, 0, 0), new Vector3(0.5f));

            if (DOUEngine.Camera as ThirdPersonCamera != null)
            {
                (DOUEngine.Camera as ThirdPersonCamera).SetThirdPersonTarget(DOUEngine.Player);
            }
            //DOUEngine.Player.SetActionMovedDelegateListener((o, e) => DOUEngine.Camera.SetThirdPerson(o as MovableEntity));
            //DOUEngine.Camera.SetFirstPerson();

            DOUEngine.shadowList = new List<IDrawable>();
            DOUEngine.City.ForEach(new Action<Building>((house) => { DOUEngine.shadowList.Add(house); }));
            DOUEngine.shadowList.Add(DOUEngine.Player);
            DOUEngine.shadowList.Add(DOUEngine.Enemy);
            DOUEngine.shadowList.Add(DOUEngine.terrain);


            DOUEngine.uiFrameCreator = new UiFrameMaster();
           
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
            DOUEngine.RENDER_TIME = (float)renderTickTime.Elapsed.TotalSeconds;
            GLControl.SwapBuffers();
            GLControl.Invalidate();

            bPostConstructor = false;
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
            actualScreenRezolution = new Point(this.Width, this.Height);
            GLControl.Invalidate();
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
                DOUEngine.Camera.Rotate(e.X, e.Y, new Point(Width, GLControl.Height));
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
            if (DOUEngine.DayCycle != null)
            {
                if (e.Delta > 0)
                {
                    DOUEngine.DayCycle.TimeFlow += 0.01f;
                }
                else if (e.Delta < 0 && DOUEngine.DayCycle.TimeFlow > 0)
                {
                    DOUEngine.DayCycle.TimeFlow -= 0.01f;
                }
                else if (DOUEngine.DayCycle.TimeFlow < 0)
                {
                    DOUEngine.DayCycle.TimeFlow = 0.0f;
                }
            }
        }
        #endregion

        #region Key events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            FirstPersonCamera firstPersonCamera = DOUEngine.Camera as FirstPersonCamera;
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
                case Keys.N: DOUEngine.NormalMapTrigger = !DOUEngine.NormalMapTrigger; break;
                case Keys.M:   
                    {
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
                case Keys.Insert:
                    {
                        DOUEngine.uiFrameCreator.PushFrame(ResourcePool.GetRenderTargetAt(renderTargetIndex));
                        Int32 count = ResourcePool.GetRenderTargetCount();
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
        }

        #endregion
    }
}
