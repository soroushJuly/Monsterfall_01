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
        SpriteBatch _spriteBatch;
        ContentManager Content;

        Texture2D mainBackground;
        // Panel to put buttons inside
        Texture2D panel;
        int panelWidth;
        int panelHeight;

        // Title that goes on the top part of the panel
        Texture2D gameName;

        // Position where we start putting UI elements
        int contentStartingPositionX;
        int contentStartingPositionY;

        // Window sizes
        int windowWidth;
        int windowHeight;

        // Buttons sound effects  
        SoundEffect buttonSwitchSound;
        SoundEffectInstance buttonSwitchSoundInstance;
        
        SoundEffect buttonSelectSound;
        SoundEffectInstance buttonSelectSoundInstance;

        // Menu Music 
        Song menuMusic;

        FSM fsm;
        // States (Pages) of the game menu
        enum States
        {
            MAIN,
            HIGHSCORES,
            CONTROLS
        }
        States currentState;


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
            Update(gameTime);
        }
        public override void Draw(object owner, GameTime gameTime, SpriteBatch spriteBatch = null)
        {
            Draw(gameTime);
        }
        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            windowWidth = Game.GraphicsDevice.Viewport.Width;
            windowHeight = Game.GraphicsDevice.Viewport.Height;
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
            // Little arrow in buttons
            Texture2D buttonIndicator = Content.Load<Texture2D>("Graphics\\UI\\arrowBeige_right");
            // Load font
            SpriteFont font = Content.Load<SpriteFont>("Graphics\\gameFont");
            // Load state textures
            panel = Content.Load<Texture2D>("Graphics\\UI\\panel_stone");
            mainBackground = Content.Load<Texture2D>("Graphics\\bg_menu");
            gameName = Content.Load<Texture2D>("Graphics\\Name");

            // Load button sound effects
            buttonSwitchSound = Content.Load<SoundEffect>("Sound\\buttonSwitch");
            buttonSwitchSoundInstance = buttonSwitchSound.CreateInstance();
            buttonSelectSound = Content.Load<SoundEffect>("Sound\\buttonSelect");
            buttonSelectSoundInstance = buttonSelectSound.CreateInstance();

            // Panel size responsive to the user screen it's bigger for bigger screens
            panelWidth = Math.Clamp((int)(windowWidth * 0.4f), 400, 600);
            panelHeight = Math.Clamp((int)(windowHeight * 0.6f), 400, 650);

            // Staring position of content of pages is related to the panel
            contentStartingPositionX = (windowWidth) / 2 - (panelWidth / 2) + 80;
            contentStartingPositionY = (windowHeight) / 2 - 50;

            fsm = new FSM(this);

            // TODO: using sound manager to pass the audio
            // Initializing state machine
            StateMenuMain statesMenuMain = new StateMenuMain(contentStartingPositionX, contentStartingPositionY,
                font, buttonIndicator, buttonSwitchSoundInstance, buttonSelectSoundInstance);
            StateMenuHighScores statesMenuHighScores = new StateMenuHighScores(contentStartingPositionX, contentStartingPositionY,
                font, buttonIndicator, buttonSelectSoundInstance);
            StateMenuControls statesMenuControls = new StateMenuControls(contentStartingPositionX, contentStartingPositionY,
                font, buttonIndicator, buttonSelectSoundInstance);

            // Listening for changes in the states
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

            // Game starts from the main menu which has 4 buttons inside
            fsm.Initialise("Main");
            currentState = States.MAIN;

            // Load the game music  
            menuMusic = Content.Load<Song>("Sound\\menuMusic");
            // Start playing the music
            MediaPlayer.Play(menuMusic);
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
        }

        void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            // Draw Background
            _spriteBatch.Draw(mainBackground, new Rectangle(0, 0, windowWidth, windowHeight),
                new Rectangle(0, 0, mainBackground.Width, mainBackground.Height), Color.White);
            // Draw Panel
            _spriteBatch.Draw(panel, new Rectangle(
                (windowWidth - panelWidth) / 2, (windowHeight - panelHeight) / 2,
                panelWidth, panelHeight), Color.White);
            // Draw Game title
            _spriteBatch.Draw(gameName, new Rectangle(
                (windowWidth - panelWidth) / 2, (windowHeight - panelHeight) / 2,
                panelWidth, 100), Color.White);

            fsm.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }
    }
}
