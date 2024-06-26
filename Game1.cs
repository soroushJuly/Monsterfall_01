﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01.StateGame;
using Monsterfall_01.Engine.StateManager;

namespace Monsterfall_01
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

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
        // Stats of the game in progress. e.g. score, timespent, enemies killed ..
        static private GameStats gameStats;
        static public GameStats GetGameStats() { return gameStats; }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            gameStats = new GameStats();
            fsm = new FSM(this);

            // Initialize Four states of the game
            StateGameMenu stateGameMenu = new StateGameMenu(this);
            StateGamePlay stateGamePlay = new StateGamePlay(this);
            StateGameFinish stateGameFinish = new StateGameFinish(this);
            StateGameDied stateGameDied = new StateGameDied(this);

            // Setting up listeners to change the state of the game
            stateGameMenu.GameStart += (object sender, EventArgs e) => currentState = States.PLAYING;
            // Update Game stats after each time player played one game
            stateGamePlay.PlayerDied += (object sender, GameStats e) => { currentState = States.DIED; gameStats = e; };
            stateGamePlay.PlayerSuccess += (object sender, GameStats e) => { currentState = States.SUCCESS; gameStats = e; };
            // Transitions from died
            stateGameDied.PlayAgain += (object sender, EventArgs e) => currentState = States.PLAYING;
            stateGameDied.BackToMenu += (object sender, EventArgs e) => currentState = States.MENU;
            // Transitions from game success
            stateGameFinish.PlayAgain += (object sender, EventArgs e) => currentState = States.PLAYING;
            stateGameFinish.BackToMenu += (object sender, EventArgs e) => currentState = States.MENU;

            stateGameMenu.AddTransition(new Transition(stateGamePlay, () => currentState == States.PLAYING));
            stateGamePlay.AddTransition(new Transition(stateGameDied, () => currentState == States.DIED));
            stateGamePlay.AddTransition(new Transition(stateGameFinish, () => currentState == States.SUCCESS));
            stateGameDied.AddTransition(new Transition(stateGamePlay, () => currentState == States.PLAYING));
            stateGameDied.AddTransition(new Transition(stateGameMenu, () => currentState == States.MENU));
            stateGameFinish.AddTransition(new Transition(stateGamePlay, () => currentState == States.PLAYING));
            stateGameFinish.AddTransition(new Transition(stateGameMenu, () => currentState == States.MENU));

            fsm.AddState(stateGameMenu);
            fsm.AddState(stateGamePlay);
            fsm.AddState(stateGameDied);
            fsm.AddState(stateGameFinish);

            // First state in which player enters is the Main Menu
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
