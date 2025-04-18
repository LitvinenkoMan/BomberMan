using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using MonoBehaviours.GroundSectionSystem.SectionObstacles;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerBombDeployer : NetworkBehaviour, IBombDeployer
    {
        private IObjectPool<GameObject> _bombsPool;
        
        private bool _canDeployBombs;
        private int _currentPlacedBombs;
        
        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        public void Initialize()
        {
            _bombsPool = GetComponent<IObjectPool<GameObject>>();
            _canDeployBombs = false;
        }

        public void SetAbilityToDeployBombs(bool canIt)
        {
            SetAbilityToDeployBombsClientRpc(canIt);
        }

        public void DeployBomb(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && _currentPlacedBombs < bombsAtTime)
            {
                var bomb = _bombsPool.GetFromPool(true).GetComponent<Bomb>();
                bomb.SetNewPosition(section.ObstaclePlacementPosition);
                bomb.transform.SetParent(null);
                //bomb.onExplode += SubtractAmountOfCurrentBombs;
                section.AddObstacle(bomb);
                bomb.Ignite(timeToExplode, bombDamage, bombSpread);
                if (!bomb.NetworkObject.IsSpawned)
                {
                    bomb.NetworkObject.Spawn();
                }
                _currentPlacedBombs++;
            }
        }

        [ClientRpc]
        private void SetAbilityToDeployBombsClientRpc(bool canIt)
        {
            _canDeployBombs = canIt;
        }
    }
}
