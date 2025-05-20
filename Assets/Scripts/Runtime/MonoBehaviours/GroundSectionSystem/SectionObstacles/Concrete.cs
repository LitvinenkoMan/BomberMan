using System;
using Runtime.MonoBehaviours.GroundSectionSystem;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class Concrete : Obstacle
    {
        private void Start()
        {
            ObstacleHealthComponent.AddHealth(255);
            CanReceiveDamage = false;
            CanPlayerStepOnIt = false;
        }
    }
}
