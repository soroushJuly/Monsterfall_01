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
        ContentManager Content;

        // You succeed background texture
        Texture2D successBackground;
        // success text texture
        Texture2D successText;
        ButtonList ButtonList;
        Text yourScoreText;
        // Panel data
        Texture2D panel;
        int panelWidth;
        int panelHeight;

        int windowWidth;
        int windowHeight;

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
            // Draw Background
            _spriteBatch.Draw(successBackground, new Rectangle(0, 0, windowWidth, windowHeight),
                new Rectangle(0, 0, successBackground.Width, successBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (windowWidth - panelWidth) / 2, (windowHeight - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);
            // Game Over text
            _spriteBatch.Draw(successText, new Rectangle(
                (windowWidth - panelWidth) / 2, (windowHeight - panelHeight) / 2,
                Math.Min(successText.Width, panelWidth - 30), 80), Color.White);

            yourScoreText.Draw(_spriteBatch);
            ButtonList.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            windowWidth = Game.GraphicsDevice.Viewport.Width;
            windowHeight = Game.GraphicsDevice.Viewport.Height;
        }
        private void LoadContent()
        {
            // Initilize and load the button indicator
            Texture2D buttonIndicator = Content.Load<Texture2D>("Graphics\\UI\\arrowBeige_right");
            // Load fonts
            SpriteFont font = Content.Load<SpriteFont>("Graphics\\gameFont");
            // Load panel texture
            panel = Content.Load<Texture2D>("Graphics\\UI\\panel_stone");
            successBackground = Content.Load<Texture2D>("Graphics\\bg_success");
            successText = Content.Load<Texture2D>("Graphics\\Success");

            panelWidth = Math.Clamp((int)(windowWidth * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(windowHeight * 0.6f), 400, 650);

            int contentStartingPositionX = windowWidth / 2 - 150;
            // creating your score text
            yourScoreText = new Text("Your Score: " + Game1.GetGameStats().score, new Vector2(windowWidth / 2 - 75, windowHeight / 2 - 30), font, Color.Green);

            ButtonList = new ButtonList(buttonIndicator, contentStartingPositionX, windowHeight / 2 + 10, font, 50);
            ButtonList.AddButton("Play Again");
            ButtonList.AddButton("Back to menu");
            ButtonList.AddButton("Exit the game");
            // Handling the selection in the list
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
