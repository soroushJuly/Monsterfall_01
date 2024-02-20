using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.State;
using Monsterfall_01.State.StatesEnemy;

namespace Monsterfall_01
{
    internal class Enemy : Collidable
    {
        // Animation list for the enemy  
        private List<Animation> enemyAnimations;
        // The position of the enemy ship relative to the top left corner of the screen  
        public Vector2 Position;
        // The state of the Enemy Ship  
        public bool Active;
        // The hit points of the enemy, if this goes to zero the enemy dies 
        public int Health;
        // The amount of damage the enemy inflicts on the player ship  
        public int Damage;
        // The amount of score the enemy will give to the player  
        public int Value;
        // Get the width of the enemy ship  
        public int Width { get { return enemyAnimations[0].frameWidth; } }
        // Get the height of the enemy ship           
        public int Height { get { return enemyAnimations[0].frameHeight; } }
        // The speed at which the enemy moves           
        float enemyMoveSpeed;

        private bool isInChaseRange;

        FSM fsm;

        public void Initialize(ref List<Animation> animations, Vector2 position)
        {
            // Load the enemy ship texture  
            this.enemyAnimations = animations;

            // Set the position of the enemy  
            Position = position;
            // We initialize the enemy to be active so it will be update in the game  
            Active = true;
            // Set the health of the enemy  
            Health = 10;
            // Set the amount of damage the enemy can do  
            Damage = 10;
            // Set how fast the enemy moves  
            enemyMoveSpeed = 1f;
            // Set the score value of the enemy  
            Value = 100;

            isInChaseRange = false;

            fsm = new FSM(this);

            StateEnemyIdle stateEnemyIdle = new StateEnemyIdle();
            StateEnemyChase stateEnemyChase = new StateEnemyChase();

            stateEnemyIdle.AddTransition(new Transition(stateEnemyChase, () => isInChaseRange));
            stateEnemyChase.AddTransition(new Transition(stateEnemyIdle, () => !isInChaseRange));

            fsm.AddState(stateEnemyIdle);
        }
        public void Update(GameTime gameTime)
        {
            fsm.Update(gameTime);

            this.box = new Rectangle((int)(Position.X - Width / 4), (int)Position.Y - Height / 2, Width / 2, Height);
            // The enemy always moves to the left so decrement it's x position  
            Position.X -= enemyMoveSpeed;
            // Update the position of the Animation  
            enemyAnimations[0].Position = Position;
            // Update Animation  
            enemyAnimations[0].Update(gameTime);
            // If the enemy is past the screen or its health reaches 0 then deactivate it  
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this object from the 
                // active game list  
                Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            // Draw the animation  
            Texture2D pixel = new Texture2D(GraphicsDevices, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            spriteBatch.Draw(pixel, box, Color.White);
            enemyAnimations[0].Draw(spriteBatch);
        }

        private void Sense()
        {
            //List<Ship> enemies = GameWorld.AllShips;
            //Ship player = GameWorld.Player;

            //Vector2 playerDistance = Game1.

            //foreach (Ship enemy in enemies)
            //{
            //    if (enemy != this)
            //    {
            //        if (enemy.IsTagged && Sensor.Intersects(enemy.BoundingSphere))
            //        {
            //            taggerSeen = true;
            //            break;
            //        }
            //        else
            //        {
            //            taggerSeen = false;
            //        }
            //    }
            //}
        }


    }
}
