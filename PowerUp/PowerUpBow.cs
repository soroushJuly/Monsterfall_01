using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    internal class PowerUpBow : ShopItem
    {
        public event EventHandler<int> OnBowUpgrade;
        public PowerUpBow(Texture2D texture) : base(texture)
        {

        }
        public override void Picked()
        {
            const int DURATION = 20;
            OnBowUpgrade(this, DURATION);
        }
    }
}
