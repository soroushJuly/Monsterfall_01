using System;
using Microsoft.Xna.Framework;

namespace Monsterfall_01
{
    internal class Decoration
    {
        public readonly Point location;
        private String name;


        public Decoration(Point location, String name)
        {
            this.location = location;
            this.name = name;
        }

        public String getName() { return name; }
    }
}
