using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.StatesMenu;

using System;

namespace Monsterfall_01.StateGame
{
    internal class StateGameMenu : State
    {
        Game Game;
        private SpriteBatch _spriteBatch;
        private ContentManager Content;

        private Texture2D mainBackground;
        private Texture2D panel;
        private Texture2D gameName;
        private int panelWidth;
        private int panelHeight;

        int contentStartingPositionX;
        int contentStartingPositionY;

        //Our Laser Sound and Instance  
        private SoundEffect buttonSwitchSound;
        private SoundEffectInstance buttonSwitchSoundInstance;
        
        private SoundEffect buttonSelectSound;
        private SoundEffectInstance buttonSelectSoundInstance;

        // Menu Music 
        private Song menuMusic;

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

            contentStartingPositionX = (Game.GraphicsDevice.Viewport.Width) / 2 - 150;
            contentStartingPositionY = (Game.GraphicsDevice.Viewport.Height) / 2 - 50;
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
        }
        private void Update(GameTime gameTime)
        {
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
            gameName = Content.Load<Texture2D>("Graphics\\Name");

            buttonSwitchSound = Content.Load<SoundEffect>("Sound\\buttonSwitch");
            buttonSwitchSoundInstance = buttonSwitchSound.CreateInstance();
            buttonSelectSound = Content.Load<SoundEffect>("Sound\\buttonSelect");
            buttonSelectSoundInstance = buttonSelectSound.CreateInstance();

            panelWidth = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Width * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(Game.GraphicsDevice.Viewport.Height * 0.6f), 400, 650);

            fsm = new FSM(this);

            // TODO: using sound manager to pass the audio
            StateMenuMain statesMenuMain = new StateMenuMain(contentStartingPositionX, contentStartingPositionY,
                font, buttonIndicator, buttonSwitchSoundInstance, buttonSelectSoundInstance);
            StateMenuHighScores statesMenuHighScores = new StateMenuHighScores(contentStartingPositionX, contentStartingPositionY,
                font, buttonIndicator, buttonSelectSoundInstance);
            StateMenuControls statesMenuControls = new StateMenuControls(contentStartingPositionX, contentStartingPositionY,
                font, buttonIndicator, buttonSelectSoundInstance);

            statesMenuMain.GameStart += (object sender, EventArgs e) => GameStart(this, e);
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
            fsm.AddState(statesMenuHighScores);
            fsm.AddState(statesMenuControls);

            fsm.Initialise("Main");
            currentState = States.MAIN;

            // Load the game music  
            menuMusic = Content.Load<Song>("Sound\\menuMusic");
            // Start playing the music
            MediaPlayer.Play(menuMusic);
            MediaPlayer.IsRepeating = true;
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
            _spriteBatch.Draw(gameName, new Rectangle(
                (Game.GraphicsDevice.Viewport.Width - panelWidth) / 2, (Game.GraphicsDevice.Viewport.Height - panelHeight) / 2,
                panelWidth, 100), Color.White);

            fsm.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }
    }
}
