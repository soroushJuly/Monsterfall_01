using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.Collision;

namespace Monsterfall_01
{
    internal abstract class ShopItem : Tile
    {
        // Texture of the power up
        private Texture2D texture;
        // Scale the size of the texture
        const float SCALE = 0.7f;
        // Position of the power up in map
        private Vector2 position;

        // the cost to buy the power Up
        protected int cost;
        // is the player in range to buy the power-up
        private bool isInRange;
        public int GetCost() { return cost; }
        public ShopItem(Texture2D texture) 
        {
            this.texture = texture;
            cost = 0;
            isInRange = false;
        }

        public void Initialize(Vector2 position)
        {
            this.position = position;
            // Collision box for the power-up
            this.box = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * SCALE / 1.5f), (int)(texture.Height * SCALE));
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice = null)
        {
            Color color = Color.Gray;
            // If it's active it's lighten up
            if (isInRange) { color = Color.White; }
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * SCALE), (int)(texture.Height * SCALE)), color);
            spriteBatch.DrawString(font, cost.ToString(), new Vector2((int)position.X + (int)(texture.Width * SCALE / 2) - 15, (int)position.Y + (int)(texture.Height * SCALE) - 15), color);
        }

        public override bool CollisionTest(Collidable obj)
        {
            if (this.Intersects(obj))
            {
                return true;
            }
            isInRange = false;
            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            Player player = obj as Player;
            if (player != null)
            {
                isInRange = true;
            }
        }
        // This function determines what to do when item is picked
        public virtual void Picked()
        {
        }
    }
}
