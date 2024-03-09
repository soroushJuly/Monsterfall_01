using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;

namespace Monsterfall_01.Engine.UI
{
    // TODO: this will go to the UI Folder in Engine Folder
    // TODO: a component for selection of a button in a list
    internal class Text
    {
        private string text;
        Vector2 position;

        // The font used to display UI elements  
        SpriteFont font;
        // Text Color
        Color color;

        // TODO: use these variables later to have fixed size buttons
        int width;
        int height;

        public Text(string text, Vector2 position, SpriteFont font, Color color)
        {
            this.position = position;
            this.text = text;
            this.font = font;
            this.color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y), color,
                0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1.0f);
        }

    }
}
