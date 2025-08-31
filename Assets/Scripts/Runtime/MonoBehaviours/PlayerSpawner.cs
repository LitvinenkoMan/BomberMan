using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.NetworkBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _bot;
        public GameObject Player { get; private set; }

        public static PlayerSpawner Instance;

        private GameObject[] _bots;
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
            _bots = new GameObject[_dataHolder.SpawnPlaces.Count - 1];
            _associatedPositions = new List<AssociatedSpawn>();
            foreach (var spawnPlace in dataHolder.SpawnPlaces)
            {
                _associatedPositions.Add(new AssociatedSpawn(spawnPlace.transform.position, false, null));
            }
        }

        public async void SpawnPlayer(float delay)
        {
            if (Player == null) AssociateRandomSpawnPlaceForPlayer();

            await Task.Delay((int)(delay * 1000));

            if (Player == null)
            {                
                Player = Instantiate(_player, GetPositionForSpawn("Player"), Quaternion.identity);
            }
            else
            {
                Destroy(Player);
                Player = Instantiate(_player, GetPositionForSpawn("Player"), Quaternion.identity);
            }
            OnPlayerSpawned?.Invoke();
            if (Player.TryGetComponent(out ICharacter playerCharacter))
            {
                playerCharacter.Reset();
            }
        }

        private void AssociateRandomSpawnPlaceForPlayer()
        {
            if (_associatedPositions == null) Debug.LogError("No DataHolder assigned to PlayerSpawner");

            int chosenNumber = UnityEngine.Random.Range(0, _associatedPositions.Count);
            if (!_associatedPositions[chosenNumber].isTaken)
            {
                var spawnPlace = _associatedPositions[chosenNumber];
                spawnPlace.isTaken = true;
                spawnPlace.name = "Player";
                _associatedPositions[chosenNumber] = spawnPlace;
            }
            else
            {
                AssociateRandomSpawnPlaceForPlayer();
            }
        }

        private Vector3 GetPositionForSpawn(string name)
        {
            Vector3 position = Vector3.zero;
            _associatedPositions.ForEach(x =>
                {
                    if (x.name == name)
                    {
                        position = x.position;
                    }
                }
            );
            return position;
        }

        public void SpawnBots()
        {
            int numberOfBot = 1;
            for (int i = 0; i < _dataHolder.SpawnPlaces.Count; i++)
            {                
                var spawnPlace = _associatedPositions[i];
                if (!spawnPlace.isTaken)
                {
                    GameObject spawnedBot = Instantiate(_bot, spawnPlace.position, Quaternion.identity);

                    spawnedBot.name = "Bot" + Convert.ToString(numberOfBot);                    
                    spawnPlace.isTaken = true;
                    spawnPlace.name = spawnedBot.name;

                    _associatedPositions[i] = spawnPlace;
                    _bots[numberOfBot - 1] = spawnedBot;

                    numberOfBot++;
                }
            }
        }
        
        public async void RespawnBot(string name, float delay)
        {
            await Task.Delay((int)delay * 1000);

            for (int i = 0; i  <= _bots.Length; i++)
            {
                if (_bots[i].name == name)
                {
                    Destroy(_bots[i]);
                    _bots[i] = Instantiate(_bot, GetPositionForSpawn(name), Quaternion.identity);
                    _bots[i].name = name;
                }
            }
        }

        public struct AssociatedSpawn
        {
            public AssociatedSpawn(Vector3 position, bool isTaken, string name)
            {
                this.position = position;
                this.isTaken = isTaken;
                this.name = name;
            }

            public string name;
            public Vector3 position;
            public bool isTaken;
        }
    }
}

