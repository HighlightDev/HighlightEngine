using GpuGraphics;
using OpenTK;
using OTKWinForm.RenderCore;
using PhysicsBox.ComponentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace OTKWinForm.Core
{
    public class Actor : Component
    {
        private RawModel rawModel;

        private ITexture texture;

        private BasicShader shader;
        public Matrix4 WorldMatrix;

        public void SetTexture(ITexture texture)
        {
            this.texture = texture;
        }

        private void UpdateWorldMatrix()
        {
            WorldMatrix = Matrix4.Identity;
            WorldMatrix *= Matrix4.CreateScale(ComponentScale);
            WorldMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(ComponentRotation.X));
            WorldMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ComponentRotation.Y));
            WorldMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(ComponentRotation.Z));
            WorldMatrix *= Matrix4.CreateTranslation(ComponentTranslation);
        }

        public override void Tick(float delta)
        {
            base.Tick(delta);
        }

        public void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            UpdateWorldMatrix();
            shader.startProgram();
            texture.BindTexture(OpenTK.Graphics.OpenGL.TextureUnit.Texture0);
            shader.SetDiffuseTexture(0);
            shader.SetTransformatrionMatrices(WorldMatrix, viewMatrix, projectionMatrix);
            shader.SetOpacity(1);
            VAOManager.renderBuffers(rawModel.Buffer, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            shader.stopProgram();

            foreach (var item in ChildrenComponents)
            {
                SceneComponent comp = item as SceneComponent;
                if (comp != null)
                    comp.Render(viewMatrix, projectionMatrix);
            }
        }

        public Actor(RawModel model, ITexture texture, BasicShader shader)
        {
            ComponentScale = new Vector3(1);
            ComponentTranslation = new Vector3(0);
            ComponentRotation = new Vector3(0);
            this.texture = texture;
            rawModel = model;
            this.shader = shader;
        }

        public Actor()
        {
            ComponentScale = new Vector3(1);
            ComponentTranslation = new Vector3(0);
            ComponentRotation = new Vector3(0);
        }

        public void CleanUp()
        {
            shader.cleanUp();
            texture.CleanUp();
        }
    }
}
