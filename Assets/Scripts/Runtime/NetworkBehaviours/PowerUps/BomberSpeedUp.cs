using Core.ScriptableObjects;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using UnityEngine;

namespace Runtime.NetworkBehaviours.PowerUps
{
    public class BomberSpeedUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private float SpeedIncreaseValue = 0.1f; 
        
        protected override void ApplyPowerUp(ICharacterUpgradable characterUpgrader)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            characterUpgrader.IncreaseMovementSpeed(SpeedIncreaseValue);
            RemovePowerUpFromGroundSection();
        }
    }
}
