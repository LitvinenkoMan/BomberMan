using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.PowerUps
{
    public class BomberHealthUp : PowerUp
    {
        [Header("Power up settings")]
        [SerializeField] private byte HealingValue = 1; 
        
        protected override void ApplyPowerUp(BaseBomberParameters Params)
        {
            Visuals.SetActive(false);
            _isTaken = true;
            Params.SetActorHealth((byte)(Params.ActorHealth + HealingValue));
            RemovePowerUpFromGroundSection();
            base.ApplyPowerUp(Params);
        }
    }
}
