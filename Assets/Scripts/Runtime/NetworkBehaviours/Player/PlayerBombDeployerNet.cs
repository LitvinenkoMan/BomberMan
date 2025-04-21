using System.Collections;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using MonoBehaviours.GroundSectionSystem.SectionObstacles;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerBombDeployerNet : NetworkBehaviour, IBombDeployer
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
            if (!_canDeployBombs) return;

            DeployBombRpc(bombsAtTime, timeToExplode, bombDamage, bombSpread);
        }

        [Rpc(SendTo.Server)]
        private void DeployBombRpc(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && _currentPlacedBombs < bombsAtTime)
            {
                var bomb = _bombsPool.GetFromPool(true).GetComponent<Bomb>();
                bomb.SetNewPosition(section.ObstaclePlacementPosition);
                bomb.transform.SetParent(null);
                bomb.onExplode += SubtractAmountOfCurrentBombs;
                section.AddObstacle(bomb);
                bomb.Ignite(timeToExplode, bombDamage, bombSpread);
                if (!bomb.NetworkObject.IsSpawned)
                {
                    bomb.NetworkObject.Spawn();
                }

                _currentPlacedBombs++;
            }
        }

        private void SubtractAmountOfCurrentBombs(Bomb explodedBomb)
        {
            _currentPlacedBombs--;
            explodedBomb.onExplode -= SubtractAmountOfCurrentBombs;
            StartCoroutine(ReturnBombBackToPoolRoutine(explodedBomb));
        }
        
        private IEnumerator ReturnBombBackToPoolRoutine(Bomb bomb)
        {
            yield return new WaitForSeconds(2.1f);                      // I pushing bombs to return explosion effects back to ObjectPool, since I do that,
            ReturnBombToPoolRpc(bomb);                 // I need to wait until coroutine will return them back, and after that I will return bomb
        }

        [Rpc(SendTo.Server)]
        private void ReturnBombToPoolRpc(NetworkBehaviourReference bomb)
        {
            if (bomb.TryGet(out Bomb explodedBomb))
            {
                _bombsPool.AddToPool(explodedBomb.gameObject);
                explodedBomb.Reset();
                explodedBomb.NetworkObject.Despawn(false);
            }
        }

        [ClientRpc]
        private void SetAbilityToDeployBombsClientRpc(bool canIt)
        {
            _canDeployBombs = canIt;
        }
    }
}
