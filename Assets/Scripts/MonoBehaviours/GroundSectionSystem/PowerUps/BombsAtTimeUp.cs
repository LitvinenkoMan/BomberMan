using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BombsAtTimeUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte BombsAmountIncreaseValue = 1; 
       
        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetBombsAtTime((byte)(Params.BombsAtTime + BombsAmountIncreaseValue));  
            RemovePowerUpFromGroundSection();
            Destroy(gameObject);
        }
    }
}
