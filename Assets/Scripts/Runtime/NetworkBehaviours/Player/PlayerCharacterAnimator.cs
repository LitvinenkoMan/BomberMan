using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterAnimator : NetworkBehaviour, ICharacterAnimator
    {
        [SerializeField] 
        private Animator PawnAnimator;
        [SerializeField]
        private float transitionSmoothness = 1;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int MoveValue = Animator.StringToHash("MoveValue");
        private static readonly int Dead = Animator.StringToHash("Dead");


        private float _currentSpeedValue;
        private float _endSpeedValue;

        void Update()
        {
            if (_currentSpeedValue == _endSpeedValue) return;
            _currentSpeedValue = Mathf.Lerp(_currentSpeedValue, _endSpeedValue, Time.deltaTime * transitionSmoothness);
            
            PawnAnimator.SetFloat(MoveValue, _currentSpeedValue);
        }

        public void Initialize()
        {
            _currentSpeedValue = 0;
            _endSpeedValue = 0;
            PawnAnimator.SetBool(Dead, false);
        }

        public void PlayWalkAnimation()
        {
            PlayWalkAnimationRpc();
        }

        public void PlayDeathAnimation()
        {
            PlayDeathAnimationRpc();
        }

        public void PlayIdleAnimation()
        {
            PlayIdleAnimationRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayWalkAnimationRpc()
        {
            _endSpeedValue = 1;
        }

        [Rpc(SendTo.Everyone)]
        private void PlayIdleAnimationRpc()
        {
            _endSpeedValue = 0;
        }

        [Rpc(SendTo.Everyone)]
        private void PlayDeathAnimationRpc()
        {
            PawnAnimator.SetBool(Dead, true);
        }
    }
}
