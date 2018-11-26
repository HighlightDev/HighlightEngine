using System;
using OpenTK;
using MassiveGame.Core.RenderCore.Lights;

namespace MassiveGame.Core.GameCore.Sun.DayCycle
{
    [Serializable]
    public class DayLightCycle
    {
        #region Definitions

        public DayPhases Phases { set; get; }
        public float TimeFlow { set; get; }
        public Int32 TimerPeriod { set; get; }
        private float time;

        [NonSerialized]
        private DirectionalLight sun;

        private float traectoryRadius;

        #endregion

        #region Constructor

        public DayLightCycle(DirectionalLight sun, float traectoryRadius, DayPhases phases)
        {
            this.Phases = phases;
            this.sun = sun;
            time = 0f;
            TimeFlow = 0.5f;
            TimerPeriod = 1;
            this.sun.Destination = new Vector3(traectoryRadius / 2, 0, traectoryRadius / 2);
            this.traectoryRadius = traectoryRadius;
        }

        #endregion

        #region Logics

        private void TimeIncrease(float deltaTime)
        {
            this.time += deltaTime * TimeFlow;
            this.time %= 99.9f;
        }

        private void CalculateCurrentLightColor()
        {
            Vector3 newAmbient = new Vector3(), newDiffuse = new Vector3(), newSpecular = new Vector3();
            // morning phase
            if (time < Phases.DayPhase.PhaseTime)
            {
                newAmbient = Phases.GetMorningAmbientIterpolatedColor(time);
                newDiffuse = Phases.GetMorningDiffuseIterpolatedColor(time);
                newSpecular = Phases.GetMorningSpecularIterpolatedColor(time);
            }
            // day phase
            if (Phases.DayPhase.PhaseTime <= time && time < Phases.EveningPhase.PhaseTime)
            {
                newAmbient = Phases.GetDayAmbientIterpolatedColor(time);
                newDiffuse = Phases.GetDayDiffuseIterpolatedColor(time);
                newSpecular = Phases.GetDaySpecularIterpolatedColor(time);
            }
            // evening phase
            if (Phases.EveningPhase.PhaseTime <= time && time < Phases.NightPhase.PhaseTime)
            {
                newAmbient = Phases.GetEveningAmbientIterpolatedColor(time);
                newDiffuse = Phases.GetEveningDiffuseIterpolatedColor(time);
                newSpecular = Phases.GetEveningSpecularIterpolatedColor(time);
            }
            // night phase
            if (Phases.NightPhase.PhaseTime <= time)
            {
                newAmbient = Phases.GetNightAmbientIterpolatedColor(time);
                newDiffuse = Phases.GetNightDiffuseIterpolatedColor(time);
                newSpecular = Phases.GetNightSpecularIterpolatedColor(time);
            }
            sun.Ambient = new Vector4(newAmbient, 1.0f);
            sun.Diffuse = new Vector4(newDiffuse, 1.0f);
            sun.Specular = new Vector4(newSpecular, 1.0f);
        }

        private void CalculateCurrentLightDirection()
        {
            float angle = (36 * time) / 10;
            float radians = MathHelper.DegreesToRadians(angle);
            Vector3 position = new Vector3(
                traectoryRadius * (float)Math.Cos(radians),
                traectoryRadius * (float)Math.Sin(radians),
                traectoryRadius);
            Vector3 direction = sun.Destination - position;
            sun.Direction = direction;
            sun.Position = position;
        }

        #endregion

        public void Tick(float deltaTime)
        {
            CalculateCurrentLightDirection();
            CalculateCurrentLightColor();
            TimeIncrease(deltaTime);
        }

        #region Manipulation

        public void SetTime(float time)
        {
            if (time > 99.9f)
            {
                time = 99.9f;
            }
            else if (time < 0.0f)
            {
                time = 0.0f;
            }
            this.time = time;
        }

        #endregion
    }
}
