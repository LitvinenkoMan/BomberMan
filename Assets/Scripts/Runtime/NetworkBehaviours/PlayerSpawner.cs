using System.Collections.Generic;
using System.Threading.Tasks;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours;
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
            if (IsHost)
            {
                NetworkManager.OnClientConnectedCallback += AssociateNewSpawnPlaceForClient;
                NetworkManager.OnClientConnectedCallback += SpawnClient;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsHost)
            {
                NetworkManager.OnClientConnectedCallback -= SpawnClient;
                NetworkManager.OnClientConnectedCallback -= AssociateNewSpawnPlaceForClient;
            }
        }

        
        /// <summary>
        /// Will spawn player GameObject for Client at Associated position for spawning, which is chosen randomly at moment when Client is Connected.
        /// </summary>
        /// <param name="clientId">Id of a client</param>
        public void SpawnClient(ulong clientId)
        {
            SpawnPlayerRpc(clientId);
        }
        
        /// <summary>
        /// Will spawn player GameObject for Client at Associated position for spawning, which is chosen randomly at moment when Client is Connected.
        /// </summary>
        /// <param name="clientId">Id of a client</param>
        /// <param name="spawnDelay">Delay for spawning, in seconds </param>
        public async Task SpawnClient(ulong clientId, int spawnDelay)
        {
            await Task.Delay(spawnDelay * 1000);
            SpawnPlayerRpc(clientId);
        }

        [Rpc(SendTo.Server)]
        private void SpawnPlayerRpc(ulong clientId)
        {
            GameObject player = Instantiate(Player, Vector3.zero, new Quaternion(0, 0, 0, 0));

            player.name = $"Player {clientId}";
            _associatedPositions.ForEach(x =>
                {
                    if (x.clientId == clientId)
                    {
                        player.transform.position = x.position;
                    }
                }
            );
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            player.GetComponent<BomberParamsProvider>().GetBomberParams().ResetValues();
        }

        private void AssociateNewSpawnPlaceForClient(ulong clientID)
        {
            var spawnPos = GetRandomSpawnPosition();
            
            _associatedPositions.ForEach(x =>
            {
                if (x.position == spawnPos.transform.position)
                {
                    AssociateNewSpawnPlaceForClient(clientID);
                }
            });
            Debug.Log($"Associated spawn position: {spawnPos.name} for {clientID}");
            _associatedPositions.Add(new ClientIdAssociatedSpawn(clientID, spawnPos.transform.position));
        }

        private GameObject GetRandomSpawnPosition()
        {
            int chosenNumber = Random.Range(0, _currentLevelDataHolder.SpawnPlaces.Count);
            return _currentLevelDataHolder.SpawnPlaces[chosenNumber];
        }

        public void SetUpCurrentDataHolder(LevelSectionsDataHolder dataHolder)
        {
            _currentLevelDataHolder = dataHolder;
        }
    }

    public struct ClientIdAssociatedSpawn
    {
        public ClientIdAssociatedSpawn(ulong clientId, Vector3 position)
        {
            this.clientId = clientId;
            this.position = position;
        }

        public ulong clientId;
        public Vector3 position;
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
