using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monsterfall_01.Engine.Input;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.UI;
using Monsterfall_01.StatesMenu;

//using Monsterfall_01.Game.UI;
using System;

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
        private Texture2D panel;
        private int panelWidth;
        private int panelHeight;

        int contentStartingPositionX;
        int contentStartingPositionY;

        FSM fsm;

        enum States
        {
            MAIN,
            HIGHSCORES,
            CONTROLS
        }
        private States currentState;


        public event EventHandler GameStart;
        public StateGameMenu(Game game)
        {
            Name = "Menu";
            Game = game;
            Content = game.Content;
            currentState = States.MAIN;

            contentStartingPositionX = (Game.GraphicsDevice.Viewport.Width) / 2 - 150;
            contentStartingPositionY = (Game.GraphicsDevice.Viewport.Height) / 2 - 50;

            fsm = new FSM(this);
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
            Update(gameTime);
        }
        public override void Draw(object owner, GameTime gameTime, SpriteBatch spriteBatch = null)
        {
            Draw(gameTime);
        }
        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            
            inputCommandManager = new InputCommandManager();
        }
        private void Update(GameTime gameTime)
        {
            inputCommandManager.Update();
            fsm.Update(gameTime);
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
            StatesMenuMain statesMenuMain = new StatesMenuMain(contentStartingPositionX, contentStartingPositionY, font, buttonIndicator);
            StateMenuHighScores statesMenuHighScores = new StateMenuHighScores(contentStartingPositionX, contentStartingPositionY, font, buttonIndicator);
            StateMenuControls statesMenuControls = new StateMenuControls(contentStartingPositionX, contentStartingPositionY, font, buttonIndicator);

            statesMenuMain.HighScores += (object sender, EventArgs e) => currentState = States.HIGHSCORES;
            statesMenuMain.Controls += (object sender, EventArgs e) => currentState = States.CONTROLS;
            statesMenuMain.ExitGame += (object sender, EventArgs e) => Game.Exit();

            statesMenuHighScores.Back += (object sender, EventArgs e) => currentState = States.MAIN;
            statesMenuControls.Back += (object sender, EventArgs e) => currentState = States.MAIN;

            statesMenuMain.AddTransition(new Transition(statesMenuHighScores, () => currentState == States.HIGHSCORES));
            statesMenuMain.AddTransition(new Transition(statesMenuControls, () => currentState == States.CONTROLS));

            statesMenuHighScores.AddTransition(new Transition(statesMenuMain, () => currentState == States.MAIN));
            statesMenuControls.AddTransition(new Transition(statesMenuMain, () => currentState == States.MAIN));


            fsm.AddState(statesMenuMain);
            fsm.AddState(statesMenuControls);
            fsm.AddState(statesMenuHighScores);

            fsm.Initialise("Main");
        }

        void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            // Draw Background
            _spriteBatch.Draw(mainBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, mainBackground.Width, mainBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);

            fsm.Draw(gameTime, _spriteBatch);

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
