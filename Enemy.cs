using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine;
using Monsterfall_01.Engine.Collision;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.StateGame;
using Monsterfall_01.StatesEnemy;

namespace Monsterfall_01
{
    public class Enemy : Collidable
    {
        // Agreement (delegate) for the enemy died event: (agreement on the signiture)
        public delegate void EnemyDiedEventHandler(object owner, int value);
        // The event with the handler(agreement)
        public event EnemyDiedEventHandler EnemyDied;
        // Event fired when enemy takes hit with position of the enemy
        public event EventHandler<Vector2> EnemyHit;
        // Animation list for the enemy  
        private List<Animation> enemyAnimations;
        // The position of the enemy relative to the top left corner of the screen  
        public Vector2 Position;
        // Amount of collision enemy has with other objects
        public Vector2 CollisionOffset;
        // The amount of damage the enemy inflicts on the player  
        public int Damage;
        // The amount of score the enemy will give to the player  
        public int Value;
        // Get the width of the enemy  
        public int Width { get { return enemyAnimations[0].frameWidth; } }
        // Get the height of the enemy           
        public int Height { get { return enemyAnimations[0].frameHeight; } }
        // The speed at which the enemy moves           
        public float enemyMoveSpeed;
        // The animation which is currently playing
        public int currentAnimation;
        // Is the enemy close enough to player to chase them
        public bool isInChaseRange;
        // Is the enemy close enought to player to attack them
        public bool isInAttackRange;
        // Determines if should enemy attack or not
        public bool isAttacking;
        // The direction at which enemy is looking at. range: [0,15]
        public int directionIndex = 0;
        // time between each take damage
        TimeSpan hitTimer;
        // direction of the player from enemy position
        public Vector2 playerDirection;

        FSM fsm;
        // States associated with animations
        public enum States
        {
            WALK,
            RUN,
            ATTACK,
            DEATH
        }
        // Current state of animation
        public States currentState;
        public void Initialize(List<Animation> animations, Vector2 position)
        {
            // Load the enemy animation texture
            this.enemyAnimations = new List<Animation>();
            foreach (var animation in animations)
                enemyAnimations.Add(new Animation(animation));

            // Set the position of the enemy  
            Position = position;
            CollisionOffset = Vector2.Zero;

            // Set the amount of damage the enemy can do  
            Damage = 10;
            // Set how fast the enemy moves  
            enemyMoveSpeed = 1.7f;
            // Set the score value of the enemy  
            Value = 20;
            // Initialize animation of the enemy with current position
            currentAnimation = 0;
            enemyAnimations[currentAnimation].Position = position;
            // We assume enemy is not in the range in the beginning
            isInChaseRange = false;
            isInAttackRange = false;

            fsm = new FSM(this);

            StateEnemyIdle stateEnemyIdle = new StateEnemyIdle();
            StateEnemyChase stateEnemyChase = new StateEnemyChase();
            StateEnemyAttack stateEnemyAttack = new StateEnemyAttack();

            stateEnemyIdle.AddTransition(new Transition(stateEnemyChase, () => isInChaseRange));
            stateEnemyIdle.AddTransition(new Transition(stateEnemyAttack, () => isInAttackRange));
            stateEnemyChase.AddTransition(new Transition(stateEnemyIdle, () => !isInChaseRange && !isInAttackRange));
            stateEnemyChase.AddTransition(new Transition(stateEnemyAttack, () => !isInChaseRange && isInAttackRange));
            stateEnemyAttack.AddTransition(new Transition(stateEnemyChase, () => !isInAttackRange));

            fsm.AddState(stateEnemyIdle);
            fsm.AddState(stateEnemyChase);
            fsm.AddState(stateEnemyAttack);

            // Enemies start from idle state
            fsm.Initialise("Idle");
        }
        public void Update(GameTime gameTime)
        {
            // Update timer for next hit
            hitTimer = hitTimer.Subtract(TimeSpan.FromSeconds(gameTime.TotalGameTime.TotalSeconds));
            // Sense the distance and direction of the player
            Sense();
            // Update state of enemy after sensing
            fsm.Update(gameTime);
            // Update enemy position
            // Stand still when attacking
            if (currentState != States.ATTACK)
                Position += playerDirection + Vector2.Multiply(CollisionOffset, 0.02f);

            // collision box the enemy
            this.box = new Rectangle((int)(Position.X - Width / 5), (int)(Position.Y - Height / 5), (int)(Width / 2.5f), (int)(Height / 2.5f));
            
            // current animation index is addition of direction and index of current animation state 
            currentAnimation = directionIndex + 16 * (int)currentState;
            // Update the position of the Animation  
            enemyAnimations[currentAnimation].Position = Position;
            // Update Animation  
            enemyAnimations[currentAnimation].Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            //DrawBoundingBox(spriteBatch, GraphicsDevices);
            // Draw the animation
            enemyAnimations[currentAnimation].Draw(spriteBatch);
        }

        private void Sense()
        {
            // Get the direction from enemy to the player
            Vector2 direction = Vector2.Normalize(StateGamePlay.player.position - Position);
            playerDirection = direction;

            // 0 sprite is upwards, so 0 degree is (0,1)
            // calculate degree between vector to player and 0 vector
            double degree = Math.Acos(Vector2.Dot(-1 * direction, new Vector2(0, 1)));
            if (direction.X < 0) { degree += 2 * (Math.PI - degree); }
            // convert dergree to index in range of [0,15]
            directionIndex = (int)((int)((degree - .1f) * 180 / Math.PI) / (360.0f /16));

            // Sensing distance of the player
            Vector2 playerDistance = StateGamePlay.player.position - Position;
            isInChaseRange = (playerDistance.Length() < 400) && (playerDistance.Length() > StateGamePlay.player.Width / 2.5) ? true: false;
            isInAttackRange = playerDistance.Length() <= StateGamePlay.player.Width / 2.5 ? true: false;
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
            Arrow arrow = obj as Arrow;
            if (arrow != null)
            {
                if (hitTimer.TotalMilliseconds < 0)
                {
                    // Collision with arrow means hit
                    EnemyHit(this, Position);
                    OnEnemyDied();
                    currentState = States.DEATH;
                    // set the timer to pass the other collision for a specific time
                    hitTimer = TimeSpan.FromMilliseconds(500);
                }
            }
            CollisionOffset = Vector2.Zero;
            Enemy enemy = obj as Enemy;
            if (enemy != null)
            {
                // to prevent enemies collide with eachother and be in exact same position
                Vector2 depth = RectangleExtensions.GetIntersectionDepth(box, enemy.GetBox());
                CollisionOffset = depth;
            }
        }
        protected async void OnEnemyDied()
        {
            // wait for the death animation before removing the enemy 
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            // Check the list of subscribers
            if (EnemyDied != null)
            {
                flagForRemoval = true;
                EnemyDied(this, Value);
            }
        }
    }
}
