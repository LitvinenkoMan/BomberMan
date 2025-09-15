using Runtime.MonoBehaviours.GroundSectionSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    public class BricksNotNet : Obstacle
    {
        [SerializeField] private GameObject visuals;

        private Collider _collider;

        private void Start()
        {
            Initialize();
        }

        public void OnEnable()
        {
            ObstacleHealthCmp.OnHealthRunOut += OnHealthRunOutResponce;
        }

        public void OnDisable()
        {
            ObstacleHealthCmp.OnHealthRunOut -= OnHealthRunOutResponce;
        }

        private void Initialize()
        {
            _collider = GetComponent<Collider>();
            ObstacleHealthCmp.SetAbilityToReceiveDamage(true);
            CanPlayerStepOnIt = false;
        }

        private void Reset()
        {
            visuals.SetActive(true);
            _collider.isTrigger = false;
            ObstacleHealthCmp.AddHealth(1);
        }

        private void OnHealthRunOutResponce()
        {
            BreakBricks();
        }

        private void BreakBricks()
        {
            GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position).RemoveObstacle();
            _collider.isTrigger = true;
            gameObject.SetActive(false);
        }
    }
}
