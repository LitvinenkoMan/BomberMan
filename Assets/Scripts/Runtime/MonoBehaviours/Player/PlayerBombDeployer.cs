using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using MonoBehaviours.GroundSectionSystem.SectionObstacles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerBombDeployer : MonoBehaviour, IBombDeployer
    {
        private IObjectPool<GameObject> _bombsPool;

        private Queue<Bomb> _dropedBombs;
        private bool _canDeployBombs;
        private int _currentPlacedBombs;

        public Action<Vector3> BombSpawned;

        private void OnEnable()
        {

        }
        private void OnDisable()
        {
            _bombsPool.Clear();
        }
        private void Start()
        {
            Initialize();
        }

        public void DeployBomb(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            if (!_canDeployBombs) return;

            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && _currentPlacedBombs < bombsAtTime)
            {
                var bomb = _bombsPool.GetFromPool(true).GetComponent<Bomb>();
                bomb.SetNewPosition(section.ObstaclePlacementPosition);
                bomb.transform.SetParent(null);
                bomb.onExplode += SubtractAmountOfCurrentBombs;
                section.AddObstacle(bomb);
                bomb.Ignite(timeToExplode, bombDamage, bombSpread);

                _currentPlacedBombs++;

                BombSpawned?.Invoke(bomb.transform.position);
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
        private IEnumerator ReturnBombBackToPoolRoutine(Bomb bomb)
        {
            //yield return new WaitForSeconds(2.1f);                      // I'm pushing bombs to return explosion effects back to ObjectPool, since I do that,
            ReturnBombToPoolRpc(bomb);                 // I need to wait until coroutine will return them back, and after that I will return bomb
            yield return null;
        }
        private void ReturnBombToPoolRpc(Bomb bomb)
        {
            _bombsPool.AddToPool(bomb.gameObject);
            bomb.Reset();
        }

        public void Initialize()
        {
            _bombsPool = GetComponent<IObjectPool<GameObject>>();
            _bombsPool.Initialize();
        }

        public void SetAbilityToDeployBombs(bool canIt)
        {
            _canDeployBombs = canIt;
        }
    }
}