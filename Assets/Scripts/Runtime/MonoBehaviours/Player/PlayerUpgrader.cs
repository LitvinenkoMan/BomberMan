using Core.ScriptableObjects;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerUpgrader : MonoBehaviour, ICharacterUpgradable
    {
        [SerializeField] private BaseBomberParameters playerParams;

        private IHealth _playerHealthComponent;

        private void Start()
        {
            _playerHealthComponent = GetComponent<IHealth>();
        }
        public void IncreaseBombsDamage(float increaseAmount)
        {
            playerParams.SetBombsDamage(playerParams.BombsDamage + (int)increaseAmount);
        }

        public void IncreaseBombsPerTime(float increaseAmount)
        {
            playerParams.SetBombsAtTime(playerParams.BombsAtTime + (int)increaseAmount);
        }

        public void IncreaseBombsSpreading(float increaseAmount)
        {
            playerParams.SetBombsSpreading(playerParams.BombsSpreading + (int)increaseAmount);
        }

        public void IncreaseHealth(float increaseAmount)
        {
            _playerHealthComponent.AddHealth((int)increaseAmount);
        }

        public void IncreaseMovementSpeed(float increaseAmount)
        {
            playerParams.SetSpeedMultiplier(playerParams.SpeedMultiplier + increaseAmount);
        }

        public void Reset()
        {
            playerParams.ResetValues();
        }
    }
}

