using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using UnityEngine;

namespace Runtime.NetworkBehaviours.PowerUps
{
    public class BomberHealthUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte HealingValue = 1; 
        
        protected override void ApplyPowerUp(ICharacterUpgradable characterUpgrader)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            characterUpgrader.IncreaseHealth(HealingValue);
            RemovePowerUpFromGroundSection();
        }
    }
}
