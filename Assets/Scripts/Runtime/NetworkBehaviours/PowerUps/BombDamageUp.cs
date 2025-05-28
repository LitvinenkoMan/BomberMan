using Interfaces;
using UnityEngine;

namespace Runtime.NetworkBehaviours.PowerUps
{
    public class BombDamageUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte BombsDamageIncreaseValue = 1; 

        protected override void ApplyPowerUp(ICharacterUpgradable characterUpgrader)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            characterUpgrader.IncreaseBombsDamage(1);  
            RemovePowerUpFromGroundSection();
        }
    }
}
