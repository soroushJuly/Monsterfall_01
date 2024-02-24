using System;

namespace Monsterfall_01
{
    public class PlayerInfo
    {
        public int health = 100;
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
        public int Score = 0;
        public void OnEnemyDied(object owner, EventArgs eventArgs)
        {
            Score += 10;
        }
    }
}
