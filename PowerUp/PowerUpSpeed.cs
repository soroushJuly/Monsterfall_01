using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    internal class PowerUpSpeed : ShopItem
    {
        public class SpeedUpArgs : EventArgs
        {
            public SpeedUpArgs(float speedUpIntensity, float duration)
            {
                this.speedUpIntensity = speedUpIntensity;
                this.duration = duration;
            }

            public readonly float speedUpIntensity;
            public readonly float duration;
        }
        public event EventHandler<SpeedUpArgs> OnSpeedUp;
        public PowerUpSpeed(Texture2D texture) : base(texture)
        {
         
        }
        public override void Picked()
        {
            OnSpeedUp(this, new SpeedUpArgs(2.0f, 5.0f));
        }
    }
}
