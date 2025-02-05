using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoBehaviours.Network;
using UnityEngine;

namespace Runtime.NetworkBehaviours.MatchManagers
{
    public class DeathMatchMm : MatchManager
    {
        [Space(20)]
        [Header("Death Match Params")]
        [SerializeField]
        private byte InitialPlayersLifeCount = 3;
        
        private Dictionary<ulong, int> _playersLifeCount;

        public override void OnNetworkSpawn()
        {
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
                //RegisterPlayerForEvents(NetworkManager.Singleton.LocalClientId);
                HostPanel.SetActive(true);
            }
            else
            {
                ClientPannel.SetActive(true);
            }

            JoinCodeText.text = RelayManager.Instance.JoinCode;
            _playersLifeCount = new Dictionary<ulong, int>();
        }

        private void AddPlayerToLifeCounter(ulong clientId)
        {
            _playersLifeCount.TryAdd(clientId, InitialPlayersLifeCount);
        }

        private void RemovePlayerFromLifeCounter(ulong clientId)
        {
            _playersLifeCount.Remove(clientId);
        }

        private void SubtractLifeForPlayer(ulong clientId)
        {
            _playersLifeCount[clientId] -= 1;
            Debug.Log($"Player {clientId} lost Life! {_playersLifeCount[clientId]} is left");
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
            base.SubscribeToRespawnEvents();
            NetworkManager.OnClientConnectedCallback += AddPlayerToLifeCounter;
            NetworkManager.OnClientDisconnectCallback += RemovePlayerFromLifeCounter;
        }

        protected override void UnsubscribeFromRespawnEvents()
        {
            base.UnsubscribeFromRespawnEvents();
            NetworkManager.OnClientConnectedCallback -= AddPlayerToLifeCounter;
            NetworkManager.OnClientDisconnectCallback -= RemovePlayerFromLifeCounter;
        }
    }
}
