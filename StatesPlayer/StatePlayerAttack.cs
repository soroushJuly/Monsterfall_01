using Microsoft.Xna.Framework;
using Monsterfall_01.Engine.StateManager;
using System;

namespace Monsterfall_01.StatesPlayer
{
    internal class StatePlayerAttack : State
    {
        public StatePlayerAttack()
        {
            Name = "Attack";
        }
        public override void Enter(object owner)
        {
            Player player = owner as Player;
            if (player == null) { return; }

            player.currentState = Player.States.ATTACK_BOW;
        }
        public override void Exit(object owner)
        {
            Player player = owner as Player;
            if (player == null) { return; }

            player.isAttacking = false;
            // Activating the animation again
            // so we can play it again next time on shooting arrow
            player.playerAnimations[player.currentAnimation].Active = true;
            player.movementSpeed = 4f;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Player player = owner as Player;
            if (player == null) { return; }

            player.movementSpeed = .4f;
        }
    }
}
