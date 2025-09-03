using Core.ScriptableObjects;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerUpgrader : MonoBehaviour, ICharacterUpgradable
    {
        private CharacterRuntimeData _characterRuntimeData;

        public void IncreaseBombsDamage(float increaseAmount)
        {
            _characterRuntimeData.SetBombsDamage(_characterRuntimeData.BombsDamage + (int)increaseAmount);
        }

        public void IncreaseBombsPerTime(float increaseAmount)
        {
            _characterRuntimeData.SetBombsAtTime(_characterRuntimeData.BombsAtTime + (int)increaseAmount);
        }

        public void IncreaseBombsSpreading(float increaseAmount)
        {
            _characterRuntimeData.SetBombsSpreading(_characterRuntimeData.BombsSpreading + (int)increaseAmount);
        }

        public void IncreaseHealth(float increaseAmount)
        {
            _characterRuntimeData.AddHealth((int)increaseAmount);
        }

        public void IncreaseMovementSpeed(float increaseAmount)
        {
            _characterRuntimeData.SetSpeedMultiplier(_characterRuntimeData.SpeedMultiplier + increaseAmount);
        }

        public void Reset()
        {
            
        }
    }
}

