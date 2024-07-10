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
            
            ExplodeToUp(startSection, PlayerParams.BombsSpreading);
            ExplodeToDown(startSection, PlayerParams.BombsSpreading);
            ExplodeToRight(startSection, PlayerParams.BombsSpreading);
            ExplodeToLeft(startSection, PlayerParams.BombsSpreading);
            
            _isTimerOn = false;
            _isExploded = true;
            BombCollider.isTrigger = true;
            onExplode.Invoke(this);
        }

        private void ExplodeToUp(GroundSection currentSection, int depth)
        {
            if (depth <= 0 )
            {
                return;
            }

            depth -= 1;
            
            PlaceExplosionEffect(currentSection);
            if (currentSection.ConnectedSections.upperSection != null)
            {
                if (currentSection.ConnectedSections.upperSection.PlacedObstacle == null)
                {
                    ExplodeToUp(currentSection.ConnectedSections.upperSection, depth);
                }
                else
                {
                    DamageObstacle(currentSection.ConnectedSections.upperSection.PlacedObstacle);
                }
            }
        }

        private void ExplodeToDown(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);

            if (currentSection.ConnectedSections.lowerSection != null)
            {
                if (currentSection.ConnectedSections.lowerSection.PlacedObstacle == null)
                {
                    ExplodeToDown(currentSection.ConnectedSections.lowerSection, depth);
                }
                else
                {
                    DamageObstacle(currentSection.ConnectedSections.lowerSection.PlacedObstacle);
                }
            }
        }

        private void ExplodeToRight(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);

            if (currentSection.ConnectedSections.rightSection != null)
            {
                if (currentSection.ConnectedSections.rightSection.PlacedObstacle == null)
                {
                    ExplodeToRight(currentSection.ConnectedSections.rightSection, depth);
                }
                else
                {
                    DamageObstacle(currentSection.ConnectedSections.rightSection.PlacedObstacle);
                }
            }
        }

        private void ExplodeToLeft(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);
            
            if (currentSection.ConnectedSections.leftSection != null)
            {
                if (currentSection.ConnectedSections.leftSection.PlacedObstacle == null)
                {
                    ExplodeToLeft(currentSection.ConnectedSections.leftSection, depth);
                }
                else
                {
                    DamageObstacle(currentSection.ConnectedSections.leftSection.PlacedObstacle);
                }
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
