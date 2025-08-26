using Core.ScriptableObjects;
using Interfaces;
using Runtime.MonoBehaviours;
using Runtime.NetworkBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.NetworkBehaviours.MatchManagers
{
    public class TrainingMatchManager : MonoBehaviour
    {
        [SerializeField] private BaseBomberParameters bomberParams;


        public UnityEvent StartMatchUnityEvent;

        private void Start()
        {
            SpawnPlayer();
            SubscribeToEvents();
            StartMatchUnityEvent?.Invoke();
        }
        private void OnDestroy()
        {
            UnSubscribeToEvents();
        }
        public void SpawnPlayer()
        {
            //PlayerSpawner.Instance.RandomSpawnPlayer(); 
        }

        private void ResetPlayerParams()
        {
            PlayerSpawner.Instance.Player.TryGetComponent(out ICharacter playerCharacter);
            playerCharacter.Reset();
        }
        
        private void SubscribeToEvents()
        {
            PlayerSpawner.Instance.OnPlayerSpawned += ResetPlayerParams;
        }
        private void UnSubscribeToEvents()
        {
            PlayerSpawner.Instance.OnPlayerSpawned -= ResetPlayerParams;
        }
    }
}