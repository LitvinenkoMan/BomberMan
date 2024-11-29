using System.Collections;
using MonoBehaviours;
using MonoBehaviours.GroundSectionSystem;
using MonoBehaviours.GroundSectionSystem.SectionObstacles;
using ScriptableObjects;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Runtime.NetworkBehaviours
{
    [RequireComponent(typeof(ObjectPoolQueue))]
    public class BombDeployer : NetworkBehaviour
    {
        [FormerlySerializedAs("PlayerParams")] [SerializeField]
        private BaseBomberParameters bomberParams;
        [SerializeField]
        private ObjectPoolQueue BombsPool;

        private int currentPlacedBombs;
        private bool CanPlaceBombs;

        // Input
        private PlayerMainControls _controls;
        private InputAction PlaceBombAction;

        private void OnEnable()
        {
            Initialize();
            PlaceBombAction.performed += DeployBombAction;
            CanPlaceBombs = true;
        }

        private void OnDisable()
        {
            PlaceBombAction.performed -= DeployBombAction;
        }

        public void SetAbilityToDeployBombs(bool canIt)
        {
            CanPlaceBombs = canIt;
        }

        [ClientRpc]
        public void SetAbilityToDeployBombsClientRpc(bool canIt)
        {
            SetAbilityToDeployBombs(canIt);
        }

        private void Initialize()
        {
            if (_controls == null)
            {
                _controls = new PlayerMainControls();
            }
            _controls.PlayerMainActionMaps.Enable();
            
            PlaceBombAction = _controls.PlayerMainActionMaps.PlaceBomb;
            if (!BombsPool)
            {
                BombsPool = GetComponent<ObjectPoolQueue>();
            }
        }
        
        private void DeployBombAction(InputAction.CallbackContext context)
        {
            if (!CanPlaceBombs) return;
            
            if (!IsOwner) return;
            
            if (IsServer)
            {
                Debug.Log("Deploying from Server");
                DeployBomb(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
            }
            else
            {
                Debug.Log("Deploying from Client");
                DeployBombRpc(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
            }
        }

        private void DeployBomb(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && currentPlacedBombs < bombsAtTime)
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
                currentPlacedBombs++;
            }
        }

        [Rpc(SendTo.Server)]
        private void DeployBombRpc(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread)
        {
            Debug.Log("Sended ask to deploy bomb to server");
            DeployBomb(bombsAtTime, timeToExplode, bombDamage, bombSpread);
        }

        private void SubtractAmountOfCurrentBombs(Bomb explodedBomb)
        {
            currentPlacedBombs--;
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
    }
}
