using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    #region FadeEnum

    public enum FadeType
    {
        LINEARLY = 0,
        EXPONENT = 1,
        QUADRATIC = 2
    }

    #endregion

    public delegate void MistEvent(float velocity);

    public sealed class MistComponent
    {
        #region Definitions

        private event MistEvent FadeOccurs;
        private event MistEvent AppearOccurs;

        private float _density;
        private float _gradient;

        private float _densityThreshold;

        private Vector3 _mistColour;

        private float _fadeVelocity;
        private float _appearVelocity;

        public bool EnableMist { set; get; }

        #endregion

        #region Update

        public void update()
        {
            /*Check fade event*/
            if (FadeOccurs != null)
            { FadeOccurs(this._fadeVelocity); }

            /*Check appear event*/
            if (AppearOccurs != null)
            { AppearOccurs(this._appearVelocity); }
        }

        #endregion

        #region Appear_mist

        private bool checkAppear(float mistDensity, float mistDensityThreshold)
        {
            if (mistDensityThreshold <= mistDensity) return false;
            else return true;
        }

        public void appear(double renderTime, int fadeTime, FadeType fType, float densityThreshold)
        {
            /*In case if  mist has already increased*/
            if (!checkAppear(this._density, densityThreshold)) return;

            int oneRedrawTime = convertFromSecToMsec(renderTime);
            int beatTime = fadeTime / oneRedrawTime;

            switch(fType)
            {
                case FadeType.LINEARLY:
                    {
                        float mistVolume = densityThreshold - this.MistDensity;
                        _appearVelocity = mistVolume / beatTime;
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

        private int convertFromSecToMsec(double sTime)
        {
            int msTime = Convert.ToInt32(sTime * 1000);
            return msTime;
        }

        public void fade(double renderTime, int fadeTime, FadeType fType, float densityThreshold)
        {
            /*In case if  mist has already dicreased*/
            if (!checkFade(this._density, densityThreshold)) return;

            int oneRedrawTime = convertFromSecToMsec(renderTime);
            int beatTime = fadeTime / oneRedrawTime;

            switch (fType)
            {
                case FadeType.LINEARLY:
                    {
                         /*Fade mist*/
                        float fadeVolume = this.MistDensity - densityThreshold;                       
                        _fadeVelocity = fadeVolume / beatTime;
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
