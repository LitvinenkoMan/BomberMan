using Core.ScriptableObjects;

namespace Interfaces
{
    public interface ICharacterAnimator
    {
        public void Initialize(ICharacterData characterData);
        public void PlayWalkAnimation();
        public void PlayDeathAnimation();
        public void PlayIdleAnimation();
        public void PlayHitAnimation();
        public void PlayKickedAnimation();
    }
}
