using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GpuGraphics;
using Grid;
using PhysicsBox;
using Programmable_PipelineLight;
using ShaderPattern;
using TextureLoader;
using AudioEngine;

using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Timer = System.Windows.Forms.Timer;


namespace MassiveGame
{
    #region PostprocessSettings

    /*Render settings*/
    public enum PostprocessSettings
    {
        POSTPROCESS_AND_LENS_DISABLED = 0,
        POSTPROCESS_DISABLED_LENS_ENABLED = 1,
        POSTPROCESS_ENABLED_LENS_DISABLED = 2,
        POSTPROCESS_AND_LENS_ENABLED = 3
    }

    #endregion

    internal class Renderer : GameWindow
    {
        AddModelForm addModelForm = new AddModelForm();

        #region Variables
        /*  Модель примитива */
        private PrimitiveType mode = PrimitiveType.Triangles;

        private const float NEAR_CLIPPING_PLANE = 1;
        private const float FAR_CLIPPING_PLANE = 1000;
        private const float FoV = 60.0f;
        private const float MAP_SIZE = 500;
        private const int DAY_TIMER_HASTE = 20;
        private const float SCREEN_ASPECT_RATIO = 16f / 9f;

        public static int SCREEN_POSITION_X = 120;
        public static int SCREEN_POSITION_Y = 50;
        public static int SCREEN_WIDTH = 1366;
        public static int SCREEN_HEIGHT = 768;
        public static int TIMER_CALLBACK = 0;
        public static WindowBorder WINDOW_BORDER = WindowBorder.Resizable;
        public static WindowState WINDOW_STATE = WindowState.Normal;
        public static int MAX_LIGHT_COUNT = 5;  //Максимальное количество источников света присутствующих на сцене

             // ///*Temp triggers*/// //
        private bool showLightSource = false;       //temp for rendering light sources
        private bool collisionBoxRender = false;    //temp for rendering collision boxes
        private bool normalMapTrigger = true;   //temp for normal mapping
        private bool borderTrigger = true;     //temp for border setup
        private bool stateTrigger = true;     //temp for window state setup
        private bool firstPersonCameraTrigger = false;     //temp for camera setup
        private bool heightBiasTrigger = false;
        private bool altTrigger = false;
        private bool controlLeftTrigger = false;
        private bool redrawScene = false;
        private bool cameraRouteFWEnabled = false;
        private bool cameraRouteBWEnabled = false;
        // ///*Temp triggers*/// //

        private float ElapsedTime;
        private Matrix4 projectionMatrix;
        private bool _postConstructor;
        private bool _postConstructorExecuted = false;
        private Timer DayCycleTimer { set; get; }
        private PointLight[] pointLight;
        private DirectionalLight sun;
        private Collision collision;
        private Terrain map;
        private Player player;
        private Player enemy;

        private List<Building> buildings = new List<Building>();
        private Camera camera;
        //private SimpleObjects flag;
        private PlantMaster grass;
        private PlantMaster plant1;
        private Skybox skybox;
        private SimpleShader shader;    //temp
        //private MousePicker picker;
        private Mirror[] mirror;
        private PostprocessRenderer pp;
        private WaterRenderer water;
        private SunRenderer sunReplica;
        private Lens_Flare.LensFlareRenderer lens;

        private PostprocessSettings _settings;

        private int[] SB_step;
        private int[] SB_collide;
        private int SB_ambient;
        private Source sourceAmbient;
        #endregion

        #region Constructors
        internal Renderer(int width, int height, GraphicsMode mode, string title)
            : base(width, height, mode, title)            //Constructor
        {
            preConstructor();
        }
        private void preConstructor()//Start initialize values
        {
            // need to delete NewMesh.msh if it exists
            if (File.Exists("NewModel.msh"))
                File.Delete("NewModel.msh");

            _postConstructor = true;
            ElapsedTime = 0.00000f;
            DayCycleTimer = new Timer();
            DayCycleTimer.Interval = 20;
            DayCycleTimer.Tick += DayCycleTimer_Tick;

            camera = new Camera(150.0f, 100, 150.0f, 15.0f + 30.0f, 100.0f, 160.0f, 0.0f, 1.0f, 0.0f);

            projectionMatrix = Matrix4.Identity;
            matrixSettings(this.Width, this.Height, FoV);

            collision = new Collision();
            Collision.CollisionBoxDetected += new CollisionBoxDelegate(Collision_CollisionBoxDetected);
            Collision.CollisionBoxMissed += new CollisionWallDelegate(Collision_CollisionBoxMissed);
            Collision.CollisionWallDetected += new CollisionWallDelegate(Collision_CollisionWallDetected);

            sun = new DirectionalLight(new Vector3(-100, -10, 50), new Vector4(0.4f, 0.4f, 0.4f, 1),
                new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1, 1, 1, 1));
            sun.prepareDayCycle(0.6f * (0.01f * DAY_TIMER_HASTE), new Vector3(MAP_SIZE / 2, 0, MAP_SIZE / 2), 25.0f,
                FAR_CLIPPING_PLANE - ((33 * FAR_CLIPPING_PLANE) / 100), new DayPhases(0, 15.3f, new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(.7f)),
                 new DayPhases(15.4f, 34.0f, new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.9f, 0.79f, 0.79f), new Vector3(1.0f)),
                 new DayPhases(34.1f, 44.3f, new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.7f, 0.30f, 0.30f), new Vector3(0.9f)),
                 new DayPhases(60.34f, 70f, new Vector3(0.09f, 0.09f, 0.09f), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.0f)));

            pointLight = new PointLight[5];
            pointLight[0] = new PointLight(new Vector4(275, 112, 233.8f, 1), new Vector4(0.09f, 0.09f, 0.09f, 1.0f),
                new Vector4(2.0f, 2.0f, 2.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.1f, 0.001f));
            pointLight[1] = new PointLight(new Vector4(310, 112, 233.8f, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
                new Vector4(2.0f, 2.0f, 2.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.1f, 0.001f));
            pointLight[2] = new PointLight(new Vector4(345, 112, 233.8f, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
                new Vector4(2.0f, 2.0f, 2.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.1f, 0.001f));
            pointLight[3] = new PointLight(new Vector4(260, 106, 233.8f, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
                new Vector4(2.0f, 2.0f, 2.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.1f, 0.001f));
            pointLight[4] = new PointLight(new Vector4(365, 106, 233.8f, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
                new Vector4(2.0f, 2.0f, 2.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.1f, 0.001f));
        }
        private void postConstructor()
        {
            if (_postConstructor)
            {
                #region Audio initialization and loading

                AudioMaster.Init(ALDistanceModel.LinearDistanceClamped);
                AudioMaster.SetListenerData(
                    camera.getPosition().X,
                    camera.getPosition().Y,
                    camera.getPosition().Z
                );
                SB_step = AudioMaster.LoadSound(new string[] {
                    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass1.wav",
                    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass2.wav",
                    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass3.wav",
                    ProjectFolders.AudioFolders.AudioActorStepPath + "wav/grass4.wav"
                });
                SB_collide = AudioMaster.LoadSound(new string[] {
                    ProjectFolders.AudioFolders.AudioActorCollidePath + "wav/clothes1.wav",
                    ProjectFolders.AudioFolders.AudioActorCollidePath + "wav/clothes2.wav",
                    ProjectFolders.AudioFolders.AudioActorCollidePath + "wav/clothes3.wav"
                });
                SB_ambient = AudioMaster.LoadSound(ProjectFolders.AudioFolders.AudioAmbientPath + "wav/howling_wind_u1.wav");

                #endregion

                TextureSet.LOAD_TEXTURE_SETS(); //Генерируем все текстурные юниты
                map = new Terrain(MAP_SIZE, 60.0f, 3,  // Генерация ландшафта
                   ProjectFolders.TextureFolders.HeightMapsTexturesPath + "heightmap2.png", TextureSet.TerrainTextureSet);

                player = new Player(PlayerModels.getPlayerModel1(true), TextureSet.PlayerTextureSet, 0.3f, 1, new Vector3(200, 0, 230),
                    new Vector3(0, 0, 0), new Vector3(0, 0, 0), SB_step, SB_collide);
                enemy = new Player(PlayerModels.getPlayerModel1(true), TextureSet.PlayerTextureSet2, 0.3f, 2, new Vector3(180, 0, 220),
                    new Vector3(0, 0, 0), new Vector3(10), SB_step, SB_collide);

                grass = new PlantMaster(2000, MAP_SIZE, PlantModels.getBillboardModel1(),new Vector3(1),
                    TextureSet.PlantTextureSet, new WindComponent(0.35f, 0.3f, new Vector3(0.6f, 0, 0.3f)));    //Добавление травы
                plant1 = new PlantMaster(13, 100, PlantModels.getPlantModel2(), new Vector3(1), TextureSet.FernTextureSet,
                  new WindComponent(2.5f, 0.5f, new Vector3(0.5f, 0, 0.5f)));
                skybox = new Skybox(TextureSet.SkyboxDayCubemapTexture, TextureSet.SkyboxNightCubemapTexture);       //Скайбокс
                shader = new SimpleShader(ProjectFolders.ShadersPath + "simpleVShader.glsl",
                 ProjectFolders.ShadersPath + "simpleFShader.glsl");
                water = new WaterRenderer(new Vector3(160, 55, 254), new Vector3(0, 0, 0), new Vector3(70, 1, 100),
                    TextureSet.WaterTextureSet, new WaterQuality(true, true, true));

                sunReplica = new SunRenderer(TextureSet.SunTextureSet);

                //picker = new MousePicker(projectionMatrix, camera, this.Mouse);

                //mirror = new Mirror[2];
                //mirror[0] = new Mirror(null, new Vector3(180, 25, 150), new Vector3(0, 45, 0), new Vector3(0.25f));
                //mirror[1] = new Mirror(null, new Vector3(180, 25, 180), new Vector3(0, 90, 0), new Vector3(0.25f));


                lens = new Lens_Flare.LensFlareRenderer(TextureSet.LensFlareTextureSet, TextureSet.LensFlareTextureSet2);

                //pp = new PostprocessRenderer(PostprocessType.DOF_BLUR);
                //pp.BloomPass = 40;
                //pp.BlurWidth = 5;

                /*TO DO - postprocess settings assigning*/
                if (pp == null && lens == null) this._settings = PostprocessSettings.POSTPROCESS_AND_LENS_DISABLED;
                if (pp != null && lens == null) this._settings = PostprocessSettings.POSTPROCESS_ENABLED_LENS_DISABLED;
                if (pp == null && lens != null) this._settings = PostprocessSettings.POSTPROCESS_DISABLED_LENS_ENABLED;
                if (pp != null && lens != null) this._settings = PostprocessSettings.POSTPROCESS_AND_LENS_ENABLED;
                /*TO DO - postprocess settings assigning*/

                sourceAmbient = new Source(SB_ambient, 0.05f, 1, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                sourceAmbient.SetMaxDistance(0);
                sourceAmbient.SetLooping(true);
                sourceAmbient.Play();

                DayCycleTimer.Start();  //Start day cycle

                _postConstructorExecuted = true;
            }
        }
        #endregion

        #region Collision
        private void gameLogics(bool redraw = false)
        {
            if (_postConstructor)
            {
                if (redraw)
                {
                    foreach (Building building in buildings)
                    {
                        collision.polygonList.Add(building.getCollisionTriangles());
                    }
                }

                _postConstructor = !_postConstructor;
            }

            collision.boxList.Add(player.Box);
            collision.boxList.Add(enemy.Box);
            collision.detectCollision();
        }

        private void Collision_CollisionBoxDetected(int ID1, int ID2)
        {
            if ((ID1 == 1 && ID2 == 2) || (ID1 == 2 && ID2 == 1))
            {
                player.popPositionStack();
                enemy.popPositionStack();
            }
        }
        private void Collision_CollisionBoxMissed(int ID)
        {
            switch (ID)
            {
                case 1:
                    {
                        player.pushPositionStack();
                        break;
                    }
                case 2:
                    {
                        enemy.pushPositionStack();
                        break;
                    }
            }
        }
        private void Collision_CollisionWallDetected(int ID)
        {
            switch (ID)
            {
                case 1:
                    {
                        player.popPositionStack();
                        break;
                    }
                case 2:
                    {
                        enemy.popPositionStack();
                        break;
                    }
            }

        }
        #endregion

        #region Render
        private void renderLamps()
        {
            /*TO DO :
             * If sun exists - show sun ray
             * If point lights exist - show them */
            Matrix4 modelMatrix = Matrix4.Identity;
            shader.startProgram();
            shader.setUniformValues(modelMatrix, camera.getViewMatrix(), projectionMatrix);
            if (showLightSource)
            {
                if (sun != null)
                {
                    GL.LineWidth(15.0f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(sun.Destination.X, sun.Destination.Y, sun.Destination.Z);
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(sun.Position.X, sun.Position.Y, sun.Position.Z);
                    GL.End();
                }
                if (pointLight != null)
                {
                    foreach (PointLight light in pointLight)
                    {
                        modelMatrix = Matrix4.Identity;
                        modelMatrix *= Matrix4.CreateTranslation(new Vector3(light.Position.X, light.Position.Y, light.Position.Z));
                        shader.setUniformValues(modelMatrix, camera.getViewMatrix(), projectionMatrix);
                        Tao.OpenGl.Glu.GLUquadric lamps = Tao.OpenGl.Glu.gluNewQuadric();
                        GL.Color3(1.0f, 1.0f, 1.0f);
                        Tao.OpenGl.Glu.gluSphere(lamps, 1, 25, 25);
                    }
                }
            }
            GL.LineWidth(1.0f);
            shader.stopProgram();
        }
        private void renderCollisionBoxes()
        {
            /*TO DO :
             * Render collision boxes for debugging  */
            if (collisionBoxRender) //Render boxes for debugging
            {
                player.Box.renderBox(camera.getViewMatrix(), projectionMatrix);
                enemy.Box.renderBox(camera.getViewMatrix(), projectionMatrix);
                collision.renderWallBoxes(camera.getViewMatrix(), projectionMatrix);
            }
        }

        private void renderDefaultScene(Camera camera, bool redraw = false)
        {
            /*TO DO :
             * Culling back facies of skybox (cause we don't see them)
             * Culling back facies of terrain
             * Culling back facies of sun
             * Clearing depthbuffer, cause skybox is infinite   */
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (skybox != null) skybox.renderSkybox(camera, sun, projectionMatrix, Convert.ToSingle(this.RenderTime));

            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (map != null) map.renderTerrain(mode, sun, pointLight, camera, projectionMatrix);
            if (sunReplica != null) sunReplica.renderSun(camera, projectionMatrix, sun);
            GL.Disable(EnableCap.CullFace);

            if (grass != null) grass.renderEntities(sun, camera, projectionMatrix, map);
            if (plant1 != null) plant1.renderEntities(sun, camera, projectionMatrix, map);

            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            GL.Disable(EnableCap.CullFace);

            if (player != null) player.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix);
            if (enemy != null) enemy.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix);

            if (redraw)
            {
                foreach (Building building in buildings)
                {
                    building.renderObject(mode, false, sun, pointLight, camera, projectionMatrix);
                }
            }
        }

        /*Render settings for water reflections*/
        private void renderWaterReflection(Camera camera, Vector4 clipPlane, WaterQuality quality, bool redraw = false)
        {
            /*TO DO :
             * Culling back facies of terrain
             * Culling back facies of skybox
             * Clearing depthbuffer, cause skybox is infinite     */
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            if (skybox != null) skybox.renderSkybox(camera, sun, projectionMatrix, Convert.ToSingle(this.RenderTime));
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (map != null) map.renderTerrain(mode, sun, pointLight, camera, projectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);

            /*TO DO : true - enable building reflections
             false - disable building reflections*/
            if (quality.EnableBuilding)
            {
                GL.CullFace(CullFaceMode.Back);
                GL.Enable(EnableCap.CullFace);

                // add code of adding models here
                if (redraw)
                {
                    foreach (Building building in buildings)
                    {
                        building.renderObject(mode, false, sun, pointLight, camera, projectionMatrix);
                    }
                }

                GL.Disable(EnableCap.CullFace);
            }

            /*TO DO : true - enable player and enemy reflections
            false - disable player and enemy reflections*/
            if (quality.EnablePlayer)
            {
                if (player != null) player.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix, clipPlane);
                if (enemy != null) enemy.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix, clipPlane);
            }

            if (sunReplica != null) sunReplica.renderSun(camera, projectionMatrix, sun);
        }

        /*Render settings for water refractions*/
        private void renderWaterRefraction(Camera camera, Vector4 clipPlane, WaterQuality quality, bool redraw = false)
        {
            /*TO DO : true - enable grass refractions
            false - disable grass refractions*/
            if (quality.EnableGrassRefraction)
            {
                if (grass != null) grass.renderEntities(sun, camera, projectionMatrix, map, clipPlane);
                if (plant1 != null) plant1.renderEntities(sun, camera, projectionMatrix, map, clipPlane);
            }

            /*TO DO : true - enable building refractions
             false - disable building refractions*/
            if (quality.EnableBuilding)
            {
                GL.CullFace(CullFaceMode.Back);
                GL.Enable(EnableCap.CullFace);

                // add code of adding models here
                if (redraw)
                {
                    foreach (Building building in buildings)
                    {
                        building.renderObject(mode, false, sun, pointLight, camera, projectionMatrix);
                    }
                }

                GL.Disable(EnableCap.CullFace);
            }

            /*TO DO : true - enable player and enemy refractions
            false - disable player and enemy refractions*/
            if (quality.EnablePlayer)
            {
                if (player != null) player.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix, clipPlane);
                if (enemy != null) enemy.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix, clipPlane);
            }

            /*TO DO :
             * Culling back facies of terrain, cause they don't refract in water*/
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            if (map != null) map.renderTerrain(mode, sun, pointLight, camera, projectionMatrix, clipPlane);
            GL.Disable(EnableCap.CullFace);
        }

        private void renderToMultipassObjects(bool redraw = false)
        {
            if (water != null)
            {
                /*TO DO :
                 * Two passes : 
                 * 1 - to reflection FBO
                 * 2 - to refraction FBO    */
                //Reflection framebuffer
                water.renderReflectionSceneToWater();
                camera.invertPitch();
                float distance = 2 * (camera.getPosition().Y - water.WaterHeight);
                camera.setPosition(new Vector3(camera.getPosition().X, camera.getPosition().Y - distance, camera.getPosition().Z));
                renderWaterReflection(camera, new Vector4(0, 1, 0, -water.WaterHeight + 1f), water.Quality, redraw);
                camera.setPosition(new Vector3(camera.getPosition().X, camera.getPosition().Y + distance, camera.getPosition().Z));
                camera.invertPitch();

                //Refraction framebuffer
                water.renderRefractionSceneToWater();
                renderWaterRefraction(camera, new Vector4(0, -1, 0, water.WaterHeight + 1f), water.Quality, redraw);
            }

            if (mirror != null)
            {
                foreach (Mirror obj in mirror)
                {
                    Camera mirrorCamera = obj.renderSceneToMirror(camera);
                    renderDefaultScene(mirrorCamera);
                }
            }
        } //Render to object's framebuffers

        private void renderMultipassObjects()
        {

            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            if (water != null) water.renderWater(camera, projectionMatrix, Convert.ToSingle(this.RenderTime),
                NEAR_CLIPPING_PLANE, FAR_CLIPPING_PLANE, sun, pointLight);

            /*True - mirror exists
             False - doesn't*/
            if (mirror != null)
            {
                foreach (Mirror obj in mirror)
                {
                    obj.renderMirror(camera, projectionMatrix, this.Width, this.Height, pointLight, sun);
                }
            }

            GL.Disable(EnableCap.CullFace);

        }   //Render object's framebuffers

        private void renderToLensFlareScene(Camera camera, bool redraw = false)
        {
            /*TO DO :
             * Culling back facies of all objects on scene
             * and enabling color masking */
            GL.ColorMask(false, false, false, true);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            if (map != null) map.renderTerrain(mode, sun, pointLight, camera, projectionMatrix);

            if (player != null) player.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix);
            if (enemy != null) enemy.renderObject(mode, normalMapTrigger, sun, pointLight, map, camera, projectionMatrix);

            // add code of adding models here
            if (redraw)
            {
                foreach (Building building in buildings)
                {
                    building.renderObject(mode, false, sun, pointLight, camera, projectionMatrix);
                }
            }

            /*Disable color masking*/
            GL.ColorMask(true, true, true, true);
            if (sunReplica != null)
                sunReplica.renderSun(camera, projectionMatrix, sun, new Vector3(sunReplica.getSunMultiplyValue()));

            /*Stop culling*/
            GL.Disable(EnableCap.CullFace);
        }

        private bool blurEnable = false;    //temp

        private void renderGraphics(bool redraw = false)
        {
            matrixSettings(SCREEN_WIDTH, SCREEN_HEIGHT, FoV); //Set projection matrix
            //camera.setThirdPersonCamera(player, map);
            //camera.cameraAttachedToObject(player);
            //picker.update();

            //Render scene to objects with framebuffers
            renderToMultipassObjects(redraw);

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
            switch (this._settings)
            {
                case PostprocessSettings.POSTPROCESS_AND_LENS_DISABLED:
                    {
                        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                        GL.Viewport(0, 0, this.Width, this.Height);

                        renderDefaultScene(camera, redraw);
                        renderMultipassObjects();

                        renderLamps();
                        renderCollisionBoxes();
                        break;
                    }
                case PostprocessSettings.POSTPROCESS_AND_LENS_ENABLED:
                    {
                        /*Render to postprocess framebuffer*/
                        pp.beginPostProcessing();

                        renderDefaultScene(camera, redraw);
                        renderMultipassObjects();

                        renderLamps();
                        renderCollisionBoxes();
                        pp.sendPostProcessingToLensFlare(this.Width, this.Height, blurEnable);
                        blurEnable = false; //disable motion blur

                        /*Render to lens flare framebuffer*/
                        lens.beginLensFlareSpecialScene();
                        renderToLensFlareScene(camera, redraw);
                        /*Pass result to screen*/
                        lens.endLensFlareWithPostprocess(camera, this.Width, this.Height, pp.PostprocessFilterResult);
                        break;
                    }
                case PostprocessSettings.POSTPROCESS_DISABLED_LENS_ENABLED:
                    {
                        lens.beginLensFlareDefaultScene();

                        renderDefaultScene(camera, redraw);
                        renderMultipassObjects();
                        renderLamps();
                        renderCollisionBoxes();

                        //Render scene with color masking
                        lens.beginLensFlareSpecialScene();
                        renderToLensFlareScene(camera, redraw);
                        /*Pass result to screen*/
                        lens.endLensFlareWithoutPostprocess(camera, this.Width, this.Height);
                        break;
                    }
                case PostprocessSettings.POSTPROCESS_ENABLED_LENS_DISABLED:
                    {
                        pp.beginPostProcessing();

                        renderDefaultScene(camera, redraw);
                        renderMultipassObjects();

                        renderLamps();
                        renderCollisionBoxes();
                        /*Pass result to screen*/
                        pp.endPostProcessing(this.Width, this.Height, blurEnable);
                        blurEnable = false; //disable motion blur
                        break;
                    }
            }
        }
        #endregion

        #region System Functions
        private void matrixSettings(int width, int heigth, float fovY)
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fovY), 4 / 2.5f,
                NEAR_CLIPPING_PLANE, FAR_CLIPPING_PLANE);
        }
        private void DayCycleTimer_Tick(object sender, EventArgs e)
        {
            LightCycle.dayCycle(sun);
        }
        private WindowBorder WinBorder()
        {
            borderTrigger = !borderTrigger;
            WINDOW_BORDER = WINDOW_BORDER == OpenTK.WindowBorder.Hidden ? OpenTK.WindowBorder.Resizable
                : OpenTK.WindowBorder.Hidden;
            return WINDOW_BORDER;
        }
        private WindowState WinState()
        {
            stateTrigger = !stateTrigger;
            WINDOW_STATE = WINDOW_STATE == OpenTK.WindowState.Fullscreen ? OpenTK.WindowState.Normal
                : OpenTK.WindowState.Fullscreen;
            return WINDOW_STATE;
        }
        private void DisplayGraphics(bool redraw = false)
        {
            GL.Enable(EnableCap.DepthTest); //Включаем тест глубины
            ClearScreen();
            postConstructor();
            gameLogics(redraw);
            renderGraphics(redraw);
            this.SwapBuffers();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Log.addToLog("Logging has started at : " + DateTime.Now.ToString());
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            ElapsedTime = ElapsedTime >= float.MaxValue ? ElapsedTime = 0.0f : ElapsedTime += Convert.ToSingle(e.Time);
            DisplayGraphics(redrawScene);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            // для корректной работы камеры с учетом рамки
            // + 8 из-за того, что при открытии на полный экран, смещение стартовой позиции окна = -8
            SCREEN_POSITION_X = ((WINDOW_BORDER != WindowBorder.Hidden) && (WINDOW_STATE != WindowState.Fullscreen))
                ? this.Location.X + 8 : this.Location.X;
            SCREEN_POSITION_Y = ((WINDOW_BORDER != WindowBorder.Hidden) && (WINDOW_STATE != WindowState.Fullscreen))
                ? this.Location.Y + 8 : this.Location.Y;

            AudioMaster.SetListenerData(
                camera.getPosition().X,
                camera.getPosition().Y,
                camera.getPosition().Z
            );

            // if toolstrip is borderless - need to move to parents location and resize it to parents height
            if (!addModelForm.windowBorderTrigger)
            {
                OnMove(EventArgs.Empty);
                OnResize(EventArgs.Empty);
            }
            
            // check existence of file with model data to load
            if (_postConstructorExecuted)
            {
                if (File.Exists("NewModel.msh"))
                {
                    StreamReader sr_meshData = new StreamReader("NewModel.msh");

                    buildings.Add(new Building(
                        BuildingModels.getBuildingModel(false, sr_meshData.ReadLine()),
                        new Texture2D(sr_meshData.ReadLine().Split('|')),
                        map,
                        new Vector3(Convert.ToInt32(sr_meshData.ReadLine()),
                                    Convert.ToInt32(sr_meshData.ReadLine()),
                                    Convert.ToInt32(sr_meshData.ReadLine())),
                        new Vector3(Convert.ToInt32(sr_meshData.ReadLine()),
                                    Convert.ToInt32(sr_meshData.ReadLine()),
                                    Convert.ToInt32(sr_meshData.ReadLine())),
                        new Vector3(Convert.ToInt32(sr_meshData.ReadLine()),
                                    Convert.ToInt32(sr_meshData.ReadLine()),
                                    Convert.ToInt32(sr_meshData.ReadLine()))
                    ));

                    sr_meshData.Close();

                    File.Delete("NewModel.msh");

                    // need to redraw the scene
                    redrawScene = true;
                }
            }
            
            // camera route
            if (cameraRouteFWEnabled) // forward
            {
                camera.movePosition(0.1f, 0);
            }
            if (cameraRouteBWEnabled) // backward
            {
                camera.movePosition(-0.1f, 0);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.Width, Size.Height);
            matrixSettings(Size.Width, Size.Height, FoV);

            if (!addModelForm.windowBorderTrigger)
                addModelForm.Height = Size.Height - SystemInformation.CaptionHeight - 16;
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            if (!addModelForm.windowBorderTrigger)
            {
                addModelForm.Left = base.Location.X + 8;
                addModelForm.Top = base.Location.Y + SystemInformation.CaptionHeight + 8;
            }
        }
        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            Vector3 prevPosition = player.Position;
            float heightBias = 0;
            switch (e.Key)
            {
                #region Camera movement
                case OpenTK.Input.Key.Up: camera.moveCamera(directions.FORWARD); break;
                case OpenTK.Input.Key.Down: camera.moveCamera(directions.BACK); break;
                case OpenTK.Input.Key.Left: camera.moveCamera(directions.LEFT); break;
                case OpenTK.Input.Key.Right: camera.moveCamera(directions.RIGHT); break;

                case OpenTK.Input.Key.Number1: camera.setPosition(new Vector3(270, 107, 233.8f)); break;
                case OpenTK.Input.Key.Number2: cameraRouteFWEnabled = !cameraRouteFWEnabled; break;
                case OpenTK.Input.Key.Number3: cameraRouteBWEnabled = !cameraRouteBWEnabled; break;
                #endregion
                #region Object movement
                case OpenTK.Input.Key.W:
                    {
                        player.objectMove(directions.FORWARD, map);

                        if (heightBiasTrigger)
                            heightBias = player.Position.Y - prevPosition.Y;
                        else
                            heightBiasTrigger = !heightBiasTrigger;

                        //if (player.Position != prevPosition)
                            //camera.movePosition(new Vector3(0, heightBias, -player.Speed));
                        break;
                    }
                case OpenTK.Input.Key.S:
                    {
                        player.objectMove(directions.BACK, map);

                        if (heightBiasTrigger)
                            heightBias = player.Position.Y - prevPosition.Y;
                        else
                            heightBiasTrigger = !heightBiasTrigger;

                        //if (player.Position != prevPosition)
                            //camera.movePosition(new Vector3(0, heightBias, player.Speed));
                        break;
                    }
                case OpenTK.Input.Key.A:
                    {
                        player.objectMove(directions.LEFT, map);

                        if (heightBiasTrigger)
                            heightBias = player.Position.Y - prevPosition.Y;
                        else
                            heightBiasTrigger = !heightBiasTrigger;

                        //if (player.Position != prevPosition)
                            //camera.movePosition(new Vector3(-player.Speed, heightBias, 0));
                        break;
                    }
                case OpenTK.Input.Key.D:
                    {
                        player.objectMove(directions.RIGHT, map);

                        if (heightBiasTrigger)
                            heightBias = player.Position.Y - prevPosition.Y;
                        else
                            heightBiasTrigger = !heightBiasTrigger;

                        //if (player.Position != prevPosition)
                            //camera.movePosition(new Vector3(player.Speed, heightBias, 0));
                        break;
                    }
                #endregion
                #region In-game settings
                case OpenTK.Input.Key.BackSpace: collisionBoxRender = !collisionBoxRender; break;
                case OpenTK.Input.Key.AltLeft: altTrigger = true; break;
                case OpenTK.Input.Key.AltRight: altTrigger = true; break;
                case OpenTK.Input.Key.ControlLeft: controlLeftTrigger = true; break;
                case OpenTK.Input.Key.Enter:
                    {
                        if (altTrigger)
                            this.WindowState = WinState();
                        break;
                    }
                case OpenTK.Input.Key.B:
                    {
                        if (controlLeftTrigger)
                            base.WindowBorder = WinBorder();
                        break;
                    }
                case OpenTK.Input.Key.R:
                    {
                        if (controlLeftTrigger)
                        {
                            addModelForm.Left = base.Location.X + 8;
                            addModelForm.Top = base.Location.Y + SystemInformation.CaptionHeight + 8;
                            addModelForm.Height = Size.Height - SystemInformation.CaptionHeight - 16;

                            try
                            {
                                addModelForm.Show();
                            }
                            catch
                            {
                                addModelForm = new AddModelForm();
                            }
                        }
                        break;
                    }
                case OpenTK.Input.Key.N: normalMapTrigger = !normalMapTrigger; break;
                case OpenTK.Input.Key.F:
                    {
                        firstPersonCameraTrigger = !firstPersonCameraTrigger;

                        if (firstPersonCameraTrigger)
                        {
                            camera.setFirstPersonCamera(player, map);
                        }
                        else
                        {
                            camera.setThirdPersonCamera(player, map);
                            camera.movePosition(0f, 10f); // need to move in right direction
                        }

                        break;
                    }
                case OpenTK.Input.Key.M:   //Меняем типы полигонов
                    {
                        showLightSource = !showLightSource;
                        if (mode == PrimitiveType.Triangles)
                        {
                            mode = PrimitiveType.Lines;
                        }
                        else
                        {
                            mode = PrimitiveType.Triangles;
                        }
                        break;
                    }
                case OpenTK.Input.Key.Escape: this.Close(); break;//Exit
                case OpenTK.Input.Key.KeypadPlus:
                    {
                        water.WaveSpeed += 0.1f;
                        water.WaveStrength += 0.1f;
                        break;
                    }
                case OpenTK.Input.Key.KeypadMinus:
                    {
                        water.WaveSpeed -= 0.1f;
                        water.WaveStrength -= 0.1f;
                        break;
                    }
                    #endregion
            }
        }
        protected override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            switch (e.Key)
            {
                case OpenTK.Input.Key.AltLeft: altTrigger = false; break;
                case OpenTK.Input.Key.AltRight: altTrigger = false; break;
                case OpenTK.Input.Key.ControlLeft: controlLeftTrigger = false; break;
            }
        }
        protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (camera.SwitchCamera)
            {
                //camera.setRotatePosByMouse(e.Mouse.X, e.Mouse.Y, this.Width, this.Height);
                camera.setRotateViewByMouse(e.Mouse.X, e.Mouse.Y, this.Width, this.Height);
                this.CursorVisible = false;

                /*True - enable blur
                 False - do nothing*/
                if ((e.XDelta > 40 || e.YDelta > 40) || (e.XDelta < -40 || e.YDelta < -40))
                {
                    blurEnable = true;
                }
            }
            else this.CursorVisible = true;
        }
        protected override void OnMouseWheel(OpenTK.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (pp != null)
            {
                if (e.DeltaPrecise > 0)
                {
                    pp.BlurWidth--;
                    pp.BloomThreshold -= 0.1f;
                }
                if (e.DeltaPrecise < 0)
                {
                    pp.BlurWidth++;
                    pp.BloomThreshold += 0.1f;
                }
            }
            else
            {
                if (e.DeltaPrecise > 0)
                {
                    camera.setThirdPersonZoom(-1);
                }
                if (e.DeltaPrecise < 0)
                {
                    camera.setThirdPersonZoom(1);
                }
            }
        }
        protected override void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            switch (e.Button)
            {
                case OpenTK.Input.MouseButton.Left:
                    {
                        //tT = Convert.ToUInt32(new Random().Next(0, 3));
                        //plant1.addNewPlant(TerrainIntersaction.getIntersactionPoint(map, picker.currentRay, this.camera.getPosition()),
                        //    new Vector3(0), new Vector3(12), 0);
                        break;
                    }
                case OpenTK.Input.MouseButton.Right:
                    {
                        camera.SwitchCamera = !camera.SwitchCamera;
                        break;
                    }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            cleanEverythingUp();
            Log.addToLog("\nTime elapsed : " + (ElapsedTime / 60.0f) + " minutes");
            Environment.Exit(0);
        }
        #endregion

        #region Cleaners

        private void cleanEverythingUp()
        {
            if (this.water != null) this.water.cleanUp();
            if (this.sunReplica != null) this.sunReplica.cleanUp();
            if (this.map != null) this.map.cleanUp();

            if (this.player != null) this.player.cleanUp();
            if (this.enemy != null) this.enemy.cleanUp();
            if (this.grass != null) this.grass.cleanUp();
            if (this.plant1 != null) this.plant1.cleanUp();

            if (this.buildings != null)
                foreach (Building building in buildings)
                    building.cleanUp();

            if (this.skybox != null) this.skybox.cleanUp();
            if (this.shader != null) this.shader.cleanUp();

            if (this.mirror != null)
                foreach (Mirror obj in mirror)
                    obj.cleanUp();

            if (this.pp != null) this.pp.cleanUp();
            if (this.lens != null) this.lens.cleanUp();

            TextureSet.CLEAN_TEXTURE_SETS();
            //this.sourceAmbient.Delete();
            AudioMaster.CleanUp();
        }
        private void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);//Очистка буфера цвета
            GL.Clear(ClearBufferMask.DepthBufferBit);//Очистка буфера глубины
            GL.ClearColor(Color4.Black);//Очистка содержимого на экране + задание цвета бэка
        }

        #endregion
    }

    public class ThreadData
    {
        public ThreadData(string modelPath, Texture2D texture, Terrain map, 
            Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            this.modelPath = modelPath;
            this.texture = texture;
            this.map = map;
            this.translation = translation;
            this.rotation = rotation;
            this.scale = scale;
        }
        

        public Building building { private set; get; }
        private string modelPath;
        private Texture2D texture;
        private Terrain map;
        private Vector3 translation;
        private Vector3 rotation;
        private Vector3 scale;


        public void threadFunction()
        {
            building = new Building(BuildingModels.getBuildingModel(true, modelPath),
                texture, map, translation, rotation, scale);

            CleanUp();
        }

        private void CleanUp()
        {
            this.modelPath = String.Empty;
            this.texture = null;
            this.map = null;
            this.translation = Vector3.Zero;
            this.rotation = Vector3.Zero;
            this.scale = Vector3.Zero;
        }
    }

    public static class ThreadManager
    {
        public static List<Thread> Threads { get { return threads; } }
        private static List<Thread> threads;


        public static void AddToThreadList(Thread thread)
        {
            if (threads == null)
                threads = new List<Thread>();

            threads.Add(thread);
        }

        public static void AddToThreadList(Thread[] thread)
        {
            if (threads == null)
                threads = new List<Thread>();

            threads.AddRange(thread);
        }

        public static void ThreadStart()
        {
            foreach (Thread thread in Threads)
                thread.IsBackground = false;

            foreach (Thread thread in Threads)
            {
                thread.Start();
                thread.Join();
            }
        }

        public static void ThreadStart(int timeout)
        {
            foreach (Thread thread in Threads)
                thread.IsBackground = false;

            foreach (Thread thread in Threads)
            {
                thread.Start();
                thread.Join(timeout);
            }
        }

    }
}

