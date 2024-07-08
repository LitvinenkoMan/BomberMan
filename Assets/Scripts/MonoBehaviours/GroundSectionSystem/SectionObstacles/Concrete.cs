using System;

namespace MonoBehaviours.GroundSectionSystem
{
    public class Concrete : Obstacle
    {
        private void Start()
        {
            HealthPoints = 255;
            CanReceiveDamage = false;
            CanPlayerStepOnIt = false;
        }
    }
}
