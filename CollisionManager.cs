using System;
using System.Collections.Generic;

namespace Monsterfall_01
{
    internal class CollisionManager
    {
        private HashSet<Collision> m_collisions;
        private List<Collidable> m_collidables;

        public CollisionManager() 
        { 
            m_collisions = new HashSet<Collision>();
            m_collidables = new List<Collidable>();
        }
        public void AddCollidable(Collidable collidable)
        {
            m_collidables.Add(collidable);
        }
        public void Update()
        {
            UpdateCollisions();
            ResolveCollisions();
            RemoveFlaggedCollidables();
        }
        private void UpdateCollisions()
        {
            foreach (Collidable collidable in m_collidables)
            {
                foreach(Collidable otherCollidable in m_collidables)
                {
                    if (collidable.Equals(otherCollidable))
                        continue;
                    if (collidable.CollisionTest(otherCollidable))
                        m_collisions.Add(new Collision(collidable, otherCollidable));
                }
            }
        }
        private void ResolveCollisions()
        {
            foreach (Collision collision in m_collisions)
            {
                collision.Resolve();
            }
            m_collisions = new HashSet<Collision>();
        }
        public void RemoveCollidable(Collidable collidable)
        {
            m_collidables.Remove(collidable);
        }
        private void RemoveFlaggedCollidables()
        {
            List<Collidable> removalList = new List<Collidable>();
            foreach (Collidable collidable in m_collidables)
            {
                if (collidable.flagForRemoval)
                {
                    removalList.Add(collidable);
                }
            }
            foreach (Collidable collidable in removalList)
            {
                m_collidables.Remove(collidable);
            }
        }
    }
}
