using System;
using UnityEngine;

namespace Core.EventBuses
{
    public class GameplayUIEvents : MonoBehaviour
    {
        public static GameplayUIEvents Instance;

        public event Action<ulong, int, int> OnHealthChanged;
        public event Action<ulong, float, float> OnSpeedMultiplierChanged;
        public event Action<ulong, float, float> OnBombsCountdownChanged;
        public event Action<ulong, int, int> OnBombsAtTimeChanged;
        public event Action<ulong, int, int> OnBombsSpreadingChanged;
        public event Action<ulong, int, int> OnBombsDamageChanged;
        public event Action<ulong, float, float> OnKickForceChanged;
        
        public event Action<ulong, float> OnAbilityUsed;
        public event Action<ulong, int> OnHealthRunOut;
        
        public event Action<ulong> OnPlayerConnected;
        
        private void Awake()
        {
            if (Instance ==  null)
            {
                Instance = this;
            }
            else Destroy(gameObject);
        }

        public void RiseOnHealthChangedEvent(ulong playerId, int prev, int current) => OnHealthChanged?.Invoke(playerId, prev, current);
        public void RiseOnSpeedMultiplierChangedEvent(ulong playerId, float prev, float current) => OnSpeedMultiplierChanged?.Invoke(playerId, prev, current);
        public void RiseOnBombsCountdownChangedEvent(ulong playerId, float prev, float current) => OnBombsCountdownChanged?.Invoke(playerId, prev, current);
        public void RiseOnBombsAtTimeChangedEvent(ulong playerId, int prev, int current) => OnBombsAtTimeChanged?.Invoke(playerId, prev, current);
        public void RiseOnBombsSpreadingChangedEvent(ulong playerId, int prev, int current) => OnBombsSpreadingChanged?.Invoke(playerId, prev, current);
        public void RiseOnBombsDamageChangedEvent(ulong playerId, int prev, int current) => OnBombsDamageChanged?.Invoke(playerId, prev, current);
        public void RiseOnKickForceChangedEvent(ulong playerId, float prev, float current) => OnKickForceChanged?.Invoke(playerId, prev, current);
        
        public void RiseOnAbilityUsedEvent(ulong playerId, float timer) => OnAbilityUsed?.Invoke(playerId, timer);
        public void RiseOnHealthRunOutEvent(ulong playerId, int last) => OnHealthRunOut?.Invoke(playerId,  last);
    }
}
