using Interfaces;
using Unity.Netcode;

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
                IncreaseBombsPerTimeRpc(increaseAmount);
            }
        }

        public void IncreaseBombsDamage(float increaseAmount)
        {
            if (IsOwner)
            {
                IncreaseBombsDamageRpc(increaseAmount);
            }
        }

        public void IncreaseMovementSpeed(float increaseAmount)
        {
            if (IsOwner)
            {
                IncreaseMovementSpeedRpc(increaseAmount);
            }
        }

        public void IncreaseBombsSpreading(float increaseAmount)
        {
            if (IsOwner)
            {
                IncreaseBombsSpreadingRpc(increaseAmount);
            }
        }

        [Rpc(SendTo.Server)]
        private void IncreaseHealthRpc(float increaseAmount)
        {
            _playerCharacter.Heal((int)increaseAmount);
        }

        [Rpc(SendTo.Server)]
        private void IncreaseBombsPerTimeRpc(float increaseAmount)
        {
            _characterRuntimeData.SetBombsAtTime(_characterRuntimeData.BombsAtTime + (int)increaseAmount);
        }

        [Rpc(SendTo.Server)]
        private void IncreaseBombsDamageRpc(float increaseAmount)
        {
            _characterRuntimeData.SetBombsDamage(_characterRuntimeData.BombsDamage + (int)increaseAmount);
            
        }

        [Rpc(SendTo.Server)]
        private void IncreaseMovementSpeedRpc(float increaseAmount)
        {
            _characterRuntimeData.SetSpeedMultiplier(_characterRuntimeData.SpeedMultiplier + increaseAmount);
        }

        [Rpc(SendTo.Server)]
        private void IncreaseBombsSpreadingRpc(float increaseAmount)
        {
            _characterRuntimeData.SetBombsSpreading(_characterRuntimeData.BombsSpreading + (int)increaseAmount);
        }
    }
}
