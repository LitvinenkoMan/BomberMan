using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
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
        public UnityEvent OnJoinToRelay;
        public UnityEvent OnClientConnected;
        public UnityEvent OnWrongCodeUsed;
        

        //private string _joinCodeResult;
        

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            await SignInAnonymously();
        }

        private async Task SignInAnonymously()
        {
            if (!AuthenticationService.Instance.IsAuthorized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
        }
        
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
            string joinCode = RoomName.text;
            if (!string.IsNullOrEmpty(joinCode))
            {
                OnClientConnectionLaunched?.Invoke();
                try
                {
                    JoinAllocation joinAllocation = await RelayManager.Instance.JoinRelay(joinCode);
                    

                    
                    NetworkManager.Singleton.GetComponent<UnityTransport>()
                        .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                    NetworkManager.Singleton.OnConnectionEvent += ClientConnected;
                    NetworkManager.Singleton.StartClient();

                    OnJoinToRelay?.Invoke();
                }
                catch (RelayServiceException e)
                {
                    OnWrongCodeUsed?.Invoke();
                    Debug.LogError(e);
                    throw;
                }

            }
        }

        private void ClientConnected(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            Debug.Log("Player Connected");
            OnClientConnected?.Invoke();
        }
    }
}
