using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.RenderCore.Lights;

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{
    public abstract class StaticEntity : Entity
    {
        #region Collision

        public override void SetCollisionHeadUnit(CollisionHeadUnit collisionHeadUnit)
        {
            base.SetCollisionHeadUnit(collisionHeadUnit);
            collisionHeadUnit.AddCollisionObserver(this);
        }

        #endregion

        #region Renderer

        public virtual void renderObject(PrimitiveType mode, DirectionalLight Sun, List<PointLight> lights, BaseCamera camera,
            ref Matrix4 ProjectionMatrix, Vector4 clipPlane) { }

        #endregion

        #region Constructor

        public StaticEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath,
            Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
        }

        #endregion
    }
}
