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
    public class HostCreator : MonoBehaviour
    {
        [Header("Buttons to create room")] [SerializeField]
        private Button CreateHostButton;
        
        [SerializeField]
        private TMP_InputField RoomPassword;
        [SerializeField]
        private TMP_Text RoomJoinCodeText;
        [SerializeField]
        private TMP_Dropdown LevelSelection;

        
        [Space(20)]
        [Header("Hosting Events:")]
        [Space(10)]
        [SerializeField, Tooltip("This will fire at moment when host is only trying to launch.")]
        public UnityEvent OnHostLaunched;
        [SerializeField]
        public UnityEvent OnHostStarted;

        private void OnEnable()
        {
            CreateHostButton.onClick.AddListener(CreateHost);
            NetworkManager.Singleton.Shutdown();
        }

        private void OnDisable()
        {
            CreateHostButton.onClick.RemoveListener(CreateHost);
        }

        private async void CreateHost()
        {
            OnHostLaunched?.Invoke();
            
            Allocation allocation = await RelayManager.Instance.CreateRelay(10);
            
            if (allocation != null)
            {
                string joinCode = await RelayManager.Instance.GetJoinCode(allocation);
                RoomJoinCodeText.text = joinCode;
                if (!string.IsNullOrEmpty(joinCode))
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData
                    );

                    NetworkManager.Singleton.StartHost();
                    ServerStartedRpc();
                    
                    // Optionally, display the join code to the user
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void ServerStartedRpc()
        {
            PlayerSpawner.Instance.SpawnPlayerRpc();
            OnHostStarted?.Invoke();
        }
    }
}
