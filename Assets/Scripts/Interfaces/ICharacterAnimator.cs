namespace Interfaces
{
    public interface ICharacterAnimator
    {
        public void Initialize();
        public void PlayWalkAnimation();
        public void PlayDeathAnimation();
        public void PlayIdleAnimation();
        public void PlayHitAnimation();
        public void PlayKickedAnimation();
    }
}
