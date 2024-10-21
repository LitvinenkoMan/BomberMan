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
                try
                {
                    JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                    
                    OnClientConnectionLaunched?.Invoke();
                

                    NetworkManager.Singleton.GetComponent<UnityTransport>()
                        .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                    NetworkManager.Singleton.OnClientStarted += ClientConnected;
                    NetworkManager.Singleton.StartClient();
                }
                catch (RelayServiceException e)
                {
                    OnWrongCodeUsed?.Invoke();
                    Debug.LogError(e);
                    throw;
                }

            }
        }

        private void ClientConnected()
        {
            Debug.Log("PlayerConnected");
            OnClientConnected?.Invoke();
        }
    }
}
