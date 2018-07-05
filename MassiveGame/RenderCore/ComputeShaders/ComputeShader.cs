using System;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace MassiveGame.RenderCore.ComputeShaders
{
    public struct FParticlePosition
    {
        public float X, Y, Z, W;
    }

    public struct FVelocity
    {
        public float VX, VY, VZ, VW;
    }

    public class ComputeShader
    {
        const Int32 PARTICLE_COUNT = 128000;
        const Int32 WorkGroupSize = 128;

        Int32 computeShaderHandler, vertexShaderHandler, fragmentShaderHandler;
        Int32 computeShaderProgram, renderShaderProgram;

        Int32 worldMatrix, viewMatrix, projectionMatrix;

        Int32[] buffers = new Int32[3];
        float[,] pos;
        float[,] vel;

        string computeShaderCode;
        string vertexShaderCode;
        string fragmentShaderCode;

        public ComputeShader()
        {
            Console.WriteLine(GL.GetString(StringName.Version));
        }

        public void GetUniformLocations()
        {
            worldMatrix = GL.GetUniformLocation(renderShaderProgram, "worldMatrix");
            viewMatrix = GL.GetUniformLocation(renderShaderProgram, "viewMatrix");
            projectionMatrix = GL.GetUniformLocation(renderShaderProgram, "projectionMatrix");
        }

        public void GetShaderCode()
        {
            string pathToShaders = ProjectFolders.ShadersPath + "/";
            using (StreamReader reader = new StreamReader(pathToShaders + "particleVS.glsl"))
            {
                vertexShaderCode = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(pathToShaders + "particleFS.glsl"))
            {
                fragmentShaderCode = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(pathToShaders + "particleCS.glsl"))
            {
                computeShaderCode = reader.ReadToEnd();
            }
        }

        public void Init()
        {
            GetShaderCode();

            // Render shader
            renderShaderProgram = GL.CreateProgram();

            vertexShaderHandler = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandler, vertexShaderCode);

            fragmentShaderHandler = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandler, fragmentShaderCode);

            GL.CompileShader(vertexShaderHandler);
            GL.CompileShader(fragmentShaderHandler);

            GL.AttachShader(renderShaderProgram, vertexShaderHandler);
            GL.AttachShader(renderShaderProgram, fragmentShaderHandler);

            GL.LinkProgram(renderShaderProgram);

            GL.DetachShader(renderShaderProgram, vertexShaderHandler);
            GL.DetachShader(renderShaderProgram, fragmentShaderHandler);

            // Compute shader

            computeShaderProgram = GL.CreateProgram();

            computeShaderHandler = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(computeShaderHandler, computeShaderCode);

            GL.CompileShader(computeShaderHandler);

            GL.AttachShader(computeShaderProgram, computeShaderHandler);

            GL.LinkProgram(computeShaderProgram);

            GL.DetachShader(computeShaderProgram, computeShaderHandler);


            // Uniforms
            GetUniformLocations();

            InitBuffers();

            showCompileLogInfo("Compute shader");
            showLinkLogInfo("Compute shader");
        }

        private float Lerp(float x1, float x2, float y1, float y2, float x)
        {
            return ((y2 - y1) / (x2 - x1)) * (x - x1) + y1;
        }

        private void InitBuffers()
        {
            GL.GenBuffers(3, buffers);
          
            pos = new float[PARTICLE_COUNT, 4];
            vel = new float[PARTICLE_COUNT, 4];

            for (Int32 i = 0; i < PARTICLE_COUNT; i++)
            {
                float theta = (float)((Lerp(0, 1, 0, MathHelper.Pi, (float)new Random(i * 1000).NextDouble())));
                float phi = (float)((Lerp(0, 1, 0, MathHelper.TwoPi, (float)new Random(i / 1000).NextDouble())));

                vel[i, 0] = (float)(Math.Sin(theta) * Math.Cos(phi));
                vel[i, 1] = (float)(Math.Cos(theta));
                vel[i, 2] = (float)(Math.Sin(theta) * Math.Sin(phi));
                vel[i, 3] = 1;
            }

            for (Int32 i = 0; i < PARTICLE_COUNT; i++)
            {
                pos[i, 0] = vel[i, 0] * (float)(Lerp(0, 1, -5, 5, (float)(new Random(i * 10000).NextDouble())));
                pos[i, 1] = vel[i, 1] * (float)(Lerp(0, 1, -5, 5, (float)(new Random(i + 10000).NextDouble())));
                pos[i, 2] = vel[i, 2] * (float)(Lerp(0, 1, -5, 5, (float)(new Random(i - 10000).NextDouble())));
                pos[i, 3] = 1;
            }

            ResetPositions();
        }

        protected void showCompileLogInfo(string ShaderName)
        {
            Int32 capacity = 0;
            /*Vertex shader log info*/
            unsafe { GL.GetShader(vertexShaderHandler, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(vertexShaderHandler, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }

            /*Fragment shader log info*/
            unsafe { GL.GetShader(fragmentShaderHandler, ShaderParameter.InfoLogLength, &capacity); }
            info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(fragmentShaderHandler, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }

          
            unsafe { GL.GetShader(computeShaderHandler, ShaderParameter.InfoLogLength, &capacity); }
            info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(computeShaderHandler, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at  :" + ShaderName + "\n" + info);
            }
    
        }

        protected void showLinkLogInfo(string ShaderName)
        {
            Int32 capacity = 0;
            /*Shader program link log info*/
            unsafe { GL.GetProgram(renderShaderProgram, GetProgramParameterName.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(renderShaderProgram, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at : render shader" + "\n" + info);
            }

            unsafe { GL.GetProgram(computeShaderProgram, GetProgramParameterName.InfoLogLength, &capacity); }
            info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(computeShaderProgram, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at : computeshader " + "\n" + info);
            }
        }

        public void Render(Matrix4 WorldMatrix, Matrix4 ViewMatrix, Matrix4 ProjectionMatrix)
        {
            var posSSbo = buffers[0];
            var velSSbo = buffers[1];

            // start compute shader 
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, posSSbo);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, velSSbo);

            GL.UseProgram(computeShaderProgram);
            GL.DispatchCompute(PARTICLE_COUNT / WorkGroupSize, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, 0);

            //// start render shader
            GL.UseProgram(renderShaderProgram);

            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, posSSbo);
            GL.EnableVertexAttribArray(12);
            GL.VertexAttribPointer(12, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.UniformMatrix4(this.worldMatrix, false, ref WorldMatrix);
            GL.UniformMatrix4(this.viewMatrix, false, ref ViewMatrix);
            GL.UniformMatrix4(this.projectionMatrix, false, ref ProjectionMatrix);

            GL.PointSize(0.5f);

            GL.DrawArrays(PrimitiveType.Points, 0, PARTICLE_COUNT);

            GL.UseProgram(0);

            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void ResetPositions()
        {
            var posSSbo = buffers[0];
            var velSSbo = buffers[1];
            var colSSbo = buffers[2];

            Int32 velMemorySize = System.Runtime.InteropServices.Marshal.SizeOf(new FVelocity()) * PARTICLE_COUNT;
            Int32 posMemorySize = System.Runtime.InteropServices.Marshal.SizeOf(new FParticlePosition()) * PARTICLE_COUNT;

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, posSSbo);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(posMemorySize), new IntPtr(0), BufferUsageHint.StaticDraw);
            IntPtr ptr = GL.MapBufferRange(BufferTarget.ShaderStorageBuffer, new IntPtr(0), new IntPtr(posMemorySize), BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);
            unsafe
            {
                FParticlePosition* posmap = (FParticlePosition*)ptr.ToPointer();

                for (Int32 i = 0; i < PARTICLE_COUNT; i++)
                {
                    posmap[i].X = pos[i, 0];
                    posmap[i].Y = pos[i, 1];
                    posmap[i].Z = pos[i, 2];
                    posmap[i].W = pos[i, 3];
                }
            }

            GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, velSSbo);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(velMemorySize), new IntPtr(0), BufferUsageHint.StaticDraw);
            ptr = GL.MapBufferRange(BufferTarget.ShaderStorageBuffer, new IntPtr(0), new IntPtr(velMemorySize), BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);

            unsafe
            {
                for (Int32 i = 0; i < PARTICLE_COUNT; i++)
                {
                    FVelocity* velocities = (FVelocity*)ptr.ToPointer();
                    velocities[i].VX = vel[i, 0];
                    velocities[i].VY = vel[i, 1];
                    velocities[i].VZ = vel[i, 2];
                    velocities[i].VW = vel[i, 3];
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
        }
    }
}
