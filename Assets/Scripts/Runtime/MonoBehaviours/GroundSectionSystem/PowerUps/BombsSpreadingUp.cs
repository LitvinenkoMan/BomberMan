using System;
using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BombsSpreadingUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte SpreadingIncreaseValue = 1;

        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetBombsSpreading(Params.BombsSpreading + SpreadingIncreaseValue);
            RemovePowerUpFromGroundSection();
            base.ApplyPowerUp(Params);
        }
    }
}
