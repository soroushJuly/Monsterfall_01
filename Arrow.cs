using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Monsterfall_01
{
    public class Arrow : Collidable
    {
        // laser animation.  
        public Texture2D arrowTexture;
        // the speed that arrow travels  
        float arrowMoveSpeed;
        // position of the laser  
        public Vector2 Position;
        float SCALE = 0.2f;
        // The damage the laser deals.  
        public int Damage = 10;
        // set the laser to active  
        public bool Active;
        // Laser beams range.  
        int Range;

        public Arrow(Vector2 position)
        {
            this.Position = position;
            this.arrowMoveSpeed = 3f;

        }
        public void Update(GameTime gameTime)
        {
            Position.X += arrowMoveSpeed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, (int)(SCALE * Game1.arrowTexture.Width), (int)(SCALE * Game1.arrowTexture.Height));
            //Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, 10, 100);
            spriteBatch.Draw(Game1.arrowTexture, dest, Color.White);
        }
    }
}
