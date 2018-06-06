using Grid;
using MassiveGame;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using VMath;

namespace MassiveGame
{
    public enum CAMERA_DIRECTIONS
    {
        LEFT,
        RIGHT,
        FORWARD,
        BACK,
        STAY
    }

    public enum CAMERA_MODE
    {
        UNDEFINDED,
        FIRST_PERSON,
        THIRD_PERSON
    }

    public class Camera : LiteCamera
    {
        #region NestedEnum

      

        #endregion

        #region Definitions

        private float MIN_CAMERA_DISTANCE = 5.0f;
        private float MAX_CAMERA_DISTANCE = 250.0f;
        private const float CAMERA_SPEED = 20.5f; 
        private float currentRotX;
        private float lastRotX;

        public CAMERA_MODE CameraMode { private set; get; }
        public bool SwitchCamera { set; get; }

        #endregion

        public void moveCamera(CAMERA_DIRECTIONS direction)
        {
            Vector3 vector = LookVector - PosVector;
            vector.Normalize();

            switch (direction)
            {
                case CAMERA_DIRECTIONS.FORWARD:
                    {
                        PosVector += vector * CAMERA_SPEED;
                        LookVector += vector * CAMERA_SPEED;
                        break;
                    }
                case CAMERA_DIRECTIONS.BACK:
                    {
                        PosVector -= vector * CAMERA_SPEED;
                        LookVector -= vector * CAMERA_SPEED;
                        break;
                    }
                case CAMERA_DIRECTIONS.LEFT:
                    {
                        Vector3 directionAxis = Vector3.Cross(vector, upVector);
                        directionAxis.Normalize();
                        PosVector -= directionAxis * CAMERA_SPEED;
                        LookVector -= directionAxis * CAMERA_SPEED;
                        break;
                    }
                case CAMERA_DIRECTIONS.RIGHT:
                    {
                        Vector3 directionAxis = Vector3.Cross(vector, upVector);
                        directionAxis.Normalize();
                        PosVector += directionAxis * CAMERA_SPEED;
                        LookVector += directionAxis * CAMERA_SPEED;
                        break;
                    }
            }
        }

        public void RotateCameraByMouse(int x, int y, int screenWidth, int screenHeight)
        {
            Point mousePosition = new Point();  //Структура, хранящяя X и Y позиции мыши

            int middleX = screenWidth >> 1;  //Половина ширины экрана
            int middleY = screenHeight >> 1; //Половина высоты экрана
            float angleY = 0.0f;    // Направление взгляда вверх/вниз
            float angleZ = 0.0f;    // Значение, необходимое для вращения влево-вправо (по оси Y)

            // Получаем текущие коорд. мыши
            mousePosition.X = x;
            mousePosition.Y = y;

            if ((mousePosition.X == DOUEngine.SCREEN_POSITION_X + middleX)
                 && (mousePosition.Y == DOUEngine.SCREEN_POSITION_Y + middleY)) return;
            // Теперь, получив координаты курсора, возвращаем его обратно в середину.
            int captionHeight = ((DOUEngine.WINDOW_BORDER != WindowBorder.Hidden) && (DOUEngine.WINDOW_STATE != WindowState.Fullscreen)) ?
                SystemInformation.CaptionHeight : 0; // для корректной работы камеры с учетом рамки
            Cursor.Position = new Point(DOUEngine.SCREEN_POSITION_X + middleX,
                DOUEngine.SCREEN_POSITION_Y + middleY + captionHeight);
            
            angleY = ((middleX - mousePosition.X)) / 1000.0f;
            angleZ = ((middleY - mousePosition.Y)) / 1000.0f;

            lastRotX = currentRotX;     // Сохраняем последний угол вращения и используем заново currentRotX
            // Если текущее вращение больше 1 градуса, обрежем его, чтобы не вращать слишком быстро
            if (currentRotX > 1.0f)
            {
                currentRotX = 1.0f;
                // вращаем на оставшийся угол
                if (lastRotX != 1.0f)
                {
                    // Чтобы найти ось, вокруг которой вращаться вверх и вниз, нужно 
                    // найти вектор, перпендикулярный вектору взгляда камеры и 
                    // вертикальному вектору.
                    // Это и будет наша ось. И прежде чем использовать эту ось, нормализовать её.
                    Vector3 vAxis = Vector3.Cross(LookVector - PosVector, upVector);
                    vAxis = Vector3.Normalize(vAxis);

                    // Вращаем камеру вокруг нашей оси на заданный угол
                    rotateCamera(1.0f - lastRotX, (float)vAxis.X, (float)vAxis.Y, (float)vAxis.Z);
                }
            }
            // Если угол меньше -1.0f, убедимся, что вращение не продолжится
            else if (currentRotX < -1.0f)
            {
                currentRotX = -1.0f;
                if (lastRotX != -1.0f)
                {
                    // Опять же вычисляем ось
                    Vector3 vAxis = Vector3.Cross(LookVector - PosVector, upVector);
                    vAxis = Vector3.Normalize(vAxis);

                    // Вращаем
                    rotateCamera(-1.0f - (float)lastRotX, (float)vAxis.X, (float)vAxis.Y, (float)vAxis.Z);
                }
            }
            // Если укладываемся в пределы 1.0f -1.0f - просто вращаем
            else
            {
                Vector3 vAxis = Vector3.Cross(LookVector - PosVector, upVector);
                vAxis = Vector3.Normalize(vAxis);

                // Вращаем
                rotateCamera((float)angleZ, (float)vAxis.X, (float)vAxis.Y, (float)vAxis.Z);  
            }
            // Всегда вращаем камеру вокруг Y-оси
            rotateCamera((float)angleY, 0, 1, 0);
        }

        public void movePosition(float xPositionBias, float zPositionBias)
        {
            PosVector += new Vector3(xPositionBias, 0, zPositionBias);
        }

        public void movePosition(Vector3 positionBias)
        {
            PosVector += positionBias;
        }
      
        public void DetachCamera() //Camera doesn't depend from any object
        {
            CameraMode = CAMERA_MODE.UNDEFINDED;
        }

        public void rotateCamera(float angle, float x, float y, float z)
        {
            Vector3 direction = cameraBridge(angle, x, y, z);
            if (CameraMode == CAMERA_MODE.FIRST_PERSON)
            {
                rotateViewCamera(direction);
            }
            else if (CameraMode == CAMERA_MODE.THIRD_PERSON)
            {
                rotatePosCamera(direction);
            }
        }

        private void rotateViewCamera(Vector3 newDirection)   //Rotate camera around the axis
        {
            LookVector = PosVector + newDirection;
        }

        private void rotatePosCamera(Vector3 newDirection)
        {
            PosVector = LookVector + newDirection;
        }

        private Vector3 cameraBridge(float angle, float x, float y, float z)   //Rotate camera around the axis
        {
            float[] vNewDirection = new float[3];
            float[] vDirection = new float[3];

            // Получим наш вектор взгляда (направление, куда мы смотрим)
            vDirection[0] = LookVector.X - PosVector.X;    //направление по X
            vDirection[1] = LookVector.Y - PosVector.Y;    //направление по Y
            vDirection[2] = LookVector.Z - PosVector.Z;    //направление по Z

            float cosTheta = Convert.ToSingle(Math.Cos(angle));
            float sinTheta = Convert.ToSingle(Math.Sin(angle));

            // Найдем новую позицию X для вращаемой точки
            vNewDirection[0] = (cosTheta + (1 - cosTheta) * x * x) * vDirection[0];
            vNewDirection[0] += ((1 - cosTheta) * x * y - z * sinTheta) * vDirection[1];
            vNewDirection[0] += ((1 - cosTheta) * x * z + y * sinTheta) * vDirection[2];

            // Найдем позицию Y
            vNewDirection[1] = ((1 - cosTheta) * x * y + z * sinTheta) * vDirection[0];
            vNewDirection[1] += (cosTheta + (1 - cosTheta) * y * y) * vDirection[1];
            vNewDirection[1] += ((1 - cosTheta) * y * z - x * sinTheta) * vDirection[2];

            // И позицию Z
            vNewDirection[2] = ((1 - cosTheta) * x * z - y * sinTheta) * vDirection[0];
            vNewDirection[2] += ((1 - cosTheta) * y * z + x * sinTheta) * vDirection[1];
            vNewDirection[2] += (cosTheta + (1 - cosTheta) * z * z) * vDirection[2];

            return new Vector3(vNewDirection[0], vNewDirection[1], vNewDirection[2]);
        }

        public void Update(Terrain terrain)
        {
            if (CameraMode == CAMERA_MODE.THIRD_PERSON)
            {
                this.LookVector = Target.GetCharacterCollisionBound().GetOrigin();
            }

            //if (terrain != null)
            //{
            //    var currentYlvl = terrain.getLandscapeHeight(PosVector.X, PosVector.Z);
            //    this.PosVector.Y = PosVector.Y < currentYlvl + 1.3f ? currentYlvl + 1.3f : PosVector.Y;
            //}
        }

        public void SetThirdPerson(MovableEntity obj)
        {
            CameraMode = CAMERA_MODE.THIRD_PERSON;
            Target = obj;

            Vector3 objCenter = new Vector3(0);
            //obj.ComponentTranslation;
            PosVector += new Vector3(objCenter.X, objCenter.Y + 40, objCenter.Z + 10);
        }

        public void SetFirstPerson()
        {
            CameraMode = CAMERA_MODE.FIRST_PERSON;
        }

        public void setThirdPersonZoom(int Zoom)
        {
            if (Zoom == -1)
            {
                var temp = LookVector - PosVector;
                float length = temp.Length;
                Vector3 zoomIn = GetNormalizedDirection();
                if (length < MIN_CAMERA_DISTANCE) return;
                PosVector += zoomIn * CAMERA_SPEED;
            }
            if (Zoom == 1)
            {
                var temp = PosVector - LookVector;
                float length = temp.Length;
                Vector3 zoomOut = Vector3.Normalize(temp);
                if (length > MAX_CAMERA_DISTANCE) return;
                PosVector += zoomOut * CAMERA_SPEED;
            }
        }

        public void UpdateHeight(Vector3 previousPosition)
        {
            if (CameraMode == CAMERA_MODE.THIRD_PERSON)
            {
                var heightBias = Target.ComponentTranslation.Y - previousPosition.Y;
                var dist = GetNormalizedDirection() * Target.Speed;
                if (Target.ComponentTranslation != previousPosition)
                {
                    movePosition(new Vector3(dist.X, heightBias, dist.Z));
                }
            }
        }

        #region Constructor

        public Camera(float eyeX, float eyeY, float eyeZ,
            float centerX, float centerY, float centerZ,
            float upX, float upY, float upZ) : base(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ)
        {
            this.CameraMode = CAMERA_MODE.UNDEFINDED;
            currentRotX = 0.0f;
            lastRotX = 0.0f;
        }

        public Camera() : base()
        {
            this.CameraMode = CAMERA_MODE.UNDEFINDED;
            currentRotX = 0.0f;
            lastRotX = 0.0f;
        }

        #endregion
    }
}
