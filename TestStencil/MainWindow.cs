using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VMath;
using TextureLoader;

namespace TestStencil
{
    public class MainWindow : GameWindow
    {
        Actor actor;
        Actor stencilActor;
        ScreenQuadActor screenQuad;

        Matrix4 projectionMatrix, viewMatrix;

        Vector3 eye, target, up;

        private Int32 framebufferHandle;
        private Int32 renderbufferHandle;
        private Texture2D framebufferTexture;

        private void CreateFramebufferTexture()
        {
            framebufferTexture = new Texture2D();
            framebufferTexture.genEmptyImg(1, this.Width, this.Height, (Int32)All.Nearest, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
        }

        private void CreateFramebuffer()
        {
            CreateFramebufferTexture();
            framebufferHandle = GL.GenFramebuffer();
            renderbufferHandle = GL.GenRenderbuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);            
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, framebufferTexture.TextureID[0], 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbufferHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, framebufferTexture.Rezolution[0].widthRezolution, framebufferTexture.Rezolution[0].heightRezolution);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbufferHandle);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void StencilPass()
        {
            GL.Enable(EnableCap.StencilTest); // Enable stencil test
            GL.DepthMask(false); // Disable write depth
            GL.ColorMask(false, false, false, false); // Disable write color
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF); // Write 1 to stencil
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace); // Replace when depth test pass
            GL.StencilMask(0xFF); // Write to stencil
            GL.Clear(ClearBufferMask.StencilBufferBit); // Clear stencil buffer

            Vector3 stencilActorColor = new Vector3(0, 1, 0);
            stencilActor.Render(ref projectionMatrix, ref viewMatrix, ref stencilActorColor);

            GL.DepthMask(true); // Enable write depth
            GL.ColorMask(true, true, true, true); // Enable write color
            GL.StencilFunc(StencilFunction.Equal, 1, 0xFF); // Set stencil function
            GL.StencilMask(0x00);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep); // Set stencil operation
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, framebufferTexture.Rezolution[0].widthRezolution, framebufferTexture.Rezolution[0].heightRezolution);
           
            viewMatrix = Matrix4.LookAt(eye, target, up);

            StencilPass();

            Vector3 actorColor = new Vector3(1.0f, 0, 1.0f);
            actor.Render(ref projectionMatrix, ref viewMatrix, ref actorColor);

            GL.Disable(EnableCap.StencilTest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(0, 0, 0, 0);
            GL.Viewport(0, 0, this.Width, this.Height);

            screenQuad.Render((Int32)framebufferTexture.TextureID[0]);
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
            {
                float rotAngleY = MathHelper.DegreesToRadians(e.XDelta);
                float rotAngleXZ = MathHelper.DegreesToRadians(e.YDelta);

                Vector3 lookDir = (target - eye).Normalized();
                Vector3 binormal = Vector3.Cross(lookDir, up);
                binormal.Normalize();

                Matrix4 rotationAxisXZ = Matrix4.CreateFromAxisAngle(binormal, rotAngleXZ);
                Matrix4 rotationAxisY = new Matrix4(
                 new Vector4((float)Math.Cos(rotAngleY), 0, -(float)Math.Sin(rotAngleY), 0),
                 new Vector4(0, 1, 0, 0),
                 new Vector4((float)Math.Sin(rotAngleY), 0, (float)Math.Cos(rotAngleY), 0),
                 new Vector4(0, 0, 0, 1));

                Matrix4 rotationMatrix = Matrix4.Identity;
                rotationMatrix *= rotationAxisY;
                rotationMatrix *= rotationAxisXZ;

                eye = new Vector3(VectorMath.multMatrix(rotationMatrix, new Vector4(eye, 1.0f)));
            }
        }

        public MainWindow() : base(1000, 800, new GraphicsMode(32, 24, 8), "Stencil")
        {
            actor = new Actor(@"C:\Users\dzinovev\Desktop\Универчик\project\MassiveGame\MassiveGame\MassiveGame\TestStencil\Models\playerCube.obj");
            stencilActor = new Actor(@"C:\Users\dzinovev\Desktop\Универчик\project\MassiveGame\MassiveGame\MassiveGame\TestStencil\Models\dragon.obj");
            actor.Scale = new Vector3(3);
            stencilActor.Scale = new Vector3(0.4f);
            stencilActor.Translation = new Vector3(0, 0, 5);
            screenQuad = new ScreenQuadActor();

            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75), 16f / 9f, 1, 100);

            eye = new Vector3(0, 0, 20);
            target = new Vector3();
            up = new Vector3(0, 1, 0);

            CreateFramebuffer();
        }
    }
}
