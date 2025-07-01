using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using UnityEngine;

namespace Runtime.NetworkBehaviours.PowerUps
{
    public class BombsAtTimeUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte bombsAmountIncreaseValue = 1; 
       
        protected override void ApplyPowerUp(ICharacterUpgradable characterUpgrader)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            characterUpgrader.IncreaseBombsPerTime(bombsAmountIncreaseValue);  
            RemovePowerUpFromGroundSection();
        }
    }
}
