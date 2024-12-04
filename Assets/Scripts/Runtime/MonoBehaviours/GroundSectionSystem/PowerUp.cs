using System;
using System.Collections;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.GroundSectionSystem;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class PowerUp : Obstacle
    {
        [Header("Base values settings")]
        [SerializeField, Tooltip("Set to true if you want this power up to disappear after time")]
        private bool TimedPowerUp;

        [SerializeField] private float LifeTime;

        [Space(20)]
        [SerializeField, Tooltip("Set to True, if you want this power up to have timed effect on Bomber Params")]
        private bool TimedEffect;

        [SerializeField] private float EffectTime;

        [Space(10)] [SerializeField] protected GameObject Visuals;
        [Space(20)] protected bool _isTaken;

        public virtual void Initialize()
        {
            ObstacleHealthComponent.SetHealth(1);
            CanReceiveDamage = true;
            CanPlayerStepOnIt = true;
            _isTaken = false;
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
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

        public override void OnNetworkSpawn()
        {
            
        }

        public override void OnNetworkDespawn()
        {
           Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BomberParamsProvider provider) && !_isTaken && provider.IsOwner)
            {
                ApplyPowerUp(provider.GetBomberParams());
            }
        }

        protected virtual void ApplyPowerUp(BaseBomberParameters Params)
        {
            // NetworkObject.Despawn();
        }

        protected virtual void RemovePowerUpFromGroundSection()
        {
            GroundSection startSection = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            startSection.RemoveObstacle();

            if (!IsServer)
            {
                RemovePowerUpFromGroundSectionRpc();
            }
            else
            {
                NetworkObject.Despawn();
            }
        }

        [Rpc(SendTo.Server)]
        protected virtual void RemovePowerUpFromGroundSectionRpc()
        {
            GroundSection startSection = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            startSection.RemoveObstacle();
            NetworkObject.Despawn();
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

        public new void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref TimedPowerUp);
            serializer.SerializeValue(ref LifeTime);
            serializer.SerializeValue(ref TimedEffect);
            serializer.SerializeValue(ref EffectTime);
            
            serializer.SerializeValue(ref _isTaken);
        }
    }
}
