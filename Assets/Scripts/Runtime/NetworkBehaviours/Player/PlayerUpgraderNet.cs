using Core.ScriptableObjects;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerUpgraderNet : NetworkBehaviour, ICharacterUpgradable
    {
        [SerializeField] private BaseBomberParameters playerParams;

        private IHealth _playerHealthComponent;

        public override void OnNetworkSpawn()
        {
            _playerHealthComponent = GetComponent<IHealth>();
        }

        public void IncreaseHealth(float increaseAmount)
        {
            if (IsOwner)
            {
                _playerHealthComponent.AddHealth((int)increaseAmount);
            }
        }

        public void IncreaseBombsPerTime(float increaseAmount)
        {
            if (IsOwner)
            {
                playerParams.SetBombsAtTime(playerParams.BombsAtTime + (int)increaseAmount);
            }
        }

        public void IncreaseBombsDamage(float increaseAmount)
        {
            if (IsOwner)
            {
                playerParams.SetBombsDamage(playerParams.BombsDamage + (int)increaseAmount);
            }
        }

        public void IncreaseMovementSpeed(float increaseAmount)
        {
            if (IsOwner)
            {
                playerParams.SetSpeedMultiplier(playerParams.SpeedMultiplier + increaseAmount);
            }
        }

        public void IncreaseBombsSpreading(float increaseAmount)
        {
            if (IsOwner)
            {
                playerParams.SetBombsSpreading(playerParams.BombsSpreading + (int)increaseAmount);
            }
        }

        public void Reset()
        {
            playerParams.ResetValues();
        }
    }
}
