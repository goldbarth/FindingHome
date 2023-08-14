using FiniteStateMachine.Base;
using NpcSettings;
using UnityEngine;
using Player;

namespace FiniteStateMachine.FollowPlayer.Transitions
{
    public class HasPlayerEdible : Transition
    {
        private readonly PlayerController _player;

        public HasPlayerEdible(PlayerController player)
        {
            _player = player;
        }

        public override bool OnCanTransitionTo()
        {
            return HasEdible();
        }
        
        private bool HasEdible()
        {
            return _player.GetEatablesCount > 0;
        }
    }
}