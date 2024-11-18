using System;
using System.Threading.Tasks;
using MonoBehaviours.Network;
using NetworkBehaviours;
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


        public UnityEvent StartMatchUnityEvent;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += RegisterPlayerForEvents;
                NetworkManager.OnClientConnectedCallback += DisablePlayerMovementOnConnect;
                RegisterPlayerForEvents(NetworkManager.Singleton.LocalClientId);
                HostPanel.SetActive(true);
                ClientPannel.SetActive(false);
            }
            else
            {
                HostPanel.SetActive(false);
                ClientPannel.SetActive(true);
            }
            JoinCodeText.text = RelayManager.Instance.JoinCode;
            SetAbilityToUseMainActionsForConnected(false, NetworkManager.Singleton.LocalClientId);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= RegisterPlayerForEvents;
                PlayerSpawner.Instance.SetToDefaults();
            }
        }

        public void StartMatch()
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

        private void SetAbilityToUseMainActionsForConnected(bool canUse, ulong clientId)
        {
            var newClientObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            if (newClientObject.TryGetComponent(out BombDeployer bombDeployer))
            {
                Debug.Log($"disabled BomDeploying for player with ID {clientId}");
                bombDeployer.SetAbilityToDeployBombsClientRpc(canUse);
            }

            if (newClientObject.TryGetComponent(out PlayerMovement playerMovement))
            {
                Debug.Log($"disabled Movement for player with ID {clientId}");
                playerMovement.SetAbilityToMoveClientRpc(canUse);
            }
        }

        private void DisablePlayerMovementOnConnect(ulong clientId)
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
            if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.TryGetComponent(out DeathResultHandler deathResultHandler))
            {
                deathResultHandler.OnPlayerDeathAction += SendRespawnRequestForPlayerWrapper;
                Debug.LogWarning($"Registered player with ID: {clientID}");
            }
            else
            {
                Debug.LogError($"Unable to find DeathResultHandler on object {NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject}: {clientID}");
            }
        }

        private void SendRespawnRequestForPlayerWrapper(ulong clientID) => SendRespawnRequestForPlayer(clientID);

        private async Task SendRespawnRequestForPlayer(ulong clientID)
        {
            // TODO Add check for ability to respawn
            await PlayerSpawner.Instance.SpawnClient(clientID, 3); // TODO Set this Time to variable
            RegisterPlayerForEvents(clientID);
        }
    }
}
