using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;


namespace Monsterfall_01.StatesMenu
{
    internal class StateMenuControls : State
    {
        private ButtonList ButtonList;
        private TextList ControlsList;
        private TextList ActionsList;
        public event EventHandler Back;
        private int offsetX;
        private int offsetY;
        private SpriteFont font;
        private Texture2D buttonIndicator;
        public StateMenuControls(int offestX, int offsetY, SpriteFont font, Texture2D buttonIndicator)
        {
            Name = "Controls";
            this.offsetX = offestX;
            this.offsetY = offsetY;
            this.font = font;
            this.buttonIndicator = buttonIndicator;
        }
        public override void Enter(object owner)
        {
            ButtonList = new ButtonList(buttonIndicator, offsetX, offsetY, font, 50);
            ControlsList = new TextList(offsetX, offsetY + 30, font, Color.DarkOliveGreen, 25);
            ActionsList = new TextList(offsetX + 50, offsetY + 30, font, Color.DarkOliveGreen, 25);
            LoadMainButtons();
            LoadControlsText();
            LoadActionsText();
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
        private void LoadControlsText()
        {
            ControlsList.AddText("W:");
            ControlsList.AddText("S:");
            ControlsList.AddText("A:");
            ControlsList.AddText("D:");
            ControlsList.AddText("J:");
            ControlsList.AddText("E:");
        }
        private void LoadActionsText()
        {
            ActionsList.AddText("Move North");
            ActionsList.AddText("Move South");
            ActionsList.AddText("Move West");
            ActionsList.AddText("Move East");
            ActionsList.AddText("Shoot Arrow");
            ActionsList.AddText("Interact/Buy");
        }
        public override void Draw(object owner, GameTime gameTime, SpriteBatch spriteBatch = null)
        {
            ButtonList.Draw(spriteBatch);
            ControlsList.Draw(spriteBatch);
            ActionsList.Draw(spriteBatch);
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
