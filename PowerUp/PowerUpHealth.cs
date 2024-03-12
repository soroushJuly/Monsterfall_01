using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    // Arguments sent with the bow upgrade event
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
        // Amount of health added to player after buying the item
        private const int HEALTH = 10;
        
        public event EventHandler<HealthArgs> AddHealth;
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
