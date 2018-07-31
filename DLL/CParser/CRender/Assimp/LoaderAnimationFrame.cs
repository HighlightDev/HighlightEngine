using Assimp;
using System;

namespace CParser.Assimp
{
    public class LoaderAnimationFrame
    {
        public double TimeStart { set; get; }
        public Nullable<Vector3D> Translation { set; get; }
        public Nullable<Quaternion> Rotation { set; get; }
        public Nullable<Vector3D> Scale { set; get; }

        public LoaderAnimationFrame(double timeStart, Nullable<Vector3D> translation, Nullable<Quaternion> rotation, Nullable<Vector3D> scale) 
        {
            TimeStart = timeStart;
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
