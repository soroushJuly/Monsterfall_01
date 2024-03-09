using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;


namespace Monsterfall_01.StatesMenu
{
    internal class StateMenuHighScores : State
    {
        private ButtonList ButtonList;
        public event EventHandler Back;
        private int offsetX;
        private int offsetY;
        private SpriteFont font;
        private Texture2D buttonIndicator;
        public StateMenuHighScores(int offestX, int offsetY, SpriteFont font, Texture2D buttonIndicator)
        {
            Name = "High Scores";
            this.offsetX = offestX;
            this.offsetY = offsetY;
            this.font = font;
            this.buttonIndicator = buttonIndicator;
        }
        public override void Enter(object owner)
        {
            ButtonList = new ButtonList(buttonIndicator, offsetX, offsetY, font, 50);
            LoadMainButtons();
        }
        public override void Exit(object owner)
        {
            ButtonList.Clear();
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            ButtonList.Update();
        }
        private void LoadMainButtons()
        {
            ButtonList.AddButton("Back");
            ButtonList.ButtonClicked += this.HandleButtonSelection;
        }
        public override void Draw(object owner, GameTime gameTime, SpriteBatch spriteBatch = null)
        {
            ButtonList.Draw(spriteBatch);
        }
        private void HandleButtonSelection(object sender, Button button)
        {
            switch (button.GetText())
            {
                case "Back":
                    Back(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }
    }
}
