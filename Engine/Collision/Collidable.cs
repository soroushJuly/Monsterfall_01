using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Monsterfall_01.Engine.Collision
{
    public class Collidable
    {
        // Bounding Box
        protected Rectangle box = new Rectangle();
        // False if we decide for object to not be collidable 
        protected bool isCollidable = true;
        public bool flagForRemoval = false;
        public Rectangle GetBox() { return box; }
        // Draw bounding box for debugging purposes
        protected void DrawBoundingBox(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            Texture2D pixel = new Texture2D(GraphicsDevices, 1, 1);
            pixel.SetData(new Color[] { Color.White });
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
