using Core.ScriptableObjects;
using Interfaces;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.Player;
using Runtime.NetworkBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            SubscribeToEvents();
            Spawn();
            StartMatchUnityEvent?.Invoke();
        }
        private void OnDestroy()
        {
            UnSubscribeToEvents();
        }
        public void Spawn()
        {
            PlayerSpawner.Instance.SpawnPlayer(0);
            PlayerSpawner.Instance.SpawnBots();
        }

        private void RespawnPlayer()
        {
            PlayerSpawner.Instance.SpawnPlayer(3);
        }

        private void RegisterPlayerForEvents()
        {
            if (PlayerSpawner.Instance.Player.TryGetComponent(out PlayerCharacter playerCharacter)) {
                playerCharacter.OnPlayerDeath += RespawnPlayer;
            }
        }
        
        private void SubscribeToEvents()
        {
            PlayerSpawner.Instance.OnPlayerSpawned += RegisterPlayerForEvents;
        }
        private void UnSubscribeToEvents()
        {
            PlayerSpawner.Instance.OnPlayerSpawned -= RegisterPlayerForEvents;
        }
    }
}