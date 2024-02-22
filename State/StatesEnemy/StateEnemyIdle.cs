using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsterfall_01.State.StatesEnemy
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
