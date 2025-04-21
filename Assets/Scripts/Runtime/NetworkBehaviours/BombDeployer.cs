using System.Collections;
using MonoBehaviours.GroundSectionSystem;
using MonoBehaviours.GroundSectionSystem.SectionObstacles;
using Runtime.MonoBehaviours;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Runtime.NetworkBehaviours
{
    [RequireComponent(typeof(ObjectPoolQueue))]
    public class BombDeployer : NetworkBehaviour, InputActions.IPlayerMapActions
    {
        [FormerlySerializedAs("PlayerParams")] [SerializeField]
        private BaseBomberParameters bomberParams;
        [SerializeField]
        private ObjectPoolQueue BombsPool;

        private int _currentPlacedBombs;
        private bool _canPlaceBombs;

        // Input
        private InputActions _input;

        private void OnEnable()
        {
            Initialize();
            _canPlaceBombs = false;
        }

        public void SetAbilityToDeployBombs(bool canIt)
        {
            _canPlaceBombs = canIt;
        }

        [ClientRpc]
        public void SetAbilityToDeployBombsClientRpc(bool canIt)
        {
            SetAbilityToDeployBombs(canIt);
        }

        public void Initialize()
        {
            if (_input == null)
            {
                _input = new InputActions();
            }
            _input.PlayerMap.AddCallbacks(this);
            _input.Enable();
            
            if (!BombsPool)
            {
                BombsPool = GetComponent<ObjectPoolQueue>();
            }
        }
        
        private void DeployBombAction()
        {
            if (!_canPlaceBombs) return;
            
            if (!IsOwner) return;
            
            if (IsServer)
            {
                DeployBomb(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
            }
            else
            {
                DeployBombRpc(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
            }
        }

        private void DeployBomb(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && _currentPlacedBombs < bombsAtTime)
            {
                var bomb = BombsPool.GetFromPool(true).GetComponent<Bomb>();
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

        [Rpc(SendTo.Server)]
        private void DeployBombRpc(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            DeployBomb(bombsAtTime, timeToExplode, bombDamage, bombSpread);
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
            ReturnBombToPool(bomb);                 // I need to wait until coroutine will return them back, and after that I will return bomb
        }

        private void ReturnBombToPool(NetworkBehaviourReference bomb)
        {
            if (bomb.TryGet(out Bomb explodedBomb))
            {
                BombsPool.AddToPool(explodedBomb.gameObject);
                explodedBomb.Reset();
                explodedBomb.NetworkObject.Despawn(false);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            //not needed 
        }

        public void OnPlaceBomb(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DeployBombAction();
            }
        }

        public void OnQuit(InputAction.CallbackContext context)
        {
            //not needed 
        }
    }
}
