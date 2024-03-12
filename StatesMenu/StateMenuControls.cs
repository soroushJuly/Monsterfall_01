using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;


namespace Monsterfall_01.StatesMenu
{
    internal class StateMenuControls : State
    {
        private ButtonList ButtonList;
        // List of Controls text
        private TextList ControlsList;
        // List of texts of Actions related to controls
        private TextList ActionsList;
        private int offsetX;
        private int offsetY;
        private SpriteFont font;
        private Texture2D buttonIndicator;

        private SoundEffectInstance selectSound;

        // Event fired when back button pressed
        public event EventHandler Back;
        public StateMenuControls(int offestX, int offsetY, SpriteFont font, Texture2D buttonIndicator, SoundEffectInstance selectSound)
        {
            Name = "Controls";
            this.offsetX = offestX;
            this.offsetY = offsetY;
            this.font = font;
            this.buttonIndicator = buttonIndicator;
            this.selectSound = selectSound;
        }
        public override void Enter(object owner)
        {
            // Initialize the list of text and buttons
            ButtonList = new ButtonList(buttonIndicator, offsetX, offsetY, font, 50);
            ControlsList = new TextList(offsetX, offsetY + 30, font, Color.DarkOliveGreen, 25);
            ActionsList = new TextList(offsetX + 50, offsetY + 30, font, Color.DarkOliveGreen, 25);
            // Load the contents of lists manually
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
            selectSound.Play();
        }
    }
}
