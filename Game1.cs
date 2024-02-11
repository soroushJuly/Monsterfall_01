using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Monsterfall_01;
using System.IO;
using Monsterfall_01.Input;

namespace Monsterfall_01
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Represents the player  
        Player player;
        // Keyboard states used to determine key presses   
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // Gamepad states used to determine button presses   
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        //Mouse states used to track Mouse button press   
        MouseState currentMouseState;
        MouseState previousMouseState;
        // A movement speed for the layer  
        float playerMoveSpeed;

        // Image used to display the static background   
        Texture2D mainBackground;
        // Parallaxing Layers   
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        //The rate at which the enemies appear  
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        // A random number generator  
        Random random;

        // govern how fast our laser can fire.  
        TimeSpan laserSpawnTime;
        TimeSpan previousLaserSpawnTime;

        // Collections of explosions  
        //List<Explosion> explosions;
        //Texture to hold explosion animation.  
        Texture2D explosionTexture;

        //Our Laser Sound and Instance  
        private SoundEffect laserSound;
        private SoundEffectInstance laserSoundInstance;

        //Our Explosion Sound.  
        private SoundEffect explosionSound;
        private SoundEffectInstance explosionSoundInstance;

        // Game Music.  
        private Song gameMusic;

        // The font used to display UI elements  
        SpriteFont font;
        //Number that holds the player score  
        int score;

        // Tile map for the first level
        Map map01;

        // Input manager
        InputCommandManager inputCommandManager;

        private Loader loader;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize the player class
            player = new Player();
            // Set a constant player move speed
            playerMoveSpeed = 3.0f;

            // Set the time keepers to zero  
            previousSpawnTime = TimeSpan.Zero;
            // Used to determine how fast enemy respawns  
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            // Initialize our random number generator  
            random = new Random();

            // init our laser
            //laserBeams = new List<Laser>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            //Background  
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            inputCommandManager = new InputCommandManager();

            // init our collection of explosions.
            //explosions = new List<Explosion>();

            //Set player's score to zero
            score = 0;

            InitializeKeyBindings();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Load the player resources
            // Create a list of player's animations
            List<Animation> playerAnimations = new List<Animation>();
            const float PLAYER_SCALE = 0.6f;
            for (int i = 0; i < 8; i++)
            {
                Animation playerAnimation = new Animation();
                int degree = i * 45;
                String degreePath;
                if(degree / 10 < 1) { degreePath = "000"; }
                else if(degree / 100 < 1) { degreePath = "0" + degree.ToString(); }
                else { degreePath = degree.ToString(); }
                String path = "Graphics\\HeroFemale\\Run_Unarmed\\Run_Unarmed_Body_" + degreePath;

                Texture2D playerTexture = Content.Load<Texture2D>(path);
                playerAnimation.Initialize(playerTexture, Vector2.Zero, 320, 320, 20, 17, Color.White, PLAYER_SCALE, true, 5);
                playerAnimations.Add(playerAnimation);

            }
            for (int i = 0; i < 8; i++)
            {
                Animation playerAnimation = new Animation();
                int degree = i * 45;
                String degreePath;
                if (degree / 10 < 1) { degreePath = "000"; }
                else if (degree / 100 < 1) { degreePath = "0" + degree.ToString(); }
                else { degreePath = degree.ToString(); }
                String path = "Graphics\\HeroFemale\\Idle_Unarmed\\Idle_Unarmed_Body_" + degreePath;

                Texture2D playerTexture = Content.Load<Texture2D>(path);
                playerAnimation.Initialize(playerTexture, Vector2.Zero, 320, 320, 16, 25, Color.White, PLAYER_SCALE, true, 4);
                playerAnimations.Add(playerAnimation);

            }

            //Animation playerAnimation = new Animation();
            //Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\HeroFemale\\Run_Unarmed\\Run_Unarmed_Body_000");
            //playerAnimation.Initialize(playerTexture, Vector2.Zero, 320, 320, 20, 30, Color.White, 1f, true, 5);
            //playerAnimations.Add(playerAnimation);

            // Load level details
            List<string> lines = new List<string>();
            List<Decoration> decorations = new List<Decoration>();
            int levelIndex = 0;
            string levelPath = string.Format("Content\\Maps\\{0}.txt", levelIndex);
            int width = 0;
            int height = 0;
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
            {
                bool isDecoraitons = false;
                loader = new Loader(fileStream);
                lines = loader.ReadLinesFromTextFile();
                foreach (string line in lines)
                {
                    if (isDecoraitons)
                    {
                        if(line == "Decorations")
                        {
                            break;
                        }
                        string[] widthLine = line.Split(":");
                        String title = widthLine[0];
                        String point = widthLine[1];
                        string[] coords = point.Split(",");
                        Point location = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                        //Point location = new Point(coords[0],coords[1]);
                        decorations.Add(new Decoration(location, title));
                        continue;
                    }
                    if (line.Contains("Width:"))
                    {
                        string[] widthLine = line.Split(":");
                        width = int.Parse(widthLine[1]);
                    }
                    if (line.Contains("Height:"))
                    {
                        string[] heightLine = line.Split(":");
                        height = int.Parse(heightLine[1]);
                    }
                    if (line.Contains("Decorations"))
                    {
                        isDecoraitons = true;
                    }
                }
            }


            // Initialize map
            map01 = new Map();
            Point MAP_SIZE = new Point(width, height);
            map01.Initialize(MAP_SIZE, Content, decorations);

            //
            loader.ReadXML("Content\\XML\\GameInfo.xml");


            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
            GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(ref playerAnimations, playerPosition, PLAYER_SCALE);

            // Load the parallaxing background   
            bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");

            // load the explosion sheet
            explosionTexture = Content.Load<Texture2D>("Graphics\\explosion");

            // Load the laserSound Effect and create the effect Instance  
            laserSound = Content.Load<SoundEffect>("Sound\\laserFire");
            laserSoundInstance = laserSound.CreateInstance();
            explosionSound = Content.Load<SoundEffect>("Sound\\explosion");
            explosionSoundInstance = explosionSound.CreateInstance();

            // Load the game music  
            //gameMusic = Content.Load<Song>("Sound\\gameMusic");
            // Start playing the music.  
            //MediaPlayer.Play(gameMusic);

            // Load the score font   
            font = Content.Load<SpriteFont>("Graphics\\gameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            inputCommandManager.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // Save the previous state of the keyboard, game pad, and mouse so we can determine single key/button presses  
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Read the current state of the keyboard, gamepad and mouse and store it  
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            //Update the player   
            //UpdatePlayer(gameTime);
            player.Update(gameTime);

            // Update the collisions   
            UpdateCollision();

            // Update the parallaxing background    
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            base.Update(gameTime);
        }
        // Getting and reacting to the inputs for the player
        private void UpdatePlayer(GameTime gameTime)
        {
            // Get Thumbstick Controls   
            player.position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            // Use the Keyboard / Dpad
            Keys[] currentPressedKeys = currentKeyboardState.GetPressedKeys();
            if (currentPressedKeys.Length != 0)
            {
                String KeyNames = "";
                foreach(Keys key in currentPressedKeys)
                {
                    KeyNames += key.ToString();
                }
                switch (KeyNames)
                {
                    case "UpRight": case "RightUp":
                        player.position.Y -= playerMoveSpeed; 
                        player.position.X += playerMoveSpeed; 
                        player.currentAnimation = 1;
                        break; 
                    default:
                        break;
                }
                // This code is here to be an example for the gamePadState
                //    if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
                //    { player.position.X -= playerMoveSpeed; player.currentAnimation = 6; }
            }
            else
            {
                player.currentAnimation = 5;
            }
            //Debug.WriteLine(currentKeyboardState.GetPressedKeys().Length);

            // Make sure that the player does not go out of bounds   
            player.position.X = MathHelper.Clamp(player.position.X, player.Width / 2,
            GraphicsDevice.Viewport.Width - player.Width / 2);
            player.position.Y = MathHelper.Clamp(player.position.Y, player.Height / 2,
            GraphicsDevice.Viewport.Height - player.Height / 2);

            // reset score if player health goes to zero  
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }
        private void UpdateCollision() { }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // Start drawing  
            _spriteBatch.Begin();

            //Draw the Main Background Texture  
            _spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
            // Draw the moving background  
            bgLayer1.Draw(_spriteBatch);
            bgLayer2.Draw(_spriteBatch);

            // Draw map
            map01.Draw(_spriteBatch);

            // Draw the Player  
            player.Draw(_spriteBatch);

            // Draw the score  
            _spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
            GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health  
            _spriteBatch.DrawString(font, "health: " + player.Health, new
            Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);


            // Stop drawing  
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        protected override void UnloadContent()
        {
            laserSoundInstance.Dispose();
            explosionSoundInstance.Dispose();
        }
        private void InitializeKeyBindings()
        {
            inputCommandManager.AddKeyboardBinding(Keys.W, player.moveNorth);
            inputCommandManager.AddKeyboardBinding(Keys.D, player.moveEast);
            inputCommandManager.AddKeyboardBinding(Keys.A, player.moveWest);
            inputCommandManager.AddKeyboardBinding(Keys.S, player.moveSouth);
        }
    }
}
