using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerCharacterAnimator : MonoBehaviour, ICharacterAnimator
    {
        [SerializeField]
        private Animator pawnAnimator;
        [SerializeField]
        private float transitionSmoothness = 1;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int MoveValue = Animator.StringToHash("MoveValue");
        private static readonly int Dead = Animator.StringToHash("Dead");

        private float _currentSpeedValue;
        private float _endSpeedValue;
        private void Update()
        {
            if (_currentSpeedValue == _endSpeedValue) return;
            _currentSpeedValue = Mathf.Lerp(_currentSpeedValue, _endSpeedValue, Time.deltaTime * transitionSmoothness);

            pawnAnimator.SetFloat(MoveValue, _currentSpeedValue);
        }

        public void Initialize()
        {
            _currentSpeedValue = 0;
            _endSpeedValue = 0;
            pawnAnimator.SetBool(Dead, false);
        }

        public void PlayDeathAnimation()
        {
            pawnAnimator.SetBool(Dead, true);
        }

        public void PlayHitAnimation()
        {

        }

        public void PlayIdleAnimation()
        {
            _endSpeedValue = 0;
        }

        public void PlayKickedAnimation()
        {

        }

        public void PlayWalkAnimation()
        {
            _endSpeedValue = 1;
        }
    }
}

