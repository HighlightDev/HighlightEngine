using GpuGraphics;
using MassiveGame.API.Collector;
using MassiveGame.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;
using System;
using System.Collections.Generic;
using VMath;
using PhysicsBox.MathTypes;

namespace MassiveGame
{
    public class Mirror
    {
        #region Definitions

        private bool _postConstructor;
        private MirrorFBO _fbo;
        private VBOArrayF _attribs;
        private VAO _buffer;
        private VAO _rectangleBuffer;
        private MirrorShader _shader;
        private CustomDepthShader _depthShader;
        private SimpleClearDepthShader _clearDepthShader;
        private Vector3 _translation, _rotation, _scaling;
        private Vector3 _reflectionDir;
        private Vector3 _intersectionPoint;
        private Vector3 _intersactionStack;

        private float[,] quadVertices = new float[6, 3] { { 0, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 }, { 0, 1, 0 }, { 1, 0, 0 }, { 1, 1, 0 } };

        /*Temporary*/
        private float[,] _vertices = new float[6, 3] { { -13, 0, 0 }, { 13, 0, 0 }, { 13, 26, 0 }, { 13, 26, 0 }, { -13, 26, 0 }, { -13, 0, 0 } },
           _texCoords = new float[6, 2] { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 1, 1 }, { 0, 1 }, { 0, 0 } },
           _normals = new float[6, 3] { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 } };
        /*Temporary*/

        #endregion

        #region Intermidiate_calculations

        private Vector3 findIntersaction(Vector3 rayStartPoint, Vector3 surfacePoint, Vector3 ray, Vector3 normal)
        {
            float testValue = Vector3.Dot(ray, normal);
            if ((testValue < 0.9f && testValue > 0) || (testValue > -0.9f && testValue < 0))
            {
                return _intersactionStack;
            }
            Vector3 planeDir = surfacePoint - rayStartPoint;
            float t = Vector3.Dot(planeDir, normal) / Vector3.Dot(ray, normal);
            Vector3 intersaction = rayStartPoint + ray * t;
            _intersactionStack = intersaction;
            return intersaction;
        }

        private Vector3 findReflection(Vector3 normal, Vector3 ray)
        {
            Vector3 R = ray - 2 * normal * Vector3.Dot(ray, normal);
            R.Normalize();
            return R;
        }

        private void calculateIntersaction(Camera camera)
        {
            /*Create transformation matrix*/
            Matrix4 transformMatrix = Matrix4.Identity;
            transformMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X));
            transformMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y));
            transformMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_rotation.Z));
            transformMatrix *= Matrix4.CreateTranslation(_translation);
            /*Transform normal*/
            Vector4 transformedNormal = new Vector4(_attribs.Normals[0, 0], _attribs.Normals[0, 1],
                _attribs.Normals[0, 2], 0.0f);
            transformedNormal = Vector4.Transform(transformedNormal, transformMatrix);
            transformedNormal.Normalize();
            /*Transform vertices*/
            Vector4 transformedVertices = new Vector4(_attribs.Vertices[0, 0], _attribs.Vertices[0, 1],
                _attribs.Vertices[0, 2], 1.0f);
            transformedVertices = Vector4.Transform(transformedVertices, transformMatrix);

            /*Определим направление луча из камеры и нормализируем его*/
            Vector3 ray = camera.getLookVector() - camera.getPositionVector();
            ray.Normalize();
            /*Найдем точку пересечения луча и полигона зеркала*/
            _intersectionPoint = findIntersaction(camera.getPositionVector(), new Vector3(transformedVertices), ray, new Vector3(transformedNormal));
            /*Найдем вектор отражения луча от полигона при имеющейся нормали*/
            this._reflectionDir = findReflection(new Vector3(transformedNormal), ray);
        }

        #endregion

        #region Renderer

        public Camera renderSceneToMirror(Camera camera)
        {
            postConstructor();
            calculateIntersaction(camera);
            _fbo.renderToFBO(1, _fbo.Texture.Rezolution[0].widthRezolution, _fbo.Texture.Rezolution[0].heightRezolution);

            Vector3 reflectionDest = _intersectionPoint;
            reflectionDest += _reflectionDir * 10;

            return new Camera(_intersectionPoint.X, _intersectionPoint.Y, _intersectionPoint.Z,
                reflectionDest.X, reflectionDest.Y, reflectionDest.Z, 0, 1, 0);
        }

        public void renderMirror(Camera camera, Matrix4 projectionMatrix, int screenWidth, int screenHeight,
            PointLight[] lights, DirectionalLight sun = null)
        {
            Matrix4 modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X));
            modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y));
            modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_rotation.Z));
            modelMatrix *= Matrix4.CreateScale(_scaling.X);
            modelMatrix *= Matrix4.CreateTranslation(_translation);

            Matrix3 normalMatrix = VectorMath.matrix4ToMatrix3
               (Matrix4.Transpose(Matrix4.Invert(camera.getViewMatrix() * modelMatrix))); // Матрица нормалей

            _shader.startProgram();

            /*Отрисовать зеркало*/
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
            _shader.setUniforms(modelMatrix, camera.getViewMatrix(), projectionMatrix, normalMatrix, 0, lights, sun);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);

            _shader.stopProgram();
        }

        public void RenderPrepassMirror(LiteCamera camera, ref Matrix4 projectionMatrix, Action MirrorPass)
        {
            // Set depth and stencil buffers
            GL.ClearStencil(0);
            GL.DepthFunc(DepthFunction.Gequal);
            GL.Enable(EnableCap.DepthTest);
            GL.StencilFunc(StencilFunction.Always, 1, 0xff);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.StencilMask(0xff);

            _depthShader.startProgram();
            _depthShader.SetUniformValues(Matrix4.Identity, camera.getViewMatrix(), projectionMatrix);
            MirrorPass();
            _depthShader.stopProgram();
            GL.StencilFunc(StencilFunction.Always, 0, 0x00);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
        }

        private Matrix4 GetMirrorMatrix()
        {
            Vector3 normal = new Vector3(_normals[0, 0], _normals[0, 1], _normals[0, 2]);
            Vector3 point = new Vector3(_vertices[0, 0], _vertices[0, 1], _vertices[0, 2]);
            FPlane mirrorPlane = new FPlane(point, normal);
            FMirrorMatrix mirrorMatrix = new FMirrorMatrix(mirrorPlane);
            return mirrorMatrix;
        }

        public void RenderDepthPrepass()
        {
            GL.StencilFunc(StencilFunction.Equal, 1, 0xff);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            _clearDepthShader.startProgram();
            VAOManager.renderBuffers(_rectangleBuffer, PrimitiveType.Triangles);
            _clearDepthShader.stopProgram();
        }

        public void RenderReflectedPrims(LiteCamera camera, ref Matrix4 projectionMatrix, List<Building> entities)
        {
        }

        #endregion

        #region Constructor

        public void postConstructor()
        {
            if (_postConstructor)
            {
                VAOManager.genVAO(_buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);

                _rectangleBuffer = new VAO(new VBOArrayF(quadVertices));
                VAOManager.genVAO(_rectangleBuffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _rectangleBuffer);

                _depthShader = (CustomDepthShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "customDepthVS.glsl",
                    ProjectFolders.ShadersPath + "customDepthFS.glsl", "", typeof(CustomDepthShader));

                _shader = (MirrorShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "mirrorVS.glsl",
                   ProjectFolders.ShadersPath + "mirrorFS.glsl", "", typeof(MirrorShader));

                _clearDepthShader = (SimpleClearDepthShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "clearDepthVS.glsl",
                    ProjectFolders.ShadersPath + "clearDepthFS.glsl", "", typeof(SimpleClearDepthShader));

                _postConstructor = !_postConstructor;
            }
        }

        public Mirror(VBOArrayF attribs, Vector3 translation, Vector3 rotation, Vector3 scaling)
        {
            _fbo = new MirrorFBO();
            this._translation = translation;
            this._rotation = rotation;
            this._scaling = scaling;
            //this._attribs = attribs;
            _attribs = new VBOArrayF(_vertices, _normals, _texCoords, false);
            this._buffer = new VAO(_attribs);
            this._postConstructor = true;
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            ResourcePool.ReleaseShaderProgram(_shader);
            ResourcePool.ReleaseShaderProgram(_depthShader);
            ResourcePool.ReleaseShaderProgram(_clearDepthShader);

            this._shader.cleanUp();
            this._fbo.cleanUp();
            VAOManager.cleanUp(_buffer);
        }

        #endregion
    }
}
