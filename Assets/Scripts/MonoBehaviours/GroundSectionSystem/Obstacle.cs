using System;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class Obstacle : MonoBehaviour
    {
        public byte HealthPoints { get; protected set; }
        public bool CanPlayerStepOnIt { get; protected set; }
        public bool CanReceiveDamage { get; protected set; }

        protected Action<byte> OnHealthChanged; 
        protected Action<bool> OnAbilityToStepOnObstacleChanged; 
        protected Action<bool> OnAbilityToReciveDamageChanged; 

        public void SetHealthPoints(byte newHealth)
        {
            HealthPoints = newHealth;
            OnHealthChanged.Invoke(HealthPoints);
        }

        public void SetAbilityToStepOnIt(bool state)
        {
            CanPlayerStepOnIt = state;
            OnAbilityToStepOnObstacleChanged.Invoke(CanPlayerStepOnIt);
        }

        public void SetAbilityToReceiveDamage(bool state)
        {
            CanReceiveDamage = state;
            OnAbilityToReciveDamageChanged.Invoke(CanReceiveDamage);
        }

        public virtual void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
