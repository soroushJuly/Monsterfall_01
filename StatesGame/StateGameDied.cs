using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.UI;
using Monsterfall_01.Engine.StateManager;
using System;

namespace Monsterfall_01.StateGame
{
    internal class StateGameDied : State
    {
        Game Game;
        SpriteBatch _spriteBatch;
        ContentManager Content;

        // You died background texture
        Texture2D diedBackground;
        // Game over text texture
        Texture2D gameOverText;
        ButtonList ButtonList;
        Text yourScoreText;
        // Panel is includes the button list
        Texture2D panel;
        int panelWidth;
        int panelHeight;

        int windowWidth;
        int windowHeight;

        public event EventHandler PlayAgain;
        public event EventHandler BackToMenu;
        public StateGameDied(Game game)
        {
            Name = "Died";
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

        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            windowWidth = Game.GraphicsDevice.Viewport.Width;
            windowHeight = Game.GraphicsDevice.Viewport.Height;
        }
        private void LoadContent()
        {
            // Load the texture for button indicator
            Texture2D buttonIndicator = Content.Load<Texture2D>("Graphics\\UI\\arrowBeige_right");
            // Load font
            SpriteFont font = Content.Load<SpriteFont>("Graphics\\gameFont");
            // Load state textures
            panel = Content.Load<Texture2D>("Graphics\\UI\\panel_stone");
            diedBackground = Content.Load<Texture2D>("Graphics\\BloodOverlay");
            gameOverText = Content.Load<Texture2D>("Graphics\\GameOver");

            panelWidth = Math.Clamp((int)(windowWidth * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(windowHeight * 0.6f), 400, 650);

            int contentStartingPositionX = (windowWidth) / 2 - 150;
            // creating your score text
            yourScoreText = new Text("Your Score: " + Game1.GetGameStats().score,
                new Vector2(windowWidth / 2 - 75, windowHeight / 2 - 30), font, Color.Red);
            // Creating list of button options 
            ButtonList = new ButtonList(buttonIndicator, contentStartingPositionX, windowHeight / 2 + 10, font, 50);
            ButtonList.AddButton("Try Again");
            ButtonList.AddButton("Back to menu");
            ButtonList.AddButton("Exit the game");
            // Handling the selection in the list
            ButtonList.ButtonClicked += HandleButtonSelection;
        }
        private void HandleButtonSelection(object sender, Button button)
        {
            switch (button.GetText())
            {
                case "Try Again":
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
        void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            // Draw Background
            _spriteBatch.Draw(diedBackground, new Rectangle(0, 0, windowWidth, windowHeight),
                new Rectangle(0, 0, diedBackground.Width, diedBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (windowWidth - panelWidth) / 2, (windowHeight - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);
            // Game Over text
            _spriteBatch.Draw(gameOverText, new Rectangle(
                (windowWidth - panelWidth) / 2, (windowHeight - panelHeight) / 2,
                Math.Min(gameOverText.Width, panelWidth), 90), Color.White);

            yourScoreText.Draw(_spriteBatch);

            ButtonList.Draw(_spriteBatch);

            _spriteBatch.End();
        }
    }
}
