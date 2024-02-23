using Microsoft.Xna.Framework;
using System;
using Monsterfall_01.StateManager;


namespace Monsterfall_01.StatesEnemy
{
    public class StateEnemyIdle : State
    {
        public StateEnemyIdle()
        {
            Name = "Idle";
        }
        public override void Enter(object owner)
        {
            Enemy enemy = owner as Enemy;
            if (enemy == null) { return; }
            enemy.currentState = Enemy.States.WALK;
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
