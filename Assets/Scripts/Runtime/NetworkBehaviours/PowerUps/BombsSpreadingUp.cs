using Core.ScriptableObjects;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BombsSpreadingUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte SpreadingIncreaseValue = 1;

        protected override void ApplyPowerUp(ICharacterUpgradable characterUpgrader)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            characterUpgrader.IncreaseBombsSpreading(SpreadingIncreaseValue);
            RemovePowerUpFromGroundSection();
        }
    }
}
