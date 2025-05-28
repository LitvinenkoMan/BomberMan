namespace Interfaces
{
    public interface ICharacterUpgradable
    {
        public void IncreaseHealth(float increaseAmount);
        public void IncreaseBombsPerTime(float increaseAmount);
        public void IncreaseBombsDamage(float increaseAmount);
        public void IncreaseMovementSpeed(float increaseAmount);
        public void IncreaseBombsSpreading(float increaseAmount);
        public void Reset();
    }
}
