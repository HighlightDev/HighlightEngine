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
        private const float CAMERA_SPEED = 2.5f; 
        private float currentRotX;
        private float lastRotX;
        private MotionEntity thirdPersonTarget;

        public CAMERA_MODE Mode { private set; get; }
        public bool SwitchCamera { set; get; }

        #endregion

        public void moveCamera(CAMERA_DIRECTIONS direction)
        {
            Vector3 vector = lookVector - posVector;
            vector.Normalize();

            switch (direction)
            {
                case CAMERA_DIRECTIONS.FORWARD:
                    {
                        posVector += vector * CAMERA_SPEED;
                        lookVector += vector * CAMERA_SPEED;
                        break;
                    }
                case CAMERA_DIRECTIONS.BACK:
                    {
                        posVector -= vector * CAMERA_SPEED;
                        lookVector -= vector * CAMERA_SPEED;
                        break;
                    }
                case CAMERA_DIRECTIONS.LEFT:
                    {
                        Vector3 directionAxis = Vector3.Cross(vector, upVector);
                        directionAxis.Normalize();
                        posVector -= directionAxis * CAMERA_SPEED;
                        lookVector -= directionAxis * CAMERA_SPEED;
                        break;
                    }
                case CAMERA_DIRECTIONS.RIGHT:
                    {
                        Vector3 directionAxis = Vector3.Cross(vector, upVector);
                        directionAxis.Normalize();
                        posVector += directionAxis * CAMERA_SPEED;
                        lookVector += directionAxis * CAMERA_SPEED;
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
                    Vector3 vAxis = Vector3.Cross(lookVector - posVector, upVector);
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
                    Vector3 vAxis = Vector3.Cross(lookVector - posVector, upVector);
                    vAxis = Vector3.Normalize(vAxis);

                    // Вращаем
                    rotateCamera(-1.0f - (float)lastRotX, (float)vAxis.X, (float)vAxis.Y, (float)vAxis.Z);
                }
            }
            // Если укладываемся в пределы 1.0f -1.0f - просто вращаем
            else
            {
                Vector3 vAxis = Vector3.Cross(lookVector - posVector, upVector);
                vAxis = Vector3.Normalize(vAxis);

                // Вращаем
                rotateCamera((float)angleZ, (float)vAxis.X, (float)vAxis.Y, (float)vAxis.Z);  
            }
            // Всегда вращаем камеру вокруг Y-оси
            rotateCamera((float)angleY, 0, 1, 0);
        }

        public void movePosition(float xPositionBias, float zPositionBias)
        {
            posVector.X += xPositionBias;
            posVector.Z += zPositionBias;
        }

        public void movePosition(Vector3 positionBias)
        {
            posVector.X += positionBias.X;
            posVector.Y += positionBias.Y;
            posVector.Z += positionBias.Z;
        }
      
        public void DetachCamera() //Camera doesn't depend from any object
        {
            Mode = CAMERA_MODE.UNDEFINDED;
        }

        public void rotateCamera(float angle, float x, float y, float z)
        {
            Vector3 direction = cameraBridge(angle, x, y, z);
            if (Mode == CAMERA_MODE.FIRST_PERSON)
            {
                rotateViewCamera(direction);
            }
            else if (Mode == CAMERA_MODE.THIRD_PERSON)
            {
                rotatePosCamera(direction);
            }
        }

        private void rotateViewCamera(Vector3 newDirection)   //Rotate camera around the axis
        {
            lookVector = posVector + newDirection;
        }

        private void rotatePosCamera(Vector3 newDirection)
        {
            posVector = lookVector + newDirection;
        }

        private Vector3 cameraBridge(float angle, float x, float y, float z)   //Rotate camera around the axis
        {
            float[] vNewDirection = new float[3];
            float[] vDirection = new float[3];

            // Получим наш вектор взгляда (направление, куда мы смотрим)
            vDirection[0] = lookVector.X - posVector.X;    //направление по X
            vDirection[1] = lookVector.Y - posVector.Y;    //направление по Y
            vDirection[2] = lookVector.Z - posVector.Z;    //направление по Z

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
            if (Mode == CAMERA_MODE.THIRD_PERSON)
            {
                this.lookVector.X = thirdPersonTarget.Box.getCenter().X;
                this.lookVector.Y = thirdPersonTarget.Box.getCenter().Y + 3.0f;
                this.lookVector.Z = thirdPersonTarget.Box.getCenter().Z;
            }

            if (terrain != null)
            {
                var currentYlvl = terrain.getLandscapeHeight(posVector.X, posVector.Z);
                this.posVector.Y = posVector.Y < currentYlvl + 1.3f ? currentYlvl + 1.3f : posVector.Y;
            }
        }

        public void SetThirdPerson(MotionEntity obj)
        {
            Mode = CAMERA_MODE.THIRD_PERSON;
            thirdPersonTarget = obj;
 
            Vector3 objCenter = obj.Box.getCenter();
            this.posVector.X = objCenter.X;
            this.posVector.Y = objCenter.Y + 40.0f;
            this.posVector.Z = objCenter.Z + 10.0f;
        }

        public void SetFirstPerson()
        {
            Mode = CAMERA_MODE.FIRST_PERSON;
        }

        public void setThirdPersonZoom(int Zoom)
        {
            if (Zoom == -1)
            {
                var temp = lookVector - posVector;
                float length = temp.Length;
                Vector3 zoomIn = GetNormalizedDirection();
                if (length < MIN_CAMERA_DISTANCE) return;
                posVector += zoomIn * CAMERA_SPEED;
            }
            if (Zoom == 1)
            {
                var temp = posVector - lookVector;
                float length = temp.Length;
                Vector3 zoomOut = Vector3.Normalize(temp);
                if (length > MAX_CAMERA_DISTANCE) return;
                posVector += zoomOut * CAMERA_SPEED;
            }
        }

        public void UpdateHeight(Vector3 previousPosition)
        {
            var heightBias = thirdPersonTarget.ObjectPosition.Y - previousPosition.Y;
            var dist = GetNormalizedDirection() * thirdPersonTarget.Speed;
            if (thirdPersonTarget.ObjectPosition != previousPosition)
            {
                movePosition(new Vector3(dist.X, heightBias, dist.Z));
            }
        }

        #region Constructor

        public Camera(float eyeX, float eyeY, float eyeZ,
            float centerX, float centerY, float centerZ,
            float upX, float upY, float upZ)
        {
            this.Mode = CAMERA_MODE.UNDEFINDED;
            posVector = new Vector3();
            lookVector = new Vector3();
            upVector = new Vector3();
            posVector.X = eyeX;
            posVector.Y = eyeY;
            posVector.Z = eyeZ;
            lookVector.X = centerX;
            lookVector.Y = centerY;
            lookVector.Z = centerZ;
            upVector.X = upX;
            upVector.Y = upY;
            upVector.Z = upZ;
            currentRotX = 0.0f;
            lastRotX = 0.0f;
        }

        #endregion
    }
}
