using GpuGraphics;
using OpenTK;
using CollisionEditor.RenderCore;
using System;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.MathCore.MathTypes;

namespace CollisionEditor.Core
{
    public class CollisionComponent : Component
    {
        private RawModel model;
        private BasicShader shader;
        private ITexture texture;
        private bool bComponentHierarchyChangedDirty = true;

        public override void AttachComponent(Component component)
        {
            bComponentHierarchyChangedDirty = true;
            base.AttachComponent(component);
        }

        public override void DetachComponent(Component component)
        {
            bComponentHierarchyChangedDirty = true;
            base.DetachComponent(component);
        }

        public override void Tick(float delta)
        {
            Bound = UpdateBound();
            base.Tick(delta);
        }

        private BoundBase UpdateBound()
        {
            BoundBase resultBound = null;
            if (bComponentHierarchyChangedDirty || bTransformationDirty)
            {
                AABB aabb = AABB.CreateFromMinMax(FindEdgeInMesh((lhv, rhv) => { return Math.Min(lhv, rhv); }),
                    FindEdgeInMesh((lhv, rhv) => { return Math.Max(lhv, rhv); }), this);
                Matrix4 TransformMatrix = GetWorldMatrix();
                Quaternion rotation = TransformMatrix.ExtractRotation();
                if (rotation.Xyz.LengthSquared > 0.01f)
                    resultBound = new OBB(aabb.GetLocalSpaceOrigin(), aabb.GetLocalSpaceExtent(), TransformMatrix, this);
                else
                {
                    aabb.ScalePlusTranslation = TransformMatrix;
                    resultBound = aabb;
                }
                bComponentHierarchyChangedDirty = false;
            }
            else
            {
                resultBound = Bound;
            }
            return resultBound;
        }

        private Vector3 FindEdgeInMesh(Func<float, float, float> func)
        {
            float[,] vertices = model.Buffer.getBufferData().Vertices;

            float edge1 = vertices[0, 0], edge2 = vertices[0, 1], edge3 = vertices[0, 2];

            var iterationCount = vertices.Length / 3;

            for (Int32 i = 0; i < iterationCount; i++)
            {
                edge1 = func(edge1, vertices[i, 0]);
                edge2 = func(edge2, vertices[i, 1]);
                edge3 = func(edge3, vertices[i, 2]);
            }

            return new Vector3(edge1, edge2, edge3);
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
                CollisionComponent comp = item as CollisionComponent;
                if (comp != null)
                    comp.Render(viewMatrix, projectionMatrix);
            }
        }

        public CollisionComponent(RawModel model, ITexture texture, BasicShader shader)
        {
            this.texture = texture;
            this.model = model;
            this.shader = shader;
        }

        public CollisionComponent() { }

        public override string ToString()
        {
            return "SceneComponent_" + ComponentCreator.GetIdByComponent(this);
        }
    }
}
