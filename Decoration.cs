using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01
{
    internal class Decoration
    {
        public readonly Vector2 location;
        public readonly Texture2D texture;


        public Decoration(Vector2 location, Texture2D texture)
        {
            this.location = location;
            this.texture = texture;
        }
    }
}
