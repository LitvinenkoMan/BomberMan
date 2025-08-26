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
        [SerializeField] private GameObject _bot;
        public GameObject Player { get; private set; }

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
            RandomSpawnPlayer();
        }

        public void RandomSpawnPlayer()
        {
            if (_associatedPositions == null) Debug.LogError("No DataHolder assigned to PlayerSpawner");
            int chosenNumber = UnityEngine.Random.Range(0, _associatedPositions.Count);
            if (!_associatedPositions[chosenNumber].isTaken)
            {
                var spawnPlace = _associatedPositions[chosenNumber];
                spawnPlace.isTaken = true;
                _associatedPositions[chosenNumber] = spawnPlace;

                Player = Instantiate(_player, spawnPlace.position, Quaternion.identity);

                OnPlayerSpawned?.Invoke();

                SpawnBots();
            }
            else
            {
                RandomSpawnPlayer();
            }
        }

        private void SpawnBots()
        {     
            for (int i = 0; i < _dataHolder.SpawnPlaces.Count; i++)
            {
                var spawnPlace = _associatedPositions[i];
                if (!spawnPlace.isTaken)
                {
                    spawnPlace.isTaken = true;
                    _associatedPositions[i] = spawnPlace;
                    Instantiate(_bot, spawnPlace.position, Quaternion.identity);
                }
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

