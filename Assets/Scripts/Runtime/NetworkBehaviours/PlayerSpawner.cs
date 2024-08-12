using MonoBehaviours.GroundSectionSystem;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject Player;

        public static PlayerSpawner Instance;

        private LevelSectionsDataHolder _currentLevelDataHolder;

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

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                NetworkManager.OnClientConnectedCallback += SpawnClient;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsHost)
            {
                NetworkManager.OnClientConnectedCallback -= SpawnClient;
            }
        }

        private void SpawnClient(ulong clientId)
        {
            SpawnPlayerRpc(clientId);
        }

        [Rpc(SendTo.Server)]
        public void SpawnPlayerRpc(ulong clientId)
        {
            GameObject player = Instantiate(Player, Vector3.zero, new Quaternion(0, 0, 0, 0));

            player.name = $"Player {clientId}";
            player.transform.position = GetRandomSpawnPosition().transform.position;
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
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
