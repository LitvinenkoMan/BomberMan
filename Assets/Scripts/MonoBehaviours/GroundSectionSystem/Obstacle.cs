using System;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class Obstacle : MonoBehaviour
    {
        public HealthComponent ObstacleHealthComponent;
        public bool CanReceiveDamage { get; protected set; }
        public bool CanPlayerStepOnIt { get; protected set; }

        protected Action<bool> OnAbilityToReciveDamageChanged;
        protected Action<bool> OnAbilityToPlayerCanStepOnIt;

        private void Start()
        {
            ObstacleHealthComponent = GetComponent<HealthComponent>();
        }

        public void SetAbilityToReceiveDamage(bool state)
        {
            CanReceiveDamage = state;
            OnAbilityToReciveDamageChanged?.Invoke(CanReceiveDamage);
        }

        public void SetAbilityToPlayerCanStepOnIt(bool state)
        {
            CanPlayerStepOnIt = state;
            OnAbilityToPlayerCanStepOnIt?.Invoke(CanReceiveDamage);
        }

        public virtual void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
