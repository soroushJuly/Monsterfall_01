using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.Collision;
using Monsterfall_01.Engine.Input;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.PowerUp;
using Monsterfall_01.StateGame;
using Monsterfall_01.StatesPlayer;
namespace Monsterfall_01
{
    public class Player : Collidable
    {
        public enum States
        {
            IDLE,
            RUN,
            ATTACK_BOW,
            HIT,
            DEATH
        }
        const float MOVEMENT_RESET_TIME = 0.05F;

        FSM animationManager;

        public Animation playerAnimation;
        private Dictionary<String, int> mapDirections;
        public List<Animation> playerAnimations;
        public float movementSpeed;
        public float scale;
       
        public Vector2 position;
        public Vector2 prevPosition;
        public bool isActive;
        public int Health;

        public int currentDirectionIndex;
        public States currentState;

        public Vector2 depth;

        private float xtimer;
        private float ytimer;
        private float blockTimer;
        private float takeDamageTimer;
        private float attackTimer;
        private float speedUpTimer;
        private float bowUpgradeTimer;

        private List<String> directions;

        public int currentAnimation;

        public bool isAttacking;

        private int playerScore;

        // Item that is in the pickup range of player
        private ShopItem itemInRange;

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
            playerScore = 0;
            isAttacking = false;

            animationManager = new FSM(this);

            StatePlayerIdle statePlayerIdle = new StatePlayerIdle();
            StatePlayerRun statePlayerRun = new StatePlayerRun();
            StatePlayerAttack statePlayerAttack = new StatePlayerAttack();
            StatePlayerHit statePlayerHit = new StatePlayerHit();
            StatePlayerDeath statePlayerDeath = new StatePlayerDeath();


            statePlayerIdle.AddTransition(new Transition(statePlayerRun, () => DeltaPosition() != Vector2.Zero));
            statePlayerIdle.AddTransition(new Transition(statePlayerAttack, () => isAttacking));
            statePlayerRun.AddTransition(new Transition(statePlayerIdle, () => DeltaPosition() == Vector2.Zero));
            statePlayerRun.AddTransition(new Transition(statePlayerAttack, () => isAttacking));
            statePlayerAttack.AddTransition(new Transition(statePlayerIdle, () => DeltaPosition() == Vector2.Zero && !this.playerAnimations[currentAnimation].Active));
            statePlayerAttack.AddTransition(new Transition(statePlayerRun, () => DeltaPosition() != Vector2.Zero && !this.playerAnimations[currentAnimation].Active));
            //statePlayerAttack.AddTransition(new Transition(statePlayerRun, () => DeltaPosition() != Vector2.Zero && !this.playerAnimations[currentAnimation].Active));

            animationManager.AddState(statePlayerIdle);
            animationManager.AddState(statePlayerRun);
            animationManager.AddState(statePlayerAttack);
            animationManager.AddState(statePlayerHit);
            animationManager.AddState(statePlayerDeath);

            animationManager.Initialise("Idle");

            mapDirections.Add("NORTH", 0);
            mapDirections.Add("NORTHEAST", 1);
            mapDirections.Add("EAST", 2);
            mapDirections.Add("SOUTHEAST", 3);
            mapDirections.Add("SOUTH", 4);
            mapDirections.Add("SOUTHWEST", 5);
            mapDirections.Add("WEST", 6);
            mapDirections.Add("NORTHWEST", 7);

            currentAnimation = 0;
            // TODO: add Timer manager
            this.xtimer = 0;
            this.blockTimer = 0;
            this.takeDamageTimer = 0;
            this.ytimer = 0;
            this.attackTimer = 0;
            this.speedUpTimer = 0;
            this.bowUpgradeTimer = 0.0f;
            playerAnimation = playerAnimations[currentAnimation];

            Health = GameInfo.Instance.PlayerInfo.health;
            this.scale = scale;

            isActive = true;
        }
        
        public void Update(GameTime gameTime)
        {
            // Draw the box to screen for debugging purposes
            animationManager.Update(gameTime);
            this.box = new Rectangle((int)(position.X - Width / 6), (int)position.Y - 90 / 2, Width / 3, 90);
            //this.box = new Rectangle((int)position.X , (int)position.Y, 10, 10);
            xtimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            ytimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            speedUpTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            blockTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            takeDamageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            bowUpgradeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //  !!!! THIS CHANGED THE MOVEMNET SPEED DURING SHOOTING ARROW !!!!
            if (speedUpTimer < 0) movementSpeed = 4;
            UpdateAnimation(gameTime);
        }
        private void UpdateAnimation(GameTime gameTime)
        {
            if(getDirection() != "")
                currentDirectionIndex = mapDirections[getDirection()];

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
            if (buttonState == eButtonState.UP)
            {
                if (attackTimer > 0)
                {
                    return;
                }
                if(bowUpgradeTimer > 0)
                {
                    StateGamePlay.arrowList.Add(new Arrow(this.position, currentDirectionIndex));
                    StateGamePlay.arrowList.Add(new Arrow(this.position, currentDirectionIndex + 1));
                    StateGamePlay.arrowList.Add(new Arrow(this.position, currentDirectionIndex - 1));
                }
                else
                    StateGamePlay.arrowList.Add(new Arrow(this.position, currentDirectionIndex));

                isAttacking = true;
                attackTimer = .6f;
            }
        }
        public void Interact(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                if (itemInRange.GetCost() > playerScore)
                {
                    return;
                }
                if (itemInRange != null)
                    itemInRange.Picked();
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
            //if (blockTimer > 0)
            //{
            //    return;
            //}
            // Only the first decoration collision is working
            ShopItem shopItem = obj as ShopItem;
            if (shopItem != null) 
            {
                itemInRange = shopItem;
                return;
            }
            Tile decoration = obj as Tile;
            if (decoration != null)
            {
                float PUSH_POWER = 1f;
                Vector2 depth = RectangleExtensions.GetIntersectionDepth(box, decoration.GetBox());
                Vector2 fallback = Vector2.Zero;
                //Vector2 direction = Vector2.Normalize(position - prevPosition);

                // Check which direction we are colliding with the object
                if (position.Y >= (decoration.GetPosition().Y + decoration.GetBox().Height) ||
                        position.Y <= (decoration.GetPosition().Y - decoration.GetBox().Height))
                {
                    fallback.Y = PUSH_POWER * depth.Y;
                }
                else
                {
                    fallback.X = PUSH_POWER * depth.X;
                }

                position += fallback;
                //blockTimer = .01f;
            }
        }

        public Vector2 DeltaPosition() { return position - prevPosition; }

        internal void AddHealth(object sender, HealthArgs e)
        {
            Health += e.health;
        }

        internal void SpeedUp(object sender, PowerUpSpeed.SpeedUpArgs e)
        {
            movementSpeed *= e.speedUpIntensity;
            speedUpTimer = e.duration;
        }

        internal void BowUpgrade(object sender, BowArgs e)
        {
            bowUpgradeTimer = e.duration;
        }
        public void UpdateScore(object sender, int e)
        {
            playerScore = e;
        }
    }
}
