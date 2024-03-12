using Microsoft.Xna.Framework;
using System;
using Monsterfall_01.Engine.StateManager;

namespace Monsterfall_01.StatesEnemy
{
    internal class StateEnemyAttack : State
    {
        // Time between each attack
        private float attackIntervalTimer;
        private const float ATTACK_INTERVAL = 1.3f;
        public StateEnemyAttack()
        {
            Name = "Attack";
        }
        public override void Enter(object owner)
        {
            Enemy enemy = owner as Enemy;
            if (enemy == null) { return; }
            enemy.currentState = Enemy.States.ATTACK;
            attackIntervalTimer = ATTACK_INTERVAL;
        }
        public override void Exit(object owner)
        {
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Enemy enemy = owner as Enemy;
            if (enemy == null) { return; }

            attackIntervalTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // If timer reached zero then attack
            if (attackIntervalTimer < 0.0f)
            {
                enemy.isAttacking = true;
                attackIntervalTimer = ATTACK_INTERVAL;
                return;
            }
            enemy.isAttacking = false;
        }
    }
}
