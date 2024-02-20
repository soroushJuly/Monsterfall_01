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
            Enemy enemy = owner as Enemy;

            if (enemy == null) { return; }
            enemy.currentAnimation = 1;

            Vector2 direction = Vector2.Normalize(Game1.player.position - enemy.Position);
            enemy.Position += direction; 
            
            // Play run animation by the direction
            // Or put the animation logic in the enemy class?
            // Other things that enemy should do in Chase state
        }
    }
}
