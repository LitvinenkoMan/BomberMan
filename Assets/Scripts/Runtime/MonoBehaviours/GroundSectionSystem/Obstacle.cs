using System;
using MonoBehaviours.GroundSectionSystem;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours.GroundSectionSystem
{
    public class Obstacle : NetworkBehaviour, INetworkSerializable
    {
        public ObstacleHealthComponent ObstacleHealthCmp { get; private set;  }
        public bool CanPlayerStepOnIt { get; protected set; }
        
        protected event Action<bool> OnAbilityToPlayerCanStepOnItChanged;

        private void Awake()
        {
            ObstacleHealthCmp = gameObject.AddComponent<ObstacleHealthComponent>();
        }

        public override void OnNetworkSpawn()
        {
            //ObstacleHealthCmp.OnHealthRunOut += DespawnObstacleRpc;
        }

        public override void OnNetworkDespawn()
        {
            //ObstacleHealthCmp.OnHealthRunOut -= DespawnObstacleRpc;
        }

        public virtual void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void AutoPlaceToNearestSection()
        {
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            section.AddObstacle(this);
        }

        public void SetAbilityToPlayerCanStepOnIt(bool state)
        {
            CanPlayerStepOnIt = state;
            OnAbilityToPlayerCanStepOnItChanged?.Invoke(CanPlayerStepOnIt);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            
        }

        [Rpc(SendTo.Server)]
        private void DespawnObstacleRpc()
        {
            NetworkObject.Despawn();        //TODO: This Only works if player entered not in the middle of the session
        }
    }
}
