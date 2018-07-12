using System;

namespace MassiveGame.Core.GameCore
{
    public class BoolMap
    {
        private bool[] map;

        public bool this[Int32 index]
        {
            set { map[index] = value; }
            get { return map[index]; }
        }

        public void Init(Int32 count, bool initValue)
        {
            map = new bool[count];
            for (var i = 0; i < count; i++)
            {
                map[i] = initValue;
            }
        }
    }
}
