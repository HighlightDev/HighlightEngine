using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System.Collections.Generic;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Pools;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.Policies;

namespace MassiveGame.Core.ComponentCore
{
    public class SceneComponent : Component
    {
        private VertexArrayObject buffer = null;
        private bool bPostConstructor = true;

        public override void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            if (bPostConstructor)
            {
                if ((new ObtainModelPool().GetPool() as ModelPool).GetModelReferenceCount("CollisionBound") == 0)
                    AddBoundModelToRoot();
                else
                    buffer = PoolProxy.GetResource<ObtainModelPool, ModelAllocationPolicy, string, VertexArrayObject>("CollisionBound");
                bPostConstructor = false;
            }
            base.Tick(ref projectionMatrix, ref viewMatrix);
        }

        public override void RenderBound(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix, Color4 color)
        {
            Matrix4 worldMatrix = Matrix4.Identity;
            if ((Bound.GetBoundType() & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
                worldMatrix = (Bound as AABB).ScalePlusTranslation;
            else
                worldMatrix = (Bound as OBB).TransformationMatrix;

            Matrix4 modelViewMatrix = worldMatrix * viewMatrix;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelViewMatrix);
            GL.Color4(color);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, (float[,])buffer.GetVertexBufferArray().First().GetBufferData());
            GL.DrawArrays(PrimitiveType.LineStrip, 0, buffer.GetVertexBufferArray().First().GetBufferElementsCount());
            GL.DisableClientState(ArrayCap.VertexArray);

            base.RenderBound(ref projectionMatrix, ref viewMatrix, color);
        }

        private void AddBoundModelToRoot()
        {
            Vector3 LBNCoordinates = Bound.GetLocalSpaceMin();
            Vector3 RTFCoordinates = Bound.GetLocalSpaceMax();

            float[,] renderCoordinates = new float[24, 3];

            renderCoordinates[0, 0] = LBNCoordinates.X; renderCoordinates[0, 1] = LBNCoordinates.Y; renderCoordinates[0, 2] = LBNCoordinates.Z;
            renderCoordinates[1, 0] = RTFCoordinates.X; renderCoordinates[1, 1] = LBNCoordinates.Y; renderCoordinates[1, 2] = LBNCoordinates.Z;
            renderCoordinates[2, 0] = RTFCoordinates.X; renderCoordinates[2, 1] = RTFCoordinates.Y; renderCoordinates[2, 2] = LBNCoordinates.Z;
            renderCoordinates[3, 0] = LBNCoordinates.X; renderCoordinates[3, 1] = RTFCoordinates.Y; renderCoordinates[3, 2] = LBNCoordinates.Z;

            renderCoordinates[4, 0] = RTFCoordinates.X; renderCoordinates[4, 1] = LBNCoordinates.Y; renderCoordinates[4, 2] = LBNCoordinates.Z;
            renderCoordinates[5, 0] = RTFCoordinates.X; renderCoordinates[5, 1] = LBNCoordinates.Y; renderCoordinates[5, 2] = RTFCoordinates.Z;
            renderCoordinates[6, 0] = RTFCoordinates.X; renderCoordinates[6, 1] = RTFCoordinates.Y; renderCoordinates[6, 2] = RTFCoordinates.Z;
            renderCoordinates[7, 0] = RTFCoordinates.X; renderCoordinates[7, 1] = RTFCoordinates.Y; renderCoordinates[7, 2] = LBNCoordinates.Z;

            renderCoordinates[8, 0] = RTFCoordinates.X; renderCoordinates[8, 1] = LBNCoordinates.Y; renderCoordinates[8, 2] = RTFCoordinates.Z;
            renderCoordinates[9, 0] = LBNCoordinates.X; renderCoordinates[9, 1] = LBNCoordinates.Y; renderCoordinates[9, 2] = RTFCoordinates.Z;
            renderCoordinates[10, 0] = LBNCoordinates.X; renderCoordinates[10, 1] = RTFCoordinates.Y; renderCoordinates[10, 2] = RTFCoordinates.Z;
            renderCoordinates[11, 0] = RTFCoordinates.X; renderCoordinates[11, 1] = RTFCoordinates.Y; renderCoordinates[11, 2] = RTFCoordinates.Z;

            renderCoordinates[12, 0] = LBNCoordinates.X; renderCoordinates[12, 1] = LBNCoordinates.Y; renderCoordinates[12, 2] = RTFCoordinates.Z;
            renderCoordinates[13, 0] = LBNCoordinates.X; renderCoordinates[13, 1] = LBNCoordinates.Y; renderCoordinates[13, 2] = LBNCoordinates.Z;
            renderCoordinates[14, 0] = LBNCoordinates.X; renderCoordinates[14, 1] = RTFCoordinates.Y; renderCoordinates[14, 2] = LBNCoordinates.Z;
            renderCoordinates[15, 0] = LBNCoordinates.X; renderCoordinates[15, 1] = RTFCoordinates.Y; renderCoordinates[15, 2] = RTFCoordinates.Z;

            renderCoordinates[16, 0] = LBNCoordinates.X; renderCoordinates[16, 1] = LBNCoordinates.Y; renderCoordinates[16, 2] = LBNCoordinates.Z;
            renderCoordinates[17, 0] = RTFCoordinates.X; renderCoordinates[17, 1] = LBNCoordinates.Y; renderCoordinates[17, 2] = LBNCoordinates.Z;
            renderCoordinates[18, 0] = RTFCoordinates.X; renderCoordinates[18, 1] = LBNCoordinates.Y; renderCoordinates[18, 2] = RTFCoordinates.Z;
            renderCoordinates[19, 0] = LBNCoordinates.X; renderCoordinates[19, 1] = LBNCoordinates.Y; renderCoordinates[19, 2] = RTFCoordinates.Z;

            renderCoordinates[20, 0] = LBNCoordinates.X; renderCoordinates[20, 1] = RTFCoordinates.Y; renderCoordinates[20, 2] = LBNCoordinates.Z;
            renderCoordinates[21, 0] = RTFCoordinates.X; renderCoordinates[21, 1] = RTFCoordinates.Y; renderCoordinates[21, 2] = LBNCoordinates.Z;
            renderCoordinates[22, 0] = RTFCoordinates.X; renderCoordinates[22, 1] = RTFCoordinates.Y; renderCoordinates[22, 2] = RTFCoordinates.Z;
            renderCoordinates[23, 0] = LBNCoordinates.X; renderCoordinates[23, 1] = RTFCoordinates.Y; renderCoordinates[23, 2] = RTFCoordinates.Z;

            buffer = new VertexArrayObject();

            var verticesVBO = new VertexBufferObjectTwoDimension<float>(renderCoordinates, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Store);
            buffer.AddVBO(verticesVBO);
            buffer.BindVbosToVao();

            PoolCollector.GetInstance().ModelPool.AddModelToRoot(buffer, "CollisionBound");
        }

        public SceneComponent() : base()
        {
        }

        public SceneComponent(Component component, bool bCopyComponents = false) : base()
        {
            this.Bound = component.Bound;
            if (bCopyComponents)
            {
                this.ChildrenComponents = component.ChildrenComponents;
                this.ParentComponent = component.ParentComponent;
            }
            else
            {
                this.ChildrenComponents = new List<Component>();
                this.ParentComponent = null;
            }
            this.Type = component.Type;
            this.ComponentTranslation = component.ComponentTranslation;
            this.ComponentRotation = component.ComponentRotation;
            this.ComponentScale = component.ComponentScale;
        }
    }
}
