using System;
using System.Runtime.Serialization;
using OpenTK;

namespace MassiveGame.Core.GameCore.EntityComponents
{
    #region FadeEnum

    [Serializable]
    public enum FadeType
    {
        LINEARLY = 0,
        EXPONENT = 1,
        QUADRATIC = 2
    }

    #endregion

    public delegate void MistEvent(float velocity);

    [Serializable]
    public class MistComponent : ISerializable
    {
        #region Definitions

        private event MistEvent FadeOccurs;
        private event MistEvent AppearOccurs;

        private float _density;
        private float _gradient;

        private float _densityThreshold;

        private Vector3 _mistColour;

        private float m_fadeVelocity;
        private float m_appearVelocity;

        public bool EnableMist { set; get; }

        #endregion

        #region Update

        public void Tick(float deltaTime)
        {
            /*Check fade event*/
            if (FadeOccurs != null)
            { FadeOccurs(this.m_fadeVelocity); }

            /*Check appear event*/
            if (AppearOccurs != null)
            { AppearOccurs(this.m_appearVelocity); }
        }

        #endregion

        #region Appear_mist

        private bool checkAppear(float mistDensity, float mistDensityThreshold)
        {
            if (mistDensityThreshold <= mistDensity) return false;
            else return true;
        }

        public void appear(double renderTime, Int32 fadeTime, FadeType fType, float densityThreshold)
        {
            /*In case if  mist has already increased*/
            if (!checkAppear(this._density, densityThreshold)) return;

            Int32 oneRedrawTime = convertFromSecToMsec(renderTime);
            Int32 beatTime = fadeTime / oneRedrawTime;

            switch(fType)
            {
                case FadeType.LINEARLY:
                    {
                        float mistVolume = densityThreshold - this.MistDensity;
                        m_appearVelocity = mistVolume / beatTime;
                        this._densityThreshold = densityThreshold;
                        this.AppearOccurs += MistComponent_AppearOccurs;
                        this.EnableMist = true;
                        break;
                    }
                case FadeType.EXPONENT:
                    {

                        break;
                    }
                case FadeType.QUADRATIC:
                    {

                        break;
                    }
            }
        }

        void MistComponent_AppearOccurs(float velocity)
        {
            if ((_density + velocity) <= _densityThreshold)
            {
                this._density += velocity;
            }
            else
            {
                this._density = _densityThreshold;
                this.AppearOccurs -= MistComponent_AppearOccurs;
                Console.WriteLine("Mist appeared!");
            }
        }

        #endregion

        #region Fade_mist

        private bool checkFade(float mistDensity, float mistDensityThreshold)
        {
            if (mistDensityThreshold >= mistDensity) return false;
            else return true;
        }

        private Int32 convertFromSecToMsec(double sTime)
        {
            Int32 msTime = Convert.ToInt32(sTime * 1000);
            return msTime;
        }

        public void fade(double renderTime, Int32 fadeTime, FadeType fType, float densityThreshold)
        {
            /*In case if  mist has already decreased*/
            if (!checkFade(this._density, densityThreshold)) return;

            Int32 oneRedrawTime = convertFromSecToMsec(renderTime);
            Int32 beatTime = fadeTime / oneRedrawTime;

            switch (fType)
            {
                case FadeType.LINEARLY:
                    {
                         /*Fade mist*/
                        float fadeVolume = this.MistDensity - densityThreshold;                       
                        m_fadeVelocity = fadeVolume / beatTime;
                        this._densityThreshold = densityThreshold;
                        this.FadeOccurs += MistComponent_FadeOccurs;
                        break;
                    }
                case FadeType.EXPONENT:
                    {

                        break;
                    }
                case FadeType.QUADRATIC:
                    {

                        break;
                    }
            }
        }

        private void MistComponent_FadeOccurs(float FadeVelocity)
        {
            if ((_density - FadeVelocity) > _densityThreshold)
            {
                this._density -= FadeVelocity;
            }
            else
            {
                this._density = _densityThreshold;
                this.FadeOccurs -= MistComponent_FadeOccurs;
                Console.WriteLine("Mist disappeared!");
            }

            if (_density <= 0.0f) this.EnableMist = false;
        }

        #endregion

        #region Set_N_Get

        public float FadeDensityThreshold { set { this._densityThreshold = value; } get { return this._densityThreshold; } }

        public Vector3 MistColour { set { this._mistColour = value; } get { return this._mistColour; } }

        public float MistGradient { set { this._gradient = value; } get { return this._gradient; } }

        public float MistDensity { set { this._density = value; } get { return this._density; } }

        #endregion

        #region Serialization

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_density", _density);
            info.AddValue("_gradient", _gradient);
            info.AddValue("_densityThreshold", _densityThreshold);
            info.AddValue("_mistColour", _mistColour, typeof(Vector3));
        }

        protected MistComponent(SerializationInfo info, StreamingContext context)
        {
            EnableMist = true;
            _density = info.GetSingle("_density");
            _gradient = info.GetSingle("_gradient");
            _densityThreshold = info.GetSingle("_densityThreshold");
            _mistColour = (Vector3)info.GetValue("_mistColour", typeof(Vector3));
        }

        #endregion

        #region Constructors

        public MistComponent(float MistDensity, float MistGradient, Vector3 MistColour)
        {
            this._density = MistDensity;
            this._gradient = MistGradient;
            this._mistColour = MistColour;
            this.EnableMist = true;
        }
            
        #endregion
    }
}
