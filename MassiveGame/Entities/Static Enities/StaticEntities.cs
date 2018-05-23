using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using PhysicsBox;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public abstract class StaticEntity : Entity
    {
        #region Definitions

        /* Return center of entity */
        protected Vector3 EntityCenter
        {
            get
            {
                return new Vector3((base._leftX + base._rightX) / 2,
                   (base._bottomY + base._topY) / 2, (base._nearZ + base._farZ) / 2);
            }
        }

        #endregion

        #region Collision

        public override void SetCollisionDetector(CollisionDetector collisionDetector)
        {
            base.SetCollisionDetector(collisionDetector);
            // add polygons to collision detector
            base.collisionDetection.addPolygonArray(base._verticesVectors);
        }

        #endregion

        #region Renderer

        public virtual void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun, List<PointLight> lights, LiteCamera camera,
            ref Matrix4 ProjectionMatrix, Vector4 clipPlane) { }  //Функция рендеринга

        #endregion

        #region Constructor

        public StaticEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath,
            Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            _box = new CollisionSphereBox(base._leftX, base._rightX, base._bottomY, base._topY, base._nearZ, base._farZ, -1);
        }

        #endregion
    }
}
