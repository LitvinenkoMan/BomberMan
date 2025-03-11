using System;
using System.Threading.Tasks;
using MonoBehaviours.Network;
using Runtime.MonoBehaviours;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.NetworkBehaviours.MatchManagers
{
    public class MatchManager : NetworkBehaviour
    {
        [Header("Base Params")]
        [Space(10)]
        [SerializeField] public TMP_Text JoinCodeText;
        [SerializeField] public GameObject HostPanel;
        [SerializeField] public GameObject ClientPannel;

        protected bool _hasMatchBegan;
        protected bool _isInitialized;
        protected float _spawnCheckTimer = 0f;

        public UnityEvent StartMatchUnityEvent;
        public UnityEvent EndMatchUnityEvent;

        public Action OnInitialized;

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
                }

                //RegisterPlayerForEvents(NetworkManager.Singleton.LocalClientId);
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

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                UnsubscribeFromRespawnEvents();
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
        public virtual void SendDisconnectRequestServerRpc(ulong clientId)
        {
            if (IsServer)
            {
                NetworkManager.DisconnectClient(clientId);
            }
        }

        protected virtual void SetAbilityToUseMainActionsForConnected(bool canUse, ulong clientId)
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

        protected void CheckForPlayerAbilities(ulong clientId)
        {
            if (!_hasMatchBegan)
            {
                SetAbilityToUseMainActionsForConnected(false, clientId);
            }
            else SetAbilityToUseMainActionsForConnected(true, clientId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        protected void DisableStartingPanelsRpc()
        {
            HostPanel.SetActive(false);
            ClientPannel.SetActive(false);
        }
 
        protected virtual void RegisterPlayerForEvents(ulong clientID)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject
                .TryGetComponent(out DeathResultHandler deathResultHandler))
            {
                deathResultHandler.OnPlayerDeathAction += SendRespawnRequestForPlayerWrapper;
            }
        }

        protected virtual async void SendRespawnRequestForPlayerWrapper(ulong clientID) => await SendRespawnRequestForPlayer(clientID);

        protected virtual async Task SendRespawnRequestForPlayer(ulong clientID)
        {
            await PlayerSpawner.Instance.SpawnPlayer(clientID, 3);         // TODO Set this Time to variable
        }

        protected virtual void SubscribeToRespawnEvents()
        {
            NetworkManager.OnClientConnectedCallback += PlayerSpawner.Instance.SpawnClientRpc;

            NetworkManager.OnClientDisconnectCallback += PlayerSpawner.Instance.ClearSpawnPositionOfPlayer;

            PlayerSpawner.Instance.OnPlayerSpawned += RegisterPlayerForEvents;
            PlayerSpawner.Instance.OnPlayerSpawned += CheckForPlayerAbilities;
        }

        protected virtual void UnsubscribeFromRespawnEvents()
        {
            NetworkManager.OnClientConnectedCallback -= PlayerSpawner.Instance.SpawnClientRpc;

            NetworkManager.OnClientDisconnectCallback -= PlayerSpawner.Instance.ClearSpawnPositionOfPlayer;

            PlayerSpawner.Instance.OnPlayerSpawned -= RegisterPlayerForEvents;
            PlayerSpawner.Instance.OnPlayerSpawned -= CheckForPlayerAbilities;

            PlayerSpawner.Instance.SetToDefaults();
        }
    }
}
