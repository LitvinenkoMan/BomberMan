using System;
using System.Collections;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    public class Bomb : Obstacle, IBomb
    {
        public bool IgniteOnStart;
        public Action<Bomb> onExplode;
        
        [SerializeField]
        private BasePlayerParameters PlayerParams;
        [SerializeField]
        private GameObject BombVisuals;

        //private float _explosionSpeed = 1;
        private float _timer;
        private bool _isTimerOn;
        
        private void OnEnable()
        {
            HealthPoints = 1;
            CanPlayerStepOnIt = false;
            CanReceiveDamage = true;
            
            _timer = PlayerParams.BombsCountdown;
            if (IgniteOnStart)
            {
                Ignite();
            }

            OnHealthChanged += OnHealthRunOutExplode;
        }

        private void OnDisable()
        {
            OnHealthChanged -=OnHealthRunOutExplode;
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
            onExplode.Invoke(this);
        }

        public void Reset()
        {
            _timer = PlayerParams.BombsCountdown;
            _isTimerOn = false;
            BombVisuals.SetActive(true);
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
