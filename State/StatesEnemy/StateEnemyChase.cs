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

            // 0 sprite is upwards then 0 degree is (0,1)

            double degree = Math.Acos(Vector2.Dot(direction, new Vector2(0, 1)));
            //enemy.distance = (float)(degree * 180 / Math.PI);
            enemy.distance = (float)(degree * 180 / Math.PI) / 16;
            // Play run animation by the direction
            // Or put the animation logic in the enemy class?
            // Other things that enemy should do in Chase state
        }
    }
}
