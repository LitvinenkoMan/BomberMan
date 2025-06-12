using System;
using System.Collections;
using System.Collections.Generic;
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


        private Queue<Bomb> _dropedBombs;
        private bool _canDeployBombs;
        private int _currentPlacedBombs;
        
        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        public override void OnNetworkDespawn()
        {
            ClearPoolRpc();
            if (IsOwner)
            {
                _bombsPool.Clear();
                DestroyPlacedBombs();
            }
        }

        private void OnDisable()
        {
            DestroyPlacedBombs();
        }

        public void Initialize()
        {
            _bombsPool = GetComponent<IObjectPool<GameObject>>();
            _dropedBombs = new Queue<Bomb>();
            _bombsPool.Initialize();
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
                section.AddObstacle(bomb);
                _dropedBombs.Enqueue(bomb);
                bomb.onExplode += SubtractAmountOfCurrentBombs;
                bomb.onExplode += RemoveBombFromDropedList;
                    
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
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ReturnBombBackToPoolRoutine(explodedBomb));
            }
        }

        private void RemoveBombFromDropedList(Bomb bomb)
        {
            _dropedBombs.Dequeue();
        }

        private void DestroyPlacedBombs()
        {
            while (_dropedBombs.Count > 1)
            {
                Destroy(_dropedBombs.Dequeue(), 5);
            }
        }

        private IEnumerator ReturnBombBackToPoolRoutine(Bomb bomb)
        {
            //yield return new WaitForSeconds(2.1f);                      // I'm pushing bombs to return explosion effects back to ObjectPool, since I do that,
            ReturnBombToPoolRpc(bomb);                 // I need to wait until coroutine will return them back, and after that I will return bomb
            yield return null;
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

        [Rpc(SendTo.Server)]
        private void ClearPoolRpc()
        {
            Debug.Log("ClearPoolRpc from server side");
           //_bombsPool.Clear();
        }

        [ClientRpc]
        private void SetAbilityToDeployBombsClientRpc(bool canIt)
        {
            _canDeployBombs = canIt;
        }
    }
}
