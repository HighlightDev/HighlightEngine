using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using PhysicsBox;
using MassiveGame.RenderCore.Lights;
using MassiveGame.Physics;

namespace MassiveGame
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

        public virtual void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun, List<PointLight> lights, LiteCamera camera,
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
