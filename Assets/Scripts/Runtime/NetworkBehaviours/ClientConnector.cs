using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
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

            string joinCode = RoomName.text;
            if (!string.IsNullOrEmpty(joinCode))
            {
                JoinAllocation joinAllocation = await RelayManager.Instance.JoinRelay(joinCode);
                
                if (joinAllocation != null)
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>()
                        .SetRelayServerData(new RelayServerData(joinAllocation,"dtls"));
                    
                    
                    await Task.Delay(5000);
                    NetworkManager.Singleton.OnClientStarted += ClientConnected;
                    NetworkManager.Singleton.StartClient();
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
