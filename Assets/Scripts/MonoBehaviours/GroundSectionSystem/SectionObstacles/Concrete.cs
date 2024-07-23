using System;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class Concrete : Obstacle
    {
        private void Start()
        {
            ObstacleHealthComponent.SetHealth(255);
            CanReceiveDamage = false;
            CanPlayerStepOnIt = false;
        }
    }
}
