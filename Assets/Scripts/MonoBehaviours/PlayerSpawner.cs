using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject Camera;
        [SerializeField] private List<GameObject> SpawnPositions;

        private void OnEnable()
        {
            SpawnPlayer();
        }

        private void OnDisable()
        {
            // make it return everything back;
        }

        void Update()
        {
            
        }

        public void SpawnPlayer()
        {
            GameObject player = Instantiate(Player, Vector3.zero, new Quaternion(0, 0, 0, 0));
            GameObject camera = Instantiate(Camera, Vector3.zero, new Quaternion(0, 0, 0, 0));
            player.transform.position = GetRandomSpawnGameObject().transform.position;


            if (camera.GetComponentInChildren<CinemachineTargetGroup>())
            {
                camera.GetComponentInChildren<CinemachineTargetGroup>().AddMember(player.transform, 1, 0);
            }                                              
        }

        private GameObject GetRandomSpawnGameObject()
        {
            int chosenNumber = Random.Range(0, SpawnPositions.Count);
            return SpawnPositions[chosenNumber];
        }
    }
}
