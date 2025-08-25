using MonoBehaviours.GroundSectionSystem;
using Runtime.NetworkBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _spawnPoint;

        public static PlayerSpawner Instance;

        private LevelSectionsDataHolder _dataHolder;
        private List<AssociatedSpawn> _associatedPositions;

        public Action OnPlayerSpawned;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }

        public void SetUpCurrentDataHolder(LevelSectionsDataHolder dataHolder)
        {
            _dataHolder = dataHolder;
            _associatedPositions = new List<AssociatedSpawn>();
            foreach (var spawnPlace in dataHolder.SpawnPlaces)
            {
                _associatedPositions.Add(new AssociatedSpawn(spawnPlace.transform.position, false));
            }
        }

        public void RandomSpawnPlayer()
        {
            int chosenNumber = UnityEngine.Random.Range(0, _associatedPositions.Count);
            if (!_associatedPositions[chosenNumber].isTaken)
            {
                var spawnPlace = _associatedPositions[chosenNumber];
                spawnPlace.isTaken = true;
                _associatedPositions[chosenNumber] = spawnPlace;

                GameObject player = Instantiate(_player, spawnPlace.position, Quaternion.identity);

                OnPlayerSpawned?.Invoke();
            }
            else
            {
                RandomSpawnPlayer();
            }
        }

        public void SpawnPlayer()
        {                        
            var spawnPoint = _associatedPositions[0];
            if (!spawnPoint.isTaken)
            {
                Instantiate(_player, spawnPoint.position, Quaternion.identity);
                OnPlayerSpawned?.Invoke();
            }
            else
            {
                return;
            }
        }

        public struct AssociatedSpawn
        {
            public AssociatedSpawn(Vector3 position, bool isTaken)
            {
                this.position = position;
                this.isTaken = isTaken;
            }
            
            public Vector3 position;
            public bool isTaken;
        }
    }
}

