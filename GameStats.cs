using System;
using Microsoft.Xna.Framework;
using Monsterfall_01.PowerUp;

namespace Monsterfall_01
{
    public class GameStats
    {
        // Player score
        public int score;
        // Time spent in game
        public int timeSpent;
        public string name;

        public event EventHandler<int> OnScoreChanged;
        public GameStats()
        {
            score = 0;
            timeSpent = 0;
            name = string.Empty;
        }
        public void OnEnemyDied(object owner, int e)
        {
            ChangeScore(e);
        }
        internal void OnAddHealth(object sender, HealthArgs e)
        {
            ChangeScore(-1 * e.cost);
        }

        internal void OnSpeedUp(object sender, PowerUpSpeed.SpeedUpArgs e)
        {
            ChangeScore(-1 * e.cost);
        }

        internal void OnBowUpgrade(object sender, BowArgs e)
        {
            ChangeScore(-1 * e.cost);
        }
        // Changes the player score by passed value
        private void ChangeScore(int change)
        {
            score += change;
            OnScoreChanged(this, score);
        }
    }
}
