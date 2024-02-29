using Microsoft.Xna.Framework;
using Monsterfall_01.Engine.StateManager;
using System;

namespace Monsterfall_01.StatesPlayer
{
    internal class StatePlayerRun : State
    {
        public StatePlayerRun()
        {
            Name = "Run";
        }
        public override void Enter(object owner)
        {
            Player player = owner as Player;
            if (player == null) { return; }
            player.currentState = Player.States.RUN;
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
