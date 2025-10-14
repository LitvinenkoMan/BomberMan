using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.Bot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _bot;
        public GameObject Player { get; private set; }

        public static Spawner Instance;

        private List<GameObject> _opponents;
        private LevelSectionsDataHolder _dataHolder;
        private List<AssociatedSpawn> _associatedPositions;

        public List<GameObject> OpponentsList => _opponents;

        public Action OnPlayerSpawned;
        public Action<string> OnBotSpawned;

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
        private void Start()
        {
            BombPositions.CreateGrid();
        }

        public void SetUpCurrentDataHolder(LevelSectionsDataHolder dataHolder)
        {
            _dataHolder = dataHolder;
            _opponents = new List<GameObject>();
            _associatedPositions = new List<AssociatedSpawn>();
            foreach (var spawnPlace in dataHolder.SpawnPlaces)
            {
                _associatedPositions.Add(new AssociatedSpawn(spawnPlace.transform.position, false, null));
            }
        }

        public async void SpawnPlayer(float delay)
        {
            if (Player == null) AssociateRandomSpawnPlaceForPlayer();

            for (int i = 0; i < _opponents.Count; i++)
            {
                if (_opponents[i] == Player)
                {
                    _opponents[i] = null;
                }
            }

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

            Player.name = "Player";
            if (!_opponents.Contains(Player))
                _opponents.Add(Player);

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
                    _opponents.Add(spawnedBot);

                    numberOfBot++;

                    if (spawnedBot.TryGetComponent(out ICharacter botCharacter))
                    {
                        botCharacter.Reset();
                        botCharacter.SetBombDeployAbility(true);
                    }

                    OnBotSpawned?.Invoke(spawnedBot.name);
                }
            }
        }

        public async void RespawnBot(string name, float delay)
        {
            GameObject respawningBot = null;
            for(int i = 0; i < _opponents.Count; i++)
            {
                if (_opponents[i] == null) continue;
                if (_opponents[i].name == name)
                {
                    respawningBot = _opponents[i];
                    _opponents[i] = null;
                    break;
                }
            }
            await Task.Delay((int)delay * 1000);

            var spawnPos = GetPositionForSpawn(name);
            Destroy(respawningBot);
            var newBot = Instantiate(_bot, spawnPos, Quaternion.identity);
            newBot.name = name;
            _opponents.Add(newBot);

            if (newBot.TryGetComponent(out ICharacter botCharacter))
            {
                botCharacter.Reset();
                botCharacter.SetBombDeployAbility(true);
            }

            OnBotSpawned?.Invoke(name);
            return;
        }

        public GameObject GetOpponentByName(string name)
        {
            _opponents.RemoveAll(item => item == null);
            for (int i = 0; i < _opponents.Count; i++)
            {
                if (_opponents[i] == null) continue;
                if (_opponents[i].name == name)
                {
                    return _opponents[i];
                }
            }
            Debug.LogError("GetOpponentByName: did not find bot by name " + name);
            return null;
        }

        public List<GameObject> GetAllOpponents()
        {
            _opponents.RemoveAll(item => item == null);
            return _opponents;
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

