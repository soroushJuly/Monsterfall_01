using Microsoft.Xna.Framework;
using System;
using Monsterfall_01.Engine.StateManager;
using System.Threading.Tasks;

namespace Monsterfall_01.StatesEnemy
{
    internal class StateEnemyAttack : State
    {
        private float attackTimer;
        public StateEnemyAttack()
        {
            Name = "Attack";
        }
        public override void Enter(object owner)
        {
            Enemy enemy = owner as Enemy;
            if (enemy == null) { return; }
            DelayBeforeAttacking(enemy);
            attackTimer = 0.5f;
        }
        public override void Exit(object owner)
        {
            attackTimer = 0;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Enemy enemy = owner as Enemy;
            if (attackTimer < 0)
            {
                enemy.isInAttackRange = false;
            }
            attackTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (enemy == null) { return; }
        }
        private async void DelayBeforeAttacking(Enemy enemy)
        {
            await Task.Delay(500);
            enemy.currentState = Enemy.States.ATTACK;
        }
    }
}
