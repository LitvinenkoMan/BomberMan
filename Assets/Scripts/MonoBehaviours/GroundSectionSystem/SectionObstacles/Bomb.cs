using System;
using System.Collections;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    [RequireComponent(typeof(SphereCollider))]
    public class Bomb : Obstacle, IBomb
    {
        public bool IgniteOnStart;
        public Action<Bomb> onExplode;
        
        [SerializeField]
        private BasePlayerParameters PlayerParams;
        [SerializeField]
        private GameObject BombVisuals;
        
        private Collider _bombCollider;
        //private float _explosionSpeed = 1;
        private float _timer;
        private bool _isTimerOn;
        private bool _isExploded;

        private void Start()
        {
            CanReceiveDamage = true;
            _bombCollider = GetComponent<SphereCollider>();
        }

        private void OnEnable()
        {
            _timer = PlayerParams.BombsCountdown;
            if (IgniteOnStart)
            {
                Ignite();
            }

            ObstacleHealthComponent.OnHealthChanged += OnHealthRunOutExplode;
        }

        private void OnDisable()
        {
            ObstacleHealthComponent.OnHealthChanged -= OnHealthRunOutExplode;
        }

        private void Update()
        {
            if (_isTimerOn)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    Explode();
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
            _timer = PlayerParams.BombsCountdown;
            _isTimerOn = false;
            _isExploded = false;
            _bombCollider.isTrigger = true;
            BombVisuals.SetActive(true);
            ObstacleHealthComponent.SetHealth(1);
        }

        public void PlaceBomb(Vector3 newPos)
        {
            transform.position = newPos;
        }

        public void Ignite()
        {
            _isTimerOn = true;
        }

        public void Explode()
        {
            BombVisuals.SetActive(false);
            
            GroundSection startSection = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(transform.position);
            startSection.RemoveObstacle();

            PlaceExplosionEffect(startSection);

            ExplodeToDirection(startSection.ConnectedSections.upperSection, PlayerParams.BombsSpreading - 1,
                SpreadDirections.Up);
            ExplodeToDirection(startSection.ConnectedSections.lowerSection, PlayerParams.BombsSpreading - 1,
                SpreadDirections.Down);
            ExplodeToDirection(startSection.ConnectedSections.rightSection, PlayerParams.BombsSpreading - 1,
                SpreadDirections.Right);
            ExplodeToDirection(startSection.ConnectedSections.leftSection, PlayerParams.BombsSpreading - 1,
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
                DamageObstacle(currentSection.PlacedObstacle);
                return;
            }

            PlaceExplosionEffect(currentSection);

            TryDamageHealthComponent(currentSection.ObstaclePlacementPosition);
            
            
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
                        ExplodeToDirection(currentSection.ConnectedSections.rightSection, depth,
                            SpreadDirections.Right);
                    break;
                case SpreadDirections.Left:
                    if (currentSection.ConnectedSections.leftSection)
                        ExplodeToDirection(currentSection.ConnectedSections.leftSection, depth, SpreadDirections.Left);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void PlaceExplosionEffect(GroundSection currentSection)
        {
            GameObject expl = GroundSectionsUtils.Instance.ExplosionsPool.GetFromPool(true);
            expl.transform.position = currentSection.ObstaclePlacementPosition;
            StartCoroutine(ReturnExplosionToPool(expl));
        }

        private void DamageObstacle(Obstacle obstacle)
        {
            if (obstacle.CanReceiveDamage)
            {
                obstacle.ObstacleHealthComponent.SetHealth((byte)(obstacle.ObstacleHealthComponent.HealthPoints - 1));
            }
        }

        private void TryDamageHealthComponent(Vector3 position)
        {
            Collider[] colliders = Array.Empty<Collider>();
            Physics.OverlapBoxNonAlloc(position, Vector3.one, colliders);

            for (int i = 0; i < colliders.Length; i++)
            {
                HealthComponent health;
                colliders[i].TryGetComponent(out health);
                if (health)
                {
                    health.SetHealth((byte)(health.HealthPoints-PlayerParams.BombsDamage));
                }
            }
        }

        private void OnHealthRunOutExplode(byte healthLeft)
        {
            if (healthLeft <= 0)
            {
                Explode();
            }
        }

        private IEnumerator ReturnExplosionToPool(GameObject expl)
        {
            yield return new WaitForSeconds(2);
            GroundSectionsUtils.Instance.ExplosionsPool.AddToPool(expl);
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