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
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;

namespace OTKWinForm.Core
{
    public class SceneComponent : Component
    {
        private RawModel model;
        private BasicShader shader;
        private ITexture texture;

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
            Bound = CreateBound();
        }

        private BoundBase CreateBound()
        {
            BoundBase resultBound = null;
            AABB aabb = AABB.CreateFromMinMax(FindMinFromModel(), FindMaxFromModel());
            Matrix4 TransformMatrix = GetWorldMatrix();
            Quaternion rotation = TransformMatrix.ExtractRotation();
            if (rotation.Xyz.LengthSquared > 0.01f)
                resultBound = new OBB(aabb.Origin, aabb.Extent, TransformMatrix);
            else
            {
                aabb.TransformBound(TransformMatrix);
                resultBound = aabb;
            }
            return resultBound;
        }

        private Vector3 FindMaxFromModel()
        {
            float[,] vertices = model.Buffer.getBufferData().Vertices;

            float tempRight = vertices[0, 0],
                  tempTop = vertices[0, 1],
                  tempFar = vertices[0, 2];

            var iterationCount = vertices.Length / 3;

            for (int i = 0; i < iterationCount; i++)
            {
                if (tempRight < vertices[i, 0])  //Находим максимум по Х
                {
                    tempRight = vertices[i, 0];
                }
                if (tempTop < vertices[i, 1])  //Находим максимум по Y
                {
                    tempTop = vertices[i, 1];
                }
                if (tempFar < vertices[i, 2])  //Находим максимум по Z
                {
                    tempFar = vertices[i, 2];
                }
            }
            return new Vector3(tempRight, tempTop, tempFar);
        }

        private Vector3 FindMinFromModel()
        {
            float[,] vertices = model.Buffer.getBufferData().Vertices;

            float tempLeft = vertices[0, 0],
                  tempBottom = vertices[0, 1],
                  tempNear = vertices[0, 2];

            var iterationCount = vertices.Length / 3;

            for (int i = 0; i < iterationCount; i++)
            {
                if (tempLeft > vertices[i, 0]) //Находим минимум по Х
                {
                    tempLeft = vertices[i, 0];
                }
                if (tempBottom > vertices[i, 1])   //Находим минимум по Y
                {
                    tempBottom = vertices[i, 1];
                }
                if (tempNear > vertices[i, 2]) //Находим минимум по Z  
                {
                    tempNear = vertices[i, 2];
                }
            }

            return new Vector3(tempLeft, tempBottom, tempNear);
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

            foreach (var item in ChildrenComponents)
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

        public SceneComponent()
        {

        }

        public override string ToString()
        {
            return "SceneComponent_" + ComponentCreator.GetIdByComponent(this);
        }
    }
}
