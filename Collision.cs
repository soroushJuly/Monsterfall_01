using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsterfall_01
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

            if ((this.A.Equals(other.A) && this.B.Equals(other.B)))
            {
                return true;
            }

            return false;
        }
        public void Resolve()
        {
            this.A.OnCollision(this.B);
        }
    }
}
