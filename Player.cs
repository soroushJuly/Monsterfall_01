using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine;
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
        // Animation States of the player
        public enum States
        {
            IDLE,
            RUN,
            ATTACK_BOW,
            //HIT,
            //DEATH
        }
        // Time it takes to reset movement on each direction (X,Y) to zero
        const float MOVEMENT_RESET_TIME = 0.05F;

        // FSM to change between player animations
        FSM animationManager;

        public List<Animation> playerAnimations;
        protected Animation playerAnimation;
        Dictionary<String, int> mapDirections;
        public float movementSpeed;
        const float RUN_SPEED = 4.0f;
        // Speed upgrade is 1 by default and increases when player picks the speed booster
        float speedUpgrade;
        public float scale;

        public Vector2 position;
        Vector2 prevPosition;
        public bool isActive;
        public int Health;

        int currentDirectionIndex;
        public States currentState;

        // Player timers
        float xtimer;
        float ytimer;
        float takeDamageTimer;
        float attackTimer;
        float speedUpTimer;
        float bowUpgradeTimer;

        List<String> directions;

        public int currentAnimation;

        public bool isAttacking;

        int playerScore;

        float deltaTime;

        // Item that is in the pickup range of player
        ShopItem itemInRange;

        Vector2 windowSize;

        public float GetMovementSpeed() { return (float)(speedUpgrade * movementSpeed * 60 * deltaTime); }
        public int Width
        { get { return (int)((float)playerAnimation.frameWidth * scale); } }
        public int Height
        { get { return (int)((float)playerAnimation.frameHeight * scale); } }

        public event EventHandler<Vector2> OnPlayerHit;
        public event EventHandler<Vector2> OnPlayerPowerUp;
        public void Initialize(ref List<Animation> playerAnimations, Vector2 position, Vector2 windowSize, float scale = 1.0f)
        {
            speedUpgrade = 1.0f;
            movementSpeed = RUN_SPEED;

            deltaTime = 0;

            this.position = position;
            this.prevPosition = this.position;
            this.playerAnimations = playerAnimations;
            this.directions = new List<String>();
            this.windowSize = windowSize;
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

            animationManager.AddState(statePlayerIdle);
            animationManager.AddState(statePlayerRun);
            animationManager.AddState(statePlayerAttack);
            animationManager.AddState(statePlayerHit);
            animationManager.AddState(statePlayerDeath);

            animationManager.Initialise("Idle");

            // Directions in which player can move
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
            this.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            animationManager.Update(gameTime);
            this.box = new Rectangle((int)(position.X - Width / 6), (int)position.Y - 90 / 2, Width / 3, 90);
            
            // Update timers
            xtimer -= deltaTime;
            ytimer -= deltaTime;
            speedUpTimer -= deltaTime;
            takeDamageTimer -= deltaTime;
            attackTimer -= deltaTime;
            bowUpgradeTimer -= deltaTime;
            if (speedUpTimer < 0) { speedUpgrade = 1.0f; }
            UpdateAnimation(gameTime);
        }
        private void UpdateAnimation(GameTime gameTime)
        {
            // If we have direction change the direction Index
            if (getDirection() != "")
                currentDirectionIndex = mapDirections[getDirection()];

            // current animation is sum of direction index and animation state index
            currentAnimation = currentDirectionIndex + 8 * (int)currentState;
            playerAnimations[currentAnimation].Position = position;
            playerAnimations[currentAnimation].Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            checkTimers();
            playerAnimations[currentAnimation].Draw(spriteBatch);
        }

        public void moveNorth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("NORTH");
                this.prevPosition.Y = position.Y;
                this.position.Y -= GetMovementSpeed();
            }
        }
        public void moveEast(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("EAST");
                this.prevPosition.X = position.X;
                this.position.X += GetMovementSpeed();
            }
        }
        public void moveSouth(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setYTimer("SOUTH");
                this.prevPosition.Y = position.Y;
                this.position.Y += GetMovementSpeed();
            }
        }
        public void moveWest(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                setXTimer("WEST");
                this.prevPosition.X = position.X;
                this.position.X -= GetMovementSpeed();
            }
        }
        private void checkTimers()
        {
            // Reset the movement if player hasn't pressed move keys
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
        // Set Direction and timer in Y Axis
        private void setXTimer(String direction)
        {
            directions[1] = direction;
            xtimer = MOVEMENT_RESET_TIME;
        }
        // Set Direction and timer in X Axis
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
                // TODO: make this a helper function
                Vector2 direction = Vector2.Normalize(new Vector2(windowSize.X / 2, windowSize.Y / 2) - amount);
                double degree = Math.Acos(Vector2.Dot(direction, new Vector2(0, 1)));
                currentDirectionIndex = (int)((degree * 180 / Math.PI) / (360.0f / 8));
                if (direction.X > 0)
                {
                    currentDirectionIndex = (int)(((2 * Math.PI - degree) * 180 / Math.PI) / (360.0f / 8));
                    degree += 2 * (Math.PI - degree);
                }

                if (bowUpgradeTimer > 0)
                {
                    StateGamePlay.arrowList.Add(new Arrow(this.position, degree));
                    StateGamePlay.arrowList.Add(new Arrow(this.position, degree + 0.25));
                    StateGamePlay.arrowList.Add(new Arrow(this.position, degree - 0.25));
                }
                else
                    StateGamePlay.arrowList.Add(new Arrow(this.position, degree));

                isAttacking = true;
                attackTimer = .5f;
            }
        }
        public void Interact(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                // Check if Item is in pickup range
                if (itemInRange != null)
                {
                    // Check if player has enough score
                    if (itemInRange.GetCost() > playerScore)
                    {
                        return;
                    }
                    OnPlayerPowerUp(this, position);
                    itemInRange.Picked();
                }
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
            // If enemy is attacking and is colliding, player takes damage
            if (enemy != null)
            {
                if (takeDamageTimer < 0 && (enemy.isAttacking))
                {
                    TakeDamage(enemy);
                    takeDamageTimer = 1.0f;
                }
            }

            // Only the first decoration collision is working
            ShopItem shopItem = obj as ShopItem;
            if (shopItem != null)
            {
                itemInRange = shopItem;
                return;
            }
            // Colllision and block with decorations
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
            }
        }

        public Vector2 DeltaPosition() { return position - prevPosition; }

        internal void AddHealth(object sender, HealthArgs e)
        {
            Health += e.health;
        }

        internal void SpeedUp(object sender, PowerUpSpeed.SpeedUpArgs e)
        {
            if (speedUpTimer > 0)
                return;
            speedUpgrade = e.speedUpIntensity;
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
        private async void TakeDamage(Enemy enemy)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            OnPlayerHit(this, position);
            Health -= enemy.Damage;
        }
    }
}
