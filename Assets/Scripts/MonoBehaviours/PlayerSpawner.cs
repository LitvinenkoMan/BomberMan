using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject Camera;
        [SerializeField] private bool SpawnOnStart;
        [SerializeField] private List<GameObject> SpawnPositions;

        private void OnEnable()
        {
            if (SpawnOnStart)
            {
                SpawnPlayerRpc();
            }
        }

        [Rpc(SendTo.Server)]
        public void SpawnPlayerRpc(RpcParams rpcParams = default)
        {
            GameObject player = Instantiate(Player, Vector3.zero, new Quaternion(0, 0, 0, 0));
            if (IsOwner)
            {
                GameObject camera = Instantiate(Camera, Vector3.zero, new Quaternion(0, 0, 0, 0));
                
                if (camera.GetComponentInChildren<CinemachineTargetGroup>())
                {
                    camera.GetComponentInChildren<CinemachineTargetGroup>().AddMember(player.transform, 1, 0);
                }                   
            }
            player.transform.position = GetRandomSpawnGameObject().transform.position;

                           
        }

        private GameObject GetRandomSpawnGameObject()
        {
            int chosenNumber = Random.Range(0, SpawnPositions.Count);
            return SpawnPositions[chosenNumber];
        }
    }
}
