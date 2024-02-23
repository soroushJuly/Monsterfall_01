using Microsoft.Xna.Framework;
using Monsterfall_01.StateManager;
using System;

namespace Monsterfall_01.StatesPlayer
{
    internal class StatePlayerHit : State
    {
        public StatePlayerHit()
        {
            Name = "Hit";
        }
        public override void Enter(object owner)
        {
            Player player = owner as Player;
            if (player == null) { return; }

            player.currentState = Player.States.HIT;
        }
        public override void Exit(object owner)
        {
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Player player = owner as Player;
            if (player == null) { return; }

        }
    }
}
