﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;


namespace Monsterfall_01.StatesMenu
{
    internal class StatesMenuMain : State
    {
        private ButtonList ButtonList;
        public event EventHandler GameStart;
        public event EventHandler HighScores;
        public event EventHandler Controls;
        public event EventHandler ExitGame;
        public StatesMenuMain(int offestX, int offsetY, SpriteFont font, Texture2D buttonIndicator)
        {
            Name = "Main";
            ButtonList = new ButtonList(buttonIndicator, offestX, offsetY, font, 50);
        }
        public override void Enter(object owner)
        {
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
            ButtonList.AddButton("Start");
            ButtonList.AddButton("High Scores");
            ButtonList.AddButton("Controls");
            ButtonList.AddButton("Exit");
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
                case "Start":
                    GameStart(this, EventArgs.Empty);
                    break;
                case "High Scores":
                    HighScores(this, EventArgs.Empty);
                    break;
                case "Controls":
                    Controls(this, EventArgs.Empty);
                    break;
                case "Exit":
                    ExitGame(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }
    }
}
