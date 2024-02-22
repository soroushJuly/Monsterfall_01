using Microsoft.Xna.Framework;
using System;

namespace Monsterfall_01.State.StatesEnemy
{
    internal class StateEnemyAttack : State
    {
        public StateEnemyAttack()
        {
            Name = "Attack";
        }
        public override void Enter(object owner)
        {
            Enemy enemy = owner as Enemy;
            if (enemy == null) { return; }
            enemy.currentState = Enemy.States.ATTACK;
        }
        public override void Exit(object owner)
        {
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Enemy enemy = owner as Enemy;

            if (enemy == null) { return; }
        }
    }
}
