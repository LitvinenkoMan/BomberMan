using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class Obstacle : MonoBehaviour
    {
        public byte HealthPoints { get; private set; }
        public bool CanPlayerStepOnIt { get; private set; }
        public bool CanReceiveDamage { get; private set; }

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

        public void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
