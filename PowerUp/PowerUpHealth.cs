using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    internal class PowerUpHealth : ShopItem
    {
        public event EventHandler AddHealth;
        public PowerUpHealth(Texture2D texture) : base(texture)
        {

        }

        public override void Picked()
        {
            AddHealth(this, EventArgs.Empty);
        }
    }
}
