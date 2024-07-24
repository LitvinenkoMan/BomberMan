using System;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    [RequireComponent(typeof(Collider))]
    public class Bricks : Obstacle
    {
        [SerializeField] private GameObject Visuals;

        private Collider _collider;
        
        private void OnEnable()
        {
            CanReceiveDamage = true;
            CanPlayerStepOnIt = false;
            ObstacleHealthComponent.OnHealthRunOut += BreakBricks;
        }

        private void OnDisable()
        {
            ObstacleHealthComponent.OnHealthRunOut -= BreakBricks;
        }

        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        private void Reset()
        {
            Visuals.SetActive(true);
            _collider.isTrigger = false;
            ObstacleHealthComponent.SetHealth(1);
        }

        private void BreakBricks()
        {
            GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position).RemoveObstacle();
            Visuals.SetActive(false);
            _collider.isTrigger = true;
        }
    }
}
