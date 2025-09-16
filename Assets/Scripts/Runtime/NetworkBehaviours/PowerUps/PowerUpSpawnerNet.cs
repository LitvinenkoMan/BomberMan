using System.Collections.Generic;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.NetworkBehaviours.PowerUps
{
    public class PowerUpSpawnerNet : NetworkBehaviour
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

        private IObjectPool<GameObject> _powerUpsPool;
        private GroundSection _currentSectionToSpawn;
        private float _timer;
        private bool _canSpawn;

        private void Awake()
        {
            _powerUpsPool = GetComponent<IObjectPool<GameObject>>();
            _currentSectionToSpawn = null;
        }

        public override void OnNetworkSpawn()
        {
            _powerUpsPool.Initialize();
            EnableSpawning();
            if (!IsServer)
            {
                return;
            }  
            CreatePowerUpsQueue();
        }

        private void Update()
        {
            if (!_canSpawn || !IsServer)
            {
                return;
            }
            _timer += Time.deltaTime;
            if (_timer >= TimePerSpawn)
            {
                if (ChooseSectionForSpawning())
                {
                    var powerUpObject = _powerUpsPool.GetFromPool(true);
                    if (powerUpObject && powerUpObject.TryGetComponent(out PowerUp powerUp))
                    {
                        if (!powerUp.IsSpawned) powerUp.NetworkObject.Spawn();

                        SpawnPowerUpRpc(powerUp, _currentSectionToSpawn.ObstaclePlacementPosition);
                    }
                }
                _timer = 0;
            }
        }

        private void CreatePowerUpsQueue()
        {
            for (int i = 0; i < AmountOfPreparedPowerUps; i++)
            {
                _powerUpsPool.AddToPool(Instantiate(PowerUpsExamples[Random.Range(0, PowerUpsExamples.Count)]));
            }
        }

        public void EnableSpawning()
        {
            _canSpawn = true;
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void SpawnPowerUpRpc(NetworkBehaviourReference powerUpRef, Vector3 position)
        {
            if (powerUpRef.TryGet(out PowerUp powerUp))
            {
                powerUp.SetNewPosition(position);
                powerUp.Initialize();
                powerUp.AutoPlaceToNearestSection();
            }
        }

        private bool ChooseSectionForSpawning()
        {
            _currentSectionToSpawn = null;
            var randomSection = SpawnPlaces[Random.Range(0, SpawnPlaces.Count)];
            if (randomSection.PlacedObstacle == null)
            {
                _currentSectionToSpawn = randomSection;
            }
            else
            {
                for (int i = 0; i < SpawnPlaces.Count; i++)
                {
                    if (SpawnPlaces[i].PlacedObstacle == null)
                    {
                        _currentSectionToSpawn = SpawnPlaces[i];
                        break;
                    }
                }
            }

            return _currentSectionToSpawn != null;
        }
    }
}


