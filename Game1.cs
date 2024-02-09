using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Monsterfall_01;

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

        // Enemies  
        Texture2D enemyTexture;
        List<Enemy> enemies;
        //The rate at which the enemies appear  
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        // A random number generator  
        Random random;

        // texture to hold the laser.  
        Texture2D laserTexture;
        //List<Laser> laserBeams;

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

            // Initialize the enemies list
            enemies = new List<Enemy>();
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

            // init our collection of explosions.
            //explosions = new List<Explosion>();

            //Set player's score to zero
            score = 0;

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
            //Animation playerAnimation = new Animation();
            //Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\HeroFemale\\Run_Unarmed\\Run_Unarmed_Body_000");
            //playerAnimation.Initialize(playerTexture, Vector2.Zero, 320, 320, 20, 30, Color.White, 1f, true, 5);
            //playerAnimations.Add(playerAnimation);


            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
            GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(ref playerAnimations, playerPosition, PLAYER_SCALE);

            // Load the enemy animation  
            enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");

            // load the texture to serve as the laser
            laserTexture = Content.Load<Texture2D>("Graphics\\laser");

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
            gameMusic = Content.Load<Song>("Sound\\gameMusic");
            // Start playing the music.  
            MediaPlayer.Play(gameMusic);

            // Load the score font   
            font = Content.Load<SpriteFont>("Graphics\\gameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            
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

            // Update the enemies  
            UpdateEnemies(gameTime);

            // update laserbeams   
            //UpdateLaserBeams(gameTime);

            // Update the collisions   
            UpdateCollision();

            // Update explosions  
            //UpdateExplosions(gameTime);

            // Update the parallaxing background    
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            base.Update(gameTime);
        }
        // Getting and reacting to the inputs for the player
        private void UpdatePlayer(GameTime gameTime)
        {
            // Get Mouse State then Capture the Button type and Respond Button Press
            // TODO: fix mouse input on reaching the mouse position 
            //Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            //if (currentMouseState.LeftButton == ButtonState.Pressed)
            //{
            //    Vector2 posDelta = mousePosition - player.position;
            //    posDelta.Normalize();
            //    posDelta = posDelta * playerMoveSpeed;
            //    player.position = player.position + posDelta;
            //}

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
                    case "UpLeft": case "LeftUp":
                        player.position.Y -= playerMoveSpeed; 
                        player.position.X -= playerMoveSpeed; 
                        player.currentAnimation = 7;
                        break;

                    case "DownRight": case "RightDown":
                        player.position.Y += playerMoveSpeed; 
                        player.position.X += playerMoveSpeed; 
                        player.currentAnimation = 3;
                        break;
                    case "DownLeft": case "LeftDown":
                        player.position.Y += playerMoveSpeed; 
                        player.position.X -= playerMoveSpeed; 
                        player.currentAnimation = 5;
                        break;
                    case "Left":
                        player.position.X -= playerMoveSpeed; player.currentAnimation = 6;
                        break;
                    case "Up":
                        player.position.Y -= playerMoveSpeed; player.currentAnimation = 0;
                        break;
                    case "Right":
                        player.position.X += playerMoveSpeed; player.currentAnimation = 2;
                        break;
                    case "Down":
                        player.position.Y += playerMoveSpeed; player.currentAnimation = 4;
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
        private void AddEnemy()
        {
            // Create the animation object  
            Animation enemyAnimation = new Animation();
            // Initialize the animation with the correct animation information  
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            // Randomly generate the position of the enemy  
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100,
            GraphicsDevice.Viewport.Height - 100));
            // Create an enemy  
            Enemy enemy = new Enemy();
            // Initialize the enemy  
            enemy.Initialize(enemyAnimation, position);
            // Add the enemy to the active enemies list 
            enemies.Add(enemy);
        }
        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to   
            // determine if two objects are overlapping   
            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            Rectangle laserRectangle;
            // Only create the rectangle once for the player  
            playerRectangle = new Rectangle((int)player.position.X, (int)player.position.Y, player.Width, player.Height);
            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemyRectangle = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y,
                    enemies[i].Width,
                    enemies[i].Height);
                // Determine if the two objects collided with each other  
                if (playerRectangle.Intersects(enemyRectangle))
                {
                    player.Health -= enemies[i].Damage;
                    // Show the explosion where the enemy was...  
                    //AddExplosion(enemies[i].Position);
                    // Since the enemy collided with the player destroy it  
                    enemies[i].Health = 0;
                    // If the player health is less than zero we died  
                    if (player.Health <= 0) player.isActive = false;
                }
                // Laserbeam vs Enemy Collision  
                //for (var l = 0; l < laserBeams.Count; l++)
                //{
                //    // create a rectangle for this laserbeam  
                //    laserRectangle = new Rectangle((int)laserBeams[l].Position.X,
                //        (int)laserBeams[l].Position.Y, laserBeams[l].Width, laserBeams[l].Height);
                //    // test the bounds of the laser and enemy  
                //    if (laserRectangle.Intersects(enemyRectangle))
                //    {
                //        // Show the explosion where the enemy was...  
                //        AddExplosion(enemies[i].Position);
                //        // kill off the enemy  
                //        enemies[i].Health = 0;
                //        // kill off the laserbeam 
                //        laserBeams[l].Active = false;
                //    }
                //}
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds  
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                // Add an Enemy  
                AddEnemy();
            }
            // Update the Enemies  
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    //Add to the player's score  
                    score += enemies[i].Value;
                    enemies.RemoveAt(i);
                }
            }
        }
        //protected void FireLaser(GameTime gameTime)
        //{
        //    // govern the rate of fire for our lasers  
        //    if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
        //    {
        //        previousLaserSpawnTime = gameTime.TotalGameTime;
        //        // Add the laer to our list.  
        //        AddLaser();
        //        // Play the laser sound!  
        //        laserSoundInstance.Play();
        //    }
        //}
        //private void UpdateLaserBeams(GameTime gameTime)
        //{
        //    // Update the Projectiles  
        //    for (int i = laserBeams.Count - 1; i >= 0; i--)
        //    {
        //        laserBeams[i].Update(gameTime);
        //        if (laserBeams[i].Active == false)
        //        {
        //            laserBeams.RemoveAt(i);
        //        }
        //    }
        //}
        //protected void AddLaser()
        //{
        //    Animation laserAnimation = new Animation();
        //    // initlize the laser animation  
        //    laserAnimation.Initialize(laserTexture, player.Position, 46, 16, 1, 30, Color.White, 1f, true);
        //    Laser laser = new Laser();
        //    // Get the starting postion of the laser.   
        //    var laserPostion = player.Position;
        //    // Adjust the position slightly to match the muzzle of the cannon.  
        //    laserPostion.X += 30;
        //    // init the laser  
        //    laser.Initialize(laserAnimation, laserPostion);
        //    laserBeams.Add(laser);
        //    /* todo: add code to create a laser. */
        //    // laserSoundInstance.Play();  
        //}
        //protected void AddExplosion(Vector2 enemyPosition)
        //{
        //    Animation explosionAnimation = new Animation();
        //    explosionAnimation.Initialize(explosionTexture, enemyPosition, 134, 134, 12, 30, Color.White, 1.0f, true);
        //    Explosion explosion = new Explosion();
        //    explosion.Initialize(explosionAnimation, enemyPosition);
        //    explosions.Add(explosion);

        //    explosionSound.Play();
        //}
        //private void UpdateExplosions(GameTime gameTime)
        //{
        //    for (var e = explosions.Count - 1; e >= 0; e--)
        //    {
        //        explosions[e].Update(gameTime);
        //        if (!explosions[e].Active)
        //            explosions.Remove(explosions[e]);
        //    }
        //}
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

            // Draw the Player  
            player.Draw(_spriteBatch);

            // Draw the Enemies   
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
            }

            //// Draw the lasers.  
            //foreach (var l in laserBeams)
            //{
            //    l.Draw(_spriteBatch);
            //}

            //// draw explosions   
            //foreach (var e in explosions)
            //{
            //    e.Draw(_spriteBatch);
            //}

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
    }
}
