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
        public event EventHandler<Vector2> EnemyHit;
        // Animation list for the enemy  
        private List<Animation> enemyAnimations;
        // The position of the enemy ship relative to the top left corner of the screen  
        public Vector2 Position;
        public Vector2 CollisionOffset;
        // The state of the Enemy Ship  
        public bool isActive;
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
        public float enemyMoveSpeed;

        public int currentAnimation;

        public bool isInChaseRange;
        public bool isInAttackRange;

        public int directionIndex = 0;

        private TimeSpan hitTimer;

        FSM fsm;
        // States associated with animations
        public enum States
        {
            WALK,
            RUN,
            ATTACK,
            DEATH,
            HIT
        }

        public States currentState;
        public void Initialize(List<Animation> animations, Vector2 position)
        {
            // Load the enemy ship texture
            this.enemyAnimations = new List<Animation>();
            foreach (var animation in animations)
            {
                enemyAnimations.Add(new Animation(animation));
            }

            // Set the position of the enemy  
            Position = position;
            CollisionOffset = Vector2.Zero;
            // We initialize the enemy to be active so it will be update in the game  
            isActive = true;
            // Set the health of the enemy  
            Health = 10;
            // Set the amount of damage the enemy can do  
            Damage = 10;
            // Set how fast the enemy moves  
            enemyMoveSpeed = 1.7f;
            // Set the score value of the enemy  
            Value = 20;

            currentAnimation = 0;
            enemyAnimations[currentAnimation].Position = position;
            isInChaseRange = false;
            isInAttackRange = false;

            fsm = new FSM(this);

            StateEnemyIdle stateEnemyIdle = new StateEnemyIdle();
            StateEnemyChase stateEnemyChase = new StateEnemyChase();
            StateEnemyAttack stateEnemyAttack = new StateEnemyAttack();

            stateEnemyIdle.AddTransition(new Transition(stateEnemyChase, () => isInChaseRange));
            stateEnemyChase.AddTransition(new Transition(stateEnemyIdle, () => !isInChaseRange && !isInAttackRange));
            stateEnemyChase.AddTransition(new Transition(stateEnemyAttack, () => isInAttackRange));
            stateEnemyAttack.AddTransition(new Transition(stateEnemyChase, () => !isInAttackRange));

            fsm.AddState(stateEnemyIdle);
            fsm.AddState(stateEnemyChase);
            fsm.AddState(stateEnemyAttack);

            fsm.Initialise("Idle");
        }
        public void Update(GameTime gameTime)
        {
            Sense();
            fsm.Update(gameTime);
            hitTimer = hitTimer.Subtract(TimeSpan.FromSeconds(gameTime.TotalGameTime.TotalSeconds));
            currentAnimation = directionIndex + 16 * (int)currentState;
            this.box = new Rectangle((int)(Position.X - Width / 5), (int)(Position.Y - Height / 5), (int)(Width / 2.5f), (int)(Height / 2.5f));
            // The enemy always moves to the left so decrement it's x position  
            
            // Update the position of the Animation  
            enemyAnimations[currentAnimation].Position = Position;
            // Update Animation  
            enemyAnimations[currentAnimation].Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            // Draw the animation
            //DrawBoundingBox(spriteBatch, GraphicsDevices);
            enemyAnimations[currentAnimation].Draw(spriteBatch);
        }

        private void Sense()
        {
            // Sensing the direction of the player
            Vector2 direction = Vector2.Normalize(StateGamePlay.player.position - Position);
            if (!isInAttackRange)
                Position += direction + Vector2.Multiply(CollisionOffset, 0.02f);

            // 0 sprite is upwards then 0 degree is (0,1)
            double degree = Math.Acos(Vector2.Dot(-1 * direction, new Vector2(0, 1)));
            if (direction.X < 0) { degree += 2 * (Math.PI - degree); }
            directionIndex = (int)((degree * 180 / Math.PI) / (360.0f /16));

            // Sensing distance of the player
            Vector2 playerDistance = StateGamePlay.player.position - Position;
            isInChaseRange = playerDistance.Length() < 400 ? true: false;
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
                    EnemyHit(this, Position);
                    OnEnemyDied();
                    currentState = States.DEATH;
                    hitTimer = TimeSpan.FromMilliseconds(1000);
                }
            }
            Player player = obj as Player;
            CollisionOffset = Vector2.Zero;
            if (player != null)
            {
                isInAttackRange = true;
                //Vector2 depth = RectangleExtensions.GetIntersectionDepth(box, player.GetBox());
                //CollisionOffset = depth;
            }
            Enemy enemy = obj as Enemy;
            if (enemy != null)
            {
                Vector2 depth = RectangleExtensions.GetIntersectionDepth(box, enemy.GetBox());
                CollisionOffset = depth;
            }
        }
        protected async void OnEnemyDied()
        {
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
