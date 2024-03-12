using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    // Arguments sent with the bow upgrade event
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
        // duration that bow upgrade will remain active
        private const int DURATION = 15;

        public event EventHandler<BowArgs> OnBowUpgrade;
        public PowerUpBow(Texture2D texture) : base(texture)
        {
            // override cost for this power-up
            cost = 200;
        }
        public override void Picked()
        {
            OnBowUpgrade(this, new BowArgs(DURATION, cost));
        }
    }
}
