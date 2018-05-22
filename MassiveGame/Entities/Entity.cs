using System;
using System.Collections.Generic;

using MassiveGame.Optimization;
using GpuGraphics;
using OpenTK;
using PhysicsBox;
using TextureLoader;
using VMath;
using MassiveGame.API.Collector;
using MassiveGame.Core;
using MassiveGame.RenderCore.Visibility;
using MassiveGame.RenderCore.Lights;
using MassiveGame.RenderCore;
using PhysicsBox.ComponentCore;

namespace MassiveGame
{
    public abstract class Entity: Component, IVisible, ILightAffection, IDrawable
    {
        #region Definitions 

        protected float _leftX, _rightX, _nearZ, _farZ, _topY, _bottomY;

        protected bool _isInCameraView, _postConstructor;

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        protected CollisionSphereBox _box;

        protected MistComponent _mist;

        protected RawModel _model;

        protected ITexture _texture;

        protected ITexture _normalMap;

        protected ITexture _specularMap;

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        protected CollisionDetector collisionDetection;

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        protected Vector3[] _verticesVectors;

        protected RenderMap LightVisibilityMap;

        public void SetComponents(List<Component> components)
        {
            foreach (Component child in components)
                child.ParentComponent = this;
            ChildrenComponents = components;
        }

        public override void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            base.Tick(ref projectionMatrix, ref viewMatrix);
        }

        #region LightOptimization

        protected virtual List<PointLight> GetRelevantPointLights(List<PointLight> PotentialyAffectedPointLights)
        {
            List<PointLight> AffectedPointLights = new List<PointLight>();
            for (int i = 0; i < PotentialyAffectedPointLights.Count; i++)
            {
                if (LightVisibilityMap[i])
                {
                    AffectedPointLights.Add(PotentialyAffectedPointLights[i]);
                }
            }
            return AffectedPointLights;
        }

        #endregion

        public bool IsInCameraView { protected set { _isInCameraView = value; } get { return _isInCameraView; } }

        public virtual CollisionSphereBox Box
        {
            protected set { this._box = value; }
            get
            {
                return this._box;
            }
        }

        #endregion

        #region Interface_realization

        public virtual bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            return IsInCameraView = FrustumCulling.isBoxIntersection(Box, viewMatrix, ref projectionMatrix);
        }

        public virtual void IsLightAffecting(List<PointLight> LightList)
        {
            LightVisibilityMap.Init(LightList.Count, false);
            for (int i = 0; i < LightList.Count; i++)
            {
                LightVisibilityMap[i] = PhysicsBox.GeometricMath.IsSphereVsSphereIntersection(this.Box.getCenter(), this.Box.Radius,
                    LightList[i].Position.Xyz, LightList[i].AttenuationRadius);
            }
        }

        #endregion

        #region Getter

        public VAO GetModel()
        {
            return _model.Buffer;
        }

        public ITexture GetDiffuseMap()
        {
            return _texture;
        }

        public ITexture GetNormalMap()
        {
            return _normalMap;
        }

        public ITexture GetSpecularMap()
        {
            return _specularMap;
        }

        /// <summary>
        ///  Находим из массива координат стороны объекта
        /// </summary>
        /// <param name="attribs"> Атрибуты модели </param>
        protected void GetEdges(float[,] vertices)
        {
            float tempLeft = vertices[0, 0],
                  tempRight = tempLeft,
                  tempBottom = vertices[0, 1],
                  tempTop = tempBottom,
                  tempNear = vertices[0, 2],
                  tempFar = tempNear;

            var iterationCount = vertices.Length / 3;

            for (int i = 0; i < iterationCount; i++)
            {
                if (tempLeft > vertices[i, 0]) //Находим минимум по Х
                {
                    tempLeft = vertices[i, 0];
                }
                if (tempRight < vertices[i, 0])  //Находим максимум по Х
                {
                    tempRight = vertices[i, 0];
                }
                if (tempBottom > vertices[i, 1])   //Находим минимум по Y
                {
                    tempBottom = vertices[i, 1];
                }
                if (tempTop < vertices[i, 1])  //Находим максимум по Y
                {
                    tempTop = vertices[i, 1];
                }
                if (tempNear > vertices[i, 2]) //Находим минимум по Z  
                {
                    tempNear = vertices[i, 2];
                }
                if (tempFar < vertices[i, 2])  //Находим максимум по Z
                {
                    tempFar = vertices[i, 2];
                }
            }
          
            this._leftX = tempLeft;
            this._rightX = tempRight;
            this._bottomY = tempBottom;
            this._topY = tempTop;
            this._nearZ = tempNear;
            this._farZ = tempFar;
        }

        #endregion

        #region Setter

        public void SetMistComponent(MistComponent mist)
        {
            this._mist = mist;
        }

        public virtual void SetCollisionDetector(CollisionDetector collisionDetector)
        {
            this.collisionDetection = collisionDetector;
        }

        #endregion

        #region Transformation

        protected Matrix4 BuildTransformationMatrix()
        {
            Matrix4 ModelMatrix = Matrix4.Identity;

            if (Rotation[0] != 0 || Rotation[1] != 0 || Rotation[2] != 0)
            {
                ModelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
                ModelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
                ModelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
            }

            if (Scale[0] != 0 && Scale[1] != 0 && Scale[2] != 0)
            {
                if (Scale[0] != 1 || Scale[1] != 1 || Scale[2] != 1)
                {
                    ModelMatrix *= Matrix4.CreateScale(Scale);
                }
            }
            else
            {
                ModelMatrix *= Matrix4.CreateScale(1);
            }

            if (Translation[0] != 0.0f || Translation[1] != 0.0f || Translation[2] != 0.0f)
            {
                ModelMatrix *= Matrix4.CreateTranslation(Translation);
            }
            return ModelMatrix;
        } 

        #endregion

        #region Constructor

        public Entity()
        {
            if (ChildrenComponents == null)
                ChildrenComponents = new List<Component>();
        }
        public Entity(string modelPath, string texturePath, string normalMapPath, string specularMapPath,
            Vector3 translation, Vector3 rotation, Vector3 scale) : base()
        {
            Translation = translation; 
            Rotation = rotation;
            Scale = scale;
            _texture = ResourcePool.GetTexture(texturePath);
            _normalMap = ResourcePool.GetTexture(normalMapPath);
            _specularMap = ResourcePool.GetTexture(specularMapPath);
            _model = new RawModel(modelPath);
            CopyVertices(_model.Buffer.getBufferData().Vertices);

            this._box = new CollisionSphereBox(_leftX, _rightX, _bottomY, _topY, _nearZ, _farZ, -1);
            this._isInCameraView = true;
            this._mist = null;
            this._postConstructor = true;
            LightVisibilityMap = new RenderMap();
        }

        private void CopyVertices(float[,] vertices)
        {
            var modelMatrix = BuildTransformationMatrix();
            float[,] tempVertices = new float[vertices.GetLength(0), vertices.GetLength(1)];
            Buffer.BlockCopy(vertices, 0, tempVertices, 0, vertices.Length * sizeof(float));
            tempVertices = GetTransformedVertices(ref modelMatrix, tempVertices);
            GetEdges(tempVertices);

            this._verticesVectors = GetVerticesVectors(tempVertices);
        }

        private float[,] GetTransformedVertices(ref Matrix4 modelMatrix, float[,] vertices)
        {
            for (int i = 0; i < vertices.Length / 3; i++)
            {
                Vector4 vertex = new Vector4(vertices[i, 0], vertices[i, 1], vertices[i, 2], 1.0f);
                vertex = VectorMath.multMatrix(modelMatrix, vertex);
                vertices[i, 0] = vertex.X;
                vertices[i, 1] = vertex.Y;
                vertices[i, 2] = vertex.Z;
            }
            return vertices;
        }

        private Vector3[] GetVerticesVectors(float[,] vertices)
        {
            Vector3[] vectors = new Vector3[vertices.Length / 3];
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = new Vector3(vertices[i, 0], vertices[i, 1], vertices[i, 2]);
            }
            return vectors;
        }

        #endregion

        #region Cleaning

        public virtual void cleanUp()
        {
            _model.Dispose();
            if (_texture != null)
                _texture.CleanUp();
            if (_normalMap != null)
                _normalMap.CleanUp();
            if (_specularMap != null)
                _specularMap.CleanUp();
        }

        #endregion
    }
}
