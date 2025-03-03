using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoBehaviours.Network;
using Runtime.MonoBehaviours;
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

        public event Action<int> OnLocalPlayerLifeCountUIUpdate;
        public event Action<int> OnLifeCountInfoReceived;

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
                .TryGetComponent(out DeathResultHandler deathResultHandler))
            {
                deathResultHandler.OnPlayerDeathAction += RequestRespawnRpc;
            }
        }

        protected override void SendRespawnRequestForPlayerWrapper(ulong clientID)
        {
            if (_playersLifeCount[clientID] > 0)
            {
                base.SendRespawnRequestForPlayerWrapper(clientID);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void RequestRespawnRpc(ulong clientId)
        {
            SubtractLifeForPlayerRpc(clientId);
            SendRespawnRequestForPlayerWrapper(clientId);
        }

        [Rpc(SendTo.Server)]
        private void SubtractLifeForPlayerRpc(ulong clientId)
        {
            _playersLifeCount[clientId] -= 1;
            SendLifeCountDataRpc(_playersLifeCount[clientId], RpcTarget.Single(clientId, RpcTargetUse.Temp));
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
