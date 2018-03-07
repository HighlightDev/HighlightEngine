using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.IDgenerator
{
    public static class IdGenerator
    {
        private static int player_id;
        
        public const int PLAYER_MIN_ID = 1;
        public const int PLAYER_MAX_ID = 1000000;

        static IdGenerator()
        {
            player_id = PLAYER_MIN_ID;
        }

        public static bool IsPlayerId(int id)
        {
            return (id >= PLAYER_MIN_ID && id <= PLAYER_MAX_ID);
        }

        public static int GeneratePlayerId()
        {
            return player_id++;
        }
        
    }
}
