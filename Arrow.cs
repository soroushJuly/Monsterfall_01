using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

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
        // Origin of the rotation
        private Vector2 origin;
        float SCALE = 0.25f;
        // The damage the laser deals.  
        public int Damage = 10;
        // set the laser to active  
        public bool Active;
        private int direction;
        private double angle;
        // Laser beams range.  
        int Range;

        public Arrow(Vector2 position, int directionIndex)
        {
            this.Position = position;
            this.direction = directionIndex;
            this.angle = (directionIndex * (Math.PI / 4));
            this.arrowMoveSpeed = 7f;
            origin.X = position.X - SCALE * Game1.arrowTexture.Width / 2;
            origin.Y = position.Y + SCALE * Game1.arrowTexture.Height / 2;
            origin = new Vector2(-Game1.player.Width / 2 * (float)Math.Sin(angle), +Game1.player.Height / 2 * (float)Math.Cos(angle));
            origin = Vector2.Zero;
            //origin = position;
        }
        public void Update(GameTime gameTime)
        {
            this.box = new Rectangle((int)(Position.X), 
                (int)(Position.Y ), 10, 10);
            Position.X += (float)(arrowMoveSpeed * Math.Sin(angle));
            Position.Y -= (float)(arrowMoveSpeed * Math.Cos(angle));
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, (int)(SCALE * Game1.arrowTexture.Width), (int)(SCALE * Game1.arrowTexture.Height));
            Rectangle src = new Rectangle(0,0, Game1.arrowTexture.Width, Game1.arrowTexture.Height);
            //Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, 10, 100);
            DrawBoundingBox(spriteBatch, graphicsDevice);
            spriteBatch.Draw(Game1.arrowTexture, dest, src, Color.White, (float)angle, origin, SpriteEffects.None, 0f);
        }
    }
}
