using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;
using Monsterfall_01.Input;
using System;
using System.Collections.Generic;

namespace Monsterfall_01.StateGame
{
    internal class StateGameMenu : State
    {
        Game Game;
        private SpriteBatch _spriteBatch;
        private ContentManager Content;

        // Input manager
        InputCommandManager inputCommandManager;

        private Texture2D mainBackground;
        private List<Button> buttonList;
        private Texture2D panel;
        private int panelWidth;
        private int panelHeight;

        int currentButtonIndex;

        public event EventHandler GameStart;
        public StateGameMenu(Game game)
        {
            Name = "Menu";
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
            currentButtonIndex = 0;
            UnloadContent();
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Update();
        }
        public override void Draw(object owner, GameTime gameTime)
        {
            Draw();
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
        private void HandleButtonSelection()
        {
            switch (currentButtonIndex)
            {
                case 0:
                    // Throw Start game Event
                    GameStart(this, EventArgs.Empty);
                    break;
                case 1:
                    // code block
                    break;
                case 2:
                    Game.Exit();
                    break;
                default:
                    // code block
                    break;
            }
        }
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

        private void Update()
        {
            inputCommandManager.Update();
            
        }
        private void UnloadContent()
        {
            Content.Unload();
        }

        void LoadContent()
        {
            Texture2D buttonIndicator = Content.Load<Texture2D>("Graphics\\UI\\arrowBeige_right");
            // Load panel texture
            panel = Content.Load<Texture2D>("Graphics\\UI\\panel_stone");
            SpriteFont font = Content.Load<SpriteFont>("Graphics\\gameFont");
            mainBackground = Content.Load<Texture2D>("Graphics\\bg_menu");

            panelWidth = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Width * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Height * 0.6f), 400, 650);

            int contentStartingPositionX = (Game.GraphicsDevice.Viewport.Width) / 2 - 150;

            buttonList.Add(new Button("Start", buttonIndicator, new Vector2(contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 - 30), font));
            buttonList.Add(new Button("Options", buttonIndicator, new Vector2(contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 + 20 ), font));
            buttonList.Add(new Button("Exit", buttonIndicator, new Vector2(contentStartingPositionX, (Game.GraphicsDevice.Viewport.Height) / 2 + 70), font));

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
            _spriteBatch.Draw(mainBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, mainBackground.Width, mainBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);

            foreach (var button in buttonList)
            {
                button.Draw(_spriteBatch);
            }

            _spriteBatch.End();
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
