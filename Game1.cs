using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01.StateGame;
using Monsterfall_01.Engine.StateManager;

namespace Monsterfall_01
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        FSM fsm;
        // States that user can be in the game
        enum States
        {
            MENU,
            PLAYING,
            DIED,
            SUCCESS
        }
        private States currentState;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1020;
            //_graphics.PreferredBackBufferHeight = 1080;
            //_graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            fsm = new FSM(this);

            StateGameMenu stateGameMenu = new StateGameMenu(this);
            StateGamePlay stateGamePlay = new StateGamePlay(this);
            StateGameFinish stateGameFinish = new StateGameFinish(this);
            StateGameDied stateGameDied = new StateGameDied(this);

            stateGameMenu.GameStart += (object sender, EventArgs e) => currentState = States.PLAYING;
            stateGamePlay.PlayerDied += (object sender, EventArgs e) => currentState = States.DIED;
            //stateGamePlay.PlayerSucceded += (object sender, EventArgs e) => currentState = States.SUCCESS;
            // Transitions from died
            stateGameDied.PlayAgain += (object sender, EventArgs e) => currentState = States.PLAYING;
            stateGameDied.BackToMenu += (object sender, EventArgs e) => currentState = States.MENU;
            // Transitions from game success
            stateGameFinish.PlayAgain += (object sender, EventArgs e) => currentState = States.PLAYING;
            stateGameFinish.BackToMenu += (object sender, EventArgs e) => currentState = States.MENU;

            stateGameMenu.AddTransition(new Transition(stateGamePlay, () => currentState == States.PLAYING));
            stateGamePlay.AddTransition(new Transition(stateGameDied, () => currentState == States.DIED));
            stateGameDied.AddTransition(new Transition(stateGamePlay, () => currentState == States.PLAYING));
            stateGameDied.AddTransition(new Transition(stateGameMenu, () => currentState == States.MENU));
            stateGameFinish.AddTransition(new Transition(stateGamePlay, () => currentState == States.PLAYING));
            stateGameFinish.AddTransition(new Transition(stateGameMenu, () => currentState == States.MENU));

            fsm.AddState(stateGameMenu);
            fsm.AddState(stateGamePlay);
            fsm.AddState(stateGameDied);
            fsm.AddState(stateGameFinish);

            fsm.Initialise("Menu");
            currentState = States.MENU;

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
