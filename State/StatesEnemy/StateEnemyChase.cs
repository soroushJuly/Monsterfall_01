using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsterfall_01.State.StatesEnemy
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
            //if
        }
        public override void Exit(object owner)
        {
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            // Play run animation
            // Other things that enemy should do in Chase state
        }
    }
}
