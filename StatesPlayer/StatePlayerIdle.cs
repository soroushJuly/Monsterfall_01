using Microsoft.Xna.Framework;
using System;
using Monsterfall_01.StateManager;


namespace Monsterfall_01.StatesPlayer
{
    internal class StatePlayerIdle : State
    {
        public StatePlayerIdle()
        {
            Name = "Idle";
        }
        public override void Enter(object owner)
        {
            Player player = owner as Player;
            if (player == null) { return; }
            player.isAttacking = false;
            player.currentState = Player.States.IDLE;
        }
        public override void Exit(object owner)
        {
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            //Enemy enemy = owner as Enemy;

            //if (enemy == null) { return; }
        }
    }
}
