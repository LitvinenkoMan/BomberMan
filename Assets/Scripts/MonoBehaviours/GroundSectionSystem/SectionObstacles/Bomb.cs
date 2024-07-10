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
        
        private Collider BombCollider;
        //private float _explosionSpeed = 1;
        private float _timer;
        private bool _isTimerOn;
        private bool _isExploded;

        private void Awake()
        {
            HealthPoints = 1;
            CanPlayerStepOnIt = false;
            CanReceiveDamage = true;

            BombCollider = GetComponent<SphereCollider>();
        }

        private void OnEnable()
        {
            _timer = PlayerParams.BombsCountdown;
            if (IgniteOnStart)
            {
                Ignite();
            }

            OnHealthChanged += OnHealthRunOutExplode;
        }

        private void OnDisable()
        {
            OnHealthChanged -= OnHealthRunOutExplode;
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
                BombCollider.isTrigger = false;
            }
        }

        public void Reset()
        {
            _timer = PlayerParams.BombsCountdown;
            _isTimerOn = false;
            _isExploded = false;
            BombCollider.isTrigger = true;
            BombVisuals.SetActive(true);
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
            
            ExplodeToDirection(startSection, PlayerParams.BombsSpreading, SpreadDirections.Up);
            ExplodeToDirection(startSection, PlayerParams.BombsSpreading, SpreadDirections.Down);
            ExplodeToDirection(startSection, PlayerParams.BombsSpreading, SpreadDirections.Right);
            ExplodeToDirection(startSection, PlayerParams.BombsSpreading, SpreadDirections.Left);
            
            _isTimerOn = false;
            _isExploded = true;
            BombCollider.isTrigger = true;
            onExplode.Invoke(this);
        }

        private void ExplodeToDirection(GroundSection currentSection, int depth, SpreadDirections direction)
        {
            if (currentSection.PlacedObstacle)
            {
                DamageObstacle(currentSection.PlacedObstacle);
                return;
            }

            PlaceExplosionEffect(currentSection);
            
            // if (Player)
            // {
            //     -HealthPoints;
            // }
            
            
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
                obstacle.SetHealthPoints((byte)(obstacle.HealthPoints - 1));
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