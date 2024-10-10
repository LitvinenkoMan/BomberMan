using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BomberSpeedUp : PowerUp
    {
        [Header("Power up settings")]
        [Header("Power up settings")]
        [SerializeField] private byte SpeedIncreaseValue = 1; 
        
        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetSpeedMultiplier((byte)(Params.SpeedMultiplier + SpeedIncreaseValue));  
            RemovePowerUpFromGroundSection();
            base.ApplyPowerUp(Params);
        }
    }
}
