using System;
using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BombsSpreadingUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte SpreadingIncreaseValue; 
        void Start()
        {
            ObstacleHealthComponent.OnHealthRunOut += RemovePowerUpFromGroundSection;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BomberParamsProvider provider) && !_isTaken)
            {
                ApplyPowerUp(provider.GetBomberParams());
            }
        }

        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetBombsSpreading((byte)(Params.BombsSpreading + SpreadingIncreaseValue));
            RemovePowerUpFromGroundSection();
        }
    }
}
