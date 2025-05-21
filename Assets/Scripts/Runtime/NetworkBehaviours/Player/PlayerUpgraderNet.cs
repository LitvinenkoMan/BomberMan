using Core.ScriptableObjects;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerUpgraderNet : NetworkBehaviour, ICharacterUpgradable
    {
        [SerializeField] private BaseBomberParameters playerParams;
        
        
        public void IncreaseHealth(float increaseAmount)
        {
            playerParams.SetActorHealth(playerParams.ActorHealth + (int)increaseAmount);
        }

        public void IncreaseBombsPerTime(float increaseAmount)
        {
            playerParams.SetBombsAtTime(playerParams.BombsAtTime + (int)increaseAmount);
        }

        public void IncreaseBombsDamage(float increaseAmount)
        {
            playerParams.SetBombsDamage(playerParams.BombsDamage + (int)increaseAmount);
        }

        public void IncreaseMovementSpeed(float increaseAmount)
        {
            playerParams.SetSpeedMultiplier(playerParams.SpeedMultiplier + increaseAmount);

        }

        public void IncreaseBombsSpreading(float increaseAmount)
        {
            playerParams.SetBombsSpreading(playerParams.BombsSpreading + (int)increaseAmount);

        }

        public void Reset()
        {
            playerParams.ResetValues();
        }
    }
}
