using System;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours.GroundSectionSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class Obstacle : NetworkBehaviour, INetworkSerializable
    {
        public HealthComponent ObstacleHealthComponent;
        public bool CanReceiveDamage { get; protected set; }
        public bool CanPlayerStepOnIt { get; protected set; }

        protected Action<bool> OnAbilityToReciveDamageChanged;
        protected Action<bool> OnAbilityToPlayerCanStepOnItChanged;


        private Vector3 _position;

        private void Start()
        {
            ObstacleHealthComponent = GetComponent<HealthComponent>();
        }

        public void SetAbilityToReceiveDamage(bool state)
        {
            CanReceiveDamage = state;
            OnAbilityToReciveDamageChanged?.Invoke(CanReceiveDamage);
        }

        public void SetAbilityToPlayerCanStepOnIt(bool state)
        {
            CanPlayerStepOnIt = state;
            OnAbilityToPlayerCanStepOnItChanged?.Invoke(CanPlayerStepOnIt);
        }

        public virtual void SetNewPosition(Vector3 position)
        {
            transform.position = position;
            _position = position;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _position);
        }
    }
}
