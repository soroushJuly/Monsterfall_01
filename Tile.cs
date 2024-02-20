using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01
{
    internal class Tile : Collidable
    {
        public Texture2D tileTexture;
        private Vector2 position;

        public Tile() { this.isCollidable = false; }
        public Tile(Texture2D tileTexture, Vector2 position, bool isCollidable)
        {
            this.tileTexture = tileTexture;
            this.position = position;
            if (!isCollidable) { 
                this.isCollidable = false; 
            }
            this.box = new Rectangle((int)(position.X), (int)(position.Y), tileTexture.Width, tileTexture.Height);
        }

        public Tile Initialize(Texture2D texture, Vector2 position)
        {
            this.tileTexture = texture;
            this.position = position;

            return this;
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            if (isCollidable)
            {
                Texture2D pixel = new Texture2D(GraphicsDevices, 1, 1);
                pixel.SetData<Color>(new Color[] { Color.Red });
                spriteBatch.Draw(pixel, this.box, Color.White);
            }
            spriteBatch.Draw(tileTexture, position, Color.White);
        }
    }
}
