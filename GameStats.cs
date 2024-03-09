using System;
using Microsoft.Xna.Framework;
using Monsterfall_01.PowerUp;

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
            score += 1000;
        }
        internal void OnAddHealth(object sender, HealthArgs e)
        {
            score -= e.cost;
        }

        internal void OnSpeedUp(object sender, PowerUpSpeed.SpeedUpArgs e)
        {
            score -= e.cost;
        }

        internal void OnBowUpgrade(object sender, BowArgs e)
        {
            score -= e.cost;
        }
    }
}
