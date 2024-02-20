using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Monsterfall_01;
using Monsterfall_01.Input;
namespace Monsterfall_01
{
    public class Player : Collidable
    {
        const float MOVEMENT_RESET_TIME = 0.1F;

        public Animation playerAnimation;
        List<Animation> playerAnimations;
        public float movementSpeed;
        public float scale;
       
        public Vector2 position;
        public Vector2 prevPosition;
        public bool isActive;
        public int Health;

        public Vector2 depth;

        private float xtimer;
        private float ytimer;
        private float blockTimer;
        private float takeDamageTimer;

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
            this.prevPosition = this.position;
            this.playerAnimations = playerAnimations;
            this.directions = new List<String>();
            String direction = "";
            directions.Add(direction);
            directions.Add(direction);
            
            currentAnimation = 0;
            this.xtimer = 0;
            this.blockTimer = 0;
            this.takeDamageTimer = 0;
            this.ytimer = 0;
            playerAnimation = playerAnimations[currentAnimation];

            Health = GameInfo.Instance.PlayerInfo.health;
            this.scale = scale;

            isActive = true;
        }
        
        public void Update(GameTime gameTime)
        {
            // Draw the box to screen for debugging purposes
            this.box = new Rectangle((int)(position.X - Width / 4), (int)position.Y - 120 / 2, Width / 2, 120);
            xtimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            ytimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            blockTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            takeDamageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
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
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            checkTimers();
            DrawBoundingBox(spriteBatch, GraphicsDevices);
            playerAnimations[currentAnimation].Draw(spriteBatch);
        }

        public void moveNorth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("North");
                this.prevPosition.Y = position.Y;
                this.position.Y -= movementSpeed;
            }
        }
        public void moveEast(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("East");
                this.prevPosition.X = position.X;
                this.position.X += movementSpeed;
            }
        }
        public void moveSouth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("South");
                this.prevPosition.Y = position.Y;
                this.position.Y += movementSpeed;
            }
        }
        public void moveWest(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("West");
                this.prevPosition.X = position.X;
                this.position.X -= movementSpeed;
            }
        }
        private void checkTimers()
        {
            if (xtimer < 0)
            {
                prevPosition.X = position.X;
                directions[0] = "";
            }
            if (ytimer < 0)
            {
                prevPosition.Y = position.Y;
                directions[1] = "";
            }
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
                if (takeDamageTimer < 0)
                {
                    Health -= 10;
                    takeDamageTimer = 1f;
                }
            }
            if (blockTimer > 0)
            {
                return;
            }
            // Only the first decoration collision is working
            Tile decoration = obj as Tile;
            if (decoration != null)
            {
                Vector2 depth = RectangleExtensions.GetIntersectionDepth(box, decoration.GetBox());
                this.depth = depth;
                Vector2 direction = position - prevPosition;
                if(direction.X != 0 && direction.Y == 0)
                {
                    prevPosition = position;
                    position = new Vector2(position.X + .5f * depth.X, position.Y);

                }
                else if(direction.Y != 0 && direction.X == 0)
                {
                    prevPosition = position;
                    position = new Vector2(position.X, position.Y + .5f * depth.Y);
                }
                else if (direction.Y != 0 && direction.X != 0)
                {
                    prevPosition = position;
                    if (depth.X < 2 * movementSpeed)
                        position = new Vector2(position.X + .5f * depth.X, position.Y);
                    else if (depth.Y < 2 * movementSpeed)
                        position = new Vector2(position.X, position.Y + .5f * depth.Y);
                    else
                        position = new Vector2(position.X + .5f * depth.X, position.Y + .5f * depth.Y);
                }

                blockTimer = 0.01f;
            }
        }
    }
}
