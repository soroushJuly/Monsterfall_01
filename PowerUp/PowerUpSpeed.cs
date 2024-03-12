using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01.PowerUp
{
    internal class PowerUpSpeed : ShopItem
    {
        // Arguments sent with the bow upgrade event
        public class SpeedUpArgs : EventArgs
        {
            public SpeedUpArgs(float speedUpIntensity, float duration, int cost)
            {
                this.speedUpIntensity = speedUpIntensity;
                this.duration = duration;
                this.cost = cost;
            }

            public readonly float speedUpIntensity;
            public readonly float duration;
            public readonly int cost;
        }
        // This is how long the effect will last
        private const float DURATION = 5.0f;
        // Player speed will be multiplied by this
        private const float INTENSITY = 2.0f;

        public event EventHandler<SpeedUpArgs> OnSpeedUp;
        public PowerUpSpeed(Texture2D texture) : base(texture)
        {
            cost = 100;
        }
        public override void Picked()
        {

            OnSpeedUp(this, new SpeedUpArgs(INTENSITY, DURATION, cost));
        }
    }
}
