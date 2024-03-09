using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    public class BowArgs : EventArgs
    {
        public BowArgs(float duration, int cost)
        {
            this.duration = duration;
            this.cost = cost;
        }

        public readonly float duration;
        public readonly int cost;
    }
    internal class PowerUpBow : ShopItem
    {
        public event EventHandler<BowArgs> OnBowUpgrade;
        private const int DURATION = 20;
        public PowerUpBow(Texture2D texture) : base(texture)
        {
            cost = 200;
        }
        public override void Picked()
        {
            OnBowUpgrade(this, new BowArgs(DURATION, cost));
        }
    }
}
