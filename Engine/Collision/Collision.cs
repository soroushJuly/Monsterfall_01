using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsterfall_01.Engine.Collision
{
    internal class Collision
    {
        private Collidable A;
        private Collidable B;
        public Collision(Collidable a, Collidable b)
        {
            A = a;
            B = b;
        }
        public bool Equals(Collision other)
        {
            if (other == null) return false;

            if (A.Equals(other.A) && B.Equals(other.B))
            {
                return true;
            }

            return false;
        }
        public void Resolve()
        {
            A.OnCollision(B);
        }
    }
}
