using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    public class HealthArgs : EventArgs
    {
        public HealthArgs(int health, int cost)
        {
            this.health = health;
            this.cost = cost;
        }

        public readonly int health;
        public readonly int cost;
    }
    internal class PowerUpHealth : ShopItem
    {
        public event EventHandler<HealthArgs> AddHealth;
        private const int HEALTH = 10; 
        public PowerUpHealth(Texture2D texture) : base(texture)
        {
            cost = 100;
        }

        public override void Picked()
        {
            AddHealth(this, new HealthArgs(HEALTH, cost));
        }
    }
}
