using System;
using System.Collections;
using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class PowerUp : Obstacle
    {
        [Header("Base values settings")]
        [SerializeField, Tooltip("Set to true if you want this power up to disappear after time")] 
        private bool TimedPowerUp;
        [SerializeField] 
        private float LifeTime;

        [Space(20)]
        [SerializeField, Tooltip("Set to True, if you want this power up to have timed effect on Bomber Params")] 
        private bool TimedEffect;
        [SerializeField]
        private float EffectTime;
        
        [Space(10)]
        [SerializeField]
        protected GameObject Visuals;
        [Space(20)]
        
        
        protected bool _isTaken; 

        protected virtual void Initialize()
        {
            ObstacleHealthComponent.SetHealth(1);
            CanReceiveDamage = true;
            CanPlayerStepOnIt = true;
            _isTaken = false;
        }

        private void OnEnable()
        {
            Initialize();
            if (TimedPowerUp)
            {
                StartCoroutine(CountLifeTime());
            }
            ObstacleHealthComponent.OnHealthRunOut += RemovePowerUpFromGroundSection;
        }

        private void OnDisable()
        {
            ObstacleHealthComponent.OnHealthRunOut -= RemovePowerUpFromGroundSection;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BomberParamsProvider provider) && !_isTaken)
            {
                ApplyPowerUp(provider.GetBomberParams());
            }
        }

        protected virtual void ApplyPowerUp(BaseBomberParameters Params) { }

        protected virtual void RemovePowerUpFromGroundSection()
        {
            GroundSection startSection = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            startSection.RemoveObstacle();
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator CountLifeTime()
        {
            yield return new WaitForSeconds(LifeTime);
            if (!_isTaken)
            {
                RemovePowerUpFromGroundSection();
                Destroy(gameObject);
            }
        }
    }
}