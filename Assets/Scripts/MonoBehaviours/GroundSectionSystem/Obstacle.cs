using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class Obstacle : MonoBehaviour
    {
        public byte HealthPoints { get; protected set; }
        public bool CanPlayerStepOnIt { get; protected set; }
        public bool CanReceiveDamage { get; protected set; }

        public void SetHealthPoints(byte newHealth)
        {
            HealthPoints = newHealth;
        }

        public void SetAbilityToStepOnIt(bool state)
        {
            CanPlayerStepOnIt = state;
        }

        public void SetAbilityToReceiveDamage(bool state)
        {
            CanReceiveDamage = state;
        }

        public virtual void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
