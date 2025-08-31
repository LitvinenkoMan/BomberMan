using Core.ScriptableObjects;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private ActorBaseParams _baseParams;

        public event Action<int> OnHealthChanged;
        public event Action OnHealthRunOut;

        public void Initialize(float initialValue)
        {
            _baseParams.SetActorHealth((int)initialValue);
        }

        public void AddHealth(int healthToAdd)
        {
            _baseParams.SetActorHealth(_baseParams.ActorHealth + healthToAdd);
            OnHealthChanged?.Invoke(healthToAdd);
        }

        public int GetHealth()
        {
            return _baseParams.ActorHealth;
        }

        public void SubtractHealth(int healthToSubtract)
        {
            _baseParams.SetActorHealth(_baseParams.ActorHealth - healthToSubtract);

            OnHealthChanged?.Invoke(_baseParams.ActorHealth);

            if (_baseParams.ActorHealth <= 0)
            {
                OnHealthRunOut?.Invoke();
            }
        }
    }
}
