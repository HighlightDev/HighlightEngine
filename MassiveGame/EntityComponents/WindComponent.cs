using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame
{
    public class WindComponent
    {
        #region Definitions 

        public Vector3 WindDirection { private set; get; }
        public float WindIntensity { private set; get; }
        public float ClimaxWindIntensity { private set; get; }
        public float WindPower { private set; get; }

        #endregion

        #region Update

        public float updateWindLoop(float currentWindLoopValue)
        {
            //grass animation
            if (!(currentWindLoopValue >= 70 && currentWindLoopValue <= 110 
                || currentWindLoopValue >= 250 && currentWindLoopValue <= 290))
            {
                // normal speed animation
                return currentWindLoopValue > 360.0f ? 0.0f : currentWindLoopValue += this.WindIntensity; 
            }
                // make a pause at climax
            else return currentWindLoopValue += ClimaxWindIntensity;
        }

        #endregion

        #region Constructor

        public WindComponent(float windIntensity, float climaxWindIntensity, float windPower, Vector3 windDirection)
        {
            this.WindIntensity = windIntensity;
            this.ClimaxWindIntensity = climaxWindIntensity;
            this.WindPower = windPower;
            if (windDirection[0] <= 1.0f || windDirection[0] >= 1.0f && windDirection[1] <= 1.0f || windDirection[1] >= 1.0f
                && windDirection[2] <= 1.0f || windDirection[2] >= 1.0f) this.WindDirection = windDirection; //Если направление уже нормализировано - не делаем этого
            else this.WindDirection = Vector3.Normalize(windDirection);  //Нормализируем вектор направления
        }

        #endregion
    }
}
