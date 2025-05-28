using System;

namespace Interfaces
{
    public interface IHealth
    {
        public event Action<int> OnHealthChanged;
        
        public event Action OnHealthRunOut;

        public void Initialize(float initialValue);
        public void AddHealth(int healthToAdd);
        public void SubtractHealth(int healthToSubtract);
        public int GetHealth();
    }
}
