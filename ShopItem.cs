using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.Collision;

namespace Monsterfall_01
{
    internal abstract class ShopItem : Tile
    {
        Texture2D texture;
        Vector2 position;
        const float SCALE = 0.7f;

        protected int cost;
        bool isInRange;
        public int GetCost() { return cost; }
        public ShopItem(Texture2D texture) 
        {
            this.texture = texture;
            this.cost = 0;
            isInRange = false;
        }

        public void Initialize(Vector2 position)
        {
            this.position = position;
            this.box = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * SCALE / 1.5f), (int)(texture.Height * SCALE));
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            //DrawBoundingBox(spriteBatch, graphicsDevice);
            Color color = Color.Gray;
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

        public virtual void Picked()
        {
            
        }
    }
}
