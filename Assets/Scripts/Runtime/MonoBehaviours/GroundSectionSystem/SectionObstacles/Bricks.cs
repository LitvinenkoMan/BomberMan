using Runtime.MonoBehaviours.GroundSectionSystem;
using Unity.Netcode;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    public class Bricks : Obstacle
    {
        [SerializeField] private GameObject visuals;

        private Collider _collider;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialize();
            NetworkManager.OnServerStarted += Initialize;
            ObstacleHealthCmp.OnHealthRunOut += OnHealthRunOutResponce;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            NetworkManager.OnServerStarted -= Initialize;
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
            BreakBricksRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void BreakBricksRpc()
        {
            BreakBricks();
        }

        private void BreakBricks()
        {
            GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position).RemoveObstacle();
            visuals.SetActive(false);
            _collider.isTrigger = true;
        }
    }
}
