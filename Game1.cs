using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01.StateManager;
using Monsterfall_01.StateGame;

namespace Monsterfall_01
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        FSM fsm;
        // Is the player playing
        private bool isPlaying;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1020;
            //_graphics.PreferredBackBufferHeight = 1080;
            //_graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Initially player is the menu
            isPlaying = false;
        }

        protected override void Initialize()
        {
            fsm = new FSM(this);

            StateGameMenu stateGameMenu = new StateGameMenu(this);
            StateGamePlay stateGamePlay = new StateGamePlay(this);
            StateGameFinish stateGameFinish = new StateGameFinish(this);

            stateGameMenu.GameStart += (object sender, EventArgs e) => isPlaying = true;

            stateGameMenu.AddTransition(new Transition(stateGamePlay, () => isPlaying));

            fsm.AddState(stateGameMenu);
            fsm.AddState(stateGamePlay);
            fsm.AddState(stateGameFinish);

            fsm.Initialise("Menu");

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            fsm.Update(gameTime);
            base.Update(gameTime);
        }       
        protected override void Draw(GameTime gameTime)
        {
            fsm.Draw(gameTime);
            base.Draw(gameTime);
        }
        protected override void UnloadContent()
        {
        }
    }
}
