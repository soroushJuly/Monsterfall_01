using System;
using Microsoft.Xna.Framework;


namespace Monsterfall_01
{
    internal class Collidable
    {
        protected Rectangle box = new Rectangle();
        //public BoundingBox BoundingBox
        //{
        //    get { return boundingBox; }
        //}

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
