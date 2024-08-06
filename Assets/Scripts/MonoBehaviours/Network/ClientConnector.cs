using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonoBehaviours.Network
{
    public class ClientConnector : MonoBehaviour
    {
        [Header("Buttons to join the room")]
        [SerializeField]
        private Button JoinHostButton;
        
        [SerializeField]
        private TMP_InputField RoomName;


        [Space(20)]
        [Header("Connection Events:")]
        [Space(10)]
        public UnityEvent OnClientConnectionLaunched;
        public UnityEvent OnClientConnected;

        //private string _joinCodeResult;
        
        private void OnEnable()
        {
            JoinHostButton.onClick.AddListener(JoinHost);
        }

        private void OnDisable()
        {
            JoinHostButton.onClick.RemoveListener(JoinHost);
        }
        
        public async void JoinHost()
        {
            OnClientConnectionLaunched?.Invoke();
            
                    
            Debug.Log($"Checking Client ({NetworkManager.Singleton.LocalClientId}) to be connected to any session");
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
                NetworkManager.Singleton.Shutdown();
                Debug.Log($"Disconnecting Client ({NetworkManager.Singleton.LocalClientId}) from Network");
            }
            
            string joinCode = RoomName.text;
            if (!string.IsNullOrEmpty(joinCode))
            {
                JoinAllocation joinAllocation = await RelayManager.Instance.JoinRelay(joinCode);
                if (joinAllocation != null)
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                        joinAllocation.RelayServer.IpV4,
                        (ushort)joinAllocation.RelayServer.Port,
                        joinAllocation.AllocationIdBytes,
                        joinAllocation.Key,
                        joinAllocation.ConnectionData,
                        joinAllocation.HostConnectionData
                    );
                    
                    NetworkManager.Singleton.StartClient();
                    NetworkManager.Singleton.OnClientStarted += ClientConnected;
                }
            }
        }

        private void ClientConnected()
        {
            Debug.Log("PlayerConnected");
            PlayerSpawner.Instance.SpawnPlayerRpc();
            OnClientConnected?.Invoke();
        }
    }
}
