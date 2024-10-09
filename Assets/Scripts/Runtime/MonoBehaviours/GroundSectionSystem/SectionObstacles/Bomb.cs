using System;
using System.Collections;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.GroundSectionSystem;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    [RequireComponent(typeof(SphereCollider))]
    public class Bomb : Obstacle, INetworkSerializable
    {
        public bool IgniteOnStart;
        public Action<Bomb> onExplode;
        
        [SerializeField]
        private BaseBomberParameters bomberParams;
        [SerializeField]
        private GameObject BombVisuals;
        
        private Collider _bombCollider;
        //private float _explosionSpeed = 1;
        private float _timer; 
        private bool _isTimerOn;
        private bool _isExploded;

        private int _bombSpread;
        private float _timeToExplode;
        private int _bombDamage;
        

        public new void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IgniteOnStart);
            serializer.SerializeValue(ref _timer);
            serializer.SerializeValue(ref _isTimerOn);
            serializer.SerializeValue(ref _isExploded);
            //bomberParams.NetworkSerialize(serializer);
        }

        private void Start()
        {
            CanReceiveDamage = true;
            CanPlayerStepOnIt = false;
            _bombCollider = GetComponent<SphereCollider>();
        }

        private void OnEnable()
        {
            // if (IgniteOnStart)
            // {
            //     Ignite(3, 1, );              TODO: Change Ignite on start logic
            // }
            //_timer = _timeToExplode;

            ObstacleHealthComponent.OnHealthRunOut += OnHealthRunOutExplode;
        }

        private void OnDisable()
        {
            ObstacleHealthComponent.OnHealthRunOut -= OnHealthRunOutExplode;
        }

        private void Update()
        {
            if (_isTimerOn)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    ExplodeRpc(_bombSpread);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isExploded)
            {
                _bombCollider.isTrigger = false;
            }
        }

        public void Reset()
        {
            _timer = _timeToExplode;
            _isTimerOn = false;
            _isExploded = false;
            _bombCollider.isTrigger = true;
            BombVisuals.SetActive(true);
            ObstacleHealthComponent.SetHealth(1);
        }

        public void Ignite(float timeToExplode, int bombDamage, int bombSpread)
        {
            _timeToExplode = timeToExplode;
            _bombDamage = bombDamage;
            _bombSpread = bombSpread;
            
            _timer = _timeToExplode;
            _isTimerOn = true;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ExplodeRpc(int bombSpreading)
        {
            BombVisuals.SetActive(false);
            
            GroundSection startSection = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            startSection.RemoveObstacle();

            PlaceExplosionEffect(startSection.ObstaclePlacementPosition);

            ExplodeToDirection(startSection.ConnectedSections.upperSection, bombSpreading - 1,
                SpreadDirections.Up);
            ExplodeToDirection(startSection.ConnectedSections.lowerSection, bombSpreading - 1,
                SpreadDirections.Down);
            ExplodeToDirection(startSection.ConnectedSections.rightSection, bombSpreading - 1,
                SpreadDirections.Right);
            ExplodeToDirection(startSection.ConnectedSections.leftSection, bombSpreading - 1,
                SpreadDirections.Left);

            _isTimerOn = false;
            _isExploded = true;
            _bombCollider.isTrigger = true;
            onExplode?.Invoke(this);
        }

        private void ExplodeToDirection(GroundSection currentSection, int depth, SpreadDirections direction)
        {
            if (currentSection.PlacedObstacle)
            {
                if (!currentSection.PlacedObstacle.CanPlayerStepOnIt)
                {
                    DamageObstacle(currentSection.PlacedObstacle);
                    return;
                }
                DamageObstacle(currentSection.PlacedObstacle);
            }

            PlaceExplosionEffect(currentSection.ObstaclePlacementPosition);

            TryDamageActorsOrPlayer(currentSection.ObstaclePlacementPosition);
            
            
            if (depth <= 0 )
            {
                return;
            }
            depth -= 1;
            
            switch (direction)
            {
                case SpreadDirections.Up:
                    if (currentSection.ConnectedSections.upperSection)
                        ExplodeToDirection(currentSection.ConnectedSections.upperSection, depth, SpreadDirections.Up);
                    break;
                case SpreadDirections.Down:
                    if (currentSection.ConnectedSections.lowerSection)
                        ExplodeToDirection(currentSection.ConnectedSections.lowerSection, depth, SpreadDirections.Down);
                    break;
                case SpreadDirections.Right:
                    if (currentSection.ConnectedSections.rightSection)
                        ExplodeToDirection(currentSection.ConnectedSections.rightSection, depth, SpreadDirections.Right);
                    break;
                case SpreadDirections.Left:
                    if (currentSection.ConnectedSections.leftSection)
                        ExplodeToDirection(currentSection.ConnectedSections.leftSection, depth, SpreadDirections.Left);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void PlaceExplosionEffect(Vector3 explPosition)
        {
            GameObject expl = GroundSectionsUtils.Instance.ExplosionsPool.GetFromPool(true);
            expl.transform.position = explPosition;
            StartCoroutine(ReturnExplosionToPool(expl));
        }

        private void DamageObstacle(Obstacle obstacle)
        {
            if (obstacle.CanReceiveDamage)
            {
                obstacle.ObstacleHealthComponent.SetHealth(obstacle.ObstacleHealthComponent.HealthPoints - _bombDamage);
            }
        }

        private void TryDamageActorsOrPlayer(Vector3 position)
        {
            Collider[] colliders = Physics.OverlapBox(position, new Vector3(1, 1, 1));

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.TryGetComponent(out HealthComponent health) 
                    &&
                    !colliders[i].gameObject.GetComponent<Obstacle>()) //TODO: recode this check, looks bad
                {
                    health.SetHealth(health.HealthPoints - bomberParams.BombsDamage);
                }

                if (colliders[i].gameObject.TryGetComponent(out BomberParamsProvider paramsProvider))
                {
                    paramsProvider.GetBomberParams()
                        .SetActorHealth((byte)(paramsProvider.GetBomberParams().ActorHealth - _bombDamage));
                }
            }
        }

        private void OnHealthRunOutExplode()
        {
            ExplodeRpc(bomberParams.BombsSpreading);
        }

        private IEnumerator ReturnExplosionToPool(GameObject expl)
        {
            yield return new WaitForSeconds(2);
            GroundSectionsUtils.Instance.ExplosionsPool.AddToPool(expl);
        }

        public override void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}

enum SpreadDirections
{
    Up,
    Down,
    Right,
    Left
} 