using GpuGraphics;
using OpenTK;
using OTKWinForm.RenderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace OTKWinForm.Core
{
    public class Actor
    {
        private RawModel rawModel;

        private ITexture texture;

        private BasicShader shader;
        public Matrix4 WorldMatrix;

        public Vector3 Translation { set; get; }
        public Vector3 Rotation { set; get; }
        public Vector3 Scale { set; get; }

        private List<Component> attachedComponents;

        public List<Component> GetComponents()
        {
            return attachedComponents;
        }

        public void SetTexture(ITexture texture)
        {
            this.texture = texture;
        }

        private void UpdateWorldMatrix()
        {
            WorldMatrix = Matrix4.Identity;
            WorldMatrix *= Matrix4.CreateScale(Scale);
            WorldMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
            WorldMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
            WorldMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
            WorldMatrix *= Matrix4.CreateTranslation(Translation);
        }

        public virtual void Tick(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            foreach (var component in attachedComponents)
            {
                component.Tick(viewMatrix, projectionMatrix);
            }
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

            foreach (var item in attachedComponents)
            {
                SceneComponent comp = item as SceneComponent;
                if (comp != null)
                    comp.Render(viewMatrix, projectionMatrix);
            }
        }

        public Actor(RawModel model, ITexture texture, BasicShader shader)
        {
            Scale = new Vector3(1);
            Translation = new Vector3(0);
            Rotation = new Vector3(0);
            this.texture = texture;
            rawModel = model;
            this.shader = shader;
            attachedComponents = new List<Component>();
        }

        public void CleanUp()
        {
            shader.cleanUp();
            texture.CleanUp();
        }

        public void AttachComponent(Component component)
        {
            component.parentActor = this;
            attachedComponents.Add(component);
        }

        public void AttachComponentToChildComponent(Component component, Int32 childrenComponentIndex)
        {
            attachedComponents[childrenComponentIndex].AttachComponent(component);
            component = attachedComponents[childrenComponentIndex].parentComponent;
        }

        public void DetachComponent(Component component)
        {
            attachedComponents.Remove(component);
            component.parentActor = null;
        }

        public void DetachComponentFromChildComponent(Component component, Int32 childrenComponentIndex)
        {
            attachedComponents[childrenComponentIndex].DetachComponent(component);
        }
    }
}
