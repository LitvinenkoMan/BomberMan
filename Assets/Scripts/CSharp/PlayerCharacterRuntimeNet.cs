using System;
using Core.EventBuses;
using Core.ScriptableObjects;
using Interfaces;
using Unity.Netcode;

namespace CSharp
{
    [Serializable]
    public class PlayerCharacterRuntimeNet : NetworkBehaviour, ICharacterRuntimeData
    {
        private NetworkVariable<int> _characterHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<float> _speedMultiplier = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<float> _bombsCountdown = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<int> _bombsAtTime = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<int> _bombsSpreading = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<int> _bombsDamage = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<float> _kickForce = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public int CharacterHealth => _characterHealth.Value;

        public float SpeedMultiplier => _speedMultiplier.Value;

        public float BombsCountdown => _bombsCountdown.Value;

        public int BombsAtTime => _bombsAtTime.Value;

        public int BombsSpreading => _bombsSpreading.Value;

        public int BombsDamage => _bombsDamage.Value;

        public float KickForce => _kickForce.Value;

        public override void OnNetworkSpawn()
        {
            _characterHealth.OnValueChanged += OnHealthChanged;
            _speedMultiplier.OnValueChanged += OnSpeedMultiplierChanged;
            _bombsCountdown.OnValueChanged += OnBombsCountdownChanged;
            _bombsAtTime.OnValueChanged += OnBombsAtTimeChanged;
            _bombsSpreading.OnValueChanged += OnBombsSpreadingChanged;
            _bombsDamage.OnValueChanged += OnBombsDamageChanged;
            _kickForce.OnValueChanged += OnKickForceChanged;
        }

        public override void OnNetworkDespawn()
        {
            _characterHealth.OnValueChanged -= OnHealthChanged;
            _speedMultiplier.OnValueChanged -= OnSpeedMultiplierChanged;
            _bombsCountdown.OnValueChanged -= OnBombsCountdownChanged;
            _bombsAtTime.OnValueChanged -= OnBombsAtTimeChanged;
            _bombsSpreading.OnValueChanged -= OnBombsSpreadingChanged;
            _bombsDamage.OnValueChanged -= OnBombsDamageChanged;
            _kickForce.OnValueChanged -= OnKickForceChanged;
        }

        public void Initialize(ICharacterData  characterData)
        {
            _characterHealth.Value = characterData.Health;
            _speedMultiplier.Value = characterData.Speed;
            _bombsCountdown.Value = characterData.BombCountdown;
            _bombsAtTime.Value = characterData.BombsAtTime;
            _bombsSpreading.Value = characterData.BombSpread;
            _bombsDamage.Value = characterData.BombDamage;
            _kickForce.Value = characterData.KickForce;
        }

        public void AddHealth(int healthToAdd)
        {
            _characterHealth.Value += healthToAdd;
            if (_characterHealth.Value <= 0)
            {
                OnHealthRunOut(_characterHealth.Value);
            }
        }

        public void SubtractHealth(int healthToSubtract)
        {
            _characterHealth.Value -= healthToSubtract;
        }

        public void SetSpeedMultiplier(float speedMultiplier)
        {
            _speedMultiplier.Value = speedMultiplier;
        }

        public void SetBombsCountdown(float bombsCountdown)
        {
            _bombsCountdown.Value = bombsCountdown;
        }

        public void SetBombsAtTime(int bombsAtTime)
        {
            _bombsAtTime.Value = bombsAtTime;
        }

        public void SetBombsSpreading(int bombsSpreading)
        {
            _bombsSpreading.Value = bombsSpreading;
        }

        public void SetBombsDamage(int bombsDamage)
        {
            _bombsDamage.Value = bombsDamage;
        }

        public void SetKickForce(float kickForce)
        {
            _kickForce.Value = kickForce;
        }
        
        public void OnHealthChanged(int prev, int current) => GameplayUIEvents.Instance.RiseOnHealthChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        public void OnSpeedMultiplierChanged(float prev, float current) => GameplayUIEvents.Instance.RiseOnSpeedMultiplierChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        public void OnBombsCountdownChanged(float prev, float current) => GameplayUIEvents.Instance.RiseOnBombsCountdownChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        public void OnBombsAtTimeChanged(int prev, int current) => GameplayUIEvents.Instance.RiseOnBombsAtTimeChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        public void OnBombsSpreadingChanged(int prev, int current) => GameplayUIEvents.Instance.RiseOnBombsSpreadingChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        public void OnBombsDamageChanged(int prev, int current) => GameplayUIEvents.Instance.RiseOnBombsDamageChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        public void OnKickForceChanged(float prev, float current) => GameplayUIEvents.Instance.RiseOnKickForceChangedEvent(NetworkManager.Singleton.LocalClientId, prev, current);
        
        public void OnHealthRunOut(int current) => GameplayUIEvents.Instance.RiseOnHealthRunOutEvent(NetworkManager.Singleton.LocalClientId, current);
    }
}