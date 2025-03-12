using Runtime.MonoBehaviours.GroundSectionSystem;
using Unity.Netcode;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    [RequireComponent(typeof(Collider))]
    public class Bricks : Obstacle
    {
        [SerializeField] private GameObject Visuals;

        private Collider _collider;

        private void Initialize()
        {
            // if (!NetworkObject.IsSpawned)
            // {
            //     NetworkObject.Spawn();
            //     Debug.Log($"Brick {name} Spawned");
            // }
            CanReceiveDamage = true;
            CanPlayerStepOnIt = false;
        }

        private void OnEnable()
        {
            NetworkManager.OnServerStarted += Initialize;
            ObstacleHealthComponent.OnHealthRunOut += OnHealthRunOutResponce;
        }

        private void OnDisable()
        {
            NetworkManager.OnServerStarted -= Initialize;
            ObstacleHealthComponent.OnHealthRunOut -= OnHealthRunOutResponce;
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
            Visuals.SetActive(false);
            _collider.isTrigger = true;
        }
    }
}
