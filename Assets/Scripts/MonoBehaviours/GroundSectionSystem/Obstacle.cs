using System;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class Obstacle : MonoBehaviour
    {
        public HealthComponent ObstacleHealthComponent;
        public bool CanReceiveDamage { get; protected set; }

        protected Action<bool> OnAbilityToReciveDamageChanged;

        private void Start()
        {
            ObstacleHealthComponent = GetComponent<HealthComponent>();
        }

        public void SetAbilityToReceiveDamage(bool state)
        {
            CanReceiveDamage = state;
            OnAbilityToReciveDamageChanged?.Invoke(CanReceiveDamage);
        }

        public virtual void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
