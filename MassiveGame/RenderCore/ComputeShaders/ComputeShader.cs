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
        const Int32 PARTICLE_COUNT = 1000;
        const Int32 WorkGroupSize = 128;

        Int32 computeShaderHandler, vertexShaderHandler, fragmentShaderHandler;
        Int32 computeShaderProgram, renderShaderProgram;

        Int32 worldMatrix, viewMatrix, projectionMatrix;

        Int32[] buffers = new Int32[3];

        string computeShaderCode = 
            @"
            
            
             ";

        string vertexShaderCode =
            @"
                #version 400
                
                layout (location = 0) in vec4 ParticlePosition;
                
                uniform mat4 worldMatrix;
                uniform mat4 viewMatrix;
                uniform mat4 projectionMatrix;

                void main()
                {
                    gl_Position = ProjectionMatrix * ViewMatrix * WorldMatrix * ParticlePosition;
                }
             ";

        string fragmentShaderCode =
            @"
                #version 400

                layout (location = 0) out vec4 FragColor;
                
                void main()
                {
                    FragColor = vec4(1);
                }

             ";

        public ComputeShader()
        {

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
        }

        private void InitBuffers()
        {
            Int32 MaxX = 2, MaxY = 2, MinX = 0, MinY = 0, MinZ = 0, MaxZ = 2;
            float MaxVelX = 1, MaxVelY = 1, MaxVelZ = 1, MinVelX = 0.5f, MinVelY = 0.5f, MinVelZ = 0.5f;


            Int32 posMemorySize = sizeof(float) * 4 * PARTICLE_COUNT;

            GL.GenBuffers(3, buffers);
            var posSSbo = buffers[0];
            var velSSbo = buffers[1];
            var colSSbo = buffers[2];


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
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(posMemorySize), new IntPtr(0), BufferUsageHint.StaticDraw);
            ptr = GL.MapBufferRange(BufferTarget.ShaderStorageBuffer, new IntPtr(0), new IntPtr(posMemorySize), BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);

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

            GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
        }

        public void Render(Matrix4 WorldMatrix, Matrix4 ViewMatrix, Matrix4 ProjectionMatrix)
        {
            var posSSbo = buffers[0];
            var velSSbo = buffers[1];

            // start compute shader 
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, posSSbo);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 5, velSSbo);

            GL.UseProgram(computeShaderProgram);
            GL.DispatchCompute(PARTICLE_COUNT / WorkGroupSize, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            // start render shader
            GL.UseProgram(renderShaderProgram);

            GL.BindBuffer(BufferTarget.ArrayBuffer, posSSbo);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.UniformMatrix4(this.worldMatrix, false, ref WorldMatrix);
            GL.UniformMatrix4(this.viewMatrix, false, ref ViewMatrix);
            GL.UniformMatrix4(this.projectionMatrix, false, ref ProjectionMatrix);

            GL.DrawArrays(PrimitiveType.Points, 0, PARTICLE_COUNT);

            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
