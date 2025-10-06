using System.Collections;
using Core.DataTransferObjects;
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
            if (IsServer)
            {
                _bombsPool = GetComponent<IObjectPool<GameObject>>();
                _bombsPool.Initialize();
            }
            //_dropedBombs = new Queue<Bomb>();
        }

        public void SetAbilityToDeployBombs(bool canIt)
        {
            SetAbilityToDeployBombsClientRpc(canIt);
        }
        
        public void DeployBomb(BombDto bombDto)
        {
            if (!_canDeployBombs) return;

            DeployBombRpc(bombDto);
        }

        public void ClearBombs()
        {
            ClearPoolRpc();
        }

        [Rpc(SendTo.Server)]
        private void DeployBombRpc(BombDto bombDto)
        {
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && _currentPlacedBombs < bombDto.BombsAtTime)
            {
                var bomb = _bombsPool.GetFromPool(true).GetComponent<BombNet>();
                bomb.SetNewPosition(section.ObstaclePlacementPosition);
                bomb.transform.SetParent(null);
                bomb.onExplode += SubtractAmountOfCurrentBombs;
                section.AddObstacle(bomb);
                bomb.Ignite(bombDto.BombCountdown, bombDto.BombsDamage, bombDto.BombsSpreading);
                if (!bomb.NetworkObject.IsSpawned)
                {
                    bomb.NetworkObject.Spawn();
                }

                _currentPlacedBombs++;
            }
        }

        private void SubtractAmountOfCurrentBombs(BombNet explodedBomb)
        {
            _currentPlacedBombs--;
            explodedBomb.onExplode -= SubtractAmountOfCurrentBombs;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ReturnBombBackToPoolRoutine(explodedBomb));
            }
        }

        private IEnumerator ReturnBombBackToPoolRoutine(BombNet bomb)
        {
            //yield return new WaitForSeconds(2.1f);                      // I'm pushing bombs to return explosion effects back to ObjectPool, since I do that,
            ReturnBombToPoolRpc(bomb);                 // I need to wait until coroutine will return them back, and after that I will return bomb
            yield return null;
        }

        [Rpc(SendTo.Server)]
        private void ReturnBombToPoolRpc(NetworkBehaviourReference bomb)
        {
            if (bomb.TryGet(out BombNet explodedBomb))
            {
                _bombsPool.AddToPool(explodedBomb.gameObject);
                explodedBomb.Reset();
                explodedBomb.NetworkObject.Despawn(false);
            }
        }

        [Rpc(SendTo.Server)]
        private void ClearPoolRpc()
        {
            Debug.Log("ClearPoolRpc from server side");
            // foreach (var bomb in _dropedBombs)
            // {
            //     bomb.NetworkObject.Despawn();
            // }

            foreach (var bomb in _bombsPool)
            {
                if (bomb.TryGetComponent(out NetworkObject bombNet))
                {
                    if (bombNet.IsSpawned)
                    {
                        bombNet.Despawn();                              //TODO: Must make an IObjectPoolNet or smth                    
                    }
                    else
                    {
                        Destroy(bomb);
                    }
                }
            }
        }

        [ClientRpc]
        private void SetAbilityToDeployBombsClientRpc(bool canIt)
        {
            _canDeployBombs = canIt;
        }
    }
}
