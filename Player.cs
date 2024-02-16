using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01;
using Monsterfall_01.Input;
namespace Monsterfall_01
{
    internal class Player : Collidable
    {
        const float MOVEMENT_RESET_TIME = 0.1F;

        public Animation playerAnimation;
        List<Animation> playerAnimations;
        public float movementSpeed;
        public float scale;
       
        public Vector2 position;
        public bool isActive;
        public int Health;

        private float xtimer;
        private float ytimer;

        private List<String> directions;

        public int currentAnimation;

        public int Width
        { get { return (int)((float)playerAnimation.frameWidth * scale); } }
        public int Height
        { get { return (int)((float)playerAnimation.frameHeight * scale); } }

        public void Initialize(ref List<Animation> playerAnimations, Vector2 position, float scale = 1.0f)
        {
            movementSpeed = 4.0f;

            this.position = position;
            this.playerAnimations = playerAnimations;
            this.directions = new List<String>();
            String direction = "";
            directions.Add(direction);
            directions.Add(direction);
            
            currentAnimation = 0;
            this.xtimer = 0;
            this.ytimer = 0;
            playerAnimation = playerAnimations[currentAnimation];

            Health = GameInfo.Instance.PlayerInfo.health;
            this.scale = scale;

            isActive = true;
        }
        
        public void Update(GameTime gameTime)
        {
            // Draw the box to screen for debugging purposes
            this.box = new Rectangle((int)position.X, (int)position.Y, Width / 2, Height);
            xtimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            ytimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateAnimation(gameTime);
        }
        private void UpdateAnimation(GameTime gameTime)
        {
            // Switch current animation based on latest direction
            switch (getDirection())
            {
                case "North":
                    currentAnimation = 0;
                    break;
                case "EastNorth":
                    currentAnimation = 1;
                    break;
                case "East":
                    currentAnimation = 2;
                    break;
                case "EastSouth":
                    currentAnimation = 3;
                    break;
                case "South":
                    currentAnimation = 4;
                    break;
                case "WestSouth":
                    currentAnimation = 5;
                    break;
                case "West":
                    currentAnimation = 6;
                    break;
                case "WestNorth":
                    currentAnimation = 7;
                    break;
                default:
                    currentAnimation = 8;
                    break;
            }

            playerAnimations[currentAnimation].Position = position;
            playerAnimations[currentAnimation].Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            checkTimers();
            playerAnimations[currentAnimation].Draw(spriteBatch);
        }

        public void moveNorth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("North");
                this.position.Y -= movementSpeed;
            }
        }
        public void moveEast(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("East");
                this.position.X += movementSpeed;
            }
        }
        public void moveSouth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("South");
                this.position.Y += movementSpeed;
            }
        }
        public void moveWest(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("West");
                this.position.X -= movementSpeed;
            }
        }
        private void checkTimers()
        {
            if (xtimer < 0)
                directions[0] = "";
            if (ytimer < 0)
                directions[1] = "";
        }
        private void setXTimer(String direction)
        {
            directions[0] = direction;
            xtimer = MOVEMENT_RESET_TIME;
        }
        private void setYTimer(String direction)
        {
            directions[1] = direction;
            ytimer = MOVEMENT_RESET_TIME;
        }
        public String getDirection()
        {
            return directions[0] + directions[1]; 
        }

        public override bool CollisionTest(Collidable obj)
        {
            if(this.Intersects(obj))
            {
                return true;
            }
            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            Enemy enemy = obj as Enemy;
            if (enemy != null)
            {
                Health -= 10;
            }
        }
    }
}
