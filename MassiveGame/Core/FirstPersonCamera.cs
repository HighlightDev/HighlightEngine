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

namespace MassiveGame.Core
{
    public enum CAMERA_DIRECTIONS
    {
        LEFT,
        RIGHT,
        FORWARD,
        BACK,
        STAY
    }
   
    public class FirstPersonCamera : BaseCamera
    {

        #region Definitions
        private const float CAMERA_SPEED = 20.5f;

        #endregion

        public FirstPersonCamera(Vector3 CamDir, Vector3 CameraPosition)
        : base()
        {

        }

        public void moveCamera(CAMERA_DIRECTIONS direction)
        {
            Vector3 forwardVector = GetEyeSpaceForwardVector();

            //switch (direction)
            //{
            //    case CAMERA_DIRECTIONS.FORWARD:
            //        {
            //            PosVector += forwardVector * CAMERA_SPEED;
            //            LookVector += forwardVector * CAMERA_SPEED;
            //            break;
            //        }
            //    case CAMERA_DIRECTIONS.BACK:
            //        {
            //            PosVector -= forwardVector * CAMERA_SPEED;
            //            LookVector -= forwardVector * CAMERA_SPEED;
            //            break;
            //        }
            //    case CAMERA_DIRECTIONS.LEFT:
            //        {
            //            Vector3 directionAxis = Vector3.Cross(forwardVector, GetUpVector());
            //            directionAxis.Normalize();
            //            PosVector -= directionAxis * CAMERA_SPEED;
            //            LookVector -= directionAxis * CAMERA_SPEED;
            //            break;
            //        }
            //    case CAMERA_DIRECTIONS.RIGHT:
            //        {
            //            Vector3 directionAxis = Vector3.Cross(forwardVector, GetUpVector());
            //            directionAxis.Normalize();
            //            PosVector += directionAxis * CAMERA_SPEED;
            //            LookVector += directionAxis * CAMERA_SPEED;
            //            break;
            //        }
            //}
        }

        public override Vector3 GetEyeVector()
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetTargetVector()
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetLocalSpaceUpVector()
        {
            throw new NotImplementedException();
        }

        //public void RotateByMouse(Int32 x, Int32 y, Int32 screenWidth, Int32 screenHeight)
        //{
        //    Int32 middleX = screenWidth >> 1;  //Половина ширины экрана
        //    Int32 middleY = screenHeight >> 1; //Половина высоты экрана


        //    // Теперь, получив координаты курсора, возвращаем его обратно в середину.
        //    Int32 captionHeight = ((DOUEngine.WINDOW_BORDER != WindowBorder.Hidden) && (DOUEngine.WINDOW_STATE != WindowState.Fullscreen)) ?
        //        SystemInformation.CaptionHeight : 0; // для корректной работы камеры с учетом рамки

        //    Cursor.Position = new Point(DOUEngine.SCREEN_POSITION_X + middleX,
        //        DOUEngine.SCREEN_POSITION_Y + middleY + captionHeight);

        //    Int32 deltaX = middleX - x;
        //    Int32 deltaY = middleY - y;


        //    RotatePosition(-deltaX, -deltaY);
        //}

        //public void RotatePosition(Int32 deltaX, Int32 deltaY)
        //{
        //    // rotate pitch
        //    Vector3 lookDir = Vector3.Normalize(LookVector - PosVector);
        //    Vector3 binormalDir = Vector3.Normalize(Vector3.Cross(lookDir, upVector));
        //    Matrix4 rotatePitch = Matrix4.CreateFromAxisAngle(binormalDir, MathHelper.DegreesToRadians(-deltaY * ROTATE_MEASURE));

        //    // rotate yaw
        //    Matrix4 rotateYaw = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-deltaX * ROTATE_MEASURE));

        //    Matrix4 rotationMatrix = Matrix4.Identity;
        //    rotationMatrix *= Matrix4.CreateTranslation(-LookVector);
        //    rotationMatrix *= rotateYaw;
        //    rotationMatrix *= rotatePitch;
        //    rotationMatrix *= Matrix4.CreateTranslation(LookVector);

        //    PosVector = new Vector3(VectorMath.multMatrix(rotationMatrix, new Vector4(PosVector, 1.0f)));
        //}

        //public void movePosition(Vector3 positionBias)
        //{
        //    PosVector += positionBias;
        //}

        //public void Update(Terrain terrain)
        //{
        //    if (CameraMode == CAMERA_MODE.THIRD_PERSON)
        //    {
        //        this.LookVector = Target.GetCharacterCollisionBound().GetOrigin();
        //    }

        //    //if (terrain != null)
        //    //{
        //    //    var currentYlvl = terrain.getLandscapeHeight(PosVector.X, PosVector.Z);
        //    //    this.PosVector.Y = PosVector.Y < currentYlvl + 1.3f ? currentYlvl + 1.3f : PosVector.Y;
        //    //}
        //}

        //public void SetThirdPerson(MovableEntity obj)
        //{
        //    CameraMode = CAMERA_MODE.THIRD_PERSON;
        //    Target = obj;

        //    Vector3 objCenter = new Vector3(0);
        //    //obj.ComponentTranslation;
        //    PosVector += new Vector3(objCenter.X, objCenter.Y + 40, objCenter.Z + 10);
        //}

        //public void setThirdPersonZoom(Int32 Zoom)
        //{
        //    if (Zoom == -1)
        //    {
        //        var temp = LookVector - PosVector;
        //        float length = temp.Length;
        //        Vector3 zoomIn = GetNormalizedDirection();
        //        if (length < MIN_CAMERA_DISTANCE) return;
        //        PosVector += zoomIn * CAMERA_SPEED;
        //    }
        //    if (Zoom == 1)
        //    {
        //        var temp = PosVector - LookVector;
        //        float length = temp.Length;
        //        Vector3 zoomOut = Vector3.Normalize(temp);
        //        if (length > MAX_CAMERA_DISTANCE) return;
        //        PosVector += zoomOut * CAMERA_SPEED;
        //    }
        //}

        //public void UpdateHeight(Vector3 previousPosition)
        //{
        //    if (CameraMode == CAMERA_MODE.THIRD_PERSON)
        //    {
        //        var heightBias = Target.ComponentTranslation.Y - previousPosition.Y;
        //        var dist = GetNormalizedDirection() * Target.Speed;
        //        if (Target.ComponentTranslation != previousPosition)
        //        {
        //            movePosition(new Vector3(dist.X, heightBias, dist.Z));
        //        }
        //    }
        //}

        #region Constructor

        //public Camera(float eyeX, float eyeY, float eyeZ,
        //    float centerX, float centerY, float centerZ,
        //    float upX, float upY, float upZ) : base(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ)
        //{
        //    this.CameraMode = CAMERA_MODE.UNDEFINDED;
        //}

        #endregion
    }
}
