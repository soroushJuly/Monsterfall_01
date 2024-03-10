using System;
using System.Collections.Generic;

namespace Monsterfall_01
{
    public class PlayerInfo
    {
        public int health = 100;
    }
    public class GameWaves
    {
        public List<Wave> waves = new List<Wave>();
    }
    public class Wave
    {
        public int enemyCount = 0;
        public float timeToNextWave = 0f;
    }
    public class GameInfo
    {
        private static GameInfo mInstance = null;
        public static GameInfo Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new GameInfo();
                return mInstance;
            }

            set { mInstance = value; }
        }

        public PlayerInfo PlayerInfo;
        public GameWaves GameWaves;
    }
}
