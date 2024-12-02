using System;
using System.Collections;
using System.Threading.Tasks;
using MonoBehaviours.Network;
using Runtime.MonoBehaviours;
using Runtime.NetworkBehaviours;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace MonoBehaviours
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text JoinCodeText;
        [SerializeField] private GameObject HostPanel;
        [SerializeField] private GameObject ClientPannel;

        private bool _hasMatchBegan;
        private float _spawnCheckTimer = 0f;

        public UnityEvent StartMatchUnityEvent;

        public override void OnNetworkSpawn()
        {
            HostPanel.SetActive(false);
            ClientPannel.SetActive(false);
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += PlayerSpawner.Instance.SpawnClientRpc;
                
                NetworkManager.OnClientDisconnectCallback += PlayerSpawner.Instance.ClearSpawnPositionOfPlayer;
                
                PlayerSpawner.Instance.OnPlayerSpawned += RegisterPlayerForEvents;
                PlayerSpawner.Instance.OnPlayerSpawned += DisablePlayerAbilitiesOnConnect;

                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    PlayerSpawner.Instance.SpawnClientRpc(client.Key);
                }

                //RegisterPlayerForEvents(NetworkManager.Singleton.LocalClientId);
                HostPanel.SetActive(true);
            }
            else
            {
                ClientPannel.SetActive(true);
            }

            JoinCodeText.text = RelayManager.Instance.JoinCode;
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= PlayerSpawner.Instance.SpawnClientRpc;

                NetworkManager.OnClientDisconnectCallback -= PlayerSpawner.Instance.ClearSpawnPositionOfPlayer;
                
                PlayerSpawner.Instance.OnPlayerSpawned -= RegisterPlayerForEvents;
                PlayerSpawner.Instance.OnPlayerSpawned -= DisablePlayerAbilitiesOnConnect;

                PlayerSpawner.Instance.SetToDefaults();
            }
        }

        public virtual void StartMatch()
        {
            if (_hasMatchBegan || !IsServer) return;

            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                SetAbilityToUseMainActionsForConnected(true, client.Key);
            }

            DisableStartingPanelsRpc();
            StartMatchUnityEvent?.Invoke();
            _hasMatchBegan = true;
        }

        [ServerRpc]
        public void SendDisconnectRequestServerRpc(ulong clientId)
        {
            if (IsServer)
            {
                NetworkManager.DisconnectClient(clientId);
            }
        }

        private void SetAbilityToUseMainActionsForConnected(bool canUse, ulong clientId)
        {
            var newClientObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            if (newClientObject.TryGetComponent(out BombDeployer bombDeployer))
            {
                bombDeployer.SetAbilityToDeployBombsClientRpc(canUse);
            }
            //
            // if (newClientObject.TryGetComponent(out PlayerMovement playerMovement))
            // {
            //     Debug.Log($"disabled Movement for player with ID {clientId}");           //TODO: Cant normally block movement from server side
            //     playerMovement.SetAbilityToMoveClientRpc(canUse);    
            // }
        }

        private void DisablePlayerAbilitiesOnConnect(ulong clientId)
        {
            if (!_hasMatchBegan)
            {
                SetAbilityToUseMainActionsForConnected(false, clientId);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void DisableStartingPanelsRpc()
        {
            HostPanel.SetActive(false);
            ClientPannel.SetActive(false);
        }

        private void RegisterPlayerForEvents(ulong clientID)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject
                .TryGetComponent(out DeathResultHandler deathResultHandler))
            {
                deathResultHandler.OnPlayerDeathAction += SendRespawnRequestForPlayerWrapper;
            }
        }

        private async void SendRespawnRequestForPlayerWrapper(ulong clientID) => await SendRespawnRequestForPlayer(clientID);

        private async Task SendRespawnRequestForPlayer(ulong clientID)
        {
            // TODO Add check for ability to respawn
            await PlayerSpawner.Instance.SpawnPlayer(clientID, 3);         // TODO Set this Time to variable
            //RegisterPlayerForEvents(clientID);
        }
    }
}
