using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoBehaviours.GroundSectionSystem;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.NetworkBehaviours
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject Player;

        public static PlayerSpawner Instance;

        private LevelSectionsDataHolder _currentLevelDataHolder;

        private List<ClientIdAssociatedSpawn> _associatedPositions;

        public event Action<ulong> OnPlayerSpawned;


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

            _associatedPositions = new List<ClientIdAssociatedSpawn>();
        }

        public override void OnNetworkSpawn()
        {
            
        }


        /// <summary>
        /// Will spawn player GameObject for Client at Associated position for spawning, which is chosen randomly at moment when Client is Connected.
        /// </summary>
        /// <param name="clientId">Id of a client</param>
        [Rpc(SendTo.Server)]
        public void SpawnClientRpc(ulong clientId)
        {
            SpawnPlayer(clientId, 0);
        }
        
        /// <summary>
        /// Will spawn player GameObject for Client at Associated position for spawning, which is chosen randomly at moment when Client is Connected.
        /// </summary>
        /// <param name="clientId">Id of a client</param>
        /// <param name="spawnDelay">Delay for spawning, in seconds </param>
        [Rpc(SendTo.Server)]
        public void SpawnClientRpc(ulong clientId, float spawnDelay)
        {
            SpawnPlayer(clientId, spawnDelay);
        }

        public async Task SpawnPlayer(ulong clientId, float spawnDelay)
        {
            await Task.Delay((int)(spawnDelay * 1000));
            
            var playerNetObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if (playerNetObject != null)
            {
                playerNetObject.Despawn(true);
            }

            GameObject player = Instantiate(Player, Vector3.zero, new Quaternion(0, 0, 0, 0));                  //TODO: I could somehow reuse already existing object instead of Reinstantiate it
            
            if (!CheckIfPlayerHaveSpawnPlace(clientId))
            {
                await AssociateRandomSpawnPlaceForClient(clientId);
            }
            
            _associatedPositions.ForEach(x =>
                {
                    if (x.clientId == clientId)
                    {
                        player.transform.position = x.position;
                    }
                }
            );
            
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            //player.GetComponent<ICharacter>().Initialize();  

            SendPlayerSpawnEventRpc(clientId);
        }

        private async Task AssociateRandomSpawnPlaceForClient(ulong clientID)
        {
            int chosenNumber = Random.Range(0, _currentLevelDataHolder.SpawnPlaces.Count);

            if (!_associatedPositions[chosenNumber].isTaken)
            {
                var spawnPlace = _associatedPositions[chosenNumber];
                spawnPlace.clientId = clientID;                         //TODO: is it okey to do like this?
                spawnPlace.isTaken = true;                         
                _associatedPositions[chosenNumber] = spawnPlace;
            }
            else
            {
                await AssociateRandomSpawnPlaceForClient(clientID);
            }
        }

        public void SetUpCurrentDataHolder(LevelSectionsDataHolder dataHolder)
        {
            _currentLevelDataHolder = dataHolder;
            foreach (var spawnPosition in _currentLevelDataHolder.SpawnPlaces)
            {
                _associatedPositions.Add(new ClientIdAssociatedSpawn(ulong.MaxValue, spawnPosition.transform.position, false));
            }
        }

        public void ClearSpawnPositionOfPlayer(ulong clientId)
        {
            for (int i = 0; i < _associatedPositions.Count; i++)
            {
                if (_associatedPositions[i].clientId == clientId)
                {
                    var spawnPos = _associatedPositions[i];
                    spawnPos.clientId = ulong.MaxValue;
                    spawnPos.isTaken = false;
                    _associatedPositions[i] = spawnPos;
                    return;
                }
            }
        }

        public void SetToDefaults()
        {
            _currentLevelDataHolder = null;
            _associatedPositions.Clear();
        }
        
        private bool CheckIfPlayerHaveSpawnPlace(ulong clientId)
        {
            foreach (var clientIdAssociatedSpawn in _associatedPositions)
            {
                if (clientIdAssociatedSpawn.clientId == clientId)
                {
                    return true;
                }
            }

            return false;
        }

        [Rpc(SendTo.Everyone)]
        private void SendPlayerSpawnEventRpc(ulong clientId)
        {
            OnPlayerSpawned?.Invoke(clientId);
        }
    }
    

    public struct ClientIdAssociatedSpawn
    {
        public ClientIdAssociatedSpawn(ulong clientId, Vector3 position, bool isTaken)
        {
            this.clientId = clientId;
            this.position = position;
            this.isTaken = isTaken;
        }

        public ulong clientId;
        public Vector3 position;
        public bool isTaken;
    }
}

// Saved state for 01,08,2024

//[SerializeField] private GameObject Player;
// [SerializeField] private GameObject Camera;
// [SerializeField] private bool SpawnOnStart;
// [SerializeField] private List<GameObject> SpawnPositions;
//
// private void Awake()
// {
//     throw new NotImplementedException();
// }
//
// private void OnEnable()
// {
//     if (SpawnOnStart)
//     {
//         SpawnPlayerRpc();
//     }
// }
//
// [Rpc(SendTo.ClientsAndHost)]
// public void SpawnPlayerRpc(RpcParams rpcParams = default)
// {
//     GameObject player = Instantiate(Player, Vector3.zero, new Quaternion(0, 0, 0, 0));
//     if (IsOwner)
//     {
//         GameObject camera = Instantiate(Camera, Vector3.zero, new Quaternion(0, 0, 0, 0));
//                 
//         if (camera.GetComponentInChildren<CinemachineTargetGroup>())
//         {
//             camera.GetComponentInChildren<CinemachineTargetGroup>().AddMember(player.transform, 1, 0);
//         }                   
//     }
//     player.transform.position = GetRandomSpawnGameObject().transform.position;
//
//                            
// }
//
// private GameObject GetRandomSpawnGameObject()
// {
//     int chosenNumber = Random.Range(0, SpawnPositions.Count);
//     return SpawnPositions[chosenNumber];
// }
