using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Monsterfall_01
{
    public class Collidable
    {
        protected Rectangle box = new Rectangle();
        protected bool isCollidable = true;
        public Rectangle GetBox() { return box; }
        protected void DrawBoundingBox(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            Texture2D pixel = new Texture2D(GraphicsDevices, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            spriteBatch.Draw(pixel, box, Color.White);
        }

        protected bool Intersects(Collidable collidable)
        {
            if (box.Intersects(collidable.box))
                return true;
            return false;
        }

        public virtual bool CollisionTest(Collidable obj)
        {
            return false;
        }

        public virtual void OnCollision(Collidable obj)
        {
        }
    }
}
