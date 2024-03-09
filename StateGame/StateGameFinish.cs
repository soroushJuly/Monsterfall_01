using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01.Engine.UI;
using Monsterfall_01.Engine.StateManager;
using System;

namespace Monsterfall_01.StateGame
{
    internal class StateGameFinish : State
    {
        Game Game;
        private SpriteBatch _spriteBatch;
        private ContentManager Content;

        private Texture2D successBackground;
        private Texture2D successText;
        private ButtonList ButtonList;
        private Text yourScoreText;
        private Texture2D panel;
        private int panelWidth;
        private int panelHeight;

        public event EventHandler PlayAgain;
        public event EventHandler BackToMenu;
        public StateGameFinish(Game game)
        {
            Name = "Success";
            Game = game;
            Content = game.Content;
        }
        public override void Enter(object owner)
        {
            Initialize();
            LoadContent();
        }

        public override void Exit(object owner)
        {
            Content.Unload();
            ButtonList.Clear();
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Update();
        }
        public override void Draw(object owner, GameTime gameTime, SpriteBatch spriteBatch = null)
        {
            Draw();
        }
        private void Update()
        {
            ButtonList.Update();
        }
        void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            //// Draw Background
            _spriteBatch.Draw(successBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, successBackground.Width, successBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);
            // Game Over text
            _spriteBatch.Draw(successText, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                Math.Min(successText.Width, panelWidth - 30), 80), Color.White);

            yourScoreText.Draw(_spriteBatch);
            ButtonList.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }
        private void LoadContent()
        {
            Texture2D buttonIndicator = Content.Load<Texture2D>("Graphics\\UI\\arrowBeige_right");
            // Load panel texture
            panel = Content.Load<Texture2D>("Graphics\\UI\\panel_stone");
            SpriteFont font = Content.Load<SpriteFont>("Graphics\\gameFont");
            successBackground = Content.Load<Texture2D>("Graphics\\bg_success");
            successText = Content.Load<Texture2D>("Graphics\\Success");

            panelWidth = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Width * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Height * 0.6f), 400, 650);

            int contentStartingPositionX = (Game.GraphicsDevice.Viewport.Width) / 2 - 150;

            yourScoreText = new Text("Your Score: " + Game1.GetGameStats().score, new Vector2((Game.GraphicsDevice.Viewport.Width) / 2 - 75, (Game.GraphicsDevice.Viewport.Height) / 2 - 30), font, Color.Green);

            ButtonList = new ButtonList(buttonIndicator, contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 + 10, font, 50);
            ButtonList.AddButton("Play Again");
            ButtonList.AddButton("Back to menu");
            ButtonList.AddButton("Exit the game");
            ButtonList.ButtonClicked += HandleButtonSelection;
        }
        private void HandleButtonSelection(object sender, Button button)
        {
            switch (button.GetText())
            {
                case "Play Again":
                    PlayAgain(this, EventArgs.Empty);
                    break;
                case "Back to menu":
                    BackToMenu(this, EventArgs.Empty);
                    break;
                case "Exit the game":
                    Game.Exit();
                    break;
                default:
                    break;
            }
        }
    }
}
