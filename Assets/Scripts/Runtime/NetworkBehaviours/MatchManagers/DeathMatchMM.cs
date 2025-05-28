using System;
using System.Collections.Generic;
using Interfaces;
using MonoBehaviours.Network;
using Runtime.NetworkBehaviours.Player;
using Unity.Netcode;
using UnityEngine;

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

        public event Action<int> OnLocalPlayerLifeCountUIUpdate;
        public event Action<int> OnLifeCountInfoReceived;
        public event Action<ulong> OnWinnerAppeared;
        public event Action OnEnableClientView; 
        public event Action OnEnableServerView; 
        public event Action<bool> OnResetUI; // if send with true - is server, else is client

        public override void OnNetworkSpawn()
        {
            _hasMatchBegan = false;
            _playersLifeCount = new Dictionary<ulong, int>();

            ResetUIRpc();
            
            if (IsServer)
            {
                SubscribeToRespawnEvents();
                
                foreach (var client in NetworkManager.ConnectedClients)
                {
                    PlayerSpawner.Instance.SpawnClientRpc(client.Key);
                    AddPlayerToLifeCounter(client.Key);
                }
                
                //TODO: Add section for bots
                
            }

            JoinCodeText.text = RelayManager.Instance.JoinCode;
            
            _isInitialized = true;
            OnInitialized?.Invoke();
        }
        
        private void AddPlayerToLifeCounter(ulong clientId)
        {
            _playersLifeCount.Add(clientId, InitialPlayersLifeCount);
            SendLifeCountDataRpc(_playersLifeCount[clientId], RpcTarget.Single(clientId, RpcTargetUse.Temp));
            Debug.Log($"Player {clientId} Added to life counter, {_playersLifeCount[clientId]} is left");
        }

        private void RemovePlayerFromLifeCounter(ulong clientId)
        {
            _playersLifeCount.Remove(clientId);
        }

        protected override void RegisterPlayerForEvents(ulong clientID)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject
                .TryGetComponent(out PlayerCharacterNet playerCharacter))
            {
                Debug.Log($"Player {clientID} Subscribed for Respawn request from DMM");
                playerCharacter.OnPlayerDeath += RequestRespawnRpc;             //TODO: I should to solve it, but how...
            }
        }

        protected override void SendRespawnRequestForPlayerWrapper(ulong clientID)
        {
            if (_playersLifeCount[clientID] > 0)
            {
                base.SendRespawnRequestForPlayerWrapper(clientID);
            }
        }

        private void CheckForVictoryConditions()
        {
            byte defeatedPlayers = 0;
            ulong winnerId = 999;
            foreach (var player in _playersLifeCount)
            {
                if (player.Value == 0)
                {
                    defeatedPlayers += 1;
                }
                else winnerId = player.Key;
            }

            if (_playersLifeCount.Count - 1 == defeatedPlayers)
            {
                ShowVictoryPanelRpc(winnerId);
            }
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void ResetUIRpc()
        {
            OnResetUI?.Invoke(true);
            if (IsServer)
            {
                HostPanel.SetActive(true);
                ClientPannel.SetActive(false);
            }
            else
            {
                HostPanel.SetActive(false);
                ClientPannel.SetActive(true);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ShowVictoryPanelRpc(ulong winnerId)
        {
            OnWinnerAppeared?.Invoke(winnerId);
        }

        [Rpc(SendTo.Server)]
        private void RequestRespawnRpc(ulong clientId)
        {
            Debug.Log($"Player {clientId} Requested for respawn");
            
            SubtractLifeForPlayer(clientId);
            SendRespawnRequestForPlayerWrapper(clientId);
        }

        private void SubtractLifeForPlayer(ulong clientId)
        {
            if (_playersLifeCount[clientId] - 1 >= 0)
            {
                _playersLifeCount[clientId] -= 1;
            }
            
            SendLifeCountDataRpc(_playersLifeCount[clientId], RpcTarget.Single(clientId, RpcTargetUse.Temp));
            CheckForVictoryConditions();
            Debug.Log($"Player {clientId} lost Life! {_playersLifeCount[clientId]} is left");
        }

        [Rpc(SendTo.Server)]
        public void RequestLifeCountDataRpc(ulong clientId)
        {
            SendLifeCountDataRpc(_playersLifeCount[clientId], RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }
        
        [Rpc(SendTo.SpecifiedInParams)]
        private void SendLifeCountDataRpc(int lifeCount, RpcParams rpcParams = default)
        {
            OnLifeCountInfoReceived?.Invoke(lifeCount);
        }

        private void ResetPlayerParams(ulong clientId)
        {
            try
            {
                NetworkManager.ConnectedClients[clientId].PlayerObject.TryGetComponent(out ICharacter playerCharacter);
                playerCharacter.Reset();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine($"Player is missing!\n {e}");
            }
        }

        protected override void SubscribeToRespawnEvents()
        {
            NetworkManager.OnClientConnectedCallback += AddPlayerToLifeCounter;
            NetworkManager.OnClientDisconnectCallback += RemovePlayerFromLifeCounter;
            PlayerSpawner.Instance.OnPlayerSpawned += ResetPlayerParams;
            base.SubscribeToRespawnEvents();
        }

        protected override void UnsubscribeFromRespawnEvents()
        {
            NetworkManager.OnClientConnectedCallback -= AddPlayerToLifeCounter;
            NetworkManager.OnClientDisconnectCallback -= RemovePlayerFromLifeCounter;
            PlayerSpawner.Instance.OnPlayerSpawned -= ResetPlayerParams;
            base.UnsubscribeFromRespawnEvents();
        }
    }
}
