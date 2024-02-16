using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monsterfall_01
{
    internal class Enemy
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
            enemyMoveSpeed = 6f;
            // Set the score value of the enemy  
            Value = 100;
        }
        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement it's x position  
            //Position.X -= enemyMoveSpeed;
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
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation  
            enemyAnimations[0].Draw(spriteBatch);
        }
    }
}
