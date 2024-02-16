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
        // One sample enemy
        //Enemy enemy;
        List<Enemy> enemies;

        // Keyboard states used to determine key presses   
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // Gamepad states used to determine button presses   
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        //Mouse states used to track Mouse button press   
        MouseState currentMouseState;
        MouseState previousMouseState;

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

        // Translation of the view when player reaches the boundries
        Vector3 viewTranslate;

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

            enemies = new List<Enemy>();
            enemies.Add(new Enemy());
            enemies.Add(new Enemy());
            enemies.Add(new Enemy());

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

            viewTranslate = Vector3.Zero;

            InitializeKeyBindings();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the player resources
            // Create a list of player's animations
            List<Animation> playerAnimations = new List<Animation>();
            const float PLAYER_SCALE = 0.6f;
            for (int i = 0; i < 8; i++)
            {
                Animation playerAnimation = new Animation();
                String path = createTexturePath("Graphics\\HeroFemale\\Run_Unarmed\\Run_Unarmed_Body_", i);

                Texture2D playerTexture = Content.Load<Texture2D>(path);
                playerAnimation.Initialize(playerTexture, Vector2.Zero, 320, 320, 20, 17, Color.White, PLAYER_SCALE, true, 5);
                playerAnimations.Add(playerAnimation);

            }
            for (int i = 0; i < 8; i++)
            {
                Animation playerAnimation = new Animation();
                String path = createTexturePath("Graphics\\HeroFemale\\Idle_Unarmed\\Idle_Unarmed_Body_", i);

                Texture2D playerTexture = Content.Load<Texture2D>(path);
                playerAnimation.Initialize(playerTexture, Vector2.Zero, 320, 320, 16, 25, Color.White, PLAYER_SCALE, true, 4);
                playerAnimations.Add(playerAnimation);
            }    
            // get enemy textures
            const float ENEMY_SCALE = 1.2f;
            List<Texture2D> monsterTextures = new List<Texture2D>();
            for (int i = 0; i < 8; i++)
            {
                String path = createTexturePath("Graphics\\MonsterIce\\Run\\Run Body ", i);
                Texture2D monsterTexture = Content.Load<Texture2D>(path);
                monsterTextures.Add(monsterTexture);
            }
            for (int i = 0; i < 8; i++)
            {
                Animation monsterIceAnimation = new Animation();
                String path = createTexturePath("Graphics\\MonsterIce\\Idle\\Idle Body ", i);
                Texture2D monsterTexture = Content.Load<Texture2D>(path);
                monsterTextures.Add(monsterTexture);                
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

            for (int i = 0; i < enemies.Count; i++)
            {
                List<Animation> monsterIceAnimations = new List<Animation>();
                foreach (Texture2D texture in monsterTextures)
                {
                    Animation monsterIceAnimation = new Animation();
                    monsterIceAnimation.Initialize(texture, playerPosition + new Vector2(i * 100, i),
                        256, 256, 20, 17, Color.White, ENEMY_SCALE, true, 4);
                    monsterIceAnimations.Add(monsterIceAnimation);
                }

                enemies[i].Initialize(ref monsterIceAnimations, playerPosition + new Vector2(i * 100, i));
            }

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
            UpdatePlayer(gameTime);
            player.Update(gameTime);
            
            foreach(Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
            }

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
            // Use the Keyboard / Dpad
            // Legacy code for getting keys on keyboard
            //Keys[] currentPressedKeys = currentKeyboardState.GetPressedKeys();
            //if (currentPressedKeys.Length != 0)
            //{
            //    String KeyNames = "";
            //    foreach(Keys key in currentPressedKeys)
            //    {
            //        KeyNames += key.ToString();
            //    }
            //    switch (KeyNames)
            //    {
            //        default:
            //            break;
            //    }
            //}
            //Debug.WriteLine(currentKeyboardState.GetPressedKeys().Length);

            // Lagecy code for moving in the scene
            // Make sure that the player does not go out of bounds
            //if (player.position.X < -viewTranslate.X)
            //{
            //    viewTranslate += new Vector3(player.movementSpeed * 100, 0, 0);
            //}
            //if (player.position.X > (GraphicsDevice.Viewport.Width - viewTranslate.X))
            //{
            //    viewTranslate -= new Vector3(player.movementSpeed * 100, 0, 0);
            //}
            // 120 will be starting position later
            // which can be data driven
            //if (player.position.Y < -viewTranslate.Y + 120)
            //{
            //    viewTranslate += new Vector3(0, player.movementSpeed * 100, 0);
            //}
            //if (player.position.Y > (GraphicsDevice.Viewport.Height - viewTranslate.Y - 80))
            //{
            //    viewTranslate -= new Vector3(0, player.movementSpeed * 100, 0);
            //}

            viewTranslate = new Vector3(GraphicsDevice.Viewport.Width / 2 - player.position.X,
                GraphicsDevice.Viewport.Height / 2 - player.position.Y, 0);

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

            // Start drawing translate Matrix to move in around the map
            _spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,
                Matrix.CreateTranslation(viewTranslate));

            //Draw the Main Background Texture  
            _spriteBatch.Draw(mainBackground, Vector2.Zero + new Vector2(-viewTranslate.X,-viewTranslate.Y), Color.White);
            // Draw the moving background  
            bgLayer1.Draw(_spriteBatch, new Vector2(-viewTranslate.X, -viewTranslate.Y));
            bgLayer2.Draw(_spriteBatch, new Vector2(-viewTranslate.X, -viewTranslate.Y));

            // Draw map
            map01.Draw(_spriteBatch);

            // Draw the Player  
            player.Draw(_spriteBatch);

            //enemy.Draw(_spriteBatch);
            foreach(Enemy enemy in enemies)
            {
                enemy.Draw(_spriteBatch);
            }

            int fixedYPosition = GraphicsDevice.Viewport.TitleSafeArea.Y - (int)viewTranslate.Y;
            int fixedXPosition = GraphicsDevice.Viewport.TitleSafeArea.X - (int)viewTranslate.X;

            // Draw the score  
            _spriteBatch.DrawString(font, "score: " + score, 
                new Vector2(fixedXPosition, fixedYPosition), Color.White);
            // Draw the player health  
            _spriteBatch.DrawString(font, "health: " + player.Health, 
                new Vector2(fixedXPosition, fixedYPosition + 30), Color.White);
            _spriteBatch.DrawString(font, "Anima: " + player.position.Y,
                new Vector2(fixedXPosition, fixedYPosition + 180), Color.White);


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

        private String createTexturePath(String basePath, int i)
        {
            int degree = i * 45;
            String degreePath;
            if (degree / 10 < 1) { degreePath = "000"; }
            else if (degree / 100 < 1) { degreePath = "0" + degree.ToString(); }
            else { degreePath = degree.ToString(); }
            String path = basePath + degreePath;
            return path;

        }
    }
}
