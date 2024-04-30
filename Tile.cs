using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.Collision;

namespace Monsterfall_01
{
    internal class Tile : Collidable
    {
        public Texture2D tileTexture;
        private Vector2 position;
        public Vector2 GetPosition() { return position; }
        public Tile() { this.isCollidable = false; }
        public Tile(Texture2D tileTexture, Vector2 position, bool isCollidable = true, float boxScaleX = 1f, float boxScaleY = 1f, int xOffset = 0, int yOffset= 0)
        {
            this.tileTexture = tileTexture;
            this.position = position;
            if (!isCollidable) { 
                this.isCollidable = false;
                return;
            }
            this.box = new Rectangle((int)(position.X + xOffset), (int)(position.Y + yOffset), (int)(tileTexture.Width * boxScaleX), (int)(tileTexture.Height * boxScaleY));
        }

        public Tile Initialize(Texture2D texture, Vector2 position)
        {
            this.tileTexture = texture;
            this.position = position;

            return this;
        }
        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            spriteBatch.Draw(tileTexture, position, Color.White);
        }
    }
}
