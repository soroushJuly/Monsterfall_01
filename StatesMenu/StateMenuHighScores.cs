using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;


namespace Monsterfall_01.StatesMenu
{
    internal class StateMenuHighScores : State
    {
        ButtonList ButtonList;
        // List of the rankings
        TextList IndexList;
        TextList ScoreList;
        // TODO: in future get player name before playing
        TextList NameList;
        // TODO: in future show time spent to finish the game
        TextList TimeList;
        int offsetX;
        int offsetY;
        SpriteFont font;
        Texture2D buttonIndicator;

        // Number of the high scores to show
        const int SCORES_COUNT = 5;

        SoundEffectInstance selectSound;

        // Table of the high score
        HighScores highScoresTable;
        // Event fired when back button pressed
        public event EventHandler Back;
        public StateMenuHighScores(int offestX, int offsetY, SpriteFont font, Texture2D buttonIndicator, SoundEffectInstance selectSound)
        {
            Name = "High Scores";
            this.offsetX = offestX;
            this.offsetY = offsetY;
            this.font = font;
            this.buttonIndicator = buttonIndicator;
            this.selectSound = selectSound;
        }
        public override void Enter(object owner)
        {
            // Initialize the lists of the buttons and text lists
            ButtonList = new ButtonList(buttonIndicator, offsetX, offsetY, font, 50);
            IndexList = new TextList(offsetX, offsetY + 30, font, Color.DarkKhaki, 25);
            ScoreList = new TextList(offsetX + 50, offsetY + 30, font, Color.DarkOliveGreen, 25);

            // Initialize indices
            for (int i = 0; i < SCORES_COUNT; i++)
                IndexList.AddText((i + 1).ToString() + '.');
            // Load buttons
            LoadMainButtons();
            // Load the high score list from the file
            highScoresTable = new HighScores();
            highScoresTable = HighScores.Load();
            LoadScores();
        }

        private void LoadScores()
        {
            // Seperate the number of top players according to SCORES_COUNT
            for (int i = 0; i < SCORES_COUNT; i++)
            {
                if (i < highScoresTable.Scores.Count)
                    ScoreList.AddText(highScoresTable.Scores[i]?.score.ToString());
            }
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
            IndexList.Draw(spriteBatch);
            ScoreList.Draw(spriteBatch);
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
