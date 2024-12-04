using System;
using Runtime.MonoBehaviours.GroundSectionSystem;
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
