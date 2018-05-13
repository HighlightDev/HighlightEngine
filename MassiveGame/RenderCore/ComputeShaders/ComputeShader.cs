using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

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
        const Int32 PARTICLE_COUNT = 1280;
        const Int32 WorkGroupSize = 128;

        Int32 computeShaderHandler, vertexShaderHandler, fragmentShaderHandler;
        Int32 computeShaderProgram, renderShaderProgram;

        Int32 worldMatrix, viewMatrix, projectionMatrix;

        Int32[] buffers = new Int32[3];

        string computeShaderCode =
            @"
                #version 430 compatibility
            
                layout (std430, binding = 0) buffer Pos
                {
                    vec4 Positions[ ];
                };
                
                layout (std430, binding = 1) buffer Vel
                {
                    vec4 Velocities[ ];
                };

                layout (local_size_x = 128, local_size_y = 1, local_size_z = 1) in;
                
                #define G vec3(0.0, -9.8, 0.0)
                #define DT 0.001
                
                void main()
                {
                    uint gid = gl_GlobalInvocationID.x;

                    vec3 p = Positions[gid].xyz;
                    vec3 v = Velocities[gid].xyz;

                    vec3 pp = p + v * DT + 0.5 * DT * DT * G;
                    vec3 vp = v + G * DT;
                            
                    Positions[gid].xyz = pp;
                    Velocities[gid].xyz = vp;
                }
             ";

        string vertexShaderCode =
            @"
                #version 400
                
                layout (location = 12) in vec4 ParticlePosition;
                
                uniform mat4 worldMatrix;
                uniform mat4 viewMatrix;
                uniform mat4 projectionMatrix;

                void main()
                {
                    gl_Position = projectionMatrix * viewMatrix * worldMatrix * ParticlePosition;
                }
             ";

        string fragmentShaderCode =
            @"
                #version 400

                layout (location = 0) out vec4 FragColor;
                
                void main()
                {
                    FragColor = vec4(1, 0, 0, 1);
                }

             ";

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

        public void Init()
        {
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

        private void InitBuffers()
        {
            Int32 MaxX = 50, MaxY = 100, MinX = 0, MinY = 50, MinZ = 0, MaxZ = 50;
            float MaxVelX = 5, MaxVelY = 5, MaxVelZ = 5, MinVelX = -5f, MinVelY = -5f, MinVelZ = -5f;


            Int32 velMemorySize = System.Runtime.InteropServices.Marshal.SizeOf(new FVelocity()) * PARTICLE_COUNT;
            Int32 posMemorySize = System.Runtime.InteropServices.Marshal.SizeOf(new FParticlePosition()) * PARTICLE_COUNT;


            GL.GenBuffers(3, buffers);
            var posSSbo = buffers[0];
            var velSSbo = buffers[1];
            var colSSbo = buffers[2];

            float[,] pos = new float[PARTICLE_COUNT, 4];
            float[,] vel = new float[PARTICLE_COUNT, 4];

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, posSSbo);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(posMemorySize), new IntPtr(0), BufferUsageHint.StaticDraw);
            IntPtr ptr = GL.MapBufferRange(BufferTarget.ShaderStorageBuffer, new IntPtr(0), new IntPtr(posMemorySize), BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);

            unsafe
            {
                FParticlePosition* posmap = (FParticlePosition*)ptr.ToPointer();

                for (Int32 i = 0; i < PARTICLE_COUNT; i++)
                {
                    posmap[i].X = (float)((new Random().NextDouble() * MaxX) - MinX);
                    posmap[i].Y = (float)((new Random().NextDouble() * MaxY) - MinY);
                    posmap[i].Z = (float)((new Random().NextDouble() * MaxZ) - MinZ);
                    posmap[i].W = 1;
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
                    velocities[i].VX = (float)((new Random().NextDouble() * MaxVelX) - MinVelX);
                    velocities[i].VY = (float)((new Random().NextDouble() * MaxVelY) - MinVelY);
                    velocities[i].VZ = (float)((new Random().NextDouble() * MaxVelZ) - MinVelZ);
                    velocities[i].VW = 1;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
        }

        protected void showCompileLogInfo(string ShaderName)
        {
            int capacity = 0;
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
            int capacity = 0;
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

            GL.PointSize(5);

            GL.DrawArrays(PrimitiveType.LineStrip, 0, PARTICLE_COUNT);

            GL.UseProgram(0);

            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
