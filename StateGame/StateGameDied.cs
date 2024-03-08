using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Monsterfall_01.Engine.UI;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Input;
using System;
using System.Collections.Generic;

namespace Monsterfall_01.StateGame
{
    internal class StateGameDied : State
    {
        Game Game;
        private SpriteBatch _spriteBatch;
        private ContentManager Content;

        // Input manager
        InputCommandManager inputCommandManager;

        private Texture2D diedBackground;
        private Texture2D gameOverText;
        private List<Button> buttonList;
        private Texture2D panel;
        private int panelWidth;
        private int panelHeight;

        int currentButtonIndex;

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
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Update();
        }
        public override void Draw(object owner, GameTime gameTime)
        {
            Draw();
        }
        private void Update()
        {
            inputCommandManager.Update();
        }
        void Draw()
        {
            // check active (hovered) button
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (i == currentButtonIndex)
                    buttonList[i].updateHovered(true);
                else
                    buttonList[i].updateHovered(false);
            }

            _spriteBatch.Begin(SpriteSortMode.Deferred);
            // Draw Background
            _spriteBatch.Draw(diedBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, diedBackground.Width, diedBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);
            // Game Over text
            _spriteBatch.Draw(gameOverText, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                Math.Min(gameOverText.Width, panelWidth), 90), Color.White);

            foreach (var button in buttonList)
            {
                button.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            buttonList = new List<Button>();
            inputCommandManager = new InputCommandManager();

            currentButtonIndex = 0;

            inputCommandManager.AddKeyboardBinding(Keys.Up, OnKeyUp);
            inputCommandManager.AddKeyboardBinding(Keys.Down, OnKeyDown);
            inputCommandManager.AddKeyboardBinding(Keys.Escape, OnExit);
            inputCommandManager.AddKeyboardBinding(Keys.Enter, OnSelect);
        }
        private void LoadContent()
        {
            Texture2D buttonIndicator = Content.Load<Texture2D>("Graphics\\UI\\arrowBeige_right");
            // Load panel texture
            panel = Content.Load<Texture2D>("Graphics\\UI\\panel_stone");
            SpriteFont font = Content.Load<SpriteFont>("Graphics\\gameFont");
            diedBackground = Content.Load<Texture2D>("Graphics\\BloodOverlay");
            gameOverText = Content.Load<Texture2D>("Graphics\\GameOver");

            panelWidth = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Width * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Height * 0.6f), 400, 650);

            int contentStartingPositionX = (Game.GraphicsDevice.Viewport.Width) / 2 - 150;

            // TODO: add the text component for the score here:
            buttonList.Add(new Button("Try Again", buttonIndicator, new Vector2(contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 - 30), font));
            buttonList.Add(new Button("Back to menu", buttonIndicator, new Vector2(contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 + 20), font));
            buttonList.Add(new Button("Exit the game", buttonIndicator, new Vector2(contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 + 70), font));
        }
        private void HandleButtonSelection()
        {
            switch (currentButtonIndex)
            {
                case 0:
                    PlayAgain(this, EventArgs.Empty);
                    break;
                case 1:
                    BackToMenu(this, EventArgs.Empty);
                    break;
                case 2:
                    Game.Exit();
                    break;
                default:
                    // code block
                    break;
            }
        }
        // TODO: these logics might go to a different encapsulated class
        private void UpdateCurrentButton(int index)
        {
            currentButtonIndex += index;
            if (currentButtonIndex > buttonList.Count - 1)
            {
                currentButtonIndex = 0;
                return;
            }
            if (currentButtonIndex < 0)
            {
                currentButtonIndex = buttonList.Count - 1;
                return;
            }
        }
        #region INPUT HANDLERS
        private void OnExit(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                Game.Exit();
        }

        private void OnKeyDown(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                UpdateCurrentButton(+1);
        }

        private void OnKeyUp(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                UpdateCurrentButton(-1);
        }
        private void OnSelect(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                HandleButtonSelection();
        }
        #endregion
    }
}
