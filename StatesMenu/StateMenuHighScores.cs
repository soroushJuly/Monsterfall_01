using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;


namespace Monsterfall_01.StatesMenu
{
    internal class StateMenuHighScores : State
    {
        private ButtonList ButtonList;
        private TextList IndexList;
        private TextList ScoreList;
        // TODO: get player name before playing
        private TextList NameList;
        private TextList TimeList;
        public event EventHandler Back;
        private int offsetX;
        private int offsetY;
        private SpriteFont font;
        private Texture2D buttonIndicator;

        private const int SCORES_COUNT = 5;

        HighScores highScoresTable;
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
            IndexList = new TextList(offsetX, offsetY + 30, font, Color.DarkKhaki, 25);
            ScoreList = new TextList(offsetX + 50, offsetY + 30, font, Color.DarkOliveGreen, 25);

            for(int i = 0; i < SCORES_COUNT; i++)
                IndexList.AddText(i.ToString() + '.');
            LoadMainButtons();
            highScoresTable = new HighScores();
            highScoresTable = HighScores.Load();
            LoadScores();
        }

        private void LoadScores()
        {
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
        }
    }
}
