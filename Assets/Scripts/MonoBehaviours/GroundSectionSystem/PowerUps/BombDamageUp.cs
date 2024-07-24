using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BombDamageUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte BombsDamageIncreaseValue = 1; 

        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetBombsDamage((byte)(Params.BombsDamage + BombsDamageIncreaseValue));  
            Debug.Log($"Current Damage now: {Params.BombsDamage}");
            RemovePowerUpFromGroundSection();
        }
    }
}
