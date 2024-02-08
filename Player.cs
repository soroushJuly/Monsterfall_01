using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using SharpDX.MediaFoundation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Monsterfall_01
{
    internal class Player
    {
        public Animation playerAnimation;
        public float movementSpeed;
       
        public Vector2 position;
        public bool isActive;
        public int Health;

        public int Width
        { get { return playerAnimation.frameWidth; } }
        public int Height
        { get { return playerAnimation.frameHeight; } }

        public void Initialize(Animation playerAnimation, Vector2 position)
        {
            movementSpeed = 1.0f;

            this.position = position;
            this.playerAnimation = playerAnimation;

            Health = 100;

            isActive = true;
        }
        
        public void Update(GameTime gameTime)
        {
            playerAnimation.Position = position;
            playerAnimation.Update(gameTime);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            playerAnimation.Draw(spriteBatch);
        }

    }
}
