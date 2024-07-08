using System;
using System.Collections;
using System.Net;
using Interfaces;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    public class Bomb : Obstacle, IBomb
    {
        public bool IgniteOnStart;
        
        [SerializeField]
        private BasePlayerParameters PlayerParams;
        [SerializeField]
        private GameObject BombVisuals; 

        //private float _explosionSpeed = 1;
        private float _timer;
        private bool _isTimerOn;
        
        private void OnEnable()
        {
            _timer = PlayerParams.BombsCountdown;
            if (IgniteOnStart)
            {
                Ignite();
            }
        }

        private void OnDisable()
        {
            
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
            //
            // for (int i = 1; i <= PlayerParams.BombsSpreading; i++)
            // {
            //     var adjustedPosition = new Vector3(0, 0, i);
            //     adjustedPosition += transform.position;
            //
            //     GroundSection nearSection = GroundSectionsUtils.Instance.GetNearestSectionFromPosition(adjustedPosition);
            //     if (nearSection.PlacedObstacle) break;
            //     
            //     GameObject expl = GroundSectionsUtils.Instance.ExplosionsPool.GetFromPool(true);
            //     expl.transform.position = nearSection.ObstaclePlacementPosition;
            //     
            //     StartCoroutine(ReturnExplosionToPool(expl));
            // }

            _isTimerOn = false;      
        }

        public void Reset()
        {
            _timer = PlayerParams.BombsCountdown;
            BombVisuals.SetActive(true);
        }

        private IEnumerator ReturnExplosionToPool(GameObject expl)
        {
            yield return new WaitForSeconds(2);
            GroundSectionsUtils.Instance.ExplosionsPool.AddToPool(expl);
        }

        private void ExplodeToUp(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);
            
            ExplodeToUp(currentSection.ConnectedSections.upperSection, depth);
        }

        private void ExplodeToDown(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);
            
            ExplodeToDown(currentSection.ConnectedSections.lowerSection, depth);
        }

        private void ExplodeToRight(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);
            
            ExplodeToRight(currentSection.ConnectedSections.rightSection, depth);
        }

        private void ExplodeToLeft(GroundSection currentSection, int depth)
        {
            if (depth <= 0 || currentSection.PlacedObstacle)
            {
                return;
            }
            depth -= 1;
            
            PlaceExplosionEffect(currentSection);
            
            ExplodeToLeft(currentSection.ConnectedSections.leftSection, depth);
        }

        private void PlaceExplosionEffect(GroundSection currentSection)
        {
            GameObject expl = GroundSectionsUtils.Instance.ExplosionsPool.GetFromPool(true);
            expl.transform.position = currentSection.ObstaclePlacementPosition;
            StartCoroutine(ReturnExplosionToPool(expl));
        }


        // Explosion Mechanic
        // private IEnumerator ExplodeToUpDirectionWithDepth(GroundSection startSection, byte Depth)
        // {
        //     var explodeToUpDirectionWithDepth = ExplodeToUpDirectionWithDepth(startSection.ConnectedSections.upperSection, Depth);
        //     
        //     if (Depth <= 0) yield break;
        //     
        //     GameObject ExplosionVFX = GroundSectionsUtils.Instance.ExplosionsPool.GetFromPool();
        //     ExplosionVFX.transform.position = startSection.ObstaclePlacementPosition;
        //     yield return new WaitForSeconds((PlayerParams.BombsSpreading - Depth) / _explosionSpeed);
        //     
        //     Depth--;
        //     StartCoroutine(explodeToUpDirectionWithDepth);
        //
        //     yield return new WaitForSeconds(2);
        //     GroundSectionsUtils.Instance.ExplosionsPool.AddToPool(ExplosionVFX);
        //     StopCoroutine(explodeToUpDirectionWithDepth);
        //     yield break;
        // }
    }
}
