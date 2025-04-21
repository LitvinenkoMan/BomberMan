using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PawnAnimationHandler : MonoBehaviour
    {
        [SerializeField] 
        private CharacterController Controller;
        [SerializeField] 
        private Animator PawnAnimator;
        [SerializeField]
        private float TransitionTime;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int MoveValue = Animator.StringToHash("MoveValue");
        private static readonly int Dead = Animator.StringToHash("Dead");

        private void Update()
        {
            var direction = Controller.velocity;
            direction = direction.normalized;
            if (direction.x != 0 || direction.z != 0)
            {
                PlayMoveAnimation();
            }
            else PlayIdleAnimation();
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
            
            if (startValue == endValue) return;
            
            while (true)
            {
                if (startValue > endValue)
                {
                    startValue -= Time.deltaTime / timeInSec;
                }
                else startValue += Time.deltaTime / timeInSec;
                
                PawnAnimator.SetFloat(MoveValue, startValue);
                if (startValue == endValue) return;
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}
