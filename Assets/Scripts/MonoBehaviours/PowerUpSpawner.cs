using System;
using System.Collections.Generic;
using MonoBehaviours.GroundSectionSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    [RequireComponent(typeof(ObjectPoolQueue))]
    public class PowerUpSpawner : MonoBehaviour
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
            _powerUpsPool = GetComponent<ObjectPoolQueue>();
            CreatePowerUpsQueue();
            EnableSpawning();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_canSpawn && _timer >= TimePerSpawn)
            {
                SpawnPowerUp();
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

        private void SpawnPowerUp()
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
            
            var powerUpObject = _powerUpsPool.GetFromPool(true);
            var powerUp = powerUpObject.GetComponent<PowerUp>();
            _currentSectionToSpawn.AddObstacle(powerUp);
        } 
    }
}
