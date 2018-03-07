using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Sun.DayCycle
{
    public class DayPhases
    {
        #region NestedTypes

        public class Morning : Phase
        {
            public float PhaseTime { private set; get; }
            public Morning(Vector3 ambient, Vector3 diffuse, Vector3 specular) :
                base(ambient, diffuse, specular)
            {
                this.PhaseTime = 0;
            }
        }

        public class Day : Phase
        {
            public float PhaseTime { private set; get; }
            public Day(Vector3 ambient, Vector3 diffuse, Vector3 specular) :
                base(ambient, diffuse, specular)
            {
                this.PhaseTime = 25;
            }
        }

        public class Evening : Phase
        {
            public float PhaseTime { private set; get; }
            public Evening(Vector3 ambient, Vector3 diffuse, Vector3 specular) :
                base(ambient, diffuse, specular)
            {
                this.PhaseTime = 55;
            }
        }

        public class Night : Phase
        {
            public float PhaseTime { private set; get; }
            public Night(Vector3 ambient, Vector3 diffuse, Vector3 specular) :
                base(ambient, diffuse, specular)
            {
                this.PhaseTime = 75;
            }
        }

        #endregion

        #region Constructor

        public DayPhases(Morning morning, Day day, Evening evening, Night night)
        {
            this.MorningPhase = morning;
            this.DayPhase = day;
            this.EveningPhase = evening;
            this.NightPhase = night;
        }

        #endregion

        #region Definitions

        public Morning MorningPhase { get; set; }

        public Day DayPhase { get; set; }

        public Evening EveningPhase { get; set; }

        public Night NightPhase { get; set; }

        #endregion

        #region Assistance

        #region Ambient

        public Vector3 GetMorningAmbientIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   MorningPhase.AmbientLight,
                   DayPhase.AmbientLight,
                   MorningPhase.PhaseTime,
                   DayPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetDayAmbientIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   DayPhase.AmbientLight,
                   EveningPhase.AmbientLight,
                   DayPhase.PhaseTime,
                   EveningPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetEveningAmbientIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   EveningPhase.AmbientLight,
                   NightPhase.AmbientLight,
                   EveningPhase.PhaseTime,
                   NightPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetNightAmbientIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   NightPhase.AmbientLight,
                   MorningPhase.AmbientLight,
                   NightPhase.PhaseTime,
                   99.0f,
                   x);
            return newAmbient;
        }

        #endregion

        #region Diffuse

        public Vector3 GetMorningDiffuseIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   MorningPhase.DiffuseLight,
                   DayPhase.DiffuseLight,
                   MorningPhase.PhaseTime,
                   DayPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetDayDiffuseIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   DayPhase.DiffuseLight,
                   EveningPhase.DiffuseLight,
                   DayPhase.PhaseTime,
                   EveningPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetEveningDiffuseIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   EveningPhase.DiffuseLight,
                   NightPhase.DiffuseLight,
                   EveningPhase.PhaseTime,
                   NightPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetNightDiffuseIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   NightPhase.DiffuseLight,
                   MorningPhase.DiffuseLight,
                   NightPhase.PhaseTime,
                   99.0f,
                   x);
            return newAmbient;
        }

        #endregion

        #region Specular

        public Vector3 GetMorningSpecularIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   MorningPhase.DiffuseLight,
                   DayPhase.DiffuseLight,
                   MorningPhase.PhaseTime,
                   DayPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetDaySpecularIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   DayPhase.DiffuseLight,
                   EveningPhase.DiffuseLight,
                   DayPhase.PhaseTime,
                   EveningPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetEveningSpecularIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   EveningPhase.DiffuseLight,
                   NightPhase.DiffuseLight,
                   EveningPhase.PhaseTime,
                   NightPhase.PhaseTime,
                   x);
            return newAmbient;
        }

        public Vector3 GetNightSpecularIterpolatedColor(float x)
        {
            var newAmbient = LightInterpolation.InterpolateColor(
                   NightPhase.DiffuseLight,
                   MorningPhase.DiffuseLight,
                   NightPhase.PhaseTime,
                   99.0f,
                   x);
            return newAmbient;
        }

        #endregion

        #endregion
    }
}
