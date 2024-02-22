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
        enum STATES
        {
            IDLE,
            RUN,
            ATTACK_BOW,
            HIT,
            DEATH
        }
        const float MOVEMENT_RESET_TIME = 0.1F;

        public Animation playerAnimation;
        private Dictionary<String, int> mapDirections;
        List<Animation> playerAnimations;
        public float movementSpeed;
        public float scale;
       
        public Vector2 position;
        public Vector2 prevPosition;
        public bool isActive;
        public int Health;

        public int currentDirectionIndex;
        public int currentState;

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
            mapDirections = new Dictionary<String, int>();
            String direction = "";
            directions.Add(direction);
            directions.Add(direction);

            currentState = 0;

            mapDirections.Add("NORTH", 0);
            mapDirections.Add("NORTHEAST", 1);
            mapDirections.Add("EAST", 2);
            mapDirections.Add("SOUTHEAST", 3);
            mapDirections.Add("SOUTH", 4);
            mapDirections.Add("SOUTHWEST", 5);
            mapDirections.Add("WEST", 6);
            mapDirections.Add("NORTHWEST", 7);

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
            // 
            if (getDirection() == "")
                currentState = 0;
            else
            {
                currentState = 1;
                currentDirectionIndex = mapDirections[getDirection()];
            }

            currentAnimation = currentDirectionIndex + 8 * (int)currentState;
            playerAnimations[currentAnimation].Position = position;
            playerAnimations[currentAnimation].Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            checkTimers();
            //DrawBoundingBox(spriteBatch, GraphicsDevices);
            playerAnimations[currentAnimation].Draw(spriteBatch);
        }

        public void moveNorth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("NORTH");
                this.prevPosition.Y = position.Y;
                this.position.Y -= movementSpeed;
            }
        }
        public void moveEast(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("EAST");
                this.prevPosition.X = position.X;
                this.position.X += movementSpeed;
            }
        }
        public void moveSouth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("SOUTH");
                this.prevPosition.Y = position.Y;
                this.position.Y += movementSpeed;
            }
        }
        public void moveWest(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("WEST");
                this.prevPosition.X = position.X;
                this.position.X -= movementSpeed;
            }
        }
        private void checkTimers()
        {
            if (xtimer < 0)
            {
                prevPosition.X = position.X;
                directions[1] = "";
            }
            if (ytimer < 0)
            {
                prevPosition.Y = position.Y;
                directions[0] = "";
            }
        }
        private void setXTimer(String direction)
        {
            directions[1] = direction;
            xtimer = MOVEMENT_RESET_TIME;
        }
        private void setYTimer(String direction)
        {
            directions[0] = direction;
            ytimer = MOVEMENT_RESET_TIME;
        }
        public String getDirection()
        {
            return directions[0] + directions[1]; 
        }

        public void ShootArrow(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                Game1.arrowList.Add(new Arrow(this.position));
            }
        }
        public override bool CollisionTest(Collidable obj)
        {
            if (this.Intersects(obj))
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
