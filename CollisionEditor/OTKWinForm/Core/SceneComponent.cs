using GpuGraphics;
using OpenTK;
using OTKWinForm.RenderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TextureLoader;

namespace OTKWinForm.Core
{
    public class SceneComponent : Component
    {
        protected RawModel model;
        protected BasicShader shader;
        protected ITexture texture;

        public override void AttachComponent(Component component)
        {
            base.AttachComponent(component);
        }

        public override void DetachComponent(Component component)
        {
            base.DetachComponent(component);
        }

        public override void Tick(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            base.Tick(viewMatrix, projectionMatrix);
        }

        public virtual void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            Matrix4 worldMatrix = GetWorldMatrix();
            texture.BindTexture(TextureUnit.Texture0);
            shader.startProgram();
            shader.SetTransformatrionMatrices(worldMatrix, viewMatrix, projectionMatrix);
            shader.SetDiffuseTexture(0);
            shader.SetOpacity(0.2f);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            VAOManager.renderBuffers(model.Buffer, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.CullFace);
            shader.stopProgram();

            foreach (var item in childrenComponents)
            {
                SceneComponent comp = item as SceneComponent;
                if (comp != null)
                    comp.Render(viewMatrix, projectionMatrix);
            }
        }

        public SceneComponent(RawModel model, ITexture texture, BasicShader shader)
        {
            this.texture = texture;
            this.model = model;
            this.shader = shader;
        }

        public override string ToString()
        {
            return "SceneComponent " + base.ToString();
        }
    }
}
