using Core.ScriptableObjects;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerUpgraderNet : NetworkBehaviour, ICharacterUpgradable
    {
        private ICharacter _playerCharacter;
        private ICharacterRuntimeData _characterRuntimeData;

        public override void OnNetworkSpawn()
        {
            _playerCharacter = GetComponent<ICharacter>();
            _characterRuntimeData = _playerCharacter.CharacterRuntimeData;
        }

        public void IncreaseHealth(float increaseAmount)
        {
            if (IsOwner)
            {
                _playerCharacter.Heal((int)increaseAmount);
            }
        }

        public void IncreaseBombsPerTime(float increaseAmount)
        {
            if (IsOwner)
            {
                _characterRuntimeData.SetBombsAtTime(_characterRuntimeData.BombsAtTime + (int)increaseAmount);
            }
        }

        public void IncreaseBombsDamage(float increaseAmount)
        {
            if (IsOwner)
            {
                _characterRuntimeData.SetBombsDamage(_characterRuntimeData.BombsDamage + (int)increaseAmount);
            }
        }

        public void IncreaseMovementSpeed(float increaseAmount)
        {
            if (IsOwner)
            {
                _characterRuntimeData.SetSpeedMultiplier(_characterRuntimeData.SpeedMultiplier + increaseAmount);
            }
        }

        public void IncreaseBombsSpreading(float increaseAmount)
        {
            if (IsOwner)
            {
                _characterRuntimeData.SetBombsSpreading(_characterRuntimeData.BombsSpreading + (int)increaseAmount);
            }
        }
    }
}
