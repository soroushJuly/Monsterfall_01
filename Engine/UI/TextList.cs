using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Monsterfall_01.Engine.Input;

namespace Monsterfall_01.Engine.UI
{
    internal class TextList
    {
        private List<Text> textList;
        private SpriteFont font;
        private Color color;
        private int offsetX;
        private int offsetY;
        private int count;
        private int paddings;

        int currentButtonIndex;

        private InputCommandManager inputCommandManager;
        public event EventHandler<Button> ButtonClicked;
        public TextList(int offsetX, int offsetY, SpriteFont font, Color color, int paddings)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.font = font;
            this.color = color;
            this.paddings = paddings;

            textList = new List<Text>();
        }

        public void AddText(string text)
        {
            textList.Add(new Text(text, new Vector2(offsetX, offsetY + textList.Count * paddings), font, color));
        }
        public void Update()
        {
            inputCommandManager?.Update();
        }
        public void Clear()
        {
            textList.Clear();
        }
        public int GetCount() { return textList.Count; }
        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (var button in textList)
            {
                button.Draw(_spriteBatch);
            }
        }

    }
}
