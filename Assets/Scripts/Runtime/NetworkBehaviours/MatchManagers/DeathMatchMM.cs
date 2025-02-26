using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoBehaviours.Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.NetworkBehaviours.MatchManagers
{
    public class DeathMatchMm : MatchManager
    {
        [Space(20)]
        [Header("Death Match Params")]
        [Space(10)]

        [SerializeField]
        private byte InitialPlayersLifeCount = 3;
        
        private Dictionary<ulong, int> _playersLifeCount;

        public Action<ulong, int> OnPlayerLifeCountSubtracted;

        public override void OnNetworkSpawn()
        {
            _hasMatchBegan = false;
            _playersLifeCount = new Dictionary<ulong, int>();
            
            HostPanel.SetActive(false);
            ClientPannel.SetActive(false);
            if (IsServer)
            {
                SubscribeToRespawnEvents();
                
                foreach (var client in NetworkManager.ConnectedClients)
                {
                    PlayerSpawner.Instance.SpawnClientRpc(client.Key);
                    AddPlayerToLifeCounter(client.Key);
                }
                RegisterPlayerForEvents(NetworkManager.Singleton.LocalClientId);
                HostPanel.SetActive(true);
            }
            else
            {
                ClientPannel.SetActive(true);
            }

            JoinCodeText.text = RelayManager.Instance.JoinCode;
            
            _isInitialized = true;
            OnInitialized?.Invoke();
        }
        
        private void AddPlayerToLifeCounter(ulong clientId)
        {
            _playersLifeCount.TryAdd(clientId, InitialPlayersLifeCount);
            AddPlayerOnLocalDataRpc(clientId, InitialPlayersLifeCount);
            Debug.Log($"Player {clientId} Added to life counter, {_playersLifeCount[clientId]} is left");
        }

        private void RemovePlayerFromLifeCounter(ulong clientId)
        {
            _playersLifeCount.Remove(clientId);
        }

        public int GetPlayerLifeCount(ulong clientId)
        {
            return _playersLifeCount[clientId];
        }

        private void SubtractLifeForPlayer(ulong clientId)
        {
            UpdateLocalDataForPlayerRpc(clientId, _playersLifeCount[clientId] - 1);
            OnPlayerLifeCountSubtracted?.Invoke(clientId, _playersLifeCount[clientId]);
            Debug.Log($"Player {clientId} lost Life! {_playersLifeCount[clientId]} is left");
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateLocalDataForPlayerRpc(ulong clientId, int lifeCount)
        {
            _playersLifeCount[clientId] = lifeCount;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void AddPlayerOnLocalDataRpc(ulong clientId, int lifeCount)
        {
            _playersLifeCount.TryAdd(clientId, lifeCount);
        }

        protected override void SendRespawnRequestForPlayerWrapper(ulong clientID)
        {
            if (_playersLifeCount[clientID] > 0)
            {
                base.SendRespawnRequestForPlayerWrapper(clientID);
            }
        }

        protected override Task SendRespawnRequestForPlayer(ulong clientID)
        {
            SubtractLifeForPlayer(clientID);
            return base.SendRespawnRequestForPlayer(clientID);
        }

        protected override void SubscribeToRespawnEvents()
        {
            NetworkManager.OnClientConnectedCallback += AddPlayerToLifeCounter;
            NetworkManager.OnClientDisconnectCallback += RemovePlayerFromLifeCounter;
            base.SubscribeToRespawnEvents();
        }

        protected override void UnsubscribeFromRespawnEvents()
        {
            NetworkManager.OnClientConnectedCallback -= AddPlayerToLifeCounter;
            NetworkManager.OnClientDisconnectCallback -= RemovePlayerFromLifeCounter;
            base.UnsubscribeFromRespawnEvents();
        }
    }
}
