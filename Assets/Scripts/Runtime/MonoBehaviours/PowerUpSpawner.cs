using System;
using System.Collections.Generic;
using MonoBehaviours.GroundSectionSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    [RequireComponent(typeof(ObjectPoolQueue))]
    public class PowerUpSpawner : NetworkBehaviour
    {
        [Header("List of Power ups to spawn")]
        [SerializeField]
        private List<GameObject> PowerUpsExamples;

        [Space(15)] [Header("Spawn Settings")] 
        [SerializeField, Tooltip("The time that Power ups need to be spawned")]
        private float TimePerSpawn;

        [SerializeField, Tooltip("Amount of power ups that should to be instanced before game starts")]
        private int AmountOfPreparedPowerUps;
        
        [SerializeField]
        private List<GroundSection> SpawnPlaces;

        private ObjectPoolQueue _powerUpsPool;
        private GroundSection _currentSectionToSpawn;
        private float _timer;
        private bool _canSpawn;

        private void Start()
        {
           
            if (!IsServer)
            {
                Debug.Log("Exiting from creating Power ups Queue");
                _canSpawn = false;
                return;
            }  
            Debug.Log("Creating Queue");
            CreatePowerUpsQueue();
            EnableSpawning();
        }

        public override void OnNetworkSpawn()
        {
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_canSpawn && _timer >= TimePerSpawn)
            {
                var powerUpObject = _powerUpsPool.GetFromPool(true);
                var powerUp = powerUpObject.GetComponent<PowerUp>();            //TODO: Change this!
                SpawnPowerUpRpc(powerUp);
                _timer = 0;
            }
        }

        private void CreatePowerUpsQueue()
        {
            for (int i = 0; i < AmountOfPreparedPowerUps; i++)
            {
                _powerUpsPool.AddToPool(Instantiate(PowerUpsExamples[Random.Range(0, PowerUpsExamples.Count)]));
            }
            _currentSectionToSpawn = SpawnPlaces[Random.Range(0, SpawnPlaces.Count)];
        }

        public void EnableSpawning()
        {
            _canSpawn = true;
        }
        
        [Rpc(SendTo.Server)]
        private void SpawnPowerUpRpc(PowerUp powerUpRef)
        {
            _currentSectionToSpawn = null;
            var randomSection = SpawnPlaces[Random.Range(0, SpawnPlaces.Count)];
            if (!randomSection.PlacedObstacle)
            {
                _currentSectionToSpawn = randomSection;
            }
            else
            {
                for (int i = 0; i < SpawnPlaces.Count; i++)
                {
                    if (!SpawnPlaces[i].PlacedObstacle)
                    {
                        _currentSectionToSpawn = SpawnPlaces[i];
                        break;
                    }
                }
            }

            if (!_currentSectionToSpawn) return;

            _currentSectionToSpawn.AddObstacle(powerUpRef);
            powerUpRef.NetworkObject.Spawn();
        }
    }
}


