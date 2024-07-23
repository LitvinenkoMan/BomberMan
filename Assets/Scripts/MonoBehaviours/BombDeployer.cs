using System;
using System.Collections;
using MonoBehaviours.GroundSectionSystem;
using MonoBehaviours.GroundSectionSystem.SectionObstacles;
using ScriptableObjects;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace MonoBehaviours
{
    [RequireComponent(typeof(ObjectPoolQueue))]
    public class BombDeployer : MonoBehaviour
    {
        [FormerlySerializedAs("PlayerParams")] [SerializeField]
        private BaseBomberParameters bomberParams;
        [SerializeField]
        private ObjectPoolQueue BombsPool;

        private byte currentPlacedBombs;

        // Input
        private PlayerMainControls _controls;
        private InputAction PlaceBombAction;

        private void OnEnable()
        {
            Initialize();
            PlaceBombAction.performed += DeployBomb;
        }

        private void OnDisable()
        {
            PlaceBombAction.performed -= DeployBomb;
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

        private void DeployBomb(InputAction.CallbackContext context)
        {
            Debug.Log("trying Deploy bomb");
            var section = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            if (section && !section.PlacedObstacle && currentPlacedBombs < bomberParams.BombsAtTime)
            {
                Debug.Log("Deploying bomb");
                var bomb = BombsPool.GetFromPool(true).GetComponent<Bomb>();
                bomb.PlaceBomb(section.ObstaclePlacementPosition);
                bomb.transform.SetParent(null);
                bomb.onExplode += SubtractAmountOfCurrentBombs;
                section.AddObstacle(bomb);
                currentPlacedBombs++;
            }
        }

        private void SubtractAmountOfCurrentBombs(Bomb explodedBomb)
        {
            currentPlacedBombs--;
            explodedBomb.onExplode -= SubtractAmountOfCurrentBombs;
            StartCoroutine(ReturnBombBackToPool(explodedBomb));
        }

        private IEnumerator ReturnBombBackToPool(Bomb bomb)
        {
            yield return new WaitForSeconds(2.1f);  // I pushing bombs to return explosion effects back to ObjectPool, since I do that,
            BombsPool.AddToPool(bomb.gameObject);   // I need to wait until coroutine will return them back, and after that I will return bomb,
            bomb.Reset();                           // because coroutines won't work if Object is Disabled
        }
    }
}
