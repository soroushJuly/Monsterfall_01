using Microsoft.Xna.Framework;
using System;
using Monsterfall_01.Engine.StateManager;


namespace Monsterfall_01.StatesEnemy
{
    internal class StateEnemyChase : State
    {
        public StateEnemyChase()
        {
            Name = "Chase";
        }
        public override void Enter(object owner)
        {
            Enemy enemy = owner as Enemy;
            if (enemy == null) { return; }
            enemy.currentState = Enemy.States.RUN;
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
