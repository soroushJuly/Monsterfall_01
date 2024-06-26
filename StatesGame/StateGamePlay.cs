﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Xna.Framework.Content;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Engine.Collision;
using Monsterfall_01.Engine;
using Monsterfall_01.Engine.Input;
using Monsterfall_01.PowerUp;
using System.Threading;

namespace Monsterfall_01.StateGame
{
    public class StateGamePlay : State
    {
        SpriteBatch _spriteBatch;

        // Represents the player  
        static public Player player;

        // Manages enemies in waves during the game;
        EnemyManager enemyManager;

        // Manages effects that will happen during game
        EffectManager effectManager;

        CollisionManager collisionManager;

        // Image used to display the static background   
        Texture2D mainBackground;
        // Parallaxing Layers   
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;
        ParallaxingBackground bgLayer3;

        // Arrow Texture
        static public Texture2D arrowTexture;
        static public List<Arrow> arrowList;

        // Aim texture
        Texture2D targetTexture;
        // Game Music.  
        Song gameMusic;

        // The font used to display UI elements  
        SpriteFont font;

        // Tile map for the first level
        Map map01;

        // Items to buy in the map
        List<ShopItem> shopItems;

        // Data for the map extracted from data
        MapData mapData;

        // Input manager
        InputCommandManager inputCommandManager;

        // Helper class to load character's animation
        AnimationLoader animationLoader;

        // Sound played when arrow hits the enemy
        SoundEffect arrowHitSound;
        SoundEffectInstance arrowHitSoundInstance;

        // Translation of the view when player reaches the boundries
        Vector3 viewTranslate;

        // Object to save the players activity
        GameStats stats;
        HighScores highScoresTable;

        ContentManager Content;
        Game Game;

        //GraphicsDevice GraphicsDevice;
        public EventHandler<GameStats> PlayerDied;
        public EventHandler<GameStats> PlayerSuccess;
        private Loader loader;
        public StateGamePlay(Game game)
        {
            Name = "Play";
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
            Draw(gameTime);
        }
        private void Initialize()
        {
            // Initialize the player class
            player = new Player();

            arrowList = new List<Arrow>();

            enemyManager = new EnemyManager();
            effectManager = new EffectManager();

            // Initilize map Data
            mapData = new MapData();

            shopItems = new List<ShopItem>();

            collisionManager = new CollisionManager();

            animationLoader = new AnimationLoader();

            //Background  
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            bgLayer3 = new ParallaxingBackground();

            inputCommandManager = new InputCommandManager();

            viewTranslate = Vector3.Zero;

            InitializeKeyBindings();
        }
        private void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            stats = new GameStats();

            Thread thread1 = new Thread(new ThreadStart(LoadEnemies));

            // load effects textures
            Texture2D bloodTexture = Content.Load<Texture2D>("Graphics\\BloodSplash");
            Texture2D pickUpTexture = Content.Load<Texture2D>("Graphics\\ItemPickedEffect");
            effectManager.AddAnimation("Blood", bloodTexture);
            effectManager.AddAnimation("PowerUp", pickUpTexture);

            // load the texture to serve as the laser
            arrowTexture = Content.Load<Texture2D>("Graphics\\Arrow");
            // Load aim texture
            targetTexture = Content.Load<Texture2D>("Graphics\\Target");

            // Load the laserSound Effect and create the effect Instance  
            arrowHitSound = Content.Load<SoundEffect>("Sound\\arrowHit");
            arrowHitSoundInstance = arrowHitSound.CreateInstance();

            // load shopItem textures
            Texture2D speedupTexture = Content.Load<Texture2D>("Graphics\\speedupPotion");
            Texture2D healthPickupTexture = Content.Load<Texture2D>("Graphics\\heartPickup");
            Texture2D arrowPickupTexture = Content.Load<Texture2D>("Graphics\\bowPickup");
            PowerUpSpeed powerUpSpeed = new PowerUpSpeed(speedupTexture);
            PowerUpBow powerUpBow = new PowerUpBow(arrowPickupTexture);
            PowerUpHealth powerUpHealth = new PowerUpHealth(healthPickupTexture);
            powerUpHealth.AddHealth += player.AddHealth;
            powerUpHealth.AddHealth += stats.OnAddHealth;
            powerUpSpeed.OnSpeedUp += player.SpeedUp;
            powerUpSpeed.OnSpeedUp += stats.OnSpeedUp;
            powerUpBow.OnBowUpgrade += player.BowUpgrade;
            powerUpBow.OnBowUpgrade += stats.OnBowUpgrade;
            shopItems.Add(powerUpSpeed);
            shopItems.Add(powerUpHealth);
            shopItems.Add(powerUpBow);


            // Load level\Enviroment details (Position of elements in the map and the map it self)
            // A helper class to get data for map
            mapData.ReadMapData(Content, 0);

            // Initialize map
            map01 = new Map();
            map01.Initialize(mapData.GetMapSize(), Content, mapData.GetDecorations(), shopItems, mapData.GetShopLocation());

            // Load data related to the gameplay
            loader = new Loader();
            loader.ReadXML("Content\\XML\\GameInfo.xml");

            highScoresTable = new HighScores();
            highScoresTable = HighScores.Load();

            thread1.Start();

            // Load the player resources
            // Create a list of player's animations
            List<Animation> playerAnimations = new List<Animation>();
            const float PLAYER_SCALE = 0.6f;
            animationLoader.LoadAnimations(Content, "Graphics\\HeroFemale\\Idle_Bow\\Idle_Bow_Body_", PLAYER_SCALE, playerAnimations,
                320, 16, 45, 8, 4);
            animationLoader.LoadAnimations(Content, "Graphics\\HeroFemale\\Run_Bow\\Run_Bow_Body_", PLAYER_SCALE, playerAnimations,
                320, 20, 17, 8, 5);
            animationLoader.LoadAnimations(Content, "Graphics\\HeroFemale\\Attack_Bow\\Attack_Bow_Body_", PLAYER_SCALE, playerAnimations,
                320, 20, 15, 8, 6, false);
            animationLoader.LoadAnimations(Content, "Graphics\\HeroFemale\\Death_Bow\\Death_Bow_Body_", PLAYER_SCALE, playerAnimations,
                320, 20, 17, 8, 6, false);


            Vector2 playerPosition = new Vector2(Game.GraphicsDevice.Viewport.TitleSafeArea.X - 100,
                Game.GraphicsDevice.Viewport.TitleSafeArea.Y + 400);
            player.Initialize(ref playerAnimations, playerPosition,
                new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), PLAYER_SCALE);
            stats.OnScoreChanged += player.UpdateScore;

            player.OnPlayerHit += effectManager.AddBloodEffect;
            player.OnPlayerPowerUp += effectManager.AddPowerUpEffect;
            //Thread thread2 = new Thread(new ThreadStart(MyThreadMethod));

            collisionManager.AddCollidable(player);
            foreach (ShopItem shopItem in map01.ShopItems)
                collisionManager.AddCollidable(shopItem);
            foreach (Tile decoration in map01.DecorTiles)
                collisionManager.AddCollidable(decoration);

            // Load the parallaxing background   
            bgLayer1.Initialize(Content, "Graphics/bkgd_6", Game.GraphicsDevice.Viewport.Width,
                Game.GraphicsDevice.Viewport.Height, 0.6f);
            bgLayer2.Initialize(Content, "Graphics/bkgd_7", Game.GraphicsDevice.Viewport.Width,
                Game.GraphicsDevice.Viewport.Height, 0.6f);
            bgLayer3.Initialize(Content, "Graphics/bkgd_3", Game.GraphicsDevice.Viewport.Width,
                Game.GraphicsDevice.Viewport.Height, 0.6f);

            mainBackground = Content.Load<Texture2D>("Graphics/bkgd_0");

            // Load the game music  
            gameMusic = Content.Load<Song>("Sound\\gameMusic");
            // Start playing the music.  
            MediaPlayer.Play(gameMusic);
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;

            // Load the score font   
            font = Content.Load<SpriteFont>("Graphics\\gameFont");

            thread1.Join();
        }
        private void LoadEnemies()
        {
            // get enemy textures
            const float ENEMY_SCALE = 1.2f;
            List<Animation> monsterIceAnimations = new List<Animation>();
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\WalkForward\\WalkForward Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 20, 17, 16, 4);
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\Run\\Run Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 20, 17, 16, 4);
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\Attack2\\Attack2 Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 24, 50, 16, 6);
            animationLoader.LoadAnimations(Content, "Graphics\\MonsterIce\\Death\\Death Body ", ENEMY_SCALE, monsterIceAnimations,
                256, 30, 25, 16, 6, false);

            // Pass the waves details received from the xml file
            // TODO: can use a factory pattern to create different types of enemies
            enemyManager.Initialize(GameInfo.Instance.GameWaves.waves, mapData.GetMapSize());
            enemyManager.AddAnimations("MonsterIce", monsterIceAnimations);

            enemyManager.OnEnemyDied += stats.OnEnemyDied;
            enemyManager.OnEnemyHit += effectManager.AddBloodEffect;
            enemyManager.OnEnemyHit += (object sender, Vector2 e) => { arrowHitSoundInstance.Play(); };
            enemyManager.OnLoadWave += (object sender, WaveArgs e) =>
            {
                foreach (Enemy enemy in enemyManager.GetEnemies())
                    collisionManager.AddCollidable(enemy);
            };
        }
        private void InitializeKeyBindings()
        {
            // Basic movements
            inputCommandManager.AddKeyboardBinding(Keys.W, player.moveNorth);
            inputCommandManager.AddKeyboardBinding(Keys.D, player.moveEast);
            inputCommandManager.AddKeyboardBinding(Keys.A, player.moveWest);
            inputCommandManager.AddKeyboardBinding(Keys.S, player.moveSouth);
            // Shoot arrow
            inputCommandManager.AddMouseBinding("Left", player.ShootArrow);
            // Interact/Buy Items in the shop
            inputCommandManager.AddKeyboardBinding(Keys.E, player.Interact);
        }

        private void Update(GameTime gameTime)
        {
            // Dynamic collidables
            foreach (Arrow arrow in arrowList)
                collisionManager.AddCollidable(arrow);
            inputCommandManager.Update();
            collisionManager.Update();

            //Update the player   
            UpdatePlayer(gameTime);
            player.Update(gameTime);

            foreach (Arrow arrow in arrowList)
            {
                arrow.Update(gameTime);
            }

            enemyManager.Update(gameTime);
            // Win game condition check
            if ((enemyManager.GetEnemies().Count == 0) && enemyManager.IsLastWave())
                PlayerSuccess(this, stats);

            effectManager.Update(gameTime);

            // Update the parallaxing background    
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);
            bgLayer3.Update(gameTime);

            ResolveRemovals();
        }
        // Getting and reacting to the inputs for the player
        private void UpdatePlayer(GameTime gameTime)
        {
            viewTranslate = new Vector3(Game.GraphicsDevice.Viewport.Width / 2 - player.position.X,
                Game.GraphicsDevice.Viewport.Height / 2 - player.position.Y, 0);

            if (player.Health <= 0)
            {
                PlayerDied(this, stats);
            }
        }
        private void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing translate Matrix to move in around the map
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateTranslation(viewTranslate));

            //Draw the Main Background Texture  
            _spriteBatch.Draw(mainBackground, new Rectangle((int)-viewTranslate.X, (int)-viewTranslate.Y, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, mainBackground.Width, mainBackground.Height), Color.White);
            // Draw the moving background  
            bgLayer1.Draw(_spriteBatch, new Vector2(-viewTranslate.X, -viewTranslate.Y));
            bgLayer2.Draw(_spriteBatch, new Vector2(-viewTranslate.X, -viewTranslate.Y));
            bgLayer3.Draw(_spriteBatch, new Vector2(-viewTranslate.X, -viewTranslate.Y));

            // Draw map
            map01.Draw(_spriteBatch, Game.GraphicsDevice, font);

            // Draw the Player  
            player.Draw(_spriteBatch, Game.GraphicsDevice);

            // Draw enemies
            enemyManager.Draw(_spriteBatch, Game.GraphicsDevice);

            // Draw aim target
            MouseState mouseState = Mouse.GetState();
            _spriteBatch.Draw(targetTexture, new Rectangle((int)-viewTranslate.X + mouseState.X - 25,
                (int)-viewTranslate.Y + mouseState.Y - 25, 50, 50), Color.White);

            // Draw effects
            effectManager.Draw(_spriteBatch);

            // Draw Arrows
            foreach (Arrow arrow in arrowList) arrow.Draw(_spriteBatch, Game.GraphicsDevice);

            // Getting the top left of screen
            int fixedYPosition = Game.GraphicsDevice.Viewport.TitleSafeArea.Y - (int)viewTranslate.Y;
            int fixedXPosition = Game.GraphicsDevice.Viewport.TitleSafeArea.X - (int)viewTranslate.X;

            // Draw the score  
            _spriteBatch.DrawString(font, "score: " + stats.score,
                new Vector2(fixedXPosition, fixedYPosition), Color.Red);
            // Draw the player health  
            _spriteBatch.DrawString(font, "health: " + player.Health,
                new Vector2(fixedXPosition, fixedYPosition + 30), Color.Red);
            // Shows the current wave and total waves
            _spriteBatch.DrawString(font, "Wave: " + (enemyManager.GetCurrentWave() + 1) + " / " + enemyManager.GetWaveCount(),
                new Vector2(fixedXPosition + (Game.GraphicsDevice.Viewport.Width / 2 - 80), fixedYPosition), Color.White);
            // Shows Time left to next wave
            if (!enemyManager.IsLastWave())
                _spriteBatch.DrawString(font, "Next wave in: " + (int)enemyManager.GetTimeToNextWave(),
                    new Vector2(fixedXPosition + (Game.GraphicsDevice.Viewport.Width / 2 - 80), fixedYPosition + 30), Color.White);
            // Shows how many enemies in map is left for killing
            _spriteBatch.DrawString(font, "Enemies Left: " + enemyManager.GetEnemies().Count,
                new Vector2(fixedXPosition, fixedYPosition + 60), Color.White);

            // Stop drawing  
            _spriteBatch.End();
        }
        private void ResolveRemovals()
        {
            // Remove all the flagged objects from lists
            List<Enemy> removals = new List<Enemy>();
            foreach (Enemy enemy in enemyManager.GetEnemies())
            {
                if (enemy.flagForRemoval)
                    removals.Add(enemy);
            }
            foreach (Enemy enemy in removals)
                enemyManager.Remove(enemy);

            List<Arrow> arrowRemovals = new List<Arrow>();
            foreach (Arrow arrow in arrowList)
            {
                if (arrow.flagForRemoval)
                    arrowRemovals.Add(arrow);
            }
            foreach (Arrow arrow in arrowRemovals)
                arrowList.Remove(arrow);
        }
        private void UnloadContent()
        {
            highScoresTable.Add(stats);
            HighScores.Save(highScoresTable);
            Content.Unload();
        }
    }
}
