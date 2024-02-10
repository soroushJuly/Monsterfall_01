using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01
{
    internal class Tile
    {
        public Texture2D tileTexture;
        private Vector2 position;

        public Tile() { }
        public Tile(Texture2D tileTexture, Vector2 position)
        {
            this.tileTexture = tileTexture;
            this.position = position;
        }

        public Tile Initialize(Texture2D texture, Vector2 position)
        {
            this.tileTexture = texture;
            this.position = position;

            return this;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileTexture, position, Color.White);
        }
    }
}
