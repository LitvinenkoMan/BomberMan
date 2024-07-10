using System;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    public class Bricks : Obstacle
    {
        [SerializeField] private GameObject Visuals;

        private Collider collider;
        
        private void OnEnable()
        {
            HealthPoints = 1;
            CanPlayerStepOnIt = false;
            CanReceiveDamage = true;

            collider = GetComponent<Collider>();

            OnHealthChanged += BreakBricks;
        }

        private void OnDisable()
        {
            OnHealthChanged -= BreakBricks;
        }

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        private void Reset()
        {
            Visuals.SetActive(true);
            collider.isTrigger = false;
            HealthPoints = 1;
        }

        private void BreakBricks(byte health)
        {
            if (health <= 0)
            {
                GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position).RemoveObstacle();
                Visuals.SetActive(false);
                collider.isTrigger = true;
            }
        }
    }
}
