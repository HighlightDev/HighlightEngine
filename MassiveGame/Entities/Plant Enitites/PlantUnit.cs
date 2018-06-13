using OpenTK;
using System;
using System.Linq;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public sealed class PlantUnit
    {
        #region Definitions

        public Vector3 Translation { set; get; }
        public Vector3 Scale { set; get; }
        public Vector3 Rotation { set; get; }
        public float WindLoop { set; get; }
        public uint textureID { set; get; }

        #endregion

        #region Randomize_values

        private void randomizePlant(Int32 seed, float MAP_SIZE, uint[] textures)
        {
            Random random = new Random(seed);
            float tempX = Translation.X, tempZ = Translation.Z;
            tempX += Convert.ToSingle(random.NextDouble() * (MAP_SIZE - 10.0f));
            tempZ += Convert.ToSingle(random.NextDouble() * (MAP_SIZE - 10.0f));
            Translation = new Vector3(tempX, Translation.Y, tempZ);

            WindLoop = 0.0f;
            WindLoop = Convert.ToSingle(random.NextDouble() * 360.0f);
            textureID = (uint)random.Next((Int32)textures[0], (Int32)textures.Last() + 1);
        }

        #endregion

        #region Constructor

        public PlantUnit(Int32 seed, Vector3 Scale, float MAP_SIZE, uint[] textures)
        {
            this.Translation = new Vector3(0);
            this.Scale = Scale;
            randomizePlant(seed, MAP_SIZE, textures);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Translation">Value of grass translation </param>
        /// <param name="Rotation">Value of grass rotation </param>
        /// <param name="Scale">Value of grass scaling </param>
        /// <param name="textureID">Texture ID param</param>
        /// <param name="previous">Previous plant for wind loop value</param> 
        public PlantUnit(Vector3 Translation, Vector3 Rotation, Vector3 Scale,  uint textureID,
            PlantUnit previous)
        {
            this.Translation = Translation;
            this.Scale = Scale;
            this.WindLoop = previous != null ? previous.WindLoop : Convert.ToSingle(new Random().NextDouble() * 360.0f);
            this.textureID = textureID;
        }

        #endregion
    }
}
