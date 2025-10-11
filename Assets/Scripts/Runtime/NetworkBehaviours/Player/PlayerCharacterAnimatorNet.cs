using Core.ScriptableObjects;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterAnimatorNet : NetworkBehaviour, ICharacterAnimator
    {
        [SerializeField]
        private float transitionSmoothness = 1;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int MoveValue = Animator.StringToHash("MoveValue");
        private static readonly int Dead = Animator.StringToHash("Dead");
        
        private Animator _pawnAnimator;

        private float _currentSpeedValue;
        private float _endSpeedValue;

        void Update()
        {
            if (_currentSpeedValue == _endSpeedValue) return;
            _currentSpeedValue = Mathf.Lerp(_currentSpeedValue, _endSpeedValue, Time.deltaTime * transitionSmoothness);
            
            _pawnAnimator.SetFloat(MoveValue, _currentSpeedValue);
        }

        public void Initialize(ICharacterData characterData)
        {
            _currentSpeedValue = 0;
            _endSpeedValue = 0;
            
            _pawnAnimator = GetComponentInChildren<Animator>();
            
            _pawnAnimator.runtimeAnimatorController = characterData.AnimatorController;
            _pawnAnimator.avatar = characterData.Avatar;
            
            _pawnAnimator.SetBool(Dead, false);
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

        public void PlayHitAnimation()
        {
            
        }

        public void PlayKickedAnimation()
        {
            
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
            _pawnAnimator.SetBool(Dead, true);
        }
    }
}
