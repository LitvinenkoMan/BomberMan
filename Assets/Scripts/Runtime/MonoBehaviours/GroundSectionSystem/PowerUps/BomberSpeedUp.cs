using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BomberSpeedUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private float SpeedIncreaseValue = 0.1f; 
        
        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetSpeedMultiplier(Params.SpeedMultiplier + SpeedIncreaseValue);  
            RemovePowerUpFromGroundSection();
            base.ApplyPowerUp(Params);
        }
    }
}
