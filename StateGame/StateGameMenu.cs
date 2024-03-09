using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monsterfall_01.Engine.Input;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;
//using Monsterfall_01.Game.UI;
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
        private ButtonList ButtonList;
        private Texture2D panel;
        private int panelWidth;
        private int panelHeight;


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
            
            inputCommandManager = new InputCommandManager();

            //inputCommandManager.AddKeyboardBinding(Keys.Up, OnKeyUp);
            //inputCommandManager.AddKeyboardBinding(Keys.Down, OnKeyDown);
            inputCommandManager.AddKeyboardBinding(Keys.Escape, OnExit);
            //inputCommandManager.AddKeyboardBinding(Keys.Enter, OnSelect);
        }
        private void HandleButtonSelection(object sender, Button button)
        {
            switch (button.GetText())
            {
                case "Start":
                    // Throw Start game Event
                    GameStart(this, EventArgs.Empty);
                    break;
                case "High Scores":
                    // High scores
                    break;
                case "Controls":
                    // controls
                case "Exit":
                    Game.Exit();
                    break;
                default:
                    // code block
                    break;
            }
        }

        private void Update()
        {
            inputCommandManager.Update();
            ButtonList.Update();
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
            int contentStartingPositionY = (Game.GraphicsDevice.Viewport.Height) / 2 - 50;

            // Main Menu button list
            ButtonList = new ButtonList(buttonIndicator, contentStartingPositionX, contentStartingPositionY, font, 50);
            ButtonList.AddButton("Start");
            ButtonList.AddButton("High Scores");
            ButtonList.AddButton("Controls");
            ButtonList.AddButton("Exit");
            ButtonList.ButtonClicked += this.HandleButtonSelection;
            // High scores text list

            // Controls texts list
        }
        void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            // Draw Background
            _spriteBatch.Draw(mainBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, mainBackground.Width, mainBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);

            ButtonList.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        #region INPUT HANDLERS
        private void OnExit(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                Game.Exit();
        }
        #endregion
    }
}
