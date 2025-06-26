namespace Interfaces
{
    public interface ICharacterAnimator
    {
        public void Initialize();
        public void PlayWalkAnimation();
        public void PlayDeathAnimation();
        public void PlayIdleAnimation();
    }
}
