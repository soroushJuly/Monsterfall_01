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
        static public Player player;
        // One sample enemy
        //Enemy enemy;
        List<Enemy> enemies;

        CollisionManager collisionManager;

        // Keyboard states used to determine key presses   
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        //Mouse states used to track Mouse button press   
        MouseState currentMouseState;
        MouseState previousMouseState;

        // Image used to display the static background   
        Texture2D mainBackground;
        // Parallaxing Layers   
        ParallaxingBackground bgLayer1;

        //The rate at which the enemies appear  
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        // A random number generator  
        Random random;

        // govern how fast our laser can fire.  
        TimeSpan laserSpawnTime;
        TimeSpan previousLaserSpawnTime;

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

        AnimationLoader animationLoader;

        // Translation of the view when player reaches the boundries
        Vector3 viewTranslate;

        private Loader loader;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;
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
            //enemies.Add(new Enemy());

            collisionManager = new CollisionManager();

            animationLoader = new AnimationLoader();

            // Set the time keepers to zero  
            previousSpawnTime = TimeSpan.Zero;
            // Used to determine how fast enemy respawns  
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            // Initialize our random number generator  
            random = new Random();

            //Background  
            bgLayer1 = new ParallaxingBackground();

            inputCommandManager = new InputCommandManager();

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
            animationLoader.LoadAnimations(Content, "Graphics\\HeroFemale\\Run_Unarmed\\Run_Unarmed_Body_", PLAYER_SCALE, playerAnimations,
                320, 20, 17, 8, 5);
            animationLoader.LoadAnimations(Content, "Graphics\\HeroFemale\\Idle_Unarmed\\Idle_Unarmed_Body_", PLAYER_SCALE, playerAnimations,
                320, 16, 25, 8, 4);
            // get enemy textures
            const float ENEMY_SCALE = 1.2f;
            List<Animation> monsterIceAnimations = new List<Animation>();
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\WalkForward\\WalkForward Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 20, 17, 16, 4);
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\Run\\Run Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 20, 17, 16, 4);
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\Attack1\\Attack1 Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 20, 42, 16, 4);
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\Death\\Death Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 20, 17, 16, 4);

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
                        Vector2 location = new Vector2(int.Parse(coords[0]), int.Parse(coords[1]));
                        //Point location = new Point(coords[0],coords[1]);
                        string path = string.Format("Graphics\\Env\\Dungeon\\{0}", title);
                        Texture2D decorTexture = Content.Load<Texture2D>(path);
                        decorations.Add(new Decoration(location, decorTexture));
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

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X - 100,
                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(ref playerAnimations, playerPosition, PLAYER_SCALE);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Initialize(monsterIceAnimations, playerPosition + new Vector2(i * 150 + 500, i));
            }

            collisionManager.AddCollidable(player);
            foreach (Enemy enemy in enemies)
                collisionManager.AddCollidable(enemy);
            foreach (Tile decoration in map01.DecorTiles)    
                collisionManager.AddCollidable(decoration);

            // Load the parallaxing background   
            bgLayer1.Initialize(Content, "Graphics/bkgd_1", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, 1);

            mainBackground = Content.Load<Texture2D>("Graphics/bkgd_0");

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
            collisionManager.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Save the previous state of the keyboard, game pad, and mouse so we can determine single key/button presses  
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Read the current state of the keyboard, gamepad and mouse and store it  
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            //Update the player   
            UpdatePlayer(gameTime);
            player.Update(gameTime);
            
            foreach(Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
            }

            // Update the parallaxing background    
            bgLayer1.Update(gameTime);

            base.Update(gameTime);
        }
        // Getting and reacting to the inputs for the player
        private void UpdatePlayer(GameTime gameTime)
        {
            viewTranslate = new Vector3(GraphicsDevice.Viewport.Width / 2 - player.position.X,
                GraphicsDevice.Viewport.Height / 2 - player.position.Y, 0);

            // reset score if player health goes to zero  
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing translate Matrix to move in around the map
            _spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,
                Matrix.CreateTranslation(viewTranslate));

            //Draw the Main Background Texture  
            _spriteBatch.Draw(mainBackground, new Rectangle((int)-viewTranslate.X, (int)-viewTranslate.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                new Rectangle(0,0,mainBackground.Width,mainBackground.Height), Color.White);
            // Draw the moving background  
            bgLayer1.Draw(_spriteBatch, new Vector2(-viewTranslate.X, -viewTranslate.Y));

            // Draw map
            map01.Draw(_spriteBatch, GraphicsDevice);

            // Draw the Player  
            player.Draw(_spriteBatch, GraphicsDevice);

            //enemy.Draw(_spriteBatch);
            foreach(Enemy enemy in enemies)
            {
                enemy.Draw(_spriteBatch, GraphicsDevice);
            }

            int fixedYPosition = GraphicsDevice.Viewport.TitleSafeArea.Y - (int)viewTranslate.Y;
            int fixedXPosition = GraphicsDevice.Viewport.TitleSafeArea.X - (int)viewTranslate.X;

            // Draw the score  
            _spriteBatch.DrawString(font, "score: " + score, 
                new Vector2(fixedXPosition, fixedYPosition), Color.White);
            // Draw the player health  
            _spriteBatch.DrawString(font, "health: " + player.Health, 
                new Vector2(fixedXPosition, fixedYPosition + 30), Color.White);
            _spriteBatch.DrawString(font, "health: " + player.depth, 
                new Vector2(fixedXPosition, fixedYPosition + 90), Color.White);
            _spriteBatch.DrawString(font, "health: " + (player.position - player.prevPosition), 
                new Vector2(fixedXPosition, fixedYPosition + 120), Color.White);
            _spriteBatch.DrawString(font, "health: " + (enemies[0].distance), 
                new Vector2(fixedXPosition, fixedYPosition + 160), Color.White);
            _spriteBatch.DrawString(font, "is attack range: " + (enemies[0].isInAttackRange), 
                new Vector2(fixedXPosition, fixedYPosition + 190), Color.White);
            _spriteBatch.DrawString(font, "is chase range: " + (enemies[0].isInChaseRange), 
                new Vector2(fixedXPosition, fixedYPosition + 220), Color.White);


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
