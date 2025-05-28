using System;
using Interfaces;
using UnityEngine;

namespace Runtime.MonoBehaviours.GroundSectionSystem
{
    public class ObstacleHealthComponent : MonoBehaviour, IHealth
    {
        
        public bool CanReceiveDamage { get; protected set; }
        
        public event Action<int> OnHealthChanged;
        public event Action OnHealthRunOut;
        
        protected event Action<bool> OnAbilityToReciveDamageChanged;
        
        private int _health;

        public ObstacleHealthComponent() { }

        public void Initialize(float initialValue)
        {
            _health = (int)initialValue;
        }

        public void AddHealth(int healthToAdd)
        {
            _health += healthToAdd;
            OnHealthChanged?.Invoke(_health);
        }

        public void SubtractHealth(int healthToSubtract)
        {
            if (!CanReceiveDamage) return;
            
            _health -= healthToSubtract;
            OnHealthChanged?.Invoke(_health);
            if (_health <= 0)
            {
                OnHealthRunOut?.Invoke();
            }
        }

        public int GetHealth()
        {
            return _health;
        }
        
        public void SetAbilityToReceiveDamage(bool state)
        {
            CanReceiveDamage = state;
            OnAbilityToReciveDamageChanged?.Invoke(CanReceiveDamage);
        }
    }
}
