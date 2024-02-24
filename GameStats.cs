using System;
using Microsoft.Xna.Framework;

namespace Monsterfall_01
{
    public class GameStats
    {
        public int score;
        public int timeSpent;
        public string name;

        public GameStats()
        {
            score = 0;
            timeSpent = 0;
            name = string.Empty;
        }
        public void OnEnemyDied(object owner, EventArgs eventArgs)
        {
            score += 10;
        }
    }
}
