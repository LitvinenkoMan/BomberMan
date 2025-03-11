using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PawnAnimationHandler : MonoBehaviour
    {
        [SerializeField] 
        private Animator PawnAnimator;
        [SerializeField]
        private float TransitionTime;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int MoveValue = Animator.StringToHash("MoveValue");
        private static readonly int Dead = Animator.StringToHash("Dead");


        private void Update()
        {
            
        }

        public void PlayMoveAnimation()
        {
            PlayMoveAnimation(1, TransitionTime);
        }

        public void PlayIdleAnimation()
        {

            PlayMoveAnimation(0, TransitionTime);
        }
        
        public void PlayDeathAnimation(bool isDead)
        {
            
            PawnAnimator.SetBool(Dead, isDead);
        }

        private async UniTask PlayMoveAnimation(float endValue, float timeInSec)
        {
            var startValue = PawnAnimator.GetFloat(MoveValue);
            
            while (true)
            {
                if (startValue == endValue) return;
                
                if (startValue > endValue)
                {
                    startValue -= Time.deltaTime / timeInSec;
                }
                else startValue += Time.deltaTime / timeInSec;
                
                PawnAnimator.SetFloat(MoveValue, startValue);
                await UniTask.WaitForEndOfFrame();
            }
        }
        
    }
}
